var app = this;
this.cableColorLstInfoD = {};
this.inputoutputLstInfoD = {};
this.preSelectedCountries = null;
this.preSelectedRegion = null;
this.preSelectedStates = null;
this.preSelectedCities = null;
this.preSelectedSubDistrict = null;
this.preSelectedBlock = null;
//this.stepper = new Stepper(document.querySelector('.bs-stepper'));

if (window.location.href.indexOf("AddUser") > -1 || window.location.href.indexOf("SaveUser") > -1) {
    $(document).ready(function () {         
        var stepper = new Stepper(document.querySelector('.bs-stepper'));
        $('.chosenfetool').chosen({
            placeholder_text_multiple: '-Select-',
            width: '100%',
            no_results_text: ''
        });
        $('.chosen-select').chosen({
            placeholder_text_multiple: 'All',
            width: '100%'
        });      

        $(".Nextbtn").on("click", function (e) {
            // 
            //console.log("checkDuplicate Email ID::" + checkDuplicateUserName());
            //console.log("checkDuplicateEmailId::" + checkDuplicateEmailId());
            checkDuplicateUserName();
            if (checkDuplicateEmailId() == false) {
                //console.log('working');
                e.preventDefault(); return true;
            }


            if (!CheckUserRole()) { e.preventDefault(); return true; }
            else if (!CheckReportingManager()) { e.preventDefault(); return true; }
            
            //if (($("#user_id").val() == 0)) {
            if (!CheckApplicationAccess()) { e.preventDefault(); return true; }
            if (!CheckUserType()) { e.preventDefault(); return true; }
            //}
            if ($('#password').length > 0 && !passwordRegex($('#password').val())) { e.preventDefault(); return true; }
            if ($('#user_email').length > 0 && !emailRegex($('#user_email').val())) { e.preventDefault(); return true; }

            //if (!checkDuplicateUserName()) { e.preventDefault(); return true; }
            //else if (!checkDuplicateEmailId()) { e.preventDefault(); return true; }

            if (!CheckModule()) { e.preventDefault(); return true; }

            if ($("#frmUserInfo").valid()) {
                if ($("#userdetails").hasClass('active')) {
                    $("#chkImg_userdetails").css("display", "block");
                    $(".userdetails_circle").css("display", "none");
                }
                if ($("#modulerights-part").hasClass('active')) {
                    $("#chkImg_modulerights").css("display", "block");
                    $(".modulerights_circle").css("display", "none");
                }
                stepper.next();
                return false;
            }
            //$.when(checkDuplicateUserName(), checkDuplicateEmailId()).done(function (ajax1Results, ajax2Results) {

            //    console.log("ajax1Results"+ajax1Results);

            //});


        });


        $("#user_name").on("blur", function () { checkDuplicateUserName() });

        $("#user_email").on("blur", function () { checkDuplicateEmailId() });

        function checkDuplicateEmailId() {
            var status = false;
            if ($("#user_email").val() != "") {
                //console.log('outside AJAX');
                ajaxReq('User/CheckDuplicateUserNameandEmail', { user_id: $("#user_id").val(), userName: '', emailId: $("#user_email").val() }, false, function (obj) {
                    //console.log('inside AJAX: ' + obj.status);
                    if (obj.status == 'ERROR') {
                        console.log(obj.message);

                        alert(obj.message);
                        //$("#user_email").val("");
                        //$('#ImgUserEmail').show();
                        //$('#ImgUserEmail').attr('src', appRoot.replace('Admin/', '') + 'Areas/Admin/Content/images/cross.gif');
                        //e.preventDefault(); 
                        status = false;

                    } else {
                        //console.log("Else Part");
                        //$('#ImgUserEmail').show();
                        //$('#ImgUserEmail').attr('src', appRoot.replace('Admin/', '') + 'Areas/Admin/Content/images/tick_1.gif');
                        status = true;
                    }
                });
                return status;
            }
        }
        var multi_manager_ids = $("#multi_manager_ids").val();
        if (multi_manager_ids != "") {
            if ($("#ddl_UserReportingManager").length) {
                $("#ddl_UserReportingManager").val(multi_manager_ids.split(',')).trigger("chosen:updated");
            }
        }

        
        var multi_tools_ids = $("#multi_tool_ids").val();
        debugger;
        if (multi_tools_ids != "") {
            if ($("#ddl_fetool").length) {
                $("#ddl_fetool").val(multi_tools_ids.split(',')).trigger("chosen:updated");
                
            }
        }
       

        var multi_warhouse_codes = $("#multi_warhouse_code").val();
        if (multi_warhouse_codes != "") {
            if ($("#ddl_WarehouseCode").length) {
                $("#ddl_WarehouseCode").val(multi_warhouse_codes.split(',')).trigger("chosen:updated");
                //app.preSelectedStates = $("#selectedProvinces").val();
                //getStates();
            }
        }
        function checkDuplicateUserName() {
            if ($("#user_name").val() != "") {
                ajaxReq('User/CheckDuplicateUserNameandEmail', { user_id: $("#user_id").val(), userName: $("#user_name").val(), emailId: '' }, false, function (obj) {
                    if (obj.status == 'ERROR') {
                        console.log(obj.message);
                        //$('#ImgUsername').show();
                        //$('#ImgUsername').attr('src', appRoot.replace('Admin/', '') + 'Areas/Admin/Content/images/cross.gif');
                        alert(obj.message);
                        $("#user_name").val("");

                    } else {
                        //console.log(obj.message);
                        //$('#ImgUsername').show();
                        //$('#ImgUsername').attr('src', appRoot.replace('Admin/', '') + 'Areas/Admin/Content/images/tick_1.gif');

                    }
                });
            }
        }


        $(".Previousbtn").on("click", function (e) {
            if ($("#frmUserInfo").valid()) {
                stepper.previous();
                if ($("#userdetails").hasClass('active')) {
                    $("#chkImg_userdetails").css("display", "none");
                    $(".userdetails_circle").css("display", "block");
                }
                if ($("#modulerights-part").hasClass('active')) {
                    $("#chkImg_modulerights").css("display", "none");
                    $(".modulerights_circle").css("display", "block");
                }
                return false;
            }
        });
        if ($("#submit_status").val() == "OK") {
            $("#chkImg_userdetails").css("display", "block");
            $(".userdetails_circle").css("display", "none");
            $("#chkImg_modulerights").css("display", "block");
            $(".modulerights_circle").css("display", "none");
            $("#chkImg_workarea").css("display", "block");
            $(".workarea_circle").css("display", "none");
            $("#modulerights-part-trigger").attr("disabled", true);
        }
        stepper = new Stepper($('.bs-stepper')[0])
        $('.chosen-select').chosen({ width: '100%' });
        if ($("#user_id").val() != 0) {
            btnSubmit.value = "Update";
        }
        if ($("#role_id").val() != 0) {

            $('#divReportingManager').show();

            BindUserServiceFacilityJoOrderTypeforContractorRole($("#ddl_UserRole option:selected").text(), $("#role_id").val(), $("#user_id").val());
            var reportingManagerId = $("#ddl_UserReportingManager").val();

            if ($("#ismutimanagar").val() == "True") {
                if (reportingManagerId != null) {
                    $('#multi_manager_ids').val($("#ddl_UserReportingManager").val().join(","));
                    // reportingManagerId=reportingManagerId[0];
                    reportingManagerId = $("#ddl_UserReportingManager").val().join(",")
                }                              
            }

            else {
                $('#multi_manager_ids').val($("#ddl_UserReportingManager").val());
            }
            BindUserServiceFacilityJoOrderTypeforFERole($("#ddl_UserRole option:selected").text(), reportingManagerId, $("#user_id").val());
        }

        //var multi_reporting_role_ids = $("#ddlroles").val();
        //if (multi_reporting_role_ids!='') {
        //    if ($("#ismutiRRmanagar").val() == "True") {
        //        if (reportingManagerId != null) {
        //            $('#multi_reporting_role_ids').val($("#ddlroles").val().join(","));
        //            multi_reporting_role_ids = $("#ddlroles").val().join(",")
        //        }
        //    }

        //    else {
        //        $('#multi_reporting_role_ids').val($("#ddlroles").val());
        //    }
        //}
        


        //country
        var SelectedCountryIds = $("#selectedCountry").val();
        if (SelectedCountryIds != "") {
            if ($("#ddlCountry").length) {
                $("#ddlCountry").val(SelectedCountryIds.split(',')).trigger("chosen:updated");
                app.preSelectedRegion = $("#selectedRegion").val();
                getRegions();
            }
        }

        var SelectedRegionIds = $("#selectedRegion").val();
        if (SelectedRegionIds != "") {
            if ($("#ddlregion").length) {
                $("#ddlregion").val(SelectedRegionIds.split(',')).trigger("chosen:updated");
                app.preSelectedStates = $("#selectedProvinces").val();
                getStates();
            }
        }

        var SelectedProvincesIds = $("#selectedProvinces").val();

        if (SelectedProvincesIds != "") {
            if ($("#ddlState").length) {
                $("#ddlState").val(SelectedProvincesIds.split(',')).trigger("chosen:updated");
                app.preSelectedSubDistrict = $("#selectedSubDistrict").val();
                getsubdistrict();
            }
        }
        var SelectedBlockIds = $("#selectedBlock").val();

        if (SelectedBlockIds != "") {
            if ($("#ddlBlock").length) {
                $("#ddlBlock").val(SelectedBlockIds.split(',')).trigger("chosen:updated");
                app.preSelectedBlock = $("#selectedBlock").val();
                getblock();
            }
        }

       
        // fillPermissionArea();

        //country
        $("#ddlCountry").on('change', function () {
       
            app.preSelectedRegion = $("#ddlregion").val() != null ? $("#ddlregion").chosen().val().join(",") : null;
            app.preSelectedCountries = $("#ddlCountry").val() != null ? $("#ddlCountry").chosen().val().join(",") : null;
            app.preSelectedSubDistrict = $("#ddlSubDistrict").val() != null ? $("#ddlSubDistrict").chosen().val().join(",") : null;
            app.preSelectedBlock = $("#ddlBlock").val() != null ? $("#ddlBlock").chosen().val().join(",") : null;
            getRegions();
            $("#ddlregion").trigger("change");
            // $("#ddlSubDistrict").trigger("change");
            //  $("#ddlblock").trigger("change");
            return false;
        });
        $("#ddlregion").on('change', function () {

            app.preSelectedStates = $("#ddlState").val() != null ? $("#ddlState").chosen().val().join(",") : null;

            getStates();
            return false;
        });
        $("#ddlState").on('change', function () {

            app.preSelectedSubDistrict = $("#ddlSubDistrict").val() != null ? $("#ddlSubDistrict").chosen().val().join(",") : null;

            getsubdistrict();
            $("#ddlSubDistrict").trigger("change");
            return false;
        });



        $("#ddlSubDistrict").on('change', function () {
            app.preSelectedBlock = $("#ddlBlock").val() != null ? $("#ddlBlock").chosen().val().join(",") : null;

            getblock();
            return false;
        });

        $("#btnSubmit").on("click", function (e) {
            // if (!CheckRegion()) { e.preventDefault(); return; }
            // if (!CheckState()) { e.preventDefault(); return; }
            if ($("#workarea-part").hasClass('active')) {
                $("#chkImg_workarea").css("display", "block");
                $(".workarea_circle").css("display", "none");
            }
         
            if ($("#isWFMUser").val() == "true") {
                if ($("#ddlCountry").val() == null) {
                    confirm(getMultilingualStringValue($.validator.format(MultilingualKey.SI_ADM_GBL_JQ_FRM_005, MultilingualKey.SI_ADM_GBL_NET_FRM_001, MultilingualKey.SI_ADM_GBL_NET_FRM_002, MultilingualKey.SI_ADM_GBL_NET_FRM_003, MultilingualKey.SI_ADM_GBL_NET_FRM_004, MultilingualKey.SI_ADM_GBL_NET_FRM_005)), function () {
                        $('#selectedCountry').val(null);
                        $('#selectedRegion').val(null);
                        $('#selectedProvinces').val(null);
                        $('#selectedSubDistrict').val(null);
                        $('#selectedBlock').val(null);
                        $("#frmUserInfo").submit();
                    });
                }
                else if ($("#ddlCountry").val() != null) {
                    $('#selectedCountry').val($("#ddlCountry").val().join(","));

                    if ($("#ddlregion").val() == null) {
                        confirm(getMultilingualStringValue($.validator.format(MultilingualKey.SI_ADM_GBL_JQ_FRM_006, MultilingualKey.SI_ADM_GBL_NET_FRM_002, MultilingualKey.SI_ADM_GBL_NET_FRM_003, MultilingualKey.SI_ADM_GBL_NET_FRM_004, MultilingualKey.SI_ADM_GBL_NET_FRM_005)), function () {
                            $('#selectedRegion').val(null);
                            $('#selectedProvinces').val(null);
                            $('#selectedSubDistrict').val(null);
                            $('#selectedBlock').val(null);
                            $("#frmUserInfo").submit();
                        });
                    }
                    else if ($("#ddlregion").val() != null) {
                        $('#selectedRegion').val($("#ddlregion").val().join(","));

                        if ($("#ddlState").val() == null) {
                            confirm(getMultilingualStringValue($.validator.format(MultilingualKey.SI_ADM_GBL_JQ_FRM_003, MultilingualKey.SI_ADM_GBL_NET_FRM_003, MultilingualKey.SI_ADM_GBL_NET_FRM_004, MultilingualKey.SI_ADM_GBL_NET_FRM_005)), function () {

                                $('#selectedProvinces').val(null);
                                $('#selectedSubDistrict').val(null);
                                $('#selectedBlock').val(null);
                                $("#frmUserInfo").submit();
                            });
                        }
                        else if ($("#ddlState").val() != null) {
                            $('#selectedProvinces').val($("#ddlState").val().join(","));

                            if ($("#ddlSubDistrict").val() == null) {
                                confirm(getMultilingualStringValue($.validator.format(MultilingualKey.SI_ADM_GBL_JQ_FRM_004, MultilingualKey.SI_ADM_GBL_NET_FRM_004, MultilingualKey.SI_ADM_GBL_NET_FRM_005)), function () {

                                    $('#selectedSubDistrict').val(null);
                                    $('#selectedBlock').val(null);
                                    $("#frmUserInfo").submit();
                                });
                            }
                            else if ($("#ddlSubDistrict").val() != null) {
                                $('#selectedSubDistrict').val($("#ddlSubDistrict").val().join(","));
                                if ($("#ddlBlock").val() == null) {
                                    $('#selectedBlock').val(null);
                                    $("#frmUserInfo").submit();
                                }
                                else if ($("#ddlBlock").val() != null) {
                                    $('#selectedBlock').val($("#ddlBlock").val().join(","));
                                    $("#frmUserInfo").submit();
                                }
                            }

                        }
                    }

                }
                //else if ($("#ddlregion").val() != null && $("#ddlState").val() == null) {
                //    $('#selectedRegion').val($("#ddlregion").val().join(","));
                //    $('#selectedProvinces').val(null);
                //    $("#frmUserInfo").submit();
                //}
                //else if ($("#ddlregion").val() != null && $("#ddlState").val() != null) {
                //    $('#selectedRegion').val($("#ddlregion").val().join(","));
                //    $('#selectedProvinces').val($("#ddlState").val().join(","));
                //    $("#frmUserInfo").submit();
                //}
            }
            else {
                if ($("#ddlCountry").val() == null) {
                    confirm(getMultilingualStringValue($.validator.format(MultilingualKey.SI_ADM_GBL_JQ_FRM_001, MultilingualKey.SI_ADM_GBL_NET_FRM_001, MultilingualKey.SI_ADM_GBL_NET_FRM_002, MultilingualKey.SI_ADM_GBL_NET_FRM_003)), function () {
                        $('#selectedCountry').val(null);
                        $('#selectedRegion').val(null);
                        $('#selectedProvinces').val(null);
                        $('#selectedSubDistrict').val(null);
                        $('#selectedBlock').val(null);
                        $("#frmUserInfo").submit();
                    });
                }
                else if ($("#ddlCountry").val() != null) {
                    $('#selectedCountry').val($("#ddlCountry").val().join(","));

                    if ($("#ddlregion").val() == null) {
                        confirm(getMultilingualStringValue($.validator.format(MultilingualKey.SI_ADM_GBL_JQ_FRM_002, MultilingualKey.SI_ADM_GBL_NET_FRM_002, MultilingualKey.SI_ADM_GBL_NET_FRM_003)), function () {
                            $('#selectedRegion').val(null);
                            $('#selectedProvinces').val(null);
                            $('#selectedSubDistrict').val(null);
                            $('#selectedBlock').val(null);
                            $("#frmUserInfo").submit();
                        });
                    }
                    else if ($("#ddlregion").val() != null) {
                        $('#selectedRegion').val($("#ddlregion").val().join(","));

                        if ($("#ddlState").val() == null) {

                            $('#selectedProvinces').val(null);
                            $('#selectedSubDistrict').val(null);
                            $('#selectedBlock').val(null);
                            $("#frmUserInfo").submit();

                        }
                        else if ($("#ddlState").val() != null) {
                            $('#selectedProvinces').val($("#ddlState").val().join(","));

                            if ($("#ddlSubDistrict").val() == null) {
                                $('#selectedSubDistrict').val(null);
                                $('#selectedBlock').val(null);
                                $("#frmUserInfo").submit();

                            }
                            else if ($("#ddlSubDistrict").val() != null) {
                                $('#selectedSubDistrict').val($("#ddlSubDistrict").val().join(","));
                                if ($("#ddlBlock").val() == null) {
                                    $('#selectedBlock').val(null);
                                    $("#frmUserInfo").submit();
                                }
                                else if ($("#ddlBlock").val() != null) {
                                    $('#selectedBlock').val($("#ddlBlock").val().join(","));
                                    $("#frmUserInfo").submit();
                                }
                            }

                        }
                    }


                    //else if ($("#ddlregion").val() != null && $("#ddlState").val() == null) {
                    //    $('#selectedRegion').val($("#ddlregion").val().join(","));
                    //    $('#selectedProvinces').val(null);
                    //    $("#frmUserInfo").submit();
                    //}
                    //else if ($("#ddlregion").val() != null && $("#ddlState").val() != null) {
                    //    $('#selectedRegion').val($("#ddlregion").val().join(","));
                    //    $('#selectedProvinces').val($("#ddlState").val().join(","));
                    //    $("#frmUserInfo").submit();
                    //}
                }
            }
        });
    });

    $("#ddl_UserRole").on('change', function () {
        CheckUserRole();
    });

  



    $("#ddl_UserReportingManager").on('change', function () {
        CheckReportingManager();
    });
    
    $(document).on("change", "#ddlApplication", function () {
        console.log("dfdf");
        CheckApplicationAccess();
        app.displayApplicationModule();
    });

    //$("#ddlApplication").change(function () {

    //});


    function CheckUserRole() {
        if ($("#ddl_UserRole").val() == null || $("#ddl_UserRole").val() == "") {
            $('#ddl_UserRole_chosen').css({ "border": "1px solid red", "border-radius": "0.25rem" });
            return false;
        }
        else {
            $('#ddl_UserRole_chosen').css({ "border": "1px solid #ced4da", "border-radius": "0.25rem" });
            return true;
        }
    }; 
 
    
    function CheckReportingManager() {
        if ($("#ddl_UserReportingManager").val() == null || $("#ddl_UserReportingManager").val() == "") {
            $('#ddl_UserReportingManager_chosen').css({ "border": "1px solid red", "border-radius": "0.25rem" });
            return false;

        } else {
            $('#ddl_UserReportingManager_chosen').css({ "border": "1px solid #ced4da", "border-radius": "0.25rem" });
            return true;
        }
    };

    function CheckApplicationAccess() {



        if ($("#ddlApplication").val() == null || $("#ddlApplication").val() == "") {
            $('#ddlApplication_chosen').css({ "border": "1px solid red", "border-radius": "0.25rem" });
            return false;

        } else {
            $('#ddlApplication_chosen').css({ "border": "1px solid #ced4da", "border-radius": "0.25rem" });
            checkUserLimit();
            return true;
        }
    };
        function CheckUserType() {



            if ($("#ddluser_type").val() == null || $("#ddluser_type").val() == "") {
                $('#ddluser_type_chosen').css({ "border": "1px solid red", "border-radius": "0.25rem" });
                return false;

            } else {
                $('#ddluser_type_chosen').css({ "border": "1px solid #ced4da", "border-radius": "0.25rem" });
                return true;
            }
        };

    function checkModuleSelected(moduleId) {

        var isChildActive = $('.chkModuleChild_' + moduleId).is(":checked");
        if (isChildActive == true)
            $(".module_" + moduleId).prop('checked', isChildActive);
        var isChildActive = $('.chkModuleSubChild_' + moduleId).is(":checked");
        if (isChildActive == true)
            $(".chkModuleChild_" + moduleId).prop('checked', isChildActive);


        setAllCheckBox();
    }


    function CheckModule() {
        if ($(".chkModule").is(":checked") == false && $("#modulerights-part").hasClass('active')) {
            $("#errModule").show();
            return false;
        }
        setAllCheckBox();
        $("#errModule").hide();
        return true;

    }
    $('#password').on('keyup', function () {
        passwordRegex($('#password').val());
    });
    $('#user_email').on('keyup', function () {
        emailRegex($('#user_email').val());
    });


    function getRegions() {

        var selectedCountries;
        var selectedcount = 0;
        if ($("#ddlCountry").chosen().val() != null) {
            selectedCountries = "" + $("#ddlCountry").chosen().val().join(",") + "";
            selectedcount = $("#ddlCountry").chosen().val().length;
        }

        $("#ddlregion").empty();
        if (selectedcount > 0) {
            ajaxReq('User/GetRegionByCountryid', { countryName: selectedCountries }, true, function (regions) {
                $.each(regions, function (i, region) {
                    $("#ddlregion").append('<option value="' + region.regionId + '">' + region.regionName + ' (' + region.regionAbbr + ')' + ' </option>');
                });
                if (app.preSelectedRegion != null) {

                    $("#ddlregion").val(app.preSelectedRegion.split(',')).trigger("chosen:updated");
                    getStates();
                }
                else { $("#ddlregion").trigger("chosen:updated"); getStates(); }


            }, true, true);
        }
        else {
            $("#ddlregion").trigger("chosen:updated");
        }

    }

    function getStates() {

        var selectedRegions;
        var selectedcount = 0;
        if ($("#ddlregion").chosen().val() != null) {
            selectedRegions = "" + $("#ddlregion").chosen().val().join(",") + "";
            selectedcount = $("#ddlregion").chosen().val().length;
        }

        $("#ddlState").empty();
        if (selectedcount > 0) {
            ajaxReq('User/GetProvinceByRegionid', { regionid: selectedRegions }, true, function (states) {
                $.each(states, function (i, state) {
                    $("#ddlState").append('<option value="' + state.provinceId + '">' + state.provinceName + ' (' + state.provinceAbbr + ')' + '</option>');
                });
                if (app.preSelectedStates != null) {

                    $("#ddlState").val(app.preSelectedStates.split(',')).trigger("chosen:updated");
                    getsubdistrict();
                }
                else
                    $("#ddlState").trigger("chosen:updated");
            }, true, true);
        }
        else {
            $("#ddlState").trigger("chosen:updated"); getsubdistrict();
        }

    }
    function getsubdistrict() {

        var selectedState;
        var selectedcount = 0;
        if ($("#ddlState").chosen().val() != null) {
            selectedStates = "" + $("#ddlState").chosen().val().join(",") + "";
            selectedcount = $("#ddlState").chosen().val().length;
        }

        $("#ddlSubDistrict").empty();
        if (selectedcount > 0) {
            ajaxReq('User/GetSubdistrictByProvinceId', { stateid: selectedStates }, true, function (states) {
                $.each(states, function (i, state) {

                    $("#ddlSubDistrict").append('<option value="' + state.subDistrictId + '">' + state.subDistrictName + '</option>');
                });

                if (app.preSelectedSubDistrict != null) {

                    $("#ddlSubDistrict").val(app.preSelectedSubDistrict.split(',')).trigger("chosen:updated");
                    getblock();
                }
                else
                    $("#ddlSubDistrict").trigger("chosen:updated");
            }, true, true);
        }
        else {
            $("#ddlSubDistrict").trigger("chosen:updated"); getblock();
        }

    }
    function getblock() {

        var selectedSubDistrict;
        var selectedcount = 0;
        if ($("#ddlSubDistrict").chosen().val() != null) {
            selectedSubDistrict = "" + $("#ddlSubDistrict").chosen().val().join(",") + "";
            selectedcount = $("#ddlSubDistrict").chosen().val().length;
        }

        $("#ddlBlock").empty();
        if (selectedcount > 0) {
            ajaxReq('User/GetBlockBySubDistrictId', { subdistrictid: selectedSubDistrict }, true, function (states) {
                $.each(states, function (i, state) {
                    $("#ddlBlock").append('<option value="' + state.blockId + '">' + state.blockName + '</option>');
                });
                if (app.preSelectedBlock != null) {

                    $("#ddlBlock").val(app.preSelectedBlock.split(',')).trigger("chosen:updated");
                }
                else
                    $("#ddlBlock").trigger("chosen:updated");
            }, true, true);
        }
        else {
            $("#ddlBlock").trigger("chosen:updated");
        }

    }


    function CheckByClass1(clsNmae, cls, obj) {
         
        $("." + clsNmae + "[type='checkbox']").prop("checked", $(obj).prop("checked"));
        $("." + cls + "[type='checkbox']").prop("checked", $(obj).prop("checked"));
        setAllCheckBox();
    }

    function CheckByClass(clsNmae, obj) {
         
        console.log("CheckByClass running");
        $("." + clsNmae + "[type='checkbox']").prop("checked", $(obj).prop("checked"));
        setAllCheckBox();
    }



    function checkModuleSelected(moduleId) {
         
        var isChildActive = $('.chkModuleChild_' + moduleId).is(":checked");
        if (isChildActive == true)
            $(".module_" + moduleId).prop('checked', isChildActive);

        if (isChildActive == false)
            $(".module_" + moduleId).prop('checked', isChildActive);

        setAllCheckBox();
    }

    function checkModuleSubSelected(moduleId, moduleId1) {
         
        var isSubChildActive = $('.chkModuleSubChild_' + moduleId).is(":checked");
        // var isChildActive = $('.chkModuleChild_' + moduleId1).is(":checked");
        //if (isSubChildActive == true) chkModuleChild_
        //$(".chkModuleChild_" + moduleId).prop('checked', isSubChildActive);
        if (isSubChildActive == true)
            $(".ch_" + moduleId).prop('checked', isSubChildActive);
        $(".module_" + moduleId1).prop('checked', isSubChildActive);
        if (isSubChildActive == false)
            $(".ch_" + moduleId).prop('checked', isSubChildActive);
        // $(".module_" + moduleId1).prop('checked', isSubChildActive);
        var isChildActive = $('.chkModuleChild_' + moduleId1).is(":checked");
        if (isChildActive == true)
            // $(".ch_" + moduleId).prop('checked', isSubChildActive);
            $(".module_" + moduleId1).prop('checked', isChildActive);

        //if (isSubChildActive == false)
        //    $(".ChildModule_" + moduleId).prop('checked', isSubChildActive);
        setAllCheckBox();
    }
    //function setAllCheckBox() {
    //    $('#CheckAll_Module').prop("checked", $('.chkModule').length == $('.chkModule:checked').length);
    //    $('#CheckAll_Web').prop("checked", $('#divWebModule > .chkModule').length == $('#divWebModule > .chkModule:checked').length);
    //    $('#CheckAll_Mobile').prop("checked", $('#divMobileModule > .chkModule').length == $('#divMobileModule > .chkModule:checked').length);
    //}
    function setAllCheckBox() {
         
        $('#CheckAll_Web').prop("checked", $("#divWebModule input[type=checkbox]").length == $("#divWebModule input[type=checkbox]:checked").length);
        $('#CheckAll_Mobile').prop("checked", $("#divMobileModule  input[type=checkbox]").length == $("#divMobileModule  input[type=checkbox]:checked").length);
        $('#CheckAll_Admin').prop("checked", $("#divAdminModule  input[type=checkbox]").length == $("#divAdminModule  input[type=checkbox]:checked").length);

        $('#CheckAll_ServiceFacility').prop("checked", $("#divServiceFacility input[type=checkbox]").length == $("#divServiceFacility  input[type=checkbox]:checked").length);
        $('#CheckAll_JobOrder').prop("checked", $("#divJobOrder input[type=checkbox]").length == $("#divJobOrder  input[type=checkbox]:checked").length);
        $('#CheckAll_JobCategory').prop("checked", $("#divJobCategory input[type=checkbox]").length == $("#divJobCategory  input[type=checkbox]:checked").length);

        var $allModuleCheckbox = $("#modulerights-part input[type=checkbox]").not("#CheckAll_Module");
        var $allModuleCheckedCheckbox = $("#modulerights-part input[type=checkbox]:checked").not("#CheckAll_Module");
        $('#CheckAll_Module').prop("checked", $allModuleCheckbox.length == $allModuleCheckedCheckbox.length);
    }

}


















this.DE = {

    "AddInputPortLstRow": "#btnAddInputPortRow",
    "InputPortInfoTable": "#tblInputPortLstInfo",
    "AddOutputPortLstRow": "#btnAddOutputPortRow",
    "OutputPortInfoTable": "#tblOutputPortLstInfo",
    "DeleteInputPortRow": "#inputPortRowDelete",
    "DeleteOutputPortRow": "#outputPortRowDelete",
    "AddNewCoreLstRow": "#btnAddNewCore",
    "tblCableInfoTable": "#tblCableTubeColor",
    "divCableTubeColor": "#divCableTubeColor"
}



$(function () {

    $(document).on('click', "#dvNav > ul > li > a", function (e) {

        $(this).find('.addon').toggleClass('icon-Hide_Menu').toggleClass('icon-Open_Menu');
        //$(this).closest('li').find('span.hasDrop').toggleClass('icomoon-icon-arrow-down-2').toggleClass('icomoon-icon-arrow-up-2');
        $(this).closest('li').find('.sidenav-second-level').slideToggle();
    });

    $('#sidenavToggler').on('click', function () {
        if ($('.sidenav').width() == 250) {
            $('.sidenav').animate({ 'width': '53px' }, 'slow', function () { $('.content-wrapper').css('width', '96.6%'), 'slow' });
            $('.sidenav-toggler').animate({ 'margin-left': '35px' }, 'slow');
            $('#sidenavToggler').addClass('fa-arrow-right');
            $('#sidenavToggler').removeClass('fa-arrow-left');
            $('.navbar-sidenav > li > a > span.nav-link-text').hide();
            $('#dvNav > ul > li > ul.sidenav-second-level').hide();
        }
        //else if ($('.sidenav').width() == 53) {
        //    
        //    $('.content-wrapper').animate({ 'width': '95%' }, 'slow', function () { });
        //    $('.content-wrapper').css('width','95%')
        //}
        else {

            $('.sidenav').animate({ 'width': '250px' }, 'slow', function () {
                $('.content-wrapper').css('width', 'calc(100% - 250px)'), 'slow';
                $('.sidenav').show();
            });
            $('.sidenav-toggler').animate({ 'margin-left': '235px' }, 'slow');
            $('#sidenavToggler').addClass('fa-arrow-left');
            $('#sidenavToggler').removeClass('fa-arrow-right');
            $('.navbar-sidenav  > li > a > span.nav-link-text').show();
            //$('.menu_set_1,.menu_set_2') .css({position: "inherit"});		

            /*$(".nav-item").on("click", function() {
                $(this).find('ul').slideToggle();
            	
            });*/


        }
    });





});

function autoClosePageMessage() {
    $(function () {
        $('div.alert-success, div.alert-danger').delay(4000).hide("slow");
    });
}

$(function () {

    $(".nav-link").on('click', function () {
        // $('.addon').toggleClass('fa-chevron-up');
        // $('.addon').toggleClass('fa-chevron-down');
        //$(this).find('.addon').toggleClass('fa-chevron-up').toggleClass('fa-chevron-down');

    });
});


function ajaxReq(url, _data, isAsync, callback, is_request_JSON, isLoaderRequired, is_response_JSON) {
    if (isAsync === true) isasync = !0; else isAsync = !1;
    if (is_response_JSON == undefined) is_response_JSON = true;

    var ajaxParams = {
        type: "POST", timeout: 3600000, url: appRoot + url, async: isAsync, error: function (a, b, c, d) {
            if (a.status === 401 || a.status === 570 || (a.status === 0 && b != "timeout"))
                window.location.reload();
            if (isLoaderRequired) { hideProgress(); }
        }, success: callback,
        complete: function (a, b) {
            if (b === "timeout") { console.log('Request timeout reached.'); }
            if (isLoaderRequired) { hideProgress(); }
        },
        beforeSend: function () { if (isLoaderRequired) { showProgress(); } }
    };
    if (is_request_JSON === true) {
        ajaxParams = $.extend(ajaxParams, { contentType: "application/json; charset=utf-8" });
        // changed on : 24 July-2018
        if (is_response_JSON) { ajaxParams = $.extend(ajaxParams, { dataType: 'json' }); }

        _data = JSON.stringify(_data);
    }
    ajaxParams = $.extend(ajaxParams, { data: _data });
    $.ajax(ajaxParams);
}
function showProgress() {
    $("#dvProgress").show();
}
function hideProgress() {
    $("#dvProgress").hide();
}
/*========================================================================== Start Dashboard==================================================*/
function drawUserLineChart() {
    var data = new google.visualization.DataTable();
    data.addColumn('string', 'Month');
    data.addColumn('number', "User Count");
    var i = 0;
    response.LastMonthActiveUsers.forEach(function (item) {

        var loginDate = new Date(item.login_date);
        data.addRows([
            [loginDate.getDate() + ' ' + getMonthName(loginDate.getMonth()), item.users]
        ]);
    });

    var options = {
        legend: { position: 'top' },
        backgroundColor: '#faffff',
        hAxis: {
           
            textStyle: { fontSize: 10 },
            direction: 1,
            slantedText: true,
            slantedTextAngle: 90
            
        },
    };
    var chart = new google.visualization.LineChart(document.getElementById('UserLineChart'));
    chart.draw(data, options);
}

function drawUserPaiChart() {
    var data = google.visualization.arrayToDataTable([
        ['Task', 'Hours per Day'],
        ['Active User', response.CurrentActiveUsers.total_curent_login],
        ['In-Active User', response.CurrentActiveUsers.total_user - response.CurrentActiveUsers.total_curent_login]
    ]);
    var options = { pieHole: 0.4, backgroundColor: '#faffff', legend: { position: 'top' } };
    var chart = new google.visualization.PieChart(document.getElementById('UserPaiChart'));
    chart.draw(data, {
        sliceVisibilityThreshold: 0
    }, options);
}

function getMonthName(index) {
    var monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
    ];
    return monthNames[index]
}
/*========================================================================== End Dashboard==================================================*/

/*========================================================================== Start OSP Settings==================================================*/

function InfoLayerChange() {
    // 
    var _layername = $('#lstLayers option:selected').text();
    var _layerId = $('#lstLayers option:selected').val();
    if (_layerId != undefined && _layerId != 0) {
        window.open(appRoot + '/OSPSettings/InfoSettings/' + _layerId, "_self");
    }
    else {
        window.open(appRoot + '/OSPSettings/InfoSettings', "_self");
    }
}

function fillImportReg_ProData() {
    var _layername = $('#ddltype option:selected').text();
    var _layerId = $('#ddltype option:selected').val();
    var postData = { layername: _layername };
    //ajaxReq('OSPSettings/ImportRegionProvinceShapeFile', { layername: _layername, layerid: _layerId }, false,
    //function (resp){}, true, true);
}

function SearchLayerChange() {
    // 
    var _layername = $('#lstLayers option:selected').text();
    var _layerId = $('#lstLayers option:selected').val();
    if (_layerId != undefined && _layerId != 0) {
        window.open(appRoot + '/OSPSettings/SearchSettings/' + _layerId, "_self");
    }
    else {
        window.open(appRoot + '/OSPSettings/SearchSettings', "_self");
    }
}
function DeleteProject(system_id) {

    confirm("Are you sure you want to delete this project?", function () {
        ajaxReq('Project/DeleteProjectDetailById', { id: system_id }, false, function (resp) {
            if (resp.status == "OK") {
                $("#frmViewProject").submit();
            }
            else
                alert(resp.message);
        }, true, true);
    });
}

function DeleteVendor(id) {

    confirm("Are you sure you want to delete this vendor?", function () {
        ajaxReq('Vendor/DeleteVendorDetailById', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                $("#frmViewVendor").submit();
            }
            else
                alert(resp.message);
        }, true, true);
    });
}


function DeleteVendorSpecification(id) {

    confirm("Are you sure you want to delete this vendor specification?", function () {
        ajaxReq('VendorSepcification/DeleteVendorSpecificationById', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                $("#frmViewVendorSpecification").submit();
                alert('Vendor specification deleted successfully.');
            }
            else
                alert(resp.message);
        }, true, true);
    });
}

function DeleteSpecificationService(id) {

    confirm("Are you sure you want to delete this specification service?", function () {
        ajaxReq('VendorSepcification/DeleteSpecificationServicebyId', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                $("#frmViewSpecificationService").submit();
                alert('Specification service deleted successfully.');
            }
            else
                alert(resp.message);
        }, true, true);
    });
}



function DeleteUserDetail(id) {

    confirm("Are you sure you want to delete this user?", function () {
        ajaxReq('User/DeleteUserById', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                $("#frmViewUsers").submit();
            }
            else
                alert(resp.message);
        }, true, true);
    });
}



function DeleteLayerGroups(group_id) {

    confirm("Are you sure you want to delete this group?", function () {
        ajaxReq('LayerSettings/DeleteLayerGroupsById', { group_id: group_id }, false, function (resp) {
            if (resp.status == "OK") {
                alert(resp.message);
                $("#frmViewLayerGroups").submit();
            }
            else
                alert(resp.message);
        }, true, true);
    });
}

function DeleteLinkBudgetDetail(id) {

    confirm("Are you sure you want to delete this record?", function () {
        ajaxReq('OpticalLinkBudget/DeleteLinkBudgetDetailById', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                $("#frmViewLinkBudgetDetail").submit();
            }
            else
                alert(resp.message);
        }, true, true);
    });
}



/*========================================================================== Start Project Specification==================================================*/

function OpenProjectModel(isNeworUpdate) {

    var network_stage = $("#ddl_Netowrkstage_Parent").val();

    var id = 0;
    if (network_stage == "") {
        alert("Please select network stage");
        return false;
    }

    if (isNeworUpdate == "edit") {
        id = $("#ddl_ProjectCode_Parent").val();
        if (id == "") {
            alert("Please select project code");
            return false;
        }

    }


    popup.LoadModalDialog('Project/AddProjectDetail', { system_id: id, network_stage: network_stage }, 'Add Project Detail', 'modal-lg');



}


function OpenPlanningModel(isNeworUpdate) {

    var id = 0;
    var project_code = $('#ddl_ProjectCode_Parent').find("option:selected").text();
    var network_stage = $("#ddl_Netowrkstage_Parent").val();
    var project_id = $('#ddl_ProjectCode_Parent').val();


    if (network_stage == "") {
        alert("Please select network stage");
        return false;
    }

    if ($('#ddl_ProjectCode_Parent').val() == "") {
        alert("Please select project code");
        return false;
    }

    if (isNeworUpdate == "edit") {

        id = $("#ddl_PlanningCode_Parent").val();

        if (id == "") {
            alert("Please select planning code");
            return false;
        }

    }


    popup.LoadModalDialog('Project/AddPlaningDetail', { system_id: id, network_stage: network_stage, project_code: project_code, project_id: project_id }, 'Add Planning Detail', 'modal-lg');

}


function OpenWorkorderModel(isNeworUpdate) {

    var id = 0;
    var planning_code = $('#ddl_PlanningCode_Parent').find("option:selected").text();
    var network_stage = $("#ddl_Netowrkstage_Parent").val();
    var planning_id = $('#ddl_PlanningCode_Parent').val();

    if (network_stage == "") {
        alert("Please select network stage");
        return false;
    }

    if (planning_id == "") {
        alert("Please select Planning code");
        return false;
    }


    if (isNeworUpdate == "edit") {

        id = $("#ddl_WorkorderCode_Parent").val();

        if (id == "") {
            alert("Please select workorder code");
            return false;
        }
    }
    popup.LoadModalDialog('Project/AddWorkorderDetail', { system_id: id, network_stage: network_stage, planning_code: planning_code, planning_id: planning_id }, 'Add Workorder Detail', 'modal-lg');


}

function OpenPurposeModel(isNeworUpdate) {

    var id = 0
    var workorder_code = $('#ddl_WorkorderCode_Parent').find("option:selected").text();
    var network_stage = $("#ddl_Netowrkstage_Parent").val();
    var workorder_id = $('#ddl_WorkorderCode_Parent').val();

    if (network_stage == "") {
        alert("Please select network stage");
        return false;
    }

    if (workorder_id == "") {
        alert("Please select Workorder code");
        return false;
    }

    if (isNeworUpdate == "edit") {

        id = $("#ddl_PurposeCode_Parent").val();

        if (id == "") {
            alert("Please select purpose code");
            return false;
        }
    }
    popup.LoadModalDialog('Project/AddPurposeDetail', { system_id: id, network_stage: network_stage, workorder_code: workorder_code, workorder_id: workorder_id }, 'Add Purpose Detail', 'modal-lg');

}




function DeleteProjectCode() {

    system_id = $("#ddl_ProjectCode_Parent").val();
    if (system_id == "") {
        alert("Please select project code");
        return false;
    }

    confirm("Are you sure you want to delete this project code detail?", function () {
        ajaxReq('Project/DeleteProjectCodeDetailById', { id: system_id }, false, function (resp) {
            if (resp.status == "OK") {
                onChangeNetworkStage();
            }
            else
                alert(resp.message);
        }, true, true);
    });
}



function DeletePlanningCode() {

    system_id = $("#ddl_PlanningCode_Parent").val();
    if (system_id == "") {
        alert("Please select planning code");
        return false;
    }

    confirm("Are you sure you want to delete this planning code detail?", function () {
        ajaxReq('Project/DeletePlanningCodeDetailById', { id: system_id }, false, function (resp) {
            if (resp.status == "OK") {
                onChangeProjectCode();
            }
            else
                alert(resp.message);
        }, true, true);
    });
}


function DeleteWorkorderCode() {

    system_id = $("#ddl_WorkorderCode_Parent").val();
    if (system_id == "") {
        alert("Please select workorder code");
        return false;
    }

    confirm("Are you sure you want to delete this workorder code detail?", function () {
        ajaxReq('Project/DeleteWorkorderCodeDetailById', { id: system_id }, false, function (resp) {
            if (resp.status == "OK") {
                onChangePlanningCode();
            }
            else
                alert(resp.message);
        }, true, true);
    });
}


function DeletePurposeCode() {

    system_id = $("#ddl_PurposeCode_Parent").val();
    if (system_id == "") {
        alert("Please select purpose code");
        return false;
    }

    confirm("Are you sure you want to delete this purpose code detail?", function () {
        ajaxReq('Project/DeletePurposeCodeDetailById', { id: system_id }, false, function (resp) {
            if (resp.status == "OK") {

                onChangeWorkorderCode();
            }
            else
                alert(resp.message);
        }, true, true);
    });
}


function onChangeNetworkStage() {
    allcleardropdownforProject();
    var network_stage = $('#ddl_Netowrkstage_Parent').val();
    ajaxReq('Project/BindProjectOnChange', { network_stage: network_stage }, false, function (resp) {
        if (resp.Data.length > 0) {

            $('#ddl_ProjectCode_Parent').empty();
            $('#ddl_ProjectCode_Parent').append($("<option></option>").val('').html('-Select-'));
            $.each(resp.Data, function (data, value) {
                $('#ddl_ProjectCode_Parent').append($("<option></option>").val(value.value).html(value.key));
            });
        }
        else {
            $('#ddl_ProjectCode_Parent').empty();
            $('#ddl_ProjectCode_Parent').append($("<option></option>").val('').html('-Select-'));
        }
    }, true, true);
    $('#ddl_ProjectCode_Parent').trigger("chosen:updated");
}

function onChangeProjectCode() {


     
    var network_stage = $('#ddl_Netowrkstage_Parent').val();
    var project_id = $('#ddl_ProjectCode_Parent').val();


    ajaxReq('Project/BindPlanningOnChange', { network_stage: network_stage, ddlproject_id: project_id }, false, function (resp) {
        if (resp.Data.length > 0) {

            $('#ddl_PlanningCode_Parent').empty();
            $('#ddl_PlanningCode_Parent').append($("<option></option>").val('').html('-Select-'));
            $.each(resp.Data, function (data, value) {
                $('#ddl_PlanningCode_Parent').append($("<option></option>").val(value.value).html(value.key));
            });


        }
        else {
            $('#ddl_PlanningCode_Parent').empty();
            $('#ddl_PlanningCode_Parent').append($("<option></option>").val('').html('-Select-'));
        }
    }, true, true);
    $('#ddl_PlanningCode_Parent').trigger("chosen:updated");
}

function onChangePlanningCode() {

    var network_stage = $('#ddl_Netowrkstage_Parent').val();
    var planning_id = $('#ddl_PlanningCode_Parent').val();


    ajaxReq('Project/BindWokorderOnChange', { network_stage: network_stage, ddlplanning_id: planning_id }, false, function (resp) {
        if (resp.Data.length > 0) {

            $('#ddl_WorkorderCode_Parent').empty();
            $('#ddl_WorkorderCode_Parent').append($("<option></option>").val('').html('-Select-'));
            $.each(resp.Data, function (data, value) {
                $('#ddl_WorkorderCode_Parent').append($("<option></option>").val(value.value).html(value.key));
            });


        }
        else {
            $('#ddl_WorkorderCode_Parent').empty();
            $('#ddl_WorkorderCode_Parent').append($("<option></option>").val('').html('-Select-'));
        }
    }, true, true);
    $('#ddl_WorkorderCode_Parent').trigger("chosen:updated");
}

function onChangeWorkorderCode() {

    var network_stage = $('#ddl_Netowrkstage_Parent').val();
    var workorder_id = $('#ddl_WorkorderCode_Parent').val();

    ajaxReq('Project/BindPurposeOnChange', { network_stage: network_stage, ddlworkorder_id: workorder_id }, false, function (resp) {
        if (resp.Data.length > 0) {

            $('#ddl_PurposeCode_Parent').empty();
            $('#ddl_PurposeCode_Parent').append($("<option></option>").val('').html('-Select-'));
            $.each(resp.Data, function (data, value) {
                $('#ddl_PurposeCode_Parent').append($("<option></option>").val(value.value).html(value.key));
            });


        }
        else {
            $('#ddl_PurposeCode_Parent').empty();
            $('#ddl_PurposeCode_Parent').append($("<option></option>").val('').html('-Select-'));
        }
    }, true, true);
    $('#ddl_PurposeCode_Parent').trigger("chosen:updated");
}


function allcleardropdownforProject() {

    $('#ddl_ProjectCode_Parent').empty();
    $('#ddl_PlanningCode_Parent').empty();
    $('#ddl_WorkorderCode_Parent').empty();
    $('#ddl_PurposeCode_Parent').empty();

    $('#ddl_ProjectCode_Parent').append($("<option></option>").val('').html('-Select-'));
    $('#ddl_PlanningCode_Parent').append($("<option></option>").val('').html('-Select-'));
    $('#ddl_WorkorderCode_Parent').append($("<option></option>").val('').html('-Select-'));
    $('#ddl_PurposeCode_Parent').append($("<option></option>").val('').html('-Select-'));

    $('#ddl_ProjectCode_Parent, #ddl_PlanningCode_Parent, #ddl_WorkorderCode_Parent,#ddl_PurposeCode_Parent ').trigger("chosen:updated");
}

function onChangeAttributeBind() {
    // 

    var layer_id = $('#ddllstLayers').val();

    ajaxReq('OSPSettings/BindAttributOnChange', { layer_id: layer_id }, false, function (resp) {

        if (resp.Data.length > 0) {

            $('#ddlAttributesDetails').empty();
            $('#ddlAttributesDetails').append($("<option></option>").val('').html('-Select-'));
            $.each(resp.Data, function (data, value) {
                var selected = value.is_selected == true ? "selected" : "";
                $('#ddlAttributesDetails').append($("<option data-selectedvalue=" + (value.is_selected) + " " + selected + " ></option>").val(value.column_name).html(value.display_name));

            });

        }
        else {
            $('#ddlAttributesDetails').empty();
            $('#ddlAttributesDetails').append($("<option></option>").val('').html('-Select-'));
        }
    }, true, true);
    $('#ddlAttributesDetails').trigger("chosen:updated");

}


/*========================================================================== End Project Specification==================================================*/

/*========================================================================== Start==================================================*/

function onChangebindEqiupmenttype() {

    var entity_id = $('#ddl_entity_type').val();
    ajaxReq('ConfigurationSettings/BindEqiupmenttypeOnChange', { entity_id: entity_id }, false, function (resp) {
        if (resp.Data.length > 0) {

            $('#ddl_equipment_type').empty();
            $('#ddl_equipment_type').append($("<option></option>").val('').html('-Select-'));
            $.each(resp.Data, function (data, value) {
                $('#ddl_equipment_type').append($("<option></option>").val(value.id).html(value.type));
            });


        }
        else {
            $('#ddl_equipment_type').empty();
            $('#ddl_equipment_type').append($("<option></option>").val('').html('-Select-'));
        }
    }, true, true);
    $('#ddl_equipment_type').trigger("chosen:updated");
}


function onChangebindBrandtype() {

    var equipmentType_id = $('#ddl_equipment_type').val();
    ajaxReq('ConfigurationSettings/BindBrandTypeOnChange', { equipmentType_id: equipmentType_id }, false, function (resp) {
        if (resp.Data.length > 0) {

            $('#ddl_brand_type').empty();
            $('#ddl_brand_type').append($("<option></option>").val('').html('-Select-'));
            $.each(resp.Data, function (data, value) {
                $('#ddl_brand_type').append($("<option></option>").val(value.id).html(value.brand));
            });


        }
        else {
            $('#ddl_brand_type').empty();
            $('#ddl_brand_type').append($("<option></option>").val('').html('-Select-'));
        }
    }, true, true);
    $('#ddl_brand_type').trigger("chosen:updated");
}


function onChangebindModeltype() {


    var BrandType_id = $('#ddl_brand_type').val();
    ajaxReq('ConfigurationSettings/BindModelTypeOnChange', { BrandType_id: BrandType_id }, false, function (resp) {
        if (resp.Data.length > 0) {

            $('#ddl_model_type').empty();
            $('#ddl_model_type').append($("<option></option>").val('').html('-Select-'));
            $.each(resp.Data, function (data, value) {
                $('#ddl_model_type').append($("<option></option>").val(value.id).html(value.model));
            });


        }
        else {
            $('#ddl_model_type').empty();
            $('#ddl_model_type').append($("<option></option>").val('').html('-Select-'));
        }
    }, true, true);
    $('#ddl_model_type').trigger("chosen:updated");
}

function OpenEquipmentTypeModel(isNeworUpdate) {

    var id = 0;
    var entity_type = $('#ddl_entity_type').find("option:selected").text();
    var entity_id = $('#ddl_entity_type').val();

    if (entity_id == "") {
        alert("Please select entity type");
        return false;
    }

    if (isNeworUpdate == "edit") {

        id = $("#ddl_equipment_type").val();

        if (id == "") {
            alert("Please select equipment type");
            return false;
        }

    }


    popup.LoadModalDialog('ConfigurationSettings/AddEquipmentTypeDetail', { id: id, entity_type: entity_type, entity_id: entity_id }, 'Add Equipment Type Detail', 'modal-lg');

}

function DeleteEquipmentTypeDetail() {

    id = $("#ddl_equipment_type").val();
    if (id == "") {
        alert("Please select equipment type");
        return false;
    }

    confirm("Are you sure you want to delete this equipment type detail?", function () {
        ajaxReq('ConfigurationSettings/DeleteEquipmentTypeDetailById', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                onChangebindEqiupmenttype();
            }
            else
                alert(resp.message);
        }, true, true);
    });
}

function OpenBrandTypeModel(isNeworUpdate) {

    var id = 0;
    var title = "Add Brand Type Detail";
    var equipment_type = $('#ddl_equipment_type').find("option:selected").text();
    var equipment_id = $('#ddl_equipment_type').val();

    if ($('#ddl_equipment_type').val() == "") {
        alert("Please select equipment type");
        return false;
    }

    if (isNeworUpdate == "edit") {
        title = "Update Brand Type Detail";
        id = $("#ddl_brand_type").val();

        if (id == "") {
            alert("Please select brand type");
            return false;
        }

    }


    popup.LoadModalDialog('ConfigurationSettings/AddBrandTypeDetail', { id: id, equipment_type: equipment_type, type_id: equipment_id }, title, 'modal-lg');

}
function DeleteBrandTypeDetail() {

    id = $("#ddl_brand_type").val();
    if (id == "") {
        alert("Please select brand type");
        return false;
    }

    confirm("Are you sure you want to delete this brand type detail?", function () {
        ajaxReq('ConfigurationSettings/DeleteBrandTypeDetailById', { id: id }, false, function (resp) {
            if (resp.status == "OK") {

                onChangebindBrandtype();
            }
            else
                alert(resp.message);
        }, true, true);
    });
}



function OpenModelTypeDetail(isNeworUpdate) {

    var id = 0;
    var title = "Add Model Type Detail";
    var brand_type = $('#ddl_brand_type').find("option:selected").text();
    var brand_id = $('#ddl_brand_type').val();

    if ($('#ddl_brand_type').val() == "") {
        alert("Please select brand type");
        return false;
    }

    if (isNeworUpdate == "edit") {
        title = "Update Model Type Detail";
        id = $("#ddl_model_type").val();

        if (id == "") {
            alert("Please select model type");
            return false;
        }

    }

    //var arr = [];
    //arr.push({ id: id, brand_type: brand_type, brand_id: brand_id });

    var modalValues = { id: id, brand_type: brand_type, brand_id: brand_id };
    popup.LoadModalDialog('ConfigurationSettings/AddModelTypeDetail', { modelTypeMaster: modalValues }, title, 'modal-lg');

}

function DeleteModelTypeDetailById() {

    id = $("#ddl_model_type").val();
    if (id == "") {
        alert("Please select model type");
        return false;
    }

    confirm("Are you sure you want to delete this model type detail?", function () {
        ajaxReq('ConfigurationSettings/DeleteModelTypeDetailById', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                onChangebindModeltype();
            }
            else
                alert(resp.message);
        }, true, true);
    });
}



/*== start InputPort Configuration Settings===*/

//var Totalcount = 0;

$(document).on('click', app.DE.AddInputPortLstRow, function (e) {
     
    var addRowInInputTbl = $(app.DE.InputPortInfoTable + ' tbody');
    var rowCount = $(app.DE.InputPortInfoTable + ' tbody tr').length;
    if (rowCount == 0) {
        $('#tblInputPortLstInfo').show();
    }
    if (rowCount < 10) {
        var inputVal = rowCount + 1;
        $("#hdn_inputport").val(inputVal);
        $("#spTotalInputPort").text(inputVal);

        var Totalcount = $("#tblOutputPortLstInfo tbody tr").length + $("#tblInputPortLstInfo tbody tr").length + 1;

        var inputRowData = '<tr>';
        inputRowData += '<td> <input style=" width:90%;" maxlength="20" id="lstPortInfo_' + rowCount + '__port_name" name="lstPortInfo[' + rowCount + '].port_name" type="text" value="InputPort_' + rowCount + '" class="valid"></td>';
        inputRowData += '<td><select class="shaft-tbl-position-ddl valid" id="lstPortInfo_' + rowCount + '__port_type" name="lstPortInfo[' + rowCount + '].port_type" style="padding:0;">';
        inputRowData += '<option value="GEO">GEO</option><option value="PON">PON</option></select></td>';
        inputRowData += '<td><input id="port_type_' + rowCount + '_Info" name="lstPortInfo[' + rowCount + '].port_type" type="hidden" value="right">';
        inputRowData += '<input data-val="true" data-val-number="The field id must be a number." data-val-required="The id field is required." id="lstPortInfo_' + rowCount + '__id" name="lstPortInfo[' + rowCount + '].id" type="hidden" value="0">';
        inputRowData += '<input data-val="true" id="lstPortInfo_' + rowCount + '__port_no" name="lstPortInfo[' + rowCount + '].port_no" type="hidden" value="' + Totalcount + '">';
        inputRowData += '<span class="InputOutputPortRowDelete" title="Delete" id="inputPortRowDelete" >x</span></td></tr>';

        addRowInInputTbl.append(inputRowData);

    }
    else {
        alert('Only 10 row can be added !!');
    }
});

$(document).on("click", app.DE.DeleteInputPortRow, function () {

    app.inputoutputLstInfoD = {};
    var row = $(this).closest("tr");
    app.inputoutputLstInfoD.selectedRow = row;
    app.inputoutputLstInfoD.selectedId = $('#lstPortInfo_' + row.index() + '__id').val();


    //var func = function () {
    if (app.inputoutputLstInfoD.selectedId == 0) {
        app.removeInputPortRow();
    }
    else {

        confirm("Do you want to delete this record?", function () {

            ajaxReq('ConfigurationSettings/DeleteInputOutputPortById', { id: app.inputoutputLstInfoD.selectedId }, true, function (resp) {

                if (resp.status == "OK") {

                    app.removeInputPortRow();

                }
            }, true, true);

        });
    }

    //};
    //confirm("Do you want to delete this row?", func);
});

this.removeInputPortRow = function () {

    var row = app.inputoutputLstInfoD.selectedRow;
    var removeRowindex = row.index();
    row.remove();
    var inputportTblbody = $(app.DE.InputPortInfoTable + ' tbody tr');
    var rowCount = inputportTblbody.length;
    var inputVal = rowCount;
    $("#hdn_inputport").val(inputVal);
    $("#spTotalInputPort").text(inputVal);



    inputportTblbody.each(function (i, val) {
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


/*== end InputPort Configuration Settings===*/

/*== start OuputPort Configuration Settings===*/
$(document).on('click', app.DE.AddOutputPortLstRow, function (e) {
     

    var addRowInOutputTbl = $(app.DE.OutputPortInfoTable + ' tbody');
    var rowCount = $(app.DE.OutputPortInfoTable + ' tbody tr').length;
    if (rowCount == 0) {
        $('#tblOutputPortLstInfo').show();
    }
    if (rowCount < 10) {
        var outputVal = rowCount + 1;
        $("#hdn_outputport").val(outputVal);
        $("#spTotalOutputPort").text(outputVal);


        //var inputvalue = $("#hdn_inputport").val();
        //var outputcount = 0;
        //if (inputvalue > 0)
        //{

        //    outputcount = inputvalue;
        //    outputcount = outputcount + 1;
        //}
        //else
        //{
        //    outputcount = rowCount + 1;
        //}
        var Totalcount = $("#tblOutputPortLstInfo tbody tr").length + $("#tblInputPortLstInfo tbody tr").length + 1;

        var outputRowData = '<tr>';
        outputRowData += '<td><input style=" width:90%;" maxlength="20" id="lstOutputPortInfo_' + rowCount + '__port_name" name="lstOutputPortInfo[' + rowCount + '].port_name" type="text" value="OutputPort_' + rowCount + '" class="valid"></td>';
        outputRowData += '<td><select class="shaft-tbl-position-ddl valid" id="lstOutputPortInfo_' + rowCount + '__port_type" name="lstOutputPortInfo[' + rowCount + '].port_type" style="padding:0;">';
        outputRowData += '<option value="GEO">GEO</option><option value="PON">PON</option></select></td>';
        outputRowData += '<td><input id="port_type_' + rowCount + '_Info" name="lstOutputPortInfo[' + rowCount + '].port_type" type="hidden" value="right">';
        outputRowData += '<input data-val="true" data-val-number="The field id must be a number." data-val-required="The id field is required." id="lstOutputPortInfo_' + rowCount + '__id" name="lstOutputPortInfo[' + rowCount + '].id" type="hidden" value="0">';
        outputRowData += '<input data-val="true" id="lstOutputPortInfo_' + rowCount + '__port_no" name="lstOutputPortInfo[' + rowCount + '].port_no" type="hidden" value="' + Totalcount + '">';
        outputRowData += '<span class="InputOutputPortRowDelete" title="Delete" id="outputPortRowDelete" >x</span></td></tr>';

        //shaftRowData += '<input type="button" value="Update" onclick="si.actionOnShaftRow(this,"edit")"><input type="button" value="Edit" onclick="si.actionOnShaftRow(this,"update")" style="display:none;"><input type="button" value="Delete" onclick="si.actionOnShaftRow(this,"delete")"></td></tr>';
        addRowInOutputTbl.append(outputRowData);
        var chk = $('#lstOutputPortInfo');
    }
    else {
        alert('Only 10 row can be added !!');
    }
});


function checkUserLimit() {
    var app_access = $("#ddlApplication").val();
    if (app_access != null || app_access != "") {
        ajaxReq('User/CheckUserLimit', { user_id: $("#user_id").val(), app_access: app_access }, false, function (resp) {

            if (resp.status == false) {
                var limitMessage = "<span style='color: red;'><br/>" + resp.message + " LIMIT: " + $("#" + resp.message.toLowerCase() + "Max").val() + "</span>";
                if (resp.message.toLowerCase() == 'both') {
                    limitMessage = "<span style='color: red;'><br/>User Limits(Web: " + $("#webMax").val() + ", Mobile: " + $("#mobileMax").val() + ")</span>";
                }

                alert("The limit to create more " + resp.message.toLowerCase() + " users has been exceeded." + limitMessage + " ")
                $("#ddlApplication").val("").trigger("chosen:updated");;
                return false;
            }
        }, true, true);
    }

}


this.displayApplicationModule = function () {
    // 
    if ($("#ddlApplication :selected").text() == 'Mobile') {
        $(".Mobile-section").show();
        $(".Web-section").hide();
        $(".mobile-unit").css("padding", "");
        // $("#divMobileModule").css("margin-left", "-44px");
        $(".all-section").hide();
    } else if ($("#ddlApplication :selected").text() == 'Web') {
        $(".Mobile-section").hide();
        $(".Admin-section").hide();
        $(".Web-section").show();
        $(".all-section").hide();
    } else {
        $(".Mobile-section").show();
        $(".Web-section").show();
        //(".mobile-unit").css("padding", "0 0 0 48px");
        $("#divMobileModule").css("margin-left", "0px");
        $(".all-section").show();
    }
    isAdminChecked();
}
function isAdminChecked() {
    // 
    if ($('#isadminrghts').is(":checked")) {
        $(".Admin-section").show();
        $(".all-section").show();
        // $("#module-data").css("margin-left", "-52px");
        // $("#divMobileModule").css("margin-left", "-40px");
    } else {
        $(".Admin-section").hide();
    }
}
function onChangeApllicationAccess() {
    if ($('#ddlApplication').val() == "BOTH" || $('#ddlApplication').val() == "WEB") {
        $('#isadminrghts').prop("disabled", "");
    }
    else {
        $('#isadminrghts').prop("checked", "");
        $('#isadminrghts').prop("disabled", "disabled");
    }
}
$(document).on("click", app.DE.DeleteOutputPortRow, function () {


    app.inputoutputLstInfoD = {};
    var row = $(this).closest("tr");
    app.inputoutputLstInfoD.selectedRow = row;
    app.inputoutputLstInfoD.selectedId = $('#lstOutputPortInfo_' + row.index() + '__id').val();


    //var func = function () {
    if (app.inputoutputLstInfoD.selectedId == 0) {
        app.removeOutputPortRow();
    }
    else {
        confirm("Do you want to delete this record?", function () {

            ajaxReq('ConfigurationSettings/DeleteInputOutputPortById', { id: app.inputoutputLstInfoD.selectedId }, true, function (resp) {

                if (resp.status == "OK") {

                    app.removeOutputPortRow();

                }
            }, true, true);

        });


    }

});
function fnReportingRoleManagerListByRoleId (role_id) {
    popup.LoadModalDialog('Roles/ViewReportingRoleManagerListByRoleId', { role_id: role_id}, 'Reporting Role List', 'modal-l');
};

this.removeOutputPortRow = function () {

    var row = app.inputoutputLstInfoD.selectedRow;
    var removeRowindex = row.index();
    row.remove();
    var outputportTblbody = $(app.DE.OutputPortInfoTable + ' tbody tr');
    var rowCount = outputportTblbody.length;
    var outputVal = rowCount;
    $("#hdn_outputport").val(outputVal);
    $("#spTotalOutputPort").text(outputVal);



    outputportTblbody.each(function (i, val) {
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

this.SignOut = function () {

    showConfirm('Are you sure you want to Sign Out?', function () {
        window.location = baseUrl + '/login/logout';
    });
}




function onChangeUserRole() {
     
    $('#ddl_UserReportingManager').empty();
    $('#ddl_WarehouseCode').empty();
    $('#multi_warhouse_code').val("");
    //$('#divReportingManager').hide();
    var userRole = $("#ddl_UserRole option:selected").text();
    if (userRole === "WFM Contractor" || userRole === "Contractor Dispatcher" || userRole === "In House Dispatcher" || userRole === "MSP Dispatcher" || userRole === "COFG Dispatcher") {
       // $('#warehouse_code').val("");

    }
    else if (userRole === "FE" || userRole === "Contractor FE" || userRole === "In House FE" || userRole === "MSP FE" || userRole === "COFG FE") {
        $('#company_name').val("");
        $('#capacity').val("");
    }
    else {
       // $('#warehouse_code').val("");
        $('#company_name').val("");
        $('#capacity').val("");
    }


    var User_Role = $('#ddl_UserRole').val();
    ajaxReq('User/BindReportingManagerOnChange', { RoleId: User_Role }, false, function (resp) {
         
        if (resp.Data.length > 0) {
           // $('#ddl_UserReportingManager').append($("<option></option>").val('').html('-Select-'));
            $('#divReportingManager').show();

            $.each(resp.Data, function (data, value) {
                $('#ddl_UserReportingManager').append($("<option></option>").val(value.value).html(value.key));
            });
        }
        else {
            $('#ddl_UserReportingManager').empty();
           // $('#ddl_UserReportingManager').append($("<option></option>").val('').html('-Select-'));
            $('#divReportingManager').show();
        }
        $('#ddl_UserReportingManager').trigger("chosen:updated");
    }, true, true);

    ajaxReq('User/BindWarehouseCodeOnChange', {  }, false, function (resp) {
         
        if (resp.Data.length > 0) {
            $('#ddl_WarehouseCode').append($("<option></option>").val('').html('-Select-'));
          //  $('#divReportingManager').show();

            $.each(resp.Data, function (data, value) {
                $('#ddl_WarehouseCode').append($("<option></option>").val(value.value).html(value.key));
            });
        }
        else {
            $('#ddl_WarehouseCode').empty();
            $('#ddl_WarehouseCode').append($("<option></option>").val('').html('-Select-'));
         //   $('#divReportingManager').show();
        }
        $('#ddl_WarehouseCode').trigger("chosen:updated");
    }, true, true);

}
function updateModuleList() {
     
    if ($("#ddl_UserRole").val() != 0 || $("#ddl_UserRole").val() != "") {
        console.log("drp changed");
        ajaxReq('Roles/GetUserRoleModuleMapping', { role_id: $("#ddl_UserRole").val(), user_id: $("#user_id").val() }, true, function (resp) {
            $('#partionTable').html(resp);

            //$('#ServiceFacilityJobOrder').html("");
            ////if ($("#ddl_UserRole").val() === "142")
            console.log($("#ddl_UserRole option:selected").text());
            //if ($("#ddl_UserRole option:selected").text() === "WFM Contractor") {
            // ajaxReq('Roles/GetUserServiceFacilityJobOrder', { role_id: $("#ddl_UserRole").val(), user_id: $("#user_id").val()}, true, function (resp) {
            //    $('#ServiceFacilityJobOrder').html(resp);
            //setAllCheckBox();
            //});
         
            BindUserServiceFacilityJoOrderTypeforContractorRole($("#ddl_UserRole option:selected").text(), $("#ddl_UserRole").val(), $("#user_id").val());
            //}

            $(".mainModule").not(".grpNetwork").treeview({
                collapsed: false,
                animated: "medium",
                persist: "location"
            });
            $("div").removeClass("expandable-hitarea");
        });

    }

}


function onChangeRMId() {
    var ddl= $('#ddl_UserReportingManager');
    if ($("#ddl_UserRole").val() != 0 || $("#ddl_UserRole").val() != "") {
        
        var reportingManagerId = $("#ddl_UserReportingManager").val();
        if ($("#ddl_UserReportingManager").val() != '' && $("#ddl_UserReportingManager").val()!=null) {

            if ($("#ismutimanagar").val() == "True") {
                $('#multi_manager_ids').val($("#ddl_UserReportingManager").val().join(","));
                reportingManagerId = $("#ddl_UserReportingManager").val().join(",")
            }
            else {
                $('#multi_manager_ids').val($("#ddl_UserReportingManager").val());
            }

        }
        else {
           
           reportingManagerId = 0
        }

        BindUserServiceFacilityJoOrderTypeforFERole($("#ddl_UserRole option:selected").text(), reportingManagerId, $("#user_id").val());
    }

}

function onChangeToolId() {
    var ddl = $('#ddl_fetool').val();
    if ($("#ddl_fetool").val() != 0 || $("#ddl_fetool").val() != "")
    {
        var toolId = $("#ddl_fetool").val();
        if ($("#ddl_fetool").val() != '' && $("#ddl_fetool").val() != null) {

            $('#multi_tool_ids').val($("#ddl_fetool").val().join(","));
            toolId = $("#ddl_fetool").val().join(",")
        }
        else
        {

            toolId = 0
        }
    }

}
function onChangeWarehouseCode() {
     
     var wareHouseCodes = $("#ddl_WarehouseCode").val();
        if ($("#ddl_WarehouseCode").val()) {
           $('#multi_warhouse_code').val($("#ddl_WarehouseCode").val().join(","));
                // reportingManagerId=reportingManagerId[0];
            

            

        }
        


     

}


function BindUserServiceFacilityJoOrderTypeforContractorRole(role_name, role_id, user_id) {
    
    $("#divwarehousecode").hide();
    $("#divcompanyname").hide();
    $("#divcapacity").hide();
    if (role_name === "FE" || role_name === "Contractor FE" || role_name === "In House FE" || role_name === "MSP FE" || role_name === "COFG FE" || role_name === "In House Full Vehicle Team" || role_name === "In House FRT") {
        $("#divwarehousecode").show();
    }
    if (role_name === "In House FRT" || role_name === "MSP FRT" || role_name === "In House Full Vehicle Team" || role_name === "MSP Full Vehicle Team") {
        $("#divcapacity").show();
    }
    if (role_name === "WFM Contractor" || role_name === "Contractor Dispatcher" || role_name === "In House Dispatcher" || role_name === "MSP Dispatcher" || role_name === "COFG Dispatcher") {
        $("#divcompanyname").show();
        $("#divcapacity").show();
        ajaxReq('Roles/GetUserServiceFacilityJobOrder', { role_id: role_id, user_id: user_id }, true, function (resp) {
            $('#ServiceFacilityJobOrder').html(resp);
        });
    }
    else {
        $('#ServiceFacilityJobOrder').html("");
    }

}


function BindUserServiceFacilityJoOrderTypeforFERole(role_name, rm_id, user_id) {
    if (role_name === "FE" || role_name === "Contractor FE" || role_name === "In House FE" || role_name === "MSP FE" || role_name === "COFG FE") {
        ajaxReq('Roles/GetUserServiceFacilityJobOrderForFE', { rm_id: rm_id, user_id: user_id }, true, function (resp) {
            $('#ServiceFacilityJobOrder').html("");
            $('#ServiceFacilityJobOrder').html(resp);
        });
    }
}


function ViewRolePermissionPopup(e) {
    var userRole = $('#ddl_UserRole').val();
    if (userRole == '') { alert('Please select user role!'); } else {
        popup.LoadModalDialog('Roles/LoadRoleTemplatePermission', { role_id: userRole, isDisabled: true }, 'View Role Permission', 'modal-xxl');
    }
}

/*== end OuputPort Configuration Settings===*/



/*========================================================================== End Configuration Settings==================================================*/

/*========================================================================== End OSP Settings ==================================================*/



/*========================================================================== Utilization Settings==================================================*/

function BindProvinceByRegion() {
    var selectedRegionId = $('#ddlRegions').val();
    if (selectedRegionId != undefined && selectedRegionId > 0) {
        ajaxReq('Miscellaneous/BindProviceByRegionId', { regionIds: selectedRegionId }, false, function (resp) {
            if (resp.Data.length > 0) {
                $('#ddlProvinces').empty();
                $('#ddlProvinces').append($("<option></option>").val('').html('-Select-'));
                $.each(resp.Data, function (data, value) {
                    $('#ddlProvinces').append($("<option></option>").val(value.provinceId).html(value.provinceName));
                });
            }
            else {
                $('#ddlProvinces').empty();
                $('#ddlProvinces').append($("<option></option>").val('').html('-Select-'));
            }
        }, true, true);
    }
    $('#ddlProvinces').trigger("chosen:updated");
}

function validateDropdown(elementId) {
    if ($('#' + elementId).length >= 0) {
        var value = $('#' + elementId).val();

        if (value == null || value == "") {
            $('#' + elementId).next().addClass('input-validation-error').removeClass('valid');
            return false;
        }
        else {
            $('#' + elementId).next().addClass('valid').removeClass('input-validation-error');
            return true;
        }
    }
    return true;
}

function DeleteUtilizationSettingsById(id) {

    confirm("Are you sure you want to delete this record?", function () {
        ajaxReq('Miscellaneous/DeleteUtilizationSettingsById', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                $("#frmViewUtilizationSetting").submit();
            }
            else
                alert(resp.message);
        }, true, true);
    });
}


function AddNewUtlization(_system_id) {
    
    if (_system_id == 0) {
        $('#OtherEntities').show();
        popup.LoadModalDialog('Miscellaneous/AddEntityUtilization', { system_id: _system_id }, 'Add New Utilization', 'modal-md');
    }
    else {
        $('#OtherEntities').hide();
        popup.LoadModalDialog('Miscellaneous/AddEntityUtilization', { system_id: _system_id }, 'Update Utilization', 'modal-md');
    }
    
}


function AddNewDropdownMaster(id) {

    popup.LoadModalDialog('Miscellaneous/AddEntityDropdownmaster', { id: id }, 'Add New Dropdown Master', 'modal-md');
    if ($('#id').val() != 0) {
        $('#ddlLayers').attr("disabled", "disabled");
        $('#ddlfieldNameDropDown').attr("disabled", "disabled");
    }


}




/*========================================================================== End Utilization Settings==================================================*/
/*========================================================================== Cable Color Settings==================================================*/
function AddNewCableColour(_id) {
    
    popup.LoadModalDialog('OSPSettings/CableMapColorSettings', { id: _id }, 'Add New Cable Map Color Setting', 'modal-md');
}

function DeleteCableColourSettingsById(id) {

    confirm("Are you sure you want to delete this record?", function () {
        ajaxReq('OSPSettings/DeleteCableMapColourSettingsById', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                $("#frmViewCableColorSettings").submit();
            }
            else
                alert(resp.message);
        }, true, true);
    });
}
/*========================================================================== End Cable Color Settings==================================================*/

/*========================================================================== Connection Label Settings==================================================*/
function EditNewConnectionLabel(id, entityTitle) {
    popup.LoadModalDialog('AdvancedSettings/EditConnectionLabelSettings', { id: id, entityTitle: entityTitle }, 'Edit Connection Label Setting', 'modal-md');
}

function SyncAllLabels(id) {
    confirm("Synchronization process is time taking. Are you sure you want to continue?", function () {
        $("#dvProgress").show();
        $("#dvProgress").css('display', 'block');
        ajaxReq('AdvancedSettings/SyncAllLabels', { id: id }, true, function (resp) {
            if (resp.status == "OK") {
                $("#dvProgress").hide();
                $("#dvProgress").css('display', 'none');
                alert(resp.message, 'Success', 'success');
                window.location.reload();
            }
            else
                alert(resp.message);
        }, '', true);
        //alert('Synchronization process has been started. Since the process will take time you can continue with your other work.');
    });
}
/*========================================================================== END Connection Label Settings==================================================*/



$(document).on("click", app.DE.AddNewCoreLstRow, function () {
    if ($('#ddlType').val() == '') {
        alert('Please select Type!');
        return false;
    }
    var addRowInFloorTbl = $(app.DE.tblCableInfoTable + ' tbody');
    var rowCount = $(app.DE.tblCableInfoTable + ' tbody tr').length;
    if (rowCount > 0) {
        $('#TubeColorRowAdd').show();
    }
    if (rowCount <= 30) {
        var coreVal = rowCount + 1;
        $("#total_core").val(coreVal);
        //  $("#spTotalFloor").text(coreVal);

        var TubeColorRowData = '<tr IsNewRecord="1"><td><p style="color:black;" value="0">' + coreVal + '</p></td>';

        //var TubeColorRowData = '<tr IsNewRecord="1"><td><input style=" width:20%;" maxlength="5" id="lstCableColor_' + rowCount + '__color_id" name="lstCableColor[' + rowCount + '].color_id" type="text" value="" ></td>';
        TubeColorRowData += '<td> <input onkeyup="this.value = this.value.toUpperCase();" ColorCharid="colorChar' + coreVal + '" class = "color_character newRow" style=" width:30%;text-transform: uppercase;" maxlength="5" id="lstCableColor_' + rowCount + '__color_character" name="lstCableColor[' + rowCount + '].color_character"  type="text" value=""  ></td>';

        //TubeColorRowData += '<td><input style=" width:30%;" data-val="true" data-val-required="The color_name field is required." id="lstCableColor_' + rowCount + '__color_name" name="lstCableColor[' + rowCount + '].color_name" type="text" value="">';
        TubeColorRowData += '<td><span><input class="full col-md-10" value="#1c4587" data-val="true" data-val-required="The color_code field is required." id="lstCableColor_' + rowCount + '__color_code" name="lstCableColor[' + rowCount + '].color_code" type="text" value=""></td>';
        // TubeColorRowData += '<td><span><input style="width: 100%; display: none;" class="full col-md-10" id="lstCableColor_' + rowCount + '__color_code" maxlength="25" name="lstCableColor[' + rowCount + '].color_code" type="text" value="#1c4587"><div class="sp-replacer sp-light"><div class="sp-preview"><div class="sp-preview-inner" style="background-color: rgb(28, 69, 135);"></div></div><i class="fa fa-paint-brush"></i></div></span></td>';

        TubeColorRowData += '<td><span class="TubeColorRowDelete" title="Delete" ><i class="fa fa-fw fa-trash" style=" font-size: 13px; color: #f50000;"></i></span></td></tr>';

        addRowInFloorTbl.append(TubeColorRowData);
        var chk = $('#lstCableColor');
        //$(app.DE.ShaftNFloorInut).unbind('keypress').keypress(function () { $(this).removeClass('NotValid'); });
        CallRunTimeCssForColorPicker();
        alert('One row added successfully.');
    }
    else {
        alert('Only 30 row can be added!');
    }
    if ($(app.DE.tblCableInfoTable + ' tbody tr').length > 0) {
        $(app.DE.tblCableInfoTable).show();
        $(app.DE.divCableTubeColor).show();
    }
    // notAllowZero('.FloorUnit');
});


$(document).on("click", "#tblCableTubeColor .TubeColorRowDelete", function () {

    var This = $(this);
    app.cableColorLstInfoD = {};
    var row = $(this).closest("tr");
    var CheckIsNewRecord = $(this).closest("tr").attr('IsNewRecord');
    app.cableColorLstInfoD.selectedRow = row;
    app.cableColorLstInfoD.selectedId = $('#lstCableColor_' + row.index() + '__color_id').val();
    var rowCount = $(app.DE.tblCableInfoTable + ' tbody tr').length;
    ajaxReq('OSPSettings/getTotalColorCount', { type: $('#ddlType option:selected').val() }, true, function (resp) {
        if (resp == 1 && $('#tblCableTubeColor .color_character:not(.newRow)').length == 1 && !(CheckIsNewRecord == 1)) { alert('Minimum One Core/color is required!'); return false; }
        var func = function () {
            if (CheckIsNewRecord == 1) {
                $(This).closest("tr").remove();
                var floorTblbody = $(app.DE.tblCableInfoTable + ' tbody tr');
                floorTblbody.each(function (i, val) {
                    if (row.index() <= i) {
                        $(this).find("input,select").each(function (n, nval) {

                            if ($(this).attr('name') != undefined)
                                $(this).attr('name', $(this).attr('name').replace(/\[\d+\]/, "[" + (i) + "]"));

                            if ($(this).attr('id') != undefined)
                                $(this).attr('id', $(this).attr('id').replace(/\_\d+\_/, "_" + (i) + "_"));

                        });
                    }
                });
            }
            else {
                ajaxReq('OSPSettings/DeleteCableById', { colorid: app.cableColorLstInfoD.selectedId, type: $('#ddlType option:selected').val() }, true, function (resp) {

                    if (resp.status == "OK") {
                        // app.removeCableColorRow();
                        window.location = "/Admin/OSPSettings/TubeCoreColorSettings?type=" + $('#ddlType option:selected').val();
                    }
                    else {
                        alert(resp.message);
                        return false;
                    }
                }, true, true);

                //$($(row).find('input')[1]).attr('name')
            }

        };
        confirm("Do you want to delete this row?", func);

    }, true, true);
});

//function createrole() {
//    var checkedlyrs = [];
//    $('#mytable').find('input[type="checkbox"]:checked').each(function () {
//        checkedlyrs.push($(this).attr('data-layername'));
//    });
//    var postData = { values:  JSON.stringify(checkedlyrs) };
//    ajaxReq('Roles/SaveRole',postData, false, function (resp) {
//        if (resp.Data.length > 0) {

//        }
//        else {

//        }
//    }, true, true);
//}

function createrole() {
    var checkedlyrs = [];
    var _roleid = 1;
    $('#mytable').find('input[type="checkbox"]:checked').each(function () {
        var postData = $(this).attr('data-layername');
        var arr = postData.split('/');

        checkedlyrs.push({ "LayerID": arr[0], "Action": arr[1], "Network": arr[2], "Type": arr[3] });
        //for (var i = 0; i < arr.length; i++) {
        //    if (i == 0)
        //    {
        //        checkedlyrs.push({ "Action": arr[i] });
        //    }
        //    else if(i==1)
        //    {
        //        checkedlyrs.push({ "Network": arr[i] });
        //    }
        //    else
        //    {
        //        checkedlyrs.push({ "Type": arr[i] });
        //    }           
        //}
        //checkedlyrs.push(arr);
    });

    var postData = { values: JSON.stringify(checkedlyrs) };
    ajaxReq('Roles/SaveRole', postData, false, function (resp) {
        if (resp.Data.length > 0) {

        }
        else {

        }
    }, true, true);
}

//textbox can shown only numeric datatype
this.allowOnlyNumber = function (evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode
    if (charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}
function GenerateResourceList() {

    popup.LoadModalDialog('Resources/AddnewResourceLang', {}, 'Add New Language', 'modal-md');
}

function AddNewResourceKey() {
    popup.LoadModalDialog('Resources/AddNewResourceKey', {}, 'Add New Resource Key', 'modal-md');
}
function ViewSplitterLosses(id) {
    popup.LoadModalDialog('OpticalLinkBudget/getSplitterLossDetails', { wavelength_id: id }, 'Splitter Loss Detail', 'modal-md');
}

$(document).on('blur', '.color_character', function () {
    $(this).css({ "color": "", "border": "" }); // this line remove style for blank record

    var curColorCharId = $(this).attr('colorcharid');
    var curColorCharValue = $(this).val();
    $('#tblCableTubeColor tbody tr').each(function () {
        var colorCharId = $(this).children().find('.color_character').attr('ColorCharid');
        var ColorCharValue = $(this).children().find('.color_character').val();
        if (curColorCharId != colorCharId && ColorCharValue != "") {
            if ($.trim(ColorCharValue).toUpperCase() == $.trim(curColorCharValue).toUpperCase()) {
                alert("Color Character already exist!");

                return false;
            }
        }

    });
});

//this.removeCableColorRow = function () {

//    var row = app.cableColorLstInfoD.selectedRow;
//    var removeRowindex = row.index();
//    row.remove();
//    var TubeTblbody = $(app.DE.tblCableTubeColor + ' tbody tr');
//    var rowCount = TubeTblbody.length;
//    var floorVal = rowCount;
//    $("#total_core").val(floorVal);
//    // $("#spTotalFloor").text(floorVal);



//    TubeTblbody.each(function (i, val) {
//        if (removeRowindex <= i) {
//            $(this).find("input,select").each(function (n, nval) {

//                if ($(this).attr('name') != undefined)
//                    $(this).attr('name', $(this).attr('name').replace(/\[\d+\]/, "[" + (i) + "]"));

//                if ($(this).attr('id') != undefined)
//                    $(this).attr('id', $(this).attr('id').replace(/\_\d+\_/, "_" + (i) + "_"));

//            });
//        }
//    });
//}


function CallRunTimeCssForColorPicker() {
    $(".full").spectrum({
        allowEmpty: true,
        color: "#ECC",
        showInput: true,
        containerClassName: "full-spectrum",
        //showInitial: true,
        showPalette: true,
        showSelectionPalette: true,
        showAlpha: true,
        maxPaletteSize: 10,
        preferredFormat: "hex",
        localStorageKey: "spectrum.demo",
        move: function (color) {
            updateBorders(color);
        },
        show: function () {

        },
        beforeShow: function () {

        },
        hide: function (color) {
            updateBorders(color);
        },

        palette: [
            ["rgb(0, 0, 0)", "rgb(67, 67, 67)", "rgb(102, 102, 102)", /*"rgb(153, 153, 153)","rgb(183, 183, 183)",*/
                "rgb(204, 204, 204)", "rgb(217, 217, 217)", /*"rgb(239, 239, 239)", "rgb(243, 243, 243)",*/ "rgb(255, 255, 255)"],
            ["rgb(152, 0, 0)", "rgb(255, 0, 0)", "rgb(255, 153, 0)", "rgb(255, 255, 0)", "rgb(0, 255, 0)",
                "rgb(0, 255, 255)", "rgb(74, 134, 232)", "rgb(0, 0, 255)", "rgb(153, 0, 255)", "rgb(255, 0, 255)"],
            ["rgb(230, 184, 175)", "rgb(244, 204, 204)", "rgb(252, 229, 205)", "rgb(255, 242, 204)", "rgb(217, 234, 211)",
                "rgb(208, 224, 227)", "rgb(201, 218, 248)", "rgb(207, 226, 243)", "rgb(217, 210, 233)", "rgb(234, 209, 220)",
                "rgb(221, 126, 107)", "rgb(234, 153, 153)", "rgb(249, 203, 156)", "rgb(255, 229, 153)", "rgb(182, 215, 168)",
                "rgb(162, 196, 201)", "rgb(164, 194, 244)", "rgb(159, 197, 232)", "rgb(180, 167, 214)", "rgb(213, 166, 189)",
                "rgb(204, 65, 37)", "rgb(224, 102, 102)", "rgb(246, 178, 107)", "rgb(255, 217, 102)", "rgb(147, 196, 125)",
                "rgb(118, 165, 175)", "rgb(109, 158, 235)", "rgb(111, 168, 220)", "rgb(142, 124, 195)", "rgb(194, 123, 160)",
                "rgb(166, 28, 0)", "rgb(204, 0, 0)", "rgb(230, 145, 56)", "rgb(241, 194, 50)", "rgb(106, 168, 79)",
                "rgb(69, 129, 142)", "rgb(60, 120, 216)", "rgb(61, 133, 198)", "rgb(103, 78, 167)", "rgb(166, 77, 121)",
                /*"rgb(133, 32, 12)", "rgb(153, 0, 0)", "rgb(180, 95, 6)", "rgb(191, 144, 0)", "rgb(56, 118, 29)",
                "rgb(19, 79, 92)", "rgb(17, 85, 204)", "rgb(11, 83, 148)", "rgb(53, 28, 117)", "rgb(116, 27, 71)",*/
                "rgb(91, 15, 0)", "rgb(102, 0, 0)", "rgb(120, 63, 4)", "rgb(127, 96, 0)", "rgb(39, 78, 19)",
                "rgb(12, 52, 61)", "rgb(28, 69, 135)", "rgb(7, 55, 99)", "rgb(32, 18, 77)", "rgb(76, 17, 48)"]
        ]
    });
    $('#tblCableTubeColor tbody tr').each(function () {
        var colorCharId = $(this).children().find('.full').val();
        $(this).children().find('.sp-preview-inner').css({ "background-color": colorCharId });
    });

};
function fnChangeType() {
    var _ddlType = $('#ddlType option:selected').val();
    if (_ddlType != undefined && _ddlType != '') {
        window.open(appRoot + '/OSPSettings/TubeCoreColorSettings?type=' + _ddlType, "_self");
    }
    else {
        window.open(appRoot + '/OSPSettings/TubeCoreColorSettings?type=', "_self");
    }
}
function OpenVendorSpecificationModel(p_systemId, p_entityType) {
     
    popup.LoadModalDialog('VendorSepcification/GetVendorSpecificationHistory', { systemId: p_systemId, eType: p_entityType }, 'Vendor Specification History', 'modal-lg');
}

function OpenUserHistory(p_userId) {
    popup.LoadModalDialog('User/GetUserHistory', { systemId: p_userId }, 'User History', 'modal-lg');
}
function OpenCBOMVariable(column_name, fsa_id, execution_sequence,label) {
     
    popup.LoadModalDialog('AdvancedSettings/GetVariables', { column_name: column_name, fsa_id: fsa_id, execution_sequence: execution_sequence }, '' + label + ' - Expression Builder', 'modal-lg');
}







function passwordRegex(val) {
    //.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%&]).*$
    var pattern = /^.*(?=.{6,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%&]).*$/;
    if (pattern.test(val)) {
        $('#ImgPwd').show();
        $('#ImgPwd').attr('src', appRoot.replace('Admin/', '') + 'Areas/Admin/Content/images/tick_1.gif')
        return true;

    } else {
        $('#ImgPwd').show();
        $('#ImgPwd').attr('src', appRoot.replace('Admin/', '') + 'Areas/Admin/Content/images/cross.gif')
        return false;

    }
}

function emailRegex(val) {

    var pattern = /^\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i;
    if (pattern.test(val)) {
        $('#ImgUserEmail').show();
        $('#ImgUserEmail').attr('src', appRoot.replace('Admin/', '') + 'Areas/Admin/Content/images/tick_1.gif')
        return true;

    } else {
        $('#ImgUserEmail').show();
        $('#ImgUserEmail').attr('src', appRoot.replace('Admin/', '') + 'Areas/Admin/Content/images/cross.gif')
        return false;

    }
}

function onChangeResourcelang() {
    $("#btnSearch").trigger("click");
}

function updateResources() {
    var p_modified_by = $("#hdnSession").val();

    var ResourceFieldList = [];
    var rowId = 0;
    var cultures = "";
    $("input[name='checkbox']:checked").each(function () {
        var txtvalue = $(this).attr('id').replace('chk', 'txtvalue');
        var txtname = $(this).attr('id').replace('chk', 'lblName');
        var txtCulture = $(this).attr('id').replace('chk', 'lblCulture');
        var txtlanguage = $(this).attr('id').replace('chk', 'hdnlanguage');
        var txtdescription = $(this).attr('id').replace('chk', 'lbldescription');
        var isjqueryused = $(this).attr('id').replace('chk', 'isJquery');
        var p_modified_on = new Date();

        var description = $("#" + txtdescription).text();
        var language = $("#" + txtlanguage).val();
        var currentval = $("#" + txtvalue).val();
        var name = $("#" + txtname).text();
        var culture = $("#" + txtCulture).text();
        var isjquery = $("#" + isjqueryused).text();


        cultures = culture;
        if (currentval != '' && name != '' && culture != '') {
            ResourceFieldList.push({
                'value': currentval, 'key': name, 'culture': culture, 'modified_by': p_modified_by, 'modified_on': p_modified_on, 'language': language, 'description': description, 'is_visible': true, is_jquery_used: isjquery
            })
        }
    });
    if (ResourceFieldList.length != 0) {
        showConfirm("Are you sure you want to Update?", function () {
            ajaxReq('Resources/Edit', { manageResource: ResourceFieldList }, true, function (resp) {

                if (resp.status == "OK") {

                    alert(resp.message, 'Success', 'success');
                    setTimeout(function () {
                        $("#btnSearch").trigger("click");
                    }, 2000);

                }

                else
                    alert(resp.message);
            }, true, true);
        });
    }
    else {
        alert("Please Check Checkbox For Update Resources.")
    }
}

//function GenerateResourceScript() {


//    var p_modified_by = $("#hdnSession").val();

//    var key = "";
//    var rowId = 0;
//    var cultures = "";
//    $("input[name='checkbox']:checked").each(function () {


//        var txtname = $(this).attr('id').replace('chk', 'lblName');
//        var txtCulture = $(this).attr('id').replace('chk', 'lblCulture');


//        var name = $("#" + txtname).text();
//        var culture = $("#" + txtCulture).text();
//        cultures = culture;
//        if (name != '') {
//            key += "'" + name + "',";


//        }
//    });
//     
//    if (key.length != 0) {
//        showConfirm("Are you sure you want to Generate Script?", function () {
//            window.location.href = "/Resources/GenerateScript?Key=" + key;

//            //ajaxReq('Resources/GenerateScript', { key: key }, true, function (resp) {

//            //    //if (resp.status == "OK") {

//            //    //    alert(resp.message, 'Success', 'success');
//            //    //    setTimeout(function () {
//            //    //        $("#btnSearch").trigger("click");
//            //    //    }, 2000);

//            //    //}

//            //    //else
//            //    //    alert(resp.message);
//            //}, true, true);
//        });
//    }
//    else {
//        alert("Please Check Checkbox For  Generate Script.")
//    }
//}


//Create Rule & Type Script:
function updateNetworkId() {
    // 
    var layerName = $("#lstLayers").val();
    var oldnetworkid = $("#txtoldnetworkcode").val();
    var newnetworkid = $("#txtMaskedNetworkId").dxTextBox("option", "text").toUpperCase();
    var inputmaskvalue = $("#txtMaskedNetworkId").dxTextBox("option", "value").replace(/ /g, '');
    var remarks = $("#txtremarks").val();
    if (inputmaskvalue.length != $("#hdnlayerformat").val()) {
        alert("New network code should be " + newnetworkid.length + " character long!");
        return false;
    }
    if (layerName == "" || oldnetworkid == "" || newnetworkid == "" || remarks == "") {
        alert("Kindly fill all values");
        return false;
    }
    else {
        var postData = { etype: layerName, old_network_id: oldnetworkid };
        ajaxReq('Miscellaneous/getNetworkIdDependency', postData, true, function (resp) {
            if (resp.length > 0) {
                var Msg = '<table border="1" class="alertgrid"><tr><td><b>Network Id</b></td><td><b>Message<b/></td></tr>';
                $.each(resp, function (index, item) {
                    Msg += '<tr><td>' + item.network_id + ' </td><td> ' + item.message + '</td></tr>';
                });
                Msg += '</table>';
                alert('Following are the dependent elements. You need to remove them first:<br/>' + Msg);
            }
            else {
                ajaxReq('Miscellaneous/SaveChangeNewNetworkcode', { etype: layerName, old_network_id: oldnetworkid, new_network_id: newnetworkid, remarks: remarks }, false, function (resp) {

                    if (resp.status) {
                        alert(resp.message);
                        $('#resetbutton').trigger("click");
                    } else { alert(resp.message); }
                }, true, false);
            }
        }, true, true);


    }
}
function getnewnetworkcodedata(p_etype, p_entitynetworkid) {
    var details = '';
    ajaxReq('Miscellaneous/getNewNetworkcode', { etype: p_etype, entity_network_id: p_entitynetworkid }, false, function (resp) {
        details = resp;
    }, true, false);
    return details;
}
function checknetworkidExistence(p_networkId, p_layername, p_networkStage) {
    var details = '';
    ajaxReq('../Library/IsNetworkIdExist', { networkId: p_networkId, entityType: p_layername, networkStage: p_networkStage }, false, function (resp) {
        details = resp;
    }, true, false);
    return details;
}
/*---------------User RIghts Management-----------------------------------*/

function validateRole() {
   
    var isValid = true;
     
    //if ($('#objRoleMaster_role_name').val() == '') { isValid = false; $('#frmSaveRole').submit(); }
    //else if ($('#objRoleMaster_remarks').val() == '') { isValid = false; $('#frmSaveRole').submit(); }
    $("#ddlTemplate_chosen").removeClass("highlight_chosen_select");
    $("#ddlroles_chosen").removeClass("highlight_chosen_select");
    if ($("#ddlTemplate").val() == null) {
        $("#ddlTemplate_chosen").addClass("highlight_chosen_select");
        //if ($('#hdnRoleId').valid() != false) {
        //    return false;
        //}
    }
     var multi_reporting_role_ids = $("#ddlroles").val();
        if (multi_reporting_role_ids!='') {
            if ($("#ismutiRRmanagar").val() == "True") {            
                    $('#multi_reporting_role_ids').val($("#ddlroles").val().join(","));
                    multi_reporting_role_ids = $("#ddlroles").val().join(",")
            }

            else {
                $('#multi_reporting_role_ids').val($("#ddlroles").val());
            }
        }

    if ($("#ddlroles").val() == '' || $("#ddlroles").val() == null) {

        $("#ddlroles_chosen").addClass("highlight_chosen_select");
        alert('Please Select Reporting Role Name');
        isValid = false;
        return false;
    }
    else {
        $("#ddlroles_chosen").removeClass("highlight_chosen_select");
        isValid = true;
}
    if ($("#rolename").val() == '' || $("#rolename").val() == null)
    {
        alert('Please Enter The Role Name');
        $("#rolename").addClass('input-validation-error').removeClass('valid');
        isValid = false;
        return false;
    }
    else {
        $("#rolename").addClass('valid').removeClass('input-validation-error');
        isValid = true;

    }

    

    if ($("#roledescription").val() == '' || $("#roledescription").val() == null)
    {       
        alert('Please  Enter Role Description');
        $("#roledescription").addClass('input-validation-error').removeClass('valid');
        isValid = false;
        return false;
    }
    else
    {
        $("#roledescription").addClass('valid').removeClass('input-validation-error');
        isValid = true;
     
    }

    if (isValid && $('#ddlTemplate').val() == '') {
        isValid = false;
        $("#ddlTemplate_chosen").addClass("highlight_chosen_select");
        alert('Please select template name first!');
    }
    else if (isValid && $('#ddlroles').val() == '') {
        isValid = false;
        $("#ddlroles_chosen").addClass("highlight_chosen_select");
        alert('Please select reporting role name first!');
    } else if (isValid && $('.innerCheck[type="checkbox"]:checked').length == 0) {
        isValid = false; alert('Please select permissions!');
    }
    $("#frmSaveRole").valid();
    if (isValid) {
        ajaxReq('Roles/CheckRoleAssignToUser', { role_id: $("#hdnRoleId").val() }, true, function (resp) {
            if (resp > 0) {
                if (!$("#objRoleMaster_is_active").is(":checked")) {
                    alert("This role is associated to " + resp + " user(s), first changes the role(s) of these(the) user(s) then deactivate this role!");
                    return;
                }
                confirm("This role is associated to " + resp + " user(s), if you make any change it will be reflected to all!", function () {
                    $('#frmSaveRole').submit();
                });
            }
            else {
                $('#frmSaveRole').submit();
            }
        }, true, true);
    }
}

function UploadResourceFile() {
    // 
    //AddNewResourceKey()removeOldMarkers();
    popup.LoadModalDialog('Resources/UploadResourcesFile', {}, 'Upload Resources File', 'modal-md');
}

//Javascript of DropdownMaster.cshtml (Written by Diksha gupta)

function DeleteDropdownMasters(id) {
     
    ajaxReq('WFM/DeletercaMaster', { id: id }, false, function (resp) {
        if (resp.status == "OK") {
            alert("Record has been deleted successfully!", 'Success', 'success');
            window.location.reload();
            $("#frmViewDropdownMaster").submit();
        }
    }, true, true);

}



function BindDrptypeByLayerIdrca() {
    //$('#dropdown_type').val("");
    GetrcaIdBy();
}
//function ResetDropTypes()
//{
//    $('#dropdown_type').val($('#fieldNameDropDown :selected').val());

//}

function GetrcaIdBy() {

    var status = ($('#lstLayerss').val());
    var options = '';
    if (status != '') {
        $('#status').val(status);
        ajaxReq('WFM/getRCADetailsByLayerId', { Status: status }, true, function (response) {
            $('#fieldNameDropDown').html('');
             
            options += "<option value>-Select-</option>";
            for (var i = 0; i < response.length; i++) {
                options += '<option value="' + response[i].rca + '">' + response[i].rca + '</option>';
            }
            $('#fieldNameDropDown').append(options);
            $('#fieldNameDropDown').trigger("chosen:updated");
        }, false, false);
    }
    else {
        $('#fieldNameDropDown').find('option').remove();
        $('#fieldNameDropDown').append("<option value>-Select-</option>");
        $('#fieldNameDropDown').trigger("chosen:updated");
    }

}


function AddDropdownMasters(id) {
     
    popup.LoadModalDialog('WFM/AddEntityDropdownmasterrca', { id: id }, 'Add New RCA Master', 'modal-md');
    if ($('#id').val() != 0) {
        $('#ddlLayers').attr("disabled", "disabled");
        $('#ddlfieldNameDropDown').attr("disabled", "disabled");
    }


}



function BindDrptypeByLayerId() {
    //$('#dropdown_type').val("");
    GetDropdownIdBy();
}
//function ResetDropTypes()
//{
//    $('#dropdown_type').val($('#fieldNameDropDown :selected').val());

//}


function GetDropdownIdBy() {
     
    var layerId = parseInt($('#lstLayers').val());
    var options = '';
    if (layerId > 0) {
        $('#layer_id').val(layerId);
        ajaxReq('miscellaneous/getDropMasterDetailsByLayerId', { Layer_Id: layerId }, true, function (response) {
            $('#fieldNameDropDown').html('');

            options += "<option value>-Select-</option>";
            for (var i = 0; i < response.length; i++) {
                options += '<option value="' + response[i].dropdown_type + '">' + response[i].dropdown_type + '</option>';
            }
            $('#fieldNameDropDown').append(options);
            $('#fieldNameDropDown').trigger("chosen:updated");
        }, false, false);
    }
    else {
        $('#fieldNameDropDown').find('option').remove();
        $('#fieldNameDropDown').append("<option value>-Select-</option>");
        $('#fieldNameDropDown').trigger("chosen:updated");
    }

}

function AddDropdownMaster(id) {

    popup.LoadModalDialog('Miscellaneous/AddEntityDropdownmaster', { id: id }, 'Add New Dropdown Master', 'modal-md');
    if ($('#id').val() != 0) {
        $('#ddlLayers').attr("disabled", "disabled");
        $('#ddlfieldNameDropDown').attr("disabled", "disabled");
    }


}

function AddNewRCAMaster(id, layername, fieldname, Value) {
     
    {
        popup.LoadModalDialog('WFM/AddEntityDropdownmasterrca', { id: id }, 'Update RCA Master', 'modal-md');
    }
}

function AddNewDropdownMaster(id, layername, fieldname, Value) {
     

    ajaxReq('Miscellaneous/GetDropdownMasterRowCount',
        {
            layer_name: layername, fieldname: fieldname,
            value: Value
        }, false, function (resp) {
            if (resp.status == "OK" && resp.rowcount >= 0) {
                if (resp.rowcount > 0) {
                    resp.rowcount = " because it has " + resp.rowcount + " dependent records";
                }
                else {
                    resp.rowcount = "";
                }
                confirm("Are you sure you want to update this" + resp.rowcount + "?", function (res) {
                    popup.LoadModalDialog('Miscellaneous/AddEntityDropdownmaster', { id: id }, 'Update Dropdown Master', 'modal-md');


                });
            }
            else {
                alert(resp.status + " " + resp.message);
            }


        }, true, true);
}

function DeleteDropdownMaster(id, layername, fieldname, Value) {
     
    ajaxReq('Miscellaneous/GetDropdownMasterRowCount', { layer_name: layername, fieldname: fieldname, value: Value }, false, function (resp) {
        if (resp.status == "OK" && resp.rowcount > 0) {
            alert("You cannot delete this record because it has " + resp.rowcount + " dependent records.");
        }
        else if (resp.rowcount == 0) {
            confirm("Are you sure you want to delete this record?", function (res) {

                 
                ajaxReq('Miscellaneous/DeleteDropdownMaster', { id: id }, false, function (resp) {
                    if (resp.status == "OK") {
                        alert("Record has been deleted successfully!", 'Success', 'success');
                        window.location.reload();
                        $("#frmViewDropdownMaster").submit();
                    }
                }, true, true);
            });
        }
        else
            alert(resp.message);
    }, true, true);

}

// Javascript of AddDropdownMaster.cshtml (Written by Diksha gupta)



function ResetddlDrp() {
    $('#dropdown_type').val($('#ddlfieldNameDropDown :selected').val());
}

function validate(elementId) {
     
    if ($('#' + elementId).length >= 0) {
        var value = $('#' + elementId).val();

        if (value == null || value == "" || value == 0) {
            $('#' + elementId).addClass('input-validation-error').removeClass('valid');
            return false;
        }
        else {
            $('#' + elementId).addClass('valid').removeClass('input-validation-error');
            return true;
        }
    }
    return true;
}
/* ================================================================ LandBase Layer Setting ===================================*/

function BindCategory() {
    var layerId = parseInt($('#ddlLayers').val());
    if ($('#ddlType').val() == "SubCategory") {
        ($('#divCat')).show();
        ajaxReq('LandbaseSettings/getCategoryByLayerId', { Layer_Id: layerId }, true, function (response) {
            $('#ddlCategory').html('');
            var options = '';
            options += "<option value>-Select-</option>";
            for (var i = 0; i < response.length; i++) {
                options += '<option value="' + response[i].id + '">' + response[i].value + '</option>';
            }
            $('#ddlCategory').append(options);
            $('#ddlCategory').trigger("chosen:updated");
        }, false, false);
    }
    else {
        ($('#divCat')).hide();
    }
}
$("#btnSaveDropdownMaster").on("click", function (e) {
     
    if (!validateDropdown('ddlLayers')) { e.preventDefault(); return false; }
    if (!validate('txtValue')) { e.preventDefault(); return false; }

    $("#frmViewDropdownMaster").submit();
    setTimeout(function () { $("#ddlDropdownMasterValueValidation").text(''); }, 5000);
});


function checkIsDuplicate_value_abbrAvailable() {
    var Id = parseInt($('#id').val());
    var layerId = parseInt($('#ddlLayers').val());
    var Type = $('#ddlType').val();

    var status = false;
    if ($("#txtValue").val() != "") {

        ajaxReq('LandbaseSettings/IsDuplicate_Value_abbrAvailable', { id: Id, Layer_Id: layerId, type: Type, Dvalue: $("#txtValue").val() }, false, function (obj) {

            if (obj.status == 'ERROR') {
                alert(obj.message);
                $('#txtValue').css('border-color', 'red');
                status = false;
                // $("#txtValue").attr("disabled", false).val("");

            }
            else {
                $('#txtValue').css('border-color', '');
                status = true;
            }
        });
        return status;
    }
}
function AddLandBaseDropdownMaster(id) {

    popup.LoadModalDialog('LandbaseSettings/AddLandBaseDropdownMaster', { id: id }, 'Add New Dropdown Master', 'modal-md');
    if ($('#id').val() != 0) {
        $('#ddlLayers').attr("disabled", "disabled");
        $('#type').attr("disabled", "disabled");
    }
}

function EditLandBaseDropdownMaster(id) {
    popup.LoadModalDialog('LandbaseSettings/AddLandBaseDropdownMaster', { id: id }, 'Add New Dropdown Master', 'modal-md');
    if ($('#id').val() != 0) {
        $('#ddlLayers').attr("disabled", "disabled");
        $('#type').attr("disabled", "disabled");
    }
}
function CheckDataExistAgainstLandBaseLayer() {
    var id = $("#id").val();
    if (parseInt(id) > 0) {
        if ($("#is_active").is(":checked") == false) {
            ajaxReq('LandbaseSettings/GetLandBaseLayerSettingRowCount', { layer_id: parseInt(id) }, false, function (resp) {
                if (resp.status == "OK" && resp.rowcount > 0) {
                    alert("You cannot inactive this record because it has " + resp.rowcount + " dependent records.");
                    $("#is_active").prop('checked', true);
                }
            }, true, true);
        }
    }
}
function DeleteLandBaseLayerMaster(id) {

    ajaxReq('LandbaseSettings/GetLandBaseLayerSettingRowCount', { layer_id: id }, false, function (resp) {
        if (resp.status == "OK" && resp.rowcount > 0) {
            alert("You cannot delete this record because it has " + resp.rowcount + " dependent records.");
        }
        else if (resp.rowcount == 0) {
            confirm("Are you sure you want to delete this record?", function (res) {

                 
                ajaxReq('LandbaseSettings/DeleteLandBaseLayerSettingById', { id: id }, false, function (resp) {
                    if (resp.status == "OK") {
                        alert("Record has been deleted successfully!", 'Success', 'success');
                        window.location.reload();
                        $("#frmViewDropdownMaster").submit();
                    }
                }, true, true);
            });
        }
        else
            alert(resp.message);
    }, true, true);
}

function CheckDataExistAgainstDropdownType() {
     
    var layer_id = $("#ddlLayers").val();
    var id = $("#id").val();
    var fieldname = $("#type").val();
    if (parseInt(id) > 0) {
        if ($("#is_active").is(":checked") == false) {
            ajaxReq('LandbaseSettings/GetLandBaseDropdownMasterRowCount', { layer_id: parseInt(layer_id), id: parseInt(id), layer_name: '', fieldname: fieldname, value: '' }, false, function (resp) {
                if (resp.status == "OK" && resp.rowcount > 0) {
                    alert("You cannot inactive this record because it has " + resp.rowcount + " dependent records.");
                    $("#is_active").prop('checked', true);
                }
            }, true, true);
        }
    }
}

function DeleteLandBaseDropdownMaster(layer_id, id, layername, fieldname, Value) {
     
    ajaxReq('LandbaseSettings/GetLandBaseDropdownMasterRowCount', { layer_id: layer_id, id: id, layer_name: layername, fieldname: fieldname, value: Value }, false, function (resp) {
        if (resp.status == "OK" && resp.rowcount > 0) {
            alert("You cannot delete this record because it has " + resp.rowcount + " dependent records.");
        }
        else if (resp.rowcount == 0) {
            confirm("Are you sure you want to delete this record?", function (res) {

                 
                ajaxReq('LandbaseSettings/DeleteLandBaseDropdownMasterById', { id: id }, false, function (resp) {
                    if (resp.status == "OK") {
                        alert("Record has been deleted successfully!", 'Success', 'success');
                        window.location.reload();
                        $("#frmViewDropdownMaster").submit();
                    }
                }, true, true);
            });
        }
        else
            alert(resp.message);
    }, true, true);
}

/*========================================================================== Add LandBaseLayer==================================================*/

function InputToUpper(obj) {
    if (obj.value != "") {
        obj.value = obj.value.toUpperCase();
    }
}

function checkIsDuplicate_layer_name() {
     
    var status = false;

    if ($.trim($("#layer_name").val()).length > 2) {
        $('#layer_name').css('border-color', '');
        ajaxReq('LandbaseSettings/IsDuplicate_abbrAvailable', { Layer_Abbr: '', Map_Abbr: '', Layer_name: $("#layer_name").val().trim() }, false, function (obj) {

            if (obj.status == 'ERROR') {
                alert(obj.message);
                status = false;
                //$("#layer_name").attr("disabled", false).val("");
                $('#layer_name').css('border-color', 'red');
            }
            else {
                $('#layer_name').css('border-color', '');
                status = true;
            }
        });
    }
    else {
        //alert("Layer name must be greater than 2 Characters"); 
        $('#layer_name').css('border-color', 'red');
        status = false;
    }
    return status;
}


function checkIsDuplicate_layer_abbrAvailable() {
    var status = false;
    if ($("#layer_abbr").val() != "") {

        ajaxReq('LandbaseSettings/IsDuplicate_abbrAvailable', { Layer_Abbr: $("#layer_abbr").val(), Map_Abbr: '', Layer_name: '' }, false, function (obj) {

            if (obj.status == 'ERROR') {
                alert(obj.message);
                status = false;
                // $("#layer_abbr").attr("disabled", false).val("");
                $('#layer_abbr').css('border-color', 'red');
            }
            else {
                $('#layer_abbr').css('border-color', '');
                status = true;
            }
        });
        return status;
    }
}


function checkIsDuplicate_map_abbrAvailable() {
    var status = false;
    if ($("#map_abbr").val() != "") {

        ajaxReq('LandbaseSettings/IsDuplicate_abbrAvailable', { Layer_Abbr: '', Map_Abbr: $("#map_abbr").val(), Layer_name: '' }, false, function (obj) {

            if (obj.status == 'ERROR') {
                alert(obj.message);
                status = false;
                // $("#map_abbr").attr("disabled", false).val("");
                $('#map_abbr').css('border-color', 'red');
            }
            else {
                $('#map_abbr').css('border-color', '');
                status = true;
            }
        });
        return status;
    }

}
function checkOpacityValue() {

    var map_bg_opacityVal = $('#map_bg_opacity').val();

    if (map_bg_opacityVal != null) {
        if (map_bg_opacityVal > 100) {
            alert("Map Background Opacity value can not be greater than 100");
            $("#map_bg_opacity").attr("disabled", false).val("80");
            return false;
        }
        if ($('#map_bg_opacity').val().length <= 1) {
            alert("Map Background Opacity value is a two digit value");
            $("#map_bg_opacity").attr("disabled", false).val("80");
            return false;
        }
    }
    else {
        alert("Please enter Map Background Opacity value");
    }
    return true;
}
function showHideEnableCenterline() {
     
    if ($('#ddlgeom_type').val() != "") {
        $('#ddlgeom_type').css({ "border": "1px solid #ced4da", "border-radius": "0.25rem" });
    }

    if ($('#ddlgeom_type').val() != null) {
        if ($('#ddlgeom_type').val() == "Line") {
            ($('#lblis_center_line_enable')).show();
            ($('#is_center_line_enable')).show();
            $('#DisplyIconOnGeomPoint').css('display', 'none');
        }
        else {
            ($('#is_center_line_enable')).prop('checked', false);
            ($('#lblis_center_line_enable')).hide();
            ($('#is_center_line_enable')).hide();
            if ($('#ddlgeom_type').val() == "Point") {
                $('#DisplyIconOnGeomPoint').css('display', '');
            }
            else {
                $('#DisplyIconOnGeomPoint').css('display', 'none');
            }
        }
    }
    else {
        ($('#is_center_line_enable')).prop('checked', false);
        ($('#lblis_center_line_enable')).hide();
        ($('#is_center_line_enable')).hide();
    }
}
/*========================================================================== view LandBaseLayers==================================================*/


function ViewLBLReport() {
    window.location = appRoot + 'LandbaseSettings/DownloadDetail';
}

function LandBaseLayerSearchChange() {
     
    var _layername = $('#lstLayers option:selected').text();
    var _layerId = $('#lstLayers option:selected').val();
    if (_layerId != undefined && _layerId != 0) {
        window.open(appRoot + '/LandbaseSettings/LandBaseLayerSearchSetting/' + _layerId, "_self");
    }
    else {
        window.open(appRoot + '/LandbaseSettings/LandBaseLayerSearchSetting', "_self");
    }
}

/* ===================================================== landbase label setting ========================================== */
function onChangeLandBaseAttributeBind() {
     
    var layer_id = $('#ddllstLayers').val();

    ajaxReq('LandbaseSettings/BindLandBaseAttributOnChange', { layer_id: layer_id }, false, function (resp) {

        if (resp.Data.length > 0) {

            $('#ddlLandBaseAttributesDetails').empty();
            $('#ddlLandBaseAttributesDetails').append($("<option></option>").val('').html('-Select-'));
            $.each(resp.Data, function (data, value) {
                var selected = value.is_selected == true ? "selected" : "";
                $('#ddlLandBaseAttributesDetails').append($("<option data-selectedvalue=" + (value.is_selected) + " " + selected + " ></option>").val(value.column_name).html(value.display_name));

            });

        }
        else {
            $('#ddlLandBaseAttributesDetails').empty();
            $('#ddlLandBaseAttributesDetails').append($("<option></option>").val('').html('-Select-'));
        }
    }, true, true);
    $('#ddlLandBaseAttributesDetails').trigger("chosen:updated");

}

/*========================================================================== Accessories Master==================================================*/
function AddNewAccessories(_id) {
     
    popup.LoadModalDialog('Accessories/AddNewAccessories', { id: _id }, 'Add Accessories', 'modal-md');
}
function EditAccessories(_id) {

    popup.LoadModalDialog('Accessories/AddNewAccessories', { id: _id }, 'Update Accessories', 'modal-md');
}

function DeleteAccessoriesById(id, name) {
     

    confirm("Are you sure you want to delete this record?", function () {
        ajaxReq('Accessories/DeleteAccessories', { id: id, AccName: name }, false, function (resp) {
            if (resp.status == "OK") {
                alert(resp.message, 'Success', 'success');
                setTimeout(function () {
                    $("#frmViewAccessories").submit();
                }, 3000);

            }
            else
                alert(resp.message);
        }, true, true);
    });
}

function AccessoriesMapping(_id) {
     
    popup.LoadModalDialog('Accessories/AccessoriesMapping', { id: _id }, 'Accessories Mapping', 'modal-md');
}
function DeleteAccessoriesMappingById(id, entity_type) {

    confirm("Are you sure you want to delete this record?", function () {
        ajaxReq('Accessories/DeleteAccessoriesMapping', { id: id, entity_type: entity_type }, false, function (resp) {
            if (resp.status == "OK") {
                alert(resp.message, 'Success', 'success');
                setTimeout(function () {
                    window.location = appRoot + 'Accessories/ViewAccessoriesMapping';
                }, 3000);

            }
            else
                alert(resp.message);
        }, true, true);
    });
}
/*========================================================================== End Accessories Master==================================================*/

/*========================================================================== Ortho Image Master==================================================*/
function AddNewOrthoImage(_id) {
     
    popup.LoadModalDialog('OrthoImageLayer/AddNewOrthoImage', { id: _id }, 'Add Ortho Image', 'modal-md');
}
function DeleteOrthoImageById(id, name) {
     

    confirm("Are you sure you want to delete this record?", function () {
        ajaxReq('OrthoImageLayer/DeleteOrthoImageById', { id: id, AccName: name }, false, function (resp) {
            if (resp.status == "OK") {
                alert(resp.message, 'Success', 'success');
                setTimeout(function () {
                    $("#frmViewOrthoImage").submit();
                }, 3000);

            }
            else
                alert(resp.message);
        }, true, true);
    });
}
/*========================================================================== End Ortho Image Master==================================================*/

/*------------------------------------------------------------------------Error Logs--------------------------------------------------------------*/

function ReadMore(_queryId) {
     
    popup.LoadModalDialog('Miscellaneous/ReadMore', { queryId: _queryId }, 'Web Error Details', 'modal-lg');
}
function ReadMoreApi(_queryId, msgtype) {

    popup.LoadModalDialog('Miscellaneous/ReadMoreApi', { queryId: _queryId }, 'Api Error Details', 'modal-lg');
}
function ReadMoreApiRequest(_queryId) {

    popup.LoadModalDialog('Miscellaneous/ReadMoreApiRequest', { queryId: _queryId }, 'Api Request Details', 'modal-lg');
}

function ReadMoreGisApiLog(id) {

    popup.LoadModalDialog('Miscellaneous/ReadMoreGisApiLog', { id: id }, 'Gis Api Log Details', 'modal-lg');
}
//Calander 
function onChangeCustomDate() {

    var value = $('#customedate option:selected').val(); {

        var endDate = new Date();
        var startDate = new Date();
        var startDateDiff = 0;
        var endDateDiff = 0;

        if (value == 1) // custom
        {
            $('#txtDateFrom').val("");
            $('#txtDateTo').val("");
            $("#imgFromDate").show();
            $('#imgToDate').show();
            return;
        }
        else if (value == 2) { //Today
            startDateDiff = 0;
            endDateDiff = 0;
            $('#imgFromDate').hide();
            $('#imgToDate').hide();
        }
        else if (value == 3) { //previous day
            startDateDiff = -1;
            endDateDiff = -1;
            $('#imgFromDate').hide();
            $('#imgToDate').hide();
        }
        else if (value == 4) {//last 7 days
            startDateDiff = -6;
            $('#imgFromDate').hide();
            $('#imgToDate').hide();
        }
        else if (value == 5) {//last 30 days
            startDateDiff = -29;
            $('#imgFromDate').hide();
            $('#imgToDate').hide();
        }
        else if (value == 6) {//last 3 Months
            startDateDiff = -89;
            $('#imgFromDate').hide();
            $('#imgToDate').hide();
        }
        else if (value == 7) {//last 6 Months
            startDateDiff = -179;
            $('#imgFromDate').hide();
            $('#imgToDate').hide();
        }
        else if (value == 8) {//last 6 Months
            startDateDiff = -364;
            $('#imgFromDate').hide();
            $('#imgToDate').hide();
        }

        startDate.setDate(startDate.getDate() + startDateDiff);
        endDate.setDate(endDate.getDate() + endDateDiff);

        var newStartDate = startDate.toDateString();
        newStartDate = new Date(Date.parse(newStartDate));

        var newEndDate = endDate.toDateString();
        newEndDate = new Date(Date.parse(newEndDate));

        $('#txtDateFrom').val(fncCurrentDate(newStartDate));
        $('#txtDateTo').val(fncCurrentDate(newEndDate));
        if (value == 0) {
            $('#txtDateFrom').val('');
            $('#txtDateTo').val('');
            $('#imgFromDate').hide();
            $('#imgToDate').hide();
        }
    }
}
function setDateTimeCalendar(startdateid, enddateid, startdateimgid, enddateimgid, isFutureDateAllowed) {
     
    Calendar.setup({
        inputField: startdateid,   // id of the input field
        button: startdateimgid,
        ifFormat: "%d-%b-%Y",       // format of the input field datetime format %d-%b-%Y %I:%M %p
        showsTime: false,
        timeFormat: "12",
        weekNumbers: false,


        disableFunc: function (date) {
             
            var endDateValue = document.getElementById(enddateid).value;

            var enddt = new Date(DateFormatter(enddateid));
            enddt = new Date(enddt);
            if (date.getTime() > enddt.getTime()) {
                return true;
            }
            ////
            if (!isFutureDateAllowed) {
                var now = new Date();
                now.setDate(now.getDate());
                if (date.getTime() > now.getTime()) {
                    return true;
                }
            }
        }
    });

    //isFutureDateAllowed = true;
    Calendar.setup({
        inputField: enddateid,
        button: enddateimgid,
        ifFormat: "%d-%b-%Y",
        showsTime: false,
        timeFormat: "12",
        weekNumbers: false,
        //maxDate: new Date(new Date().setMonth(new Date().getMonth() + 6)),
        align: "Br",
        //maxDate: "+6m",
        disableFunc: function (date) {
            var startDateValue = document.getElementById(startdateid).value;

            var startdt = DateFormatter(startDateValue);
            startdt = new Date(startdt);
            if (date.getTime() < startdt.getTime()) {
                return true;
            }
            if (!isFutureDateAllowed) {
                var now = new Date();
                now.setDate(now.getDate());
                if (date.getTime() > now.getTime()) {
                    return true;
                }
            }

        },

    });
}

//End Calendar\\\\\\\\\\\\\\\\\\\\
/*------------------------------------------------------------------------ END Error Logs--------------------------------------------------------------*/

/*----------------------------------------------------------------------------------------------------------------------------------------------------------*/
function AddNewFaq(_id) {
     
    popup.LoadModalDialog('Miscellaneous/AddNewFaq', { id: _id }, 'Add FAQ', 'modal-md');
}
function AddNewFaqUserManual(_id) {
     
    popup.LoadModalDialog('Miscellaneous/AddNewFaqUserManual', { id: _id }, 'Add User Manual', 'modal-md');
}

this.UserManual = {

    upload: function () {
         
        if ($('#ddlcategory').val() == "") {
            $('#ddlcategory').addClass('errorClass');
            return false;
        }
        else {
            $('#ddlcategory').removeClass("errorClass");
        }
        if ($('#DocUpload').val() == '') {
            alert("Please select a file to upload!");
        }
        else {
            app.UserManual.uploadDocumentFile();
        }
    },
    uploadDocumentFile: function () {
         
        var frmData = new FormData();
        var filesize = $('#hdnMaxFileUploadSizeLimit').val();

        var Sizeinbytes = filesize * 1024;
        if ($('#DocUpload').get(0).files[0].size > Sizeinbytes) {

            alert($.validator.format("File size is too large. Maximum file size allowed is <b> {0} </b> MB !", (filesize / 1024).toFixed(2)));
        }
        else {

            var uploadedfile = $('#DocUpload').get(0).files[0];
            if (!app.UserManual.validateDocumentFileType()) {
                return false;
            }
            frmData.append(uploadedfile.name, uploadedfile);
            frmData.append('category', $('#ddlcategory').val());
            frmData.append('id', $('#hdnid').val());
            frmData.append('feature_name', 'UserManual');
             
            ajaxReqforFileUpload('Miscellaneous/UploadUserManual', frmData, true, function (resp) {
                if (resp.status == "OK") {
                    //app.UserManual.getAttachmentFiles(systemId, entityType);
                    $("#closeModalPopup").trigger("click");
                    alert(resp.message, 'success', 'success');
                    setTimeout(function () {
                        window.location = appRoot + 'Miscellaneous/ViewFaqUserManual';
                    }, 3000);
                    if ($('#DocUpload')[0] != undefined)
                        $('#DocUpload')[0].value = '';
                }
                else {
                    alert(resp.message, 'warning');
                }

            }, true);
        }
    },
    validateDocumentFileType: function () {
        var validFilesTypes = ["dwg", "pdf", "jpeg", "jpg", "doc", "docx", "xls", "xlsx", "csv", "vsd", "ppt", "pptx", "png"];
        var file = $('#DocUpload').val();
        var filepath = file;
        return app.UserManual.ValidateFileType(validFilesTypes, filepath);
    },
    ValidateFileType: function (validFilesTypes, filepath) {
        var ext = filepath.substring(filepath.lastIndexOf(".") + 1, filepath.length).toLowerCase();
        var isValidFile = false;
        for (var i = 0; i < validFilesTypes.length; i++) {
            if (ext == validFilesTypes[i]) {
                isValidFile = true;
                break;
            }
        }
        if (!isValidFile) {
            alert("Invalid File. Please upload a File with	File Name" +
                " extension:\n\n" + validFilesTypes.join(", "), 'warning');
        }
        return isValidFile;
    },


}
function DeleteFaqById(id) {
     

    confirm("Are you sure you want to delete this record?", function () {
        ajaxReq('Miscellaneous/DeleteFaqById', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                alert(resp.message, 'Success', 'success');
                setTimeout(function () {
                    $("#frmViewFaq").submit();
                }, 3000);

            }
            else
                alert(resp.message);
        }, true, true);
    });
}
function DeleteUserManualById(id) {
     

    confirm("Are you sure you want to delete this record?", function () {
        ajaxReq('Miscellaneous/DeleteUserManualById', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                alert(resp.message, 'Success', 'success');
                setTimeout(function () {
                    $("#frmViewFaqUserManual").submit();
                }, 3000);

            }
            else
                alert(resp.message);
        }, true, true);
    });
}
function faqReadMore(_queryId) {
     
    popup.LoadModalDialog('Miscellaneous/FaqReadMore', { queryId: _queryId }, 'Read More', 'modal-lg');
}
/*----------------------------------------------------------------------------------------------------------------------------------------------------------*/
//--------------------------------------------------------------------TerminationPoint-=---------------------------------------------------------
function AddNewTerminationPoint(_id) {
     
    popup.LoadModalDialog('Miscellaneous/AddNewTerminationPoint', { id: _id }, 'Add Termination Point', 'modal-md');
}
function UpdateNewTerminationPoint(_id) {
    popup.LoadModalDialog('Miscellaneous/AddNewTerminationPoint', { id: _id }, 'Update Termination Point', 'modal-md');
}
function EditConfigurationSettings(_id) {
    popup.LoadModalDialog('ConfigurationSettings/EditConfigurationSettings', { id: _id }, 'Edit Configuration Setting', 'modal-md');
}

function DeleteTerminationPointById(id) {
     

    confirm("Are you sure you want to delete this record?", function () {
        ajaxReq('Miscellaneous/DeleteTerminationPointById', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                alert(resp.message, 'Success', 'success');
                $("#frmViewterminationpoint").submit();
                //setTimeout(function () {
                //    $("#frmViewterminationpoint").submit();
                //}, 3000);

            }
            else
                alert(resp.message);
        }, true, true);
    });
}
function AddLayerSettings(layer_id) {
     
    popup.LoadModalDialog('LayerSettings/AddLayerDetailSettings', { layer_id: layer_id }, 'Add Layer Detail', 'modal-md');
}
function ReadMoreLayer(layer_id) {
     
    popup.LoadModalDialog('LayerSettings/ReadMoreLayer', { layer_id: layer_id }, 'Layer Details', 'modal-lg');
}
//-------------------------------End-----------------------------------------------------------------------------------------------------------------------
function AddNewTemplateColumn(_id) {
     
    popup.LoadModalDialog('Miscellaneous/AddNewTemplateColumn', { id: _id }, 'Add Template Column', 'modal-md');
}
function onMappedColumnName() {

    $('#ddl_db_column_name').empty();
    $('#ddl_db_column_name').append($("<option></option>").val('').html('-Select-'));
    var layer_id = $('#ddlLayerName').val();
    ajaxReq('Miscellaneous/BindDb_Column_name', { layer_id: layer_id }, false, function (resp) {
         
        if (resp.Data.length > 0) {
            // $('#ddl_db_column_naem').append($("<option></option>").val('').html('-Select-'));


            $.each(resp.Data, function (data, value) {
                $('#txtSequence').val(value.sequence);
                var data_type = value.data_type;
                $('#ddl_db_column_name').append($("<option data-type='" + data_type + "'  data-column-sequence=" + value.sequence + "></option>").val(value.db_column_name).html(value.db_column_name));
            });
        }
        else {
            $('#ddl_db_column_name').empty();
            $('#ddl_db_column_name').append($("<option></option>").val('').html('-Select-'));

        }
        $('#ddl_db_column_name').trigger("chosen:updated");
    }, true, true);
}

this.bindheddenfild = function (ddlId) {
     
    var eType = $(ddlId + ' :selected').attr('data-type');
};

function DeleteTemplateColumnById(id) {
     

    confirm("Are you sure you want to delete this record?", function () {
        ajaxReq('Miscellaneous/DeleteTemplateColumnById', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                alert(resp.message, 'Success', 'success');
                setTimeout(function () {
                    $("#frmViewTemplateColumn").submit();
                }, 3000);

            }
            else
                alert(resp.message);
        }, true, true);
    });
}
//-------------------------------End---------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------LayerGroupMapping-=-----------------------------------------------------------------
function AddNewLayerGroupMapping(_mappingId) {
     
    popup.LoadModalDialog('LayerSettings/AddNewLayerGroupMapping', { mappingId: _mappingId }, 'Add Layer Group Mapping', 'modal-md');
}

function AddLayerGroups(_group_id) {
     
    popup.LoadModalDialog('LayerSettings/AddLayerGroups', { group_id: _group_id }, 'Add Layer Group', 'modal-md');
}

function Addfeetool(_group_id) {
    

    popup.LoadModalDialog('FeTools/AddFEtools', { group_id: _group_id }, 'Add User Tools', 'modal-md');
}


function AddLayerStyleMaster(_layer_id,_id, _layer_name, _entity_category) {
    if (_entity_category !='') {
        _entity_category = '('+_entity_category+')';
    }
    popup.LoadModalDialog('LayerSettings/AddLayerStyleMaster', { layer_id: _layer_id, id: _id, layer_name: _layer_name, entity_category: _entity_category }, _layer_name + ' '+ _entity_category+' '+'Style Settings', 'modal-md');
}
//-------------------------------------------------END------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------AssociationMapping---------------------------------------------------------
function AddNewAccessoriesMapping(_id) {
     
    popup.LoadModalDialog('Miscellaneous/AddNewAccessoriesMapping', { id: _id }, 'Add Accessories Entity', 'modal-md');
}
function DeleteAssociateEntityById(id) {
    confirm("Are you sure you want to delete this record?", function () {
        ajaxReq('Miscellaneous/DeleteAssociateEntityById', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                alert(resp.message, 'Success', 'success');
                setTimeout(function () {
                    $("#frmViewAccessories").submit();
                }, 3000);

            }
            else
                alert(resp.message);
        }, true, true);
    });
}
//---------------------------------------------------------End------------------------------------------------------------

//----------------------------------Equipment Builder----------------------------------------------------------------------------------------------------
function AddNewISPModelImage(_id) {
     
    popup.LoadModalDialog('Equipment/UploadNewModleImage', { id: _id }, 'Add Model Image', 'modal-md');
}
this.ModelImage = {

    upload: function () {
         

        if ($('#ddlModelType').val() == "") {
            $('#ddlModelType').addClass('errorClass');
            return false;
        }
        else {
            $('#ddlModelType').removeClass("errorClass");
        }
        if ($('#DocUpload').val() == '' && $('#hdnid').val() == 0) {
            alert("Please select a file to upload!");
        }
        else {
            app.ModelImage.uploadDocumentFile();
        }
    },
    uploadDocumentFile: function () {
        var frmData = new FormData();
        var filesize = $('#hdnMaxFileUploadSizeLimit').val();
        var Sizeinbytes = filesize * 1024;
        if ($('#hdnid').val() == 0) {
            if ($('#DocUpload').get(0).files[0].size > Sizeinbytes) {

                alert($.validator.format("File size is too large. Maximum file size allowed is <b> {0} </b> MB !", (filesize / 1024).toFixed(2)));
                return false;
            }


            var uploadedfile = $('#DocUpload').get(0).files[0];
            if (!app.ModelImage.validateDocumentFileType()) {
                return false;
            }
            frmData.append(uploadedfile.name, uploadedfile);
        }
        else { frmData.append('name', ''); }
        var is_active = $("#chkis_activemodeleImage").prop("checked");
        frmData.append('modelType', $('#ddlModelType').val());
        frmData.append('id', $('#hdnid').val());
        frmData.append('feature_name', 'ModelImage');

        frmData.append('is_active', is_active);
        $("#dvModelProgress").show();
         
        ajaxReqforFileUpload('Equipment/UploadModleImage', frmData, true, function (resp) {
            if (resp.status == "OK") {
                //app.UserManual.getAttachmentFiles(systemId, entityType);
                $("#dvModelProgress").hide();
                alert(resp.message, 'success', 'success');
                setTimeout(function () {
                    window.location = appRoot + 'Equipment/ViewModelsImage';
                }, 3000);
                if ($('#DocUpload')[0] != undefined)
                    $('#DocUpload')[0].value = '';
            }
            else {
                $("#dvModelProgress").hide();
                alert(resp.message, 'warning');

            }

        }, true);

    },
    validateDocumentFileType: function () {
        var validFilesTypes = ["svg", "SVG"];
        var file = $('#DocUpload').val();
        var filepath = file;
        return app.ModelImage.ValidateFileType(validFilesTypes, filepath);
    },
    ValidateFileType: function (validFilesTypes, filepath) {
        var ext = filepath.substring(filepath.lastIndexOf(".") + 1, filepath.length).toLowerCase();
        var isValidFile = false;
        for (var i = 0; i < validFilesTypes.length; i++) {
            if (ext == validFilesTypes[i]) {
                isValidFile = true;
                break;
            }
        }
        if (!isValidFile) {
            alert("Invalid File. Please upload a File with	File Name" +
                " extension:\n\n" + validFilesTypes.join(", "), 'warning');
        }
        return isValidFile;
    },


}
function DeleteISPModelImageById(id) {
     

    confirm("Are you sure you want to delete this record?", function () {
        ajaxReq('Equipment/DeleteISPModelImageById', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                alert(resp.message, 'Success', 'success');
                setTimeout(function () {
                    $("#frmViewISPModelImage").submit();
                }, 3000);

            }
            else
                alert(resp.message);
        }, true, true);
    });
}
function OpenUploadImageDocumentModel(id) {
     
    popup.LoadModalDialog('VendorSepcification/UploadVendorSpecificationImageDoc', { id: id }, 'Upload Image/Document', 'modal-md');

}
this.SpecificationDoc = {

    upload: function (id) {
         

        if ($('#fuDocumentUpload').val() == '') {
            alert("Please select a file to upload!");
        }
        else {
            app.SpecificationDoc.uploadDocumentFile(id);
        }
    },
    uploadDocumentFile: function (id) {
         
        var frmData = new FormData();
        var filesize = $('#hdnMaxFileUploadSizeLimit').val();

        var Sizeinbytes = filesize * 1024;
        if ($('#fuDocumentUpload').get(0).files[0].size > Sizeinbytes) {

            alert($.validator.format("File size is too large. Maximum file size allowed is <b> {0} </b> MB !", (filesize / 1024).toFixed(2)));
        }
        else {

            var uploadedfile = $('#fuDocumentUpload').get(0).files[0];
            if (!app.SpecificationDoc.validateDocumentFileType()) {
                return false;
            }
            frmData.append(uploadedfile.name, uploadedfile);
            frmData.append('id', id);
            frmData.append('feature_name', 'VendorSpecification');
            frmData.append('document_type', 'Document');
            /*frmData.append('is_active', $('#chkis_active').val());*/
             
            ajaxReqforFileUpload('VendorSepcification/CheckFileExist', frmData, true, function (resp) {

                if (resp.status == "OK") {

                    ajaxReqforFileUpload('VendorSepcification/UploadDocument', frmData, true, function (resp) {
                        if (resp.status == "OK") {
                            //app.UserManual.getAttachmentFiles(systemId, entityType);
                            alert(resp.message, 'success', 'success');

                            app.getSpecificationImgDoc(id, 'documents');

                            if ($('#fuDocumentUpload')[0] != undefined)
                                $('#fuDocumentUpload')[0].value = '';
                        }
                        else {
                            alert(resp.message, 'warning');
                        }

                    }, true);
                }
                else if (resp.status == "DUPLICATE_EXIST") {
                    confirm(resp.message, function () {

                        ajaxReqforFileUpload('VendorSepcification/UploadDocument', frmData, true, function (resp) {
                            if (resp.status == "OK") {
                                //app.UserManual.getAttachmentFiles(systemId, entityType);
                                alert(resp.message, 'success', 'success');

                                app.getSpecificationImgDoc(id, 'documents');

                                if ($('#fuDocumentUpload')[0] != undefined)
                                    $('#fuDocumentUpload')[0].value = '';
                            }
                            else {
                                alert(resp.message, 'warning');
                            }

                        }, true);
                    });
                }
                else {
                    console.log("MSG false:" + resp.message);

                    alert(resp.message);
                }
            }, true);

        }
    },
    validateDocumentFileType: function () {

        var validFilesTypes = ["dwg", "pdf", "jpeg", "jpg", "doc", "docx", "xls", "xlsx", "csv", "vsd", "ppt", "pptx", "png", "htm", "html"];
        var file = $('#fuDocumentUpload').val();
        var filepath = file;
        return app.SpecificationDoc.ValidateFileType(validFilesTypes, filepath);
    },
    ValidateFileType: function (validFilesTypes, filepath) {

        var ext = filepath.substring(filepath.lastIndexOf(".") + 1, filepath.length).toLowerCase();
        var isValidFile = false;
        for (var i = 0; i < validFilesTypes.length; i++) {
            if (ext == validFilesTypes[i]) {
                isValidFile = true;
                break;
            }
        }
        if (!isValidFile) {
            alert("Invalid File. Please upload a File with	File Name" +
                " extension:\n\n" + validFilesTypes.join(", "), 'warning');
        }
        return isValidFile;
    },
}
this.specificationimage = {

    upload: function (id) {


        if ($('#fuImgUpload').val() == '') {
            alert("Please select an image to upload!");
        }
        else {
            app.specificationimage.uploadspecificationimage(id);
        }
    },
    uploadspecificationimage: function (id) {
         
        //get file from uploader and prepare form data to post.
        var frmdata = new FormData();
        var id = id;
        var filesize = $('#hdnMaxFileUploadSizeLimit').val();
        var sizeinbytes = filesize * 1024;
        if ($('#fuImgUpload').get(0).files[0].size > sizeinbytes) {
            alert($.validator.format("Image size is too large. Maximum image size allowed is <b> {0} </b> MB !", (filesize / 1024).toFixed(2)));
        }
        else {
            var uploadedfile = $('#fuImgUpload').get(0).files[0];;

            if (!app.specificationimage.validateimagefiletype()) { return false; }
            frmdata.append(uploadedfile.name, uploadedfile);
            frmdata.append('id', id);
            frmdata.append('feature_name', 'VendorSpecification');
            frmdata.append('document_type', 'Image');

            ajaxReqforFileUpload('VendorSepcification/CheckFileExist', frmdata, true, function (resp) {
                 
                if (resp.status == "OK") {
                    // $('#dvImgDocImgProgress').addClass('.ImgDocLinkProgress');
                    ajaxReqforFileUpload('VendorSepcification/UploadImage', frmdata, true, function (resp) {

                        if (resp.status.toUpperCase() == "OK") {
                             
                            alert(resp.message, 'success', 'success');
                            app.getSpecificationImgDoc(id, 'Images');

                            if ($('#fuImgUpload')[0] != undefined)
                                $('#fuImgUpload')[0].value = '';
                        }
                        else {
                             
                            alert(resp.message, 'warning');

                        }
                        // $('#dvImgDocImgProgress').removeClass('.ImgDocLinkProgress');
                    }, true);
                }
                else if (resp.status == "DUPLICATE_EXIST") {
                    confirm(resp.message, function () {

                        ajaxReqforFileUpload('VendorSepcification/UploadImage', frmdata, true, function (resp) {
                            if (resp.status.toUpperCase() == "OK") {
                                 
                                alert(resp.message, 'success', 'success');
                                app.getSpecificationImgDoc(id, 'Images');

                                if ($('#fuImgUpload')[0] != undefined)
                                    $('#fuImgUpload')[0].value = '';
                            }
                            else {
                                 
                                alert(resp.message, 'warning');

                            }

                        }, true);
                    });
                }
                else {
                    console.log("MSG false:" + resp.message);

                    alert(resp.message);
                }
            }, false);

        }
    },
    validateimagefiletype: function () {

        var validfilestypes = ["bmp", "gif", "png", "jpg", "jpeg"];
        var file = $('#fuImgUpload').val();
        var filepath = file;
        return app.specificationimage.ValidateFileType(validfilestypes, filepath);
    },
    ValidateFileType: function (validFilesTypes, filepath) {
        var ext = filepath.substring(filepath.lastIndexOf(".") + 1, filepath.length).toLowerCase();
        var isValidFile = false;
        for (var i = 0; i < validFilesTypes.length; i++) {
            if (ext == validFilesTypes[i]) {
                isValidFile = true;
                break;
            }
        }
        if (!isValidFile) {
            //Invalid File. Please upload a File with
            alert('Invalid File.Please upload a File with' +
                " extension:\n\n" + validFilesTypes.join(", "), 'warning');
        }
        return isValidFile;
    },

}
/* for refreshing the div after delete and upload*/
this.getSpecificationImgDoc = function (specification_id, type) {
     
    if (type.toUpperCase() == 'IMAGES') {
        ajaxReq('VendorSepcification/GetImageDetails', { specification_Id: specification_id }, false, function (resp) {
            $("#existingImages").html(resp);
            $("#existingImages").css('background-image', 'none');

        }, false, false);
    }
    else if (type.toUpperCase() == 'DOCUMENTS') {
        ajaxReq('VendorSepcification/GetDocumentDetails', { specification_Id: specification_id }, false, function (resp) {
            $("#existingDocument").html(resp);
            $("#existingDocument").css('background-image', 'none');
        }, false, false);
    }
    else {
        ajaxReq('VendorSepcification/getReferenceLink', { specification_Id: specification_id }, false, function (resp) {
            $("#ExistingReflink").html(resp);
            $("#ExistingReflink").css('background-image', 'none');

        }, false, false);
    }
}
this.deleteSpecificationImgDoc = function (type, id, specification_id) {
     
    //Ready the list of selected images for delete

    if (type.toUpperCase() == 'IMAGES') {
        var func = function () {
            ajaxReq('VendorSepcification/DeleteSpecificationImgDoc', { Id: id }, true, function (j) {
                alert(j.message, 'success', 'success');
                app.getSpecificationImgDoc(specification_id, 'Images');

            }, true, true)
        };
        showConfirm('Are you sure you want to delete this image ?', func);

    }
    else if (type.toUpperCase() == 'DOCUMENT') {
        var func = function () {
            ajaxReq('VendorSepcification/DeleteSpecificationImgDoc', { Id: id }, true, function (j) {
                alert(j.message, 'success', 'success');
                app.getSpecificationImgDoc(specification_id, 'Documents');
            }, true, true)
        };
        showConfirm('Are you sure you want to delete this file?', func);

    }
    else {
        var func = function () {
            ajaxReq('VendorSepcification/DeleteSpecificationImgDoc', { Id: id }, true, function (j) {
                alert(j.message, 'success', 'success');
                app.getSpecificationImgDoc(specification_id, 'RefLink');
            }, true, true)
        };
        showConfirm('Are you sure you want to delete this RefLink?', func);
    }
}
this.downloadSpecificationImgDocLink = function (type, id) {
     
    if (type.toUpperCase() == 'REFLINK') {
         
        window.location.href = 'DownlTextFormatFile?' + 'specification_id=' + 0 + '&feature_name=' + "VendorSpecification" + '&DocType=' + type + '&id=' + id;
    }
    else {
        window.location.href = 'DownloadFileById?json=' + JSON.stringify(id);
    }

}
this.downloadAll = function (specification_id, type) {
     
    if (type.toUpperCase() == 'REFLINK') {
         
        window.location.href = 'DownlTextFormatFile?' + 'specification_id=' + specification_id + '&DocType=' + type + '&feature_name=' + "VendorSpecification" + '&id=' + 0;
    }
    else {
        window.location.href = 'DownloadFiles?json=' + specification_id + '&type=' + type;
    }
}
this.uploadRefLink = function (specification_id) {
     

    // var msg = $('#DocrefLinkText').val().trim() == "" ? "Display text is required" : $('#DocrefLink').val().trim() == "" ? "Refernce link is required" : "";
    if ($('#DocrefLinkText').val().trim() != "") {
        if ($('#DocrefLink').val().trim() != "") {
            $('#DocrefLinkText').removeClass("NotValid");
            $('#DocrefLink').removeClass("NotValid");
            var url = $("#DocrefLink").val().trim();
            var pattern = /^(http|https)?:\/\/[a-zA-Z0-9-\.]+\.[a-z]{2,4}/;
            if (pattern.test(url)) {
                var frmdata = new FormData();
                frmdata.append('specification_id', specification_id);
                frmdata.append('feature_name', 'VendorSpecification');
                frmdata.append('refDisplayTxt', $('#DocrefLinkText').val());
                frmdata.append('refLink', $('#DocrefLink').val());
                ajaxReqforFileUpload('VendorSepcification/UploadRefLink', frmdata, true, function (resp) {
                    if (resp.status == "OK") {
                        app.getSpecificationImgDoc(specification_id, 'Reflink');
                        alert(resp.message, 'success', 'success');
                        document.getElementById("DocrefLink").value = '';
                        document.getElementById("DocrefLinkText").value = '';

                    }
                    else {
                        alert(resp.message);
                    }

                }, true);
            }
            else {
                alert("Invalid reference link! <br> (Ex: 'http or https://www.xyz.com')");
            }
        }
        else {
            $('#DocrefLink').addClass("NotValid");
            return false;
        }
    }
    else {
        $('#DocrefLinkText').addClass("NotValid");
        return false;
    }
}
this.ResetDocumentFile = function () {
    document.getElementById("fuDocumentUpload").value = '';
}
this.ResetImage = function () {
    document.getElementById("fuImgUpload").value = '';

}
this.ResetRefLink = function () {
    document.getElementById("DocrefLink").value = '';
    document.getElementById("DocrefLinkText").value = '';

}
//-------------------------------End---------------------------------------------------------------------------------------------------------------------
//----------------------------------Port Status----------------------------------------------------------------------------------------------------
function AddNewPortStatus(_id) {
     
    popup.LoadModalDialog('Miscellaneous/AddPortStatus', { id: _id }, 'Add Port Status', 'modal-md');
}
function DeletePortStatusById(id) {
     

    confirm("Are you sure you want to delete this record?", function () {
        ajaxReq('Miscellaneous/DeletePortStatusById', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                alert(resp.message, 'Success', 'success');
                setTimeout(function () {
                    $("#frmViewPortStatus").submit();
                }, 3000);

            }
            else
                alert(resp.message);
        }, true, true);
    });
}
//--------------------------------------------END--------------------------------------------------------------------
//----------------------------------------Layer Icon Master----------------------------------------------------------
function AddNewlayerIcon(_id) {
     
    popup.LoadModalDialog('Miscellaneous/UploadNewLayerIcon', { id: _id }, 'Add Layer Icon', 'modal-md');
}
this.LayerIcon = {

    upload: function () {
         

        if ($('#ddlLayerName').val() == "") {
            $('#ddlLayerName').next().addClass('errorClass');
            return false;
        }
        else {
            $('#ddlLayerName').next().removeClass("errorClass");
        }

        if ($('#DocUpload').val() == '' && $('#hdnid').val() == 0) {
            alert("Please select a file to upload!");
        }
        else {
            app.LayerIcon.uploadDocumentFile();
        }
    },
    uploadDocumentFile: function () {
         
        var frmData = new FormData();
        var filesize = 10;
        var Sizeinbytes = filesize * 1024;
        if ($('#hdnid').val() == 0) {
            if ($('#DocUpload').get(0).files[0].size > Sizeinbytes) {

                alert($.validator.format("File size is too large. Maximum file size allowed is <b> {0} </b> MB !", (filesize / 1024).toFixed(2)));
                return false;
            }


            var uploadedfile = $('#DocUpload').get(0).files[0];
            if (!app.LayerIcon.validateDocumentFileType()) {
                return false;
            }
            frmData.append(uploadedfile.name, uploadedfile);
        }
        else { frmData.append('name', ''); }
        var status = $("#chkis_LayerIconStatus").prop("checked");
        var isvirtual = $("#chkisvirtual").prop("checked");
        frmData.append('layer_id', $('#ddlLayerName').val());
        frmData.append('network_status', $('#ddlnetworkStatus').val());
        frmData.append('Category', $('#ddlCategory').val());
        frmData.append('network_status_text', $('#ddlnetworkStatus :selected').text());
        frmData.append('id', $('#hdnid').val());
        frmData.append('feature_name', 'ModelImage');

        frmData.append('status', status);
        frmData.append('isvirtual', isvirtual);
        $("#dvModelProgress").show();
         
        ajaxReqforFileUpload('Miscellaneous/UploadLayerIcon', frmData, true, function (resp) {
            if (resp.status == "OK") {
                //app.UserManual.getAttachmentFiles(systemId, entityType);
                $("#dvModelProgress").hide();
                alert(resp.message, 'success', 'success');
                setTimeout(function () {
                    window.location = appRoot + 'Miscellaneous/LayerIconMapping';
                }, 3000);
                if ($('#DocUpload')[0] != undefined)
                    $('#DocUpload')[0].value = '';
            }
            else {
                $("#dvModelProgress").hide();
                alert(resp.message, 'warning');

            }

        }, true);

    },
    validateDocumentFileType: function () {
        var validFilesTypes = ["dwg", "pdf", "jpeg", "jpg", "doc", "docx", "xls", "xlsx", "csv", "vsd", "ppt", "pptx", "png"];

        var file = $('#DocUpload').val();
        var filepath = file;
        return app.LayerIcon.ValidateFileType(validFilesTypes, filepath);
    },
    ValidateFileType: function (validFilesTypes, filepath) {
        var ext = filepath.substring(filepath.lastIndexOf(".") + 1, filepath.length).toLowerCase();
        var isValidFile = false;
        for (var i = 0; i < validFilesTypes.length; i++) {
            if (ext == validFilesTypes[i]) {
                isValidFile = true;
                break;
            }
        }
        if (!isValidFile) {
            alert("Invalid File. Please upload a File with	File Name" +
                " extension:\n\n" + validFilesTypes.join(", "), 'warning');
        }
        return isValidFile;
    },


}
function DeleteISPModelImageById(id) {
     

    confirm("Are you sure you want to delete this record?", function () {
        ajaxReq('Equipment/DeleteISPModelImageById', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                alert(resp.message, 'Success', 'success');
                setTimeout(function () {
                    $("#frmViewISPModelImage").submit();
                }, 3000);

            }
            else
                alert(resp.message);
        }, true, true);
    });
}
//---------------------------------------------------End-------------------------------------------------------------
this.LayerActionColumn = function () {
     
    $('#tblLayerGrid thead tr th a').each(function () {
        var html = $(this).html();
        $(this).parent().attr('data-column-name', html);

    })
    $('#tblLayerGrid tbody tr td').each(function () {
        var html = $(this).html();
        $(this).attr('data-value', html);
    })

    $('#tblLayerGrid  tbody tr').each(function (index, value) {
         
        var colIndex = $('#tblLayerGrid thead tr th[data-column-name="' + "status" + '"]').index();

        var fiber_link_status = $($(this).children('td')[colIndex]).text().trim();

        $('#tblLayerGrid thead tr th:eq(0)').html( "Action");

         
        var layer_id = $('#tblLayerGrid tbody tr:eq(' + index + ') td:eq(0)').attr('data-value');
        var rowAction = ' <a href="javascript:void(0);" id="lnkdownloadLink' + index + '" style = "color:blue;" onclick=ReadMoreLayer(' + layer_id + ')>Read More</a>';
        rowAction = rowAction + '<a href ="javascript:void(0);" onclick=EditLayers(' + layer_id + ') class="auto" title = "Edit" > <i class="fa fa-pencil action_icon fa-fw m-r-xs"></i></a >';

        //rowAction = rowAction + '<a href = @Url.Action("AddLayerDetailSettings",new{id="'+layer_id +'"} class="auto" title = "Edit" > <i class="fa fa-pencil action_icon fa-fw m-r-xs" style="color: #428600;"></i></a >';
        $('#tblLayerGrid tbody tr:eq(' + index + ') td:eq(0)').html(rowAction);
    });
}

function EditLayers(layer_id) {
     
    window.open(appRoot + '/LayerSettings/AddLayerDetailSettings/?layer_id=' + layer_id, "_self");
    //window.open(appRoot + '/AdvancedSettings/ViewGlobalSettings/?msg=' + response.message + '&status=' + response.status, "_self");
}

this.LandBaseSettings = {

    upload: function () {
         
        if ($('#layer_name').css("border-color") === "rgb(255, 0, 0)") {
            return false;
        }
        if ($('#layer_name').val() == "") {
            $('#layer_name').addClass('input-validation-error').removeClass('valid');
            return false;
        }
        if ($('#ddlgeom_type').val() == "") {
            $('#ddlgeom_type').css({ "border": "1px solid red", "border-radius": "0.25rem" });
            return false;
        }
        else if ($('#map_seq').val() == "") {
            $('#map_seq').addClass('input-validation-error');
            return false;
        }
        else if ($('#map_border_thickness').val() == "") {
            $('#map_border_thickness').addClass('input-validation-error');
            return false;
        }
        else if ($('#layer_abbr').val() == "") {
            $('#layer_abbr').addClass('input-validation-error');
            return false;
        }
        else if ($('#map_abbr').val() == "") {
            $('#map_abbr').addClass('input-validation-error');
            return false;
        }
        else if ($('#txtmap_border_color').val() == "#ffffff" || $('#txtmap_border_color').val() == '') {
            alert("Please select Map Border Color!");
            return false;
        }
        else if ($('#txtColorCode').val() == "#ffffff" || $('#txtColorCode').val() == '') {
            alert("Please select Map Background Color!");
            return false;
        }
        else if ($('#map_bg_opacity').val() == '') {
            $('#map_bg_opacity').css({ "border": "1px solid red", "border-radius": "0.25rem" });
            return false;
        }
        else if ($('#ddlnetwork_id_type').val() == "") {
            $('#ddlnetwork_id_type').css({ "border": "1px solid red", "border-radius": "0.25rem" });
            return false;
        }
        else if ($('#ddlnetwork_code_seperator').val() == "") {
            $('#ddlnetwork_code_seperator').css({ "border": "1px solid red", "border-radius": "0.25rem" });
            return false;
        }
        else if ($('#DocUpload').val() == '' && $('#ddlgeom_type').val().toUpperCase() == 'POINT' && $("input[name='rdbIconType']:checked").val() == "true"
            && document.getElementById('uploadedImage').src == '') {
            alert("Please select an icon to upload!");
        }


        else {
            var frmData = new FormData();
            var filesize = $('#hdnMaxLandbaseFileUploadSizeLimit').val();
            var Sizeinbytes = filesize * 1024;
            //if ($('#hdn_id').val() == 0) {
            if ($("input[name='rdbIconType']:checked").val().toUpperCase() == "TRUE" && $('#DocUpload').val() != '') {
                if ($('#DocUpload').get(0).files[0].size > Sizeinbytes) {

                    alert($.validator.format("File size is too large. Maximum file size allowed is <b> {0} </b> KB !", (filesize)));
                    return false;
                }
                var uploadedfile = $('#DocUpload').get(0).files[0];
                if (!app.LayerIcon.validateDocumentFileType()) {
                    return false;
                }
                frmData.append(uploadedfile.name, uploadedfile);
            }
            else {
                 
                var iconPath = $('#hdnIconPath').val().replace($("#hdnFTPServiceURL").val(), '');
                frmData.append('icon_path', iconPath);
                frmData.append('icon_name', iconPath.replace('icons/Landbase/', ''));
            }
            //}
            frmData.append('id', $('#hdn_id').val());
            frmData.append('layer_name', $('#layer_name').val());
            frmData.append('geom_type', $('#ddlgeom_type').val());
            frmData.append('layer_abbr', $('#layer_abbr').val());
            frmData.append('map_abbr', $('#map_abbr').val());
            frmData.append('map_border_color', $('#txtmap_border_color').val());
            frmData.append('map_seq', $('#map_seq').val());
            frmData.append('map_border_thickness', $('#map_border_thickness').val());
            frmData.append('map_bg_color', $('#txtColorCode').val());
            frmData.append('map_bg_opacity', $('#map_bg_opacity').val());
            frmData.append('network_id_type', $('#ddlnetwork_id_type').val());
            frmData.append('network_code_seperator', $('#ddlnetwork_code_seperator').val());
            frmData.append('is_active', $("#is_active").prop("checked"));
            frmData.append('is_center_line_enable', $("#is_center_line_enable").prop("checked"));
            frmData.append('is_icon_display_enabled', $("input[name='rdbIconType']:checked").val());

            $("#dvModelProgress").show();
             
            ajaxReqforFileUpload('LandbaseSettings/SaveLayer', frmData, true, function (resp) {
                if (resp.pageMsg.status == "OK") {
                    $("#dvModelProgress").hide();
                    alert(resp.pageMsg.message, 'success', 'success');
                    setTimeout(function () {
                        window.location.reload();
                    }, 2000);
                }
                else {
                    $("#dvModelProgress").hide();
                    alert(resp.pageMsg.message, 'warning');

                }

            }, true);
        }
    }
}

//--------------------------------------------------------------------TerminationPoint-=---------------------------------------------------------
function AddNewTicketType(_id) {
     
    popup.LoadModalDialog('Miscellaneous/AddNewTicketType', { id: _id }, 'Add Ticket Type', 'modal-md');
}

function EditConfigurationSettings(_id) {
    popup.LoadModalDialog('ConfigurationSettings/EditConfigurationSettings', { id: _id }, 'Edit Configuration Setting', 'modal-md');
}

function DeleteTicketTypeById(id) {
     

    confirm("Are you sure you want to delete this record?", function () {
        ajaxReq('Miscellaneous/DeleteTicketTypeById', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                alert(resp.message, 'Success', 'success');
                setTimeout(function () {
                    $("#frmViewticketType").submit();
                }, 3000);

            }
            else
                alert(resp.message);
        }, true, true);
    });
}
this.checkCblTotalCore = function () {
     
    var ddlFiberCount = $("#ddlPortRatio").val();
    var tubeCount = parseInt($('#no_of_tube_val').val());
    var corePerTubeCount = parseInt($('#no_of_core_per_tube_val').val());

    if (ddlFiberCount == "Select Fiber Count") {
        //alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_039);
        alert("Please select Fiber Count!");
        return false;
    }
    else {
        var totalCount = tubeCount * corePerTubeCount;
        ddlFiberCount = ddlFiberCount.split(' ');
        if (totalCount != ddlFiberCount[0]) {
            alert("Multiple of no. of tube and no. of core per tube should be equal to fiber count !");
            return false;
        }
    }
}
this.checkCblLength = function () {
     
    var measureLen = parseFloat($("#cable_measured_length").val());
    var calculatedLen = parseFloat($("#cable_calculated_length").val());

    if (calculatedLen < measureLen) {
        var layerTitle = getLayerTltle("Cable");
        alert($.validator.format(MultilingualKey.SI_OSP_CAB_JQ_FRM_001, layerTitle, layerTitle));
        return false;
    }
}
this.SearchActionNames = function () {
     
    srchText = $("#txtSearchText").val();
    if (srchText.length > 2) {
        $("#tblReportColumnsInfo tbody tr").each(function () {
            $("#hdnNoRecFound").show();
             
            // ($('td', $(this))[1].find("").val().toLowerCase().indexOf(srchText.toLowerCase()) > -1)
            if (($('td', $(this))[0].innerHTML.toLowerCase().indexOf(srchText.toLowerCase()) > -1) || ($('td', $(this))[1].innerHTML.toLowerCase().indexOf(srchText.toLowerCase()) > -1)) {
                $(this).show();
                $("#hdnNoRecFound").hide();
            }
            else {
                $(this).hide();
            }
        });
    }
    else {
        $("#tblReportColumnsInfo tbody tr").show();
        $("#hdnNoRecFound").hide();
    }
}
this.SearchAttributes = function () {
     
    srchText = $("#txtSearchAttribute").val();
    if (srchText.length > 1) {
        $("#UlAttribute li").each(function () {
            if ($(this).text().search(new RegExp(srchText, "i")) < 0) {
                $(this).hide();
                $('#selectLi').show();
            }
            else {
                $(this).show();
            }
        });
    }
    else {
        $("#UlAttribute li").show();
    }
}


/*js for table footer*/

$(document).ready(function () {
    setWebGridCurrentPageStyle();
});

$(document).ajaxStop(function () {
    setWebGridCurrentPageStyle();
});
function setWebGridCurrentPageStyle() {
    $("tfoot td ")
        .contents()
        .filter(function () {
            if (this.nodeType === 3 && this.length > 1) {
                return this.nodeType
            }
        }).wrap('<span class="GridCurrentPage" />');
}
function LogoutAllMobileUsers() {

    confirm("All users will be logged out from Mobile device, Do you want to continue ?", function () {
        ajaxReq('User/LogoutUser', {}, false, function (resp) {
            if (resp.status == "OK") {
                $("#frmViewUsers").submit();
            }
            else
                alert(resp.message);
        }, true, true);
    });
}

function LogoutUser(id, application_access, event, e) {
    e.preventDefault();

    var item_id = '#Logout_' + id;
    applicationAccess = $('#ddlApplication').val();
    var msg = event.checked == true ? "User not Logged in Mobile Device yet." : "User will be logged out from Mobile Device ,Do you want to continue?";

    //var csts = $(item_id).is(":checked");
    showConfirm(msg, function () {


        ajaxReq('User/LogoutUser', { user_id: id, application_access: application_access }, true, function (resp) {
            if (resp.status == true) {
                alert(resp.message, 'Success', 'success');
                setTimeout(function () {

                    $("#frmViewUsers").submit();
                }, 1000);
                return $(item_id).is(":checked") == true ? $(item_id).prop('checked', false) : $(item_id).prop('checked', true);
            }
            else
                alert(resp.message);
        }, true, true);
    });
}
function onChangeRRMId() {
    var multi_reporting_role_ids = $("#ddlroles").val();
    if ($("#ddlroles").val()) {
        $("#ddlroles_chosen").removeClass("highlight_chosen_select");
        //if ($("#ismutiRRmanagar").val() == "True") {
            $('#multi_reporting_role_ids').val($("#ddlroles").val().join(","));
            multi_reporting_role_ids = $("#ddlroles").val().join(",")
       // }

    }

}
$(document).ready(function () {
    var multi_reporting_role_ids = $("#multi_reporting_role_ids").val();
    if ($("#ddlroles").length) {
        $("#ddlroles").val(multi_reporting_role_ids.split(',')).trigger("chosen:updated");
    }
});
//---------------------fetools  start
function Deletefetool(id) {

    confirm("Are you sure you want to delete this User Name Record?", function () {
        ajaxReq('FeTools/DeleteFettolsSpecification', { id: id }, false, function (resp) {
            if (resp.status == "OK") {
                $("#frmViewfetools").submit();
                alert('User Name deleted successfully.');
            }
            else
                alert(resp.message);
        }, true, true);
    });
}

function fn_accepted_user_tools(id) {
   

    confirm("Are you sure you want to confirm this tool?", function () {
 
        ajaxReq('FeTools/AcceptedUserTools', { id: id }, false, function (resp) {
     
            if (resp.status == "OK") {
                $("#frmViewfetools").submit();
                alert('User Tool accepted successfully.');
            }
            else {
                alert(resp.message);
            }
        }, true, true);
       

    });
}


function handleFileChange() {

    uploadFEImagedownload();
    upload_fetools_document();
}
function uploadFEImagedownload() {

    var frmData = new FormData();
    var uploadedfile = $('#fuImgUpload12')[0].files;
    if (uploadedfile.length > 0) {
        for (var i = 0; i < uploadedfile.length; i++) {
            frmData.append('images[]', uploadedfile[i]);
            
        }
        frmData.append('document_type', 'Image');

        ajaxReqforFileUpload('FeTools/UploadFTEImage', frmData, true, function (resp) {
            if (resp.status == "OK") {
                //$('#closeModalPopup').trigger("click");
                //app.getElementImages();
                //console.log("MSG true:" + resp.message);
                //alert(resp.message);
            }
            else {
                console.log("MSG false:" + resp.message);

                alert(resp.message);
            }
        }, true);

    }
    //else {
    //    alert(MultilingualKey.SI_OSP_GBL_GBL_GBL_099);
    //    return false;
    //}
}
function upload_fetools_document() {

    var frmData = new FormData();
    var uploadedfile = $('#fuAttachmentUploadfetools')[0].files;
    if (uploadedfile.length > 0) {
        for (var i = 0; i < uploadedfile.length; i++) {
            frmData.append('images[]', uploadedfile[i]);
            
        }
        frmData.append('document_type', 'Image');
        ajaxReqforFileUpload('FeTools/UploadFTEDocument', frmData, true, function (resp) {
            if (resp.status == "OK") {
                //$('#closeModalPopup').trigger("click");
                //app.getElementImages();
                //console.log("MSG true:" + resp.message);
                //alert(resp.message);
            }
            else {
                // console.log("MSG false:" + resp.message);

                alert(resp.message);
            }
        }, true);



    }
    //else {
    //    alert(MultilingualKey.SI_OSP_GBL_GBL_GBL_099);
    //    return false;
    //}
}
//---------------------fetools  end


//-------------------------------End-----------------------------------------------------------------------------------------------------------------------

