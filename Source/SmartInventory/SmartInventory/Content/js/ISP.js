var ISPMain = function () {
            
    var app = this;
    var gMapObj = {};
    this.ParentModel = 'PARENT';
    this.ChildModel = 'CHILD';
    this.gMapObj = {};
    this.isTPPointEnable = true;
    this.lstTerminationPoint = [];
    this.lstISPCablePt = [];
    this.TerminationPoints = '';
    this.reqver = 0;
    this.FloorElement = '<div class="EntityBox"><div class="Element elementDotBox entityInfo " id="divBox" data-additionalattr="" data-entity-type="" data-entity-title="" data-entity-geomtype="Point" data-is-isp-child-layer="" data-system_id="">';
    app.FloorElement += '<span class="dot top"></span>';
    app.FloorElement += '<span class="dot left"></span>';
    app.FloorElement += '<span id="spnEntityName"></span>';
    app.FloorElement += '<span class="dot bottom"></span>';
    app.FloorElement += '<span class="dot right"></span>';
    app.FloorElement += '</div><div class="ParentBox hide"><div class="ParentFirstBox" id=""></div></div></div>';
    app.currentElement = null;
    this.sourceCableId = null;
    this.destinationCableId = null;
    this.parentEntities = [];
    this.DE = {
 
        'leftMenuActions': '#divleftMenu div',
        'ItemTemplate': '.clsTemplateIcon',
        'SearchElement': '#txtSearchElement',
        'ElementList': '.ISPElementList>.ElementboxParent',
        'BaseLocation': '#ddlBaseLocation',
        'ShaftDDLParent': '#divShaftDdl',
        'FloorDDLParent': '#divFloorDdl',
        'ddlShaft': '#ddlShaft',
        'ddlFloor': '#ddlFloor',
        "ElementTemplate": ".ISPElementList .ElementTemplate",
        "ddlParentEntity": "#ddlParentEntity",
        "ParentEntityTab": "#liParent",
        "ChildEntityTab": "#liChild",
        'ShowAllEntity': '#chkSelectAll',
        'SearchNetworkEntity': '#txtSearchNetworkEntity',
        'NetworkLyrEntityList': '.mainlyr>#liNetworkLyrEntity',
        'SortEntity': '.ispShort',
        'ExpendCollapseEntity': '.expendCollapse',
        'alertClose': '.alert-close',
        'NetworkLyrExportBOM': '#NetworkLyrExportBOM',
        'StructureId': '#hdnStructureId',
        'EntityExportSplicing': '#dvEntityInformationDetail #EntityExportSplicing:not(.roledisabled)',
        'EntityCustomerSLD': '#dvEntityInformationDetail #EntityCustomerSLD:not(.roledisabled)',
        'EntityExport': '#dvEntityInformationDetail #EntityExport:not(.roledisabled)',
        'EntityExportWithConnection': '#dvEntityInformationDetail #EntityExportWithConnection:not(.roledisabled)',
        'EntityExportWithoutConnection': '#dvEntityInformationDetail #EntityExportWithoutConnection:not(.roledisabled)',
        'EntityHistory': '#dvEntityInformationDetail #EntityHistory:not(.roledisabled)',
        'EntityDetail': '#dvEntityInformationDetail #EntityDetail:not(.roledisabled)',
        'EntityUpdateGeographicDetails': '#dvEntityInformationDetail #EntityUpdateGeographicDetails:not(.roledisabled)',
        'EntityCustomerDetails': '#dvEntityInformationDetail #EntityCustomerDetails:not(.roledisabled)',
        'EntityInformationDetail': '#dvEntityInformationDetail',
        'FloorDot': '.floor-strip-circle',
        'ShaftDot': '.shaft-strip-circle',
        'FloorLeftControll': '#divFloorLeftControl',
        'FloorRightControll': '#divFloorRightControl',
        'ShaftTopControl': '#divShaftTopControl',
        'ShaftBottomControl': '#divShaftBottomControl',
        'EditFloorName': '.FloorEditIcon',
        'FloorName': '.FloorName',
        'ShaftEditIcon': '.ShaftEditIcon:not(.roledisabled)',
        'ShaftName': '.ShaftName',
        'refreshStructureInfo': '#ancrStructureInfo',
        'ParentEntityBox': '#divParentEntity > .ISPElementList > .ElementboxParent',
        'ChildEntityBox': '#divChildEntity > .ISPElementList > .ElementboxParent',
        'LibraryTab': '.ullibraryEntity >li > a',
        'filterEntity': '.filterEntity',
        'StructureEntityList': '.str-main-table td .EntityBox',
        'EntityInfo': '.entityInfo',
        'EntityDelete': '#EntityDelete:not(.roledisabled)',
        "Cable_A_End": "#ddlCableAEnd",
        "Cable_B_End": "#ddlCableBEnd",
        "CreateISPCable": "#btnCreateISPCable",
        "ISPCableTemplate": "#btnCableTemplate",
        "ISPNetworkLayer": "#divISPNetworkLayer",
        "InformationTool": "#divInformation",
        "entityboxcount": ".entityboxcount",
        "iconCollapseChildElements": ".icon-Collapse_expand",
        "EditFloorInfo": "#btnEditFloor:not(.roledisabled)",
        "EntityShift": "#EntityShift:not(.roledisabled)",
        "SplicingTool": "#divManualSplicing",
        "SplicingDiv": "#SplicingDiv",
        "splicingMain": "#SplicingDiv .splicingMain",
        "SourceCable": "#ddlSourceCable",
        "DestinationCable": "#ddlDestinationCable",
        "ConnectingEntity": "#ddlConnectingEntity",
        "divImage": "#dvEntityInformationDetail #divImage",
        "divDocument": "#dvEntityInformationDetail #divDocument",
        "liImage": "#dvEntityInformationDetail #liImage",
        "liDocument": "#dvEntityInformationDetail #liDocument",
        "UploadImage": "#dvEntityInformationDetail #ImgUpload",
        "DeleteImages": "#dvEntityInformationDetail #divDeleteImages",
        "DownloadImages": "#dvEntityInformationDetail #divDownloadImages",
        "UploadDocument": "#dvEntityInformationDetail #DocUpload",
        "DeleteDocuments": "#dvEntityInformationDetail #divDeleteDocuments",
        "DownloadDocuments": "#dvEntityInformationDetail #divDownloadDocuments",
        "liDetailTab": "#dvEntityInformationDetail #liDetail",
        "EntityLocationEdit": "#EntityLocationEdit:not(.roledisabled)",
        "EntityCancel": "#EntityCancel:not(.roledisabled)",
        "EntitySave": "#EntitySave:not(.roledisabled)",
        "ElementDots": ".elementDotBox .dot",
        "EntityInformationList": "#dvEntityInformationList",
        "tempPath": "#tempPath",
        "entityInfoList": ".entityInfoList",
        "EditShaftInfo": "#btnEditShaft:not(.roledisabled)",
        "AddMoreShaftRange": "#addMore",
        "ShaftInfoTable": "#tblShaftLstInfo",
        "EntityConvertToPlanned": "#dvEntityInformationDetail #EntityConvertToPlanned:not(.roledisabled)",
        "EntityConvertToAsBuilt": "#dvEntityInformationDetail #EntityConvertToAsBuilt:not(.roledisabled)",
        "EntityConvertToDormant": "#dvEntityInformationDetail #EntityConvertToDormant:not(.roledisabled)",
        "DisabledFloor": "shaftdisabled",
        "CablePaths": 'path.entityInfo',
        "mainTable": '.str-main-table',
        "EntitySelected": 'entitySelected',
        'SVGCable': '#svgCable',
        'PathSelected': 'pathAnimation',
        'SVGPatchCable': '#svgPatchCable',
        'circleMarker': '#circleMarker',
        'cableDot': 'span.dot',
        'EntitySplit': '#EntitySplit:not(.roledisabled)',
        'DoorPosition': '.doorPosition',
        'DoorPositionVal': '#doorPosition',
        'DoorClass': 'doorPosition',
        'EntityLogicalView': '#EntityLogicalView:not(.roledisabled)',
        'EntityConvertSCtoCDB': '#EntityConvertSCtoCDB:not(.roledisabled)',
        'EntityConvertCDBtoSC': '#EntityConvertCDBtoSC:not(.roledisabled)',
        "EntityViewAccessories": "#EntityViewAccessories:not(.roledisabled)",
        //"portRect": '.element-rect.port',
        "portRect": 'svg[data-port-status=2]',
        "connectionDV": '#dvConnection',
        "TabNavigateRight": ".go-right",
        "TabNavigateLeft": ".go-left",
        "liRefLink": "#dvEntityInformationDetail #liRefLink",
        "UploadRefLink": "#dvEntityInformationDetail #RefLinkUpload",
        "divRefLink": "#dvEntityInformationDetail #divRefLink"
        }
    this.tableScroll = { top: 0, left: 0 };
    this.responseData = undefined;
    this.convert = {
        toBoolean: function (attr) {
            return JSON.parse(attr.toLowerCase());
        }
    };
    this.EnumEntityType = {
        BDB: 'BDB',
        POD: 'POD',
        Splitter: 'Splitter',
        Spliceclosure: 'SpliceClosure',
        Cable: 'Cable',
        MPOD: 'MPOD',
        Customer: 'Customer',
        ONT: 'ONT',
        HTB: 'HTB',
        FDB: 'FDB',
        UNIT: 'UNIT',
        FMS: 'FMS',
        Structure: 'Structure',
        Rack: 'Rack',
        Equipment: 'Equipment',
        Cabinet: 'Cabinet',
        PatchPanel: 'PatchPanel'
    }
    this.initApp = function () {
        app.bindEvents();
        app.cableActions.resize();
        app.getCPEConnections();
        app.updateAllCablePath();
    }
    this.bindEvents = function () {

        $(document).on('click', app.DE.leftMenuActions, function (e) {
            var panelClass = $(this).data().panelClass;
            $('.leftPanel:not(.' + panelClass + ')').hide();
            $('.' + panelClass).animate({ width: 'toggle' });
            $(app.DE.leftMenuActions).removeClass('activeISPTab');
            $('#divSplicingWindow').hide();
            $(this).addClass('activeISPTab');

            app.clearControl();
            $('.ullibraryEntity>li').removeClass('active');
            $(app.DE.ParentEntityTab + '> a').trigger("click");
            $(app.DE.liDetailTab + '> a').trigger("click");

            if (panelClass == 'clslibraryEntity') {
                $(app.DE.ShowAllEntity).prop('checked', false);
                $(app.DE.ShowAllEntity)[0].click();
                $.each($(".mainlyr  input[type='checkbox']"), function (indx, itm) {
                    $(this).prop('checked', false);
                    $(this)[0].click();
                });
            }
            //remove info events..
            if ($(this).hasClass('neLibrary')) {
                $(app.DE.circleMarker).css('visibility', 'hidden');
                $(app.DE.InformationTool).removeClass("activeToolBar");
                app.DisableInfoTool();

            }
            app.cancelSplice();
        });

        $(document).on('click', app.DE.ItemTemplate, function (e) {
            var attr = $(this).data();
            if (attr.roleId != 3) {
                popup.LoadModalDialog(app.ParentModel, attr.href, { eType: attr.lyrname, cblType: 'ISP' }, attr.title + ' Template', attr.lyrname == app.EnumEntityType.UNIT ? 'modal-sm' : 'modal-lg');
            }
            else {
                alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_016, 'warning');//You are not authorized to create template!
            }
            e.stopPropagation();
        });

        $(document).on('keyup', app.DE.SearchElement, function (e) {
            var entityListObj = '';
            if ($(app.DE.ParentEntityTab).hasClass('active')) {
                entityListObj = '#divParentEntity>.ISPElementList';
            } else if ($(app.DE.ChildEntityTab).hasClass('active')) {
                entityListObj = '#divChildEntity>.ISPElementList';
            }
            app.filterbyBaselocation($(this).val(), entityListObj + '>.ElementboxParent', 'entityType', entityListObj);
        });
        $(document).on('change', app.DE.BaseLocation, function () {
            $(app.DE.ddlShaft).val('').trigger("chosen:updated");
            $(app.DE.ddlFloor).val('').trigger("chosen:updated");
            $(app.DE.ShaftDDLParent).removeClass('dvdisabled');
            var location = $(this).val();
            if (location == 'Floor') { $(app.DE.ShaftDDLParent).addClass('dvdisabled'); }
        })
        $(app.DE.ElementTemplate).on('click', function (e) {
            app.setElement(e);

        });

        $(document).on('change', app.DE.ShowAllEntity, function (e) {
            app.OnOffNetworkLayers(this.checked);
        });

        $(document).on('keyup', app.DE.SearchNetworkEntity, function (e) {
            app.filter($(this).val(), app.DE.NetworkLyrEntityList, 'entityTitle', function () {
                $('#divLyrNoRecordFound').hide();
                $('.clsNetworkLyrActions').show();
                if ($('.mainlyr>.show').length == 0) {
                    $('#divLyrNoRecordFound').show();
                    $('.clsNetworkLyrActions').hide();
                }
            });
        });

        $(document).on('click', app.DE.SortEntity, function (e) {
             
            $(".mainlyr #liNetworkLyrEntity").sort(sort_li).appendTo('.mainlyr');
            function sort_li(a, b) {
                return parseInt($(b).data('position')) < parseInt($(a).data('position')) ? 1 : -1;
            }
            $(this).toggleClass('icon-sort-2').toggleClass('icon-Filter');
        });

        $(document).on('click', app.DE.ExpendCollapseEntity, function (e) {
            $(this).parent().next().animate({ height: 'toggle' });
            $(this).parent().parent().toggleClass('addBackground');
            $(this).toggleClass('icon-expand').toggleClass('icon-collapse');
        });
        $(document).on('click', app.DE.alertClose, function (e) {
            $(this).parent('.allert').animate({ width: 'hide' }, "fast");
        });

        $(document).on('click', app.DE.NetworkLyrExportBOM, function (e) {
            window.location = appRoot + 'Report/ExportISPBOMDetailInExcel?structure_id=' + $(this).data().structureId;
        });
        $(document).on('click', app.DE.EntityLogicalView, function (e) {
            var modelClass = getPopUpModelClass('LogicalView');
            var titleText = $(this).data().entityType + ' Logical View';
            var formURL = 'Splicing/EntityLogicalView';
            popup.LoadModalDialog(app.ParentModel, formURL, { systemId: $(this).data().systemId, entityType: $(this).data().entityType }, titleText, modelClass, function () { });
        });

        $(document).on('click', app.DE.EntityExport, app.layerActions.exportData.download);
        $(document).on('click', app.DE.EntityExportSplicing, app.layerActions.exportSplicing.showSplicingReport);
        $(document).on('click', app.DE.EntityCustomerSLD, app.layerActions.customerSLD.getSLD);

        $(document).on('click', app.DE.EntityConvertSCtoCDB, function (e) {
             
            var attr = $(this).data();
            var strucIdVal = $(app.DE.StructureId).val();
            ajaxReq('Main/GetPointTypeEntityDetails', { pSystemId: attr.systemId, pEntityType: attr.entityType }, true, function (resp) {
                if (resp.geom != null || resp.geom != undefined) {
                    ajaxReq('Main/ValidateItemSpecificaton', { no_of_ports: resp.no_of_ports, entityType: 'CDB', vendor_id: resp.vendor_id }, true, function (result) {
                        if (result.length > 0) {
                            // var func = function () {
                            ajaxReq('Main/ValidateEntityGeom', { geomType: 'Point', enType: 'CDB', txtGeom: resp.geom, isTemplate: false, subEntityType: '' }, true,
                                function (res) {

                                    if (res.status == "OK") {
                                        confirm($.validator.format(MultilingualKey.SI_OSP_CDB_JQ_FRM_002, getLayerTltle(attr.entityType), getLayerTltle('CDB')), function () {

                                            popup.LoadModalDialog('Parent', 'ISP/AddCDB', { networkIdType: 'M', no_of_ports: resp.no_of_ports, vendor_id: resp.vendor_id, geom: resp.geom, isConvert: true, sc_system_id: resp.system_id, ModelInfo: { structureid: strucIdVal } }, 'CDB', 'modal-lg');
                                            if (response.status == "OK") {
                                                alert(response.message);
                                            }
                                            else {
                                                alert(response.message, 'warning');
                                            }
                                        });

                                    }
                                    else {
                                        alert(res.error_message);

                                    }
                                }, true, true);
                        }
                        else {
                            // alert('Specifications are not compatible to convert!');
                            var Msg = '<table border="1" class="alertgrid"><tr><td><b>Vendor Name</b></td><td><b>Total Ports<b/></td></tr>';
                            Msg += '<tr><td>' + resp.vendor_name + ' </td><td> ' + resp.no_of_ports + '</td></tr>';
                            Msg += '</table>';

                            alert($.validator.format(getMultilingualStringValue(MultilingualKey.SI_OSP_CDB_JQ_FRM_001), Msg));

                        }
                    });


                }
                else {
                    alert(resp.message);

                }
            });

        });

        $(document).on('click', app.DE.EntityConvertCDBtoSC, function (e) {
             
            var attr = $(this).data();
            var strucIdVal = $(app.DE.StructureId).val();
            ajaxReq('Main/GetPointTypeEntityDetails', { pSystemId: attr.systemId, pEntityType: attr.entityType }, true, function (resp) {
                if (resp.geom != null || resp.geom != undefined) {
                    ajaxReq('Main/ValidateItemSpecificaton', { no_of_ports: resp.no_of_port, entityType: 'SpliceClosure', vendor_id: resp.vendor_id }, true, function (result) {
                        if (result.length > 0) {
                            // var func = function () {
                            var postData = { systemId: $(this).data().systemId, entityType: attr.entityType, geomType: 'Point' };

                            ajaxReq('Main/getDependentChildElements', postData, true, function (response) {
                                if (response.status == "OK") {

                                    if (response.results.ChildElements.length > 0) {
                                        var Msg = '<table border="1" class="alertgrid"><tr><td><b>Entity</b></td><td><b>Display Name<b/></td></tr>';
                                        $.each(response.results.ChildElements, function (index, item) {
                                            Msg += '<tr><td>' + item.Entity_Type + ' </td><td> ' + item.display_name + '</td></tr>';
                                        });
                                        Msg += '</table>';
                                        alert($.validator.format(getMultilingualStringValue(MultilingualKey.SI_OSP_SBA_JQ_FRM_003), Msg));
                                        //alert(MultilingualKey.SI_OSP_SBA_JQ_FRM_003 + " " + Msg);
                                    }
                                    else {
                                        confirm($.validator.format(MultilingualKey.SI_OSP_CDB_JQ_FRM_002, getLayerTltle(attr.entityType), getLayerTltle('SpliceClosure')), function () {

                                            ajaxReq('ISP/SaveSpliceClosure', { geom: resp.geom, networkIdType: 'A', isDirectSave: true, no_of_ports: resp.no_of_port, vendor_id: resp.vendor_id, isConvert: true, cdb_system_id: resp.system_id, 'objIspEntityMap.structure_id': strucIdVal }, true, function (response) {
                                                if (response.status == "OK") {
                                                    alert(response.message);
                                                }
                                                else {
                                                    alert(response.message, 'warning');
                                                }
                                            })
                                        });
                                    }
                                }
                                else {
                                    alert(response.message);

                                }
                            }, true, true);

                        }
                        else {
                            // alert('Specifications are not compatible to convert!');
                            var Msg = '<table border="1" class="alertgrid"><tr><td><b>Vendor Name</b></td><td><b>Total Ports<b/></td></tr>';
                            Msg += '<tr><td>' + resp.vendor_name + ' </td><td> ' + resp.no_of_port + '</td></tr>';
                            Msg += '</table>';
                            //alert(MultilingualKey.SI_OSP_SC_JQ_FRM_001 + " " + Msg + " " + ' <b>' + MultilingualKey.SI_OSP_GBL_JQ_FRM_035 + '</b>');
                            alert($.validator.format(getMultilingualStringValue(MultilingualKey.SI_OSP_SC_JQ_FRM_001), Msg));
                        }
                    });


                }
                else {
                    alert(resp.message);
                }
            });

        });

        //$(document).on('click', app.DE.EntityExport, function () {   $('#SourceType').val('Eqp'); app.ExportRoomViewDetail('false') });
        //$(document).on('click', app.DE.EntityExportWithoutConnection, function () {   $('#SourceType').val('Equipment'); app.ExportRoomViewDetail('false') });
        //$(document).on('click', app.DE.EntityExportWithConnection, function () {   $('#SourceType').val('Equipment'); app.ExportRoomViewDetail('true') });



        $(document).on('click', app.DE.EntityHistory, app.layerActions.history.get);
        
        $(document).on('click', app.DE.EntityDetail, app.layerActions.detail.get);
        //connected customer details
        $(document).on('click', app.DE.EntityCustomerDetails, app.layerActions.ConnectedCustomerDetails.get);

        $(document).on('click', app.DE.EntityUpdateGeographicDetails, app.layerActions.UpdateGeographicDetails.get);

        $(document).on('click', app.DE.EntityLocationEdit, app.layerActions.entity.edit)
        $(document).on('click', app.DE.EntityCancel, app.layerActions.entity.cancel);

        $(document).on('click', app.DE.EntitySave, app.layerActions.entity.save);

        $(document).on('click', app.DE.FloorDot, function () {
            //$(this).toggleClass('dark-strip-color');
            //$(this).prev().find(app.DE.FloorLeftControll).toggle('slide', { direction: 'right' }, 500);
            //$(this).next().find(app.DE.FloorRightControll).toggle('slide', { direction: 'left' }, 500);
        })
        $(document).on('click', app.DE.ShaftDot, function () {
            //$(this).toggleClass('dark-strip-color');
            //$(this).next().find(app.DE.ShaftBottomControl).toggle('slide', { direction: 'up' }, 500);
            //$(this).prev().find(app.DE.ShaftTopControl).toggle('slide', { direction: 'down' }, 500);

        })
        $(document).on('click', app.DE.entityboxcount, function () {
            // $(this).next('.entityParentBox').show('slide', { direction: 'left' }, 500); 
            $('.Element').css("z-index", "8");
            $('.str-shaft .entityParentBox').hide();
            $(this).prev().css("z-index", "15");
            $(this).next('.entityParentBox').animate({ width: 'toggle', height: 'toggle' }, 500);

        })
        $(document).on('click', app.DE.iconCollapseChildElements, function () {
            $(this).parent('.entityParentBox').animate({ width: 'toggle', height: 'toggle' }, 500, function () {
                $(this).parent().find('.Element').css("z-index", "8");
            });

        })
        $(document).on('click', app.DE.EditFloorName, function () {
            $(app.DE.FloorName).addClass('labledTextbox');
            var floorObj = $(this).prev('.FloorName');
            floorObj.val(floorObj.data().currentName).removeClass('labledTextbox').focus();
        })

        $(document).on('click', "#dropdown", function (e) {
            $(".dropMenu").slideToggle();
            $(".arrowDropUp").toggle();
        });

        $(document).on('click', '.dropMenu ul li', function (e) {
            $('.dropMenu ul li').removeClass("activeSideMenu ");
            $(this).addClass("activeSideMenu");
        });

        $(document).on('click', app.DE.TabNavigateRight, function (e) {
            var pos = $('div.overflow-hidden').scrollLeft() + 160;
            $('div.overflow-hidden').css("scroll-behavior", "smooth");
            $('div.overflow-hidden').scrollLeft(pos);
        });
        $(document).on('click', app.DE.TabNavigateLeft, function (e) {
            var pos = $('div.overflow-hidden').scrollLeft() - 160;
            $('div.overflow-hidden').css("scroll-behavior", "smooth");
            $('div.overflow-hidden').scrollLeft(pos);
        });

        //$(document).on('paste', app.DE.FloorName, function (e) {
        //    if (app.trim($(this).val()).length > 20) {
        //        e.preventDefault();
        //    }
        //})
        //$(document).on('blur', app.DE.FloorName, function (e) {
        //    var attr = $(this).data();
        //    if (app.trim($(this).val()) == '') {
        //        alert('Please enter the floor name!', 'warning');
        //        return false;            
        //    } else if ($('.FloorInfo .FloorName').filter(function () { return $(this).data().currentName.toUpperCase() == app.trim($(e.currentTarget).val()).toUpperCase() && parseInt($(this).data().systemId) != parseInt(attr.systemId) }).length > 0) {
        //        alert('Floor name already exist!', 'warning');
        //        return false;
        //    }
        //    if (app.trim(attr.currentName) != app.trim($(this).val())) {
        //        $(this).addClass('tinyloading');
        //        app.updateFloorName(attr.systemId, attr.structureId, app.trim($(this).val()), this);
        //    } else {
        //        $(this).val(($(this).val().length > 10 ? $(this).val().substring(0, 10) + '...' : $(this).val())).removeClass('labledTextbox')
        //    }
        //})
        //$(document).on('click', app.DE.ShaftEditIcon, function () {
        //    $(app.DE.ShaftName).addClass('labledTextbox');
        //    var shaftObj = $(this).prev('.ShaftName');
        //    shaftObj.val(shaftObj.data().currentName).removeClass('labledTextbox').focus();

        //})
        //$(document).on('paste', app.DE.ShaftName, function (e) {
        //    if (app.trim($(this).val()).length > 20) {
        //        e.preventDefault();
        //    }
        //})
        //$(document).on('blur', app.DE.ShaftName, function (e) {
        //    var attr = $(this).data();
        //    if (app.trim($(this).val()) == '') {
        //        alert('Please enter the shaft name!', 'warning');
        //        return false;
        //    } else if ($('.ShaftNameContainer .ShaftName').filter(function () { return $(this).data().currentName.toUpperCase() == app.trim($(e.currentTarget).val()).toUpperCase() && parseInt($(this).data().systemId) != parseInt(attr.systemId) }).length > 0) {
        //        alert('Shaft name already exist!', 'warning');
        //        return false;
        //    }
        //    if (app.trim(attr.currentName) != app.trim($(this).val())) {
        //        $(this).addClass('tinyloading');
        //        app.updateShaftName(attr.systemId, attr.structureId, app.trim($(this).val()), this);
        //    } else {
        //        $(this).val(($(this).val().length > 10 ? $(this).val().substring(0, 10) + '...' : $(this).val())).removeClass('labledTextbox')
        //    }
        //})
        $(document).on('click', app.DE.LibraryTab, function () {
            app.clearControl();
        })
        $(document).on('change', app.DE.ddlFloor, function () {

            //$('.table-scroll').animate({
            //    scrollTop: $("#divFloor_" + $(this).val()).offset().top + 155
            //}, 2000);
        })

        $(document).on('click', app.DE.filterEntity, function (e) {
            app.filterStructureEntity($(this).data().entityType, $(this).data().parentEntityType, $(this).is(':checked'));
        })

        //$(document).on('click', '.ElementboxParent', function (e) {
        //    app.flyEntity(e, $(app.DE.ddlFloor).val());
        //})
        $(document).on('click', app.DE.EntityDelete, app.layerActions.entity.remove)

        $(document).on("click", app.DE.CreateISPCable, function (e) {
            app.lstISPCablePt = [];
            if ($(app.DE.Cable_A_End).val() <= 0 || ($(app.DE.Cable_B_End).val() <= 0)) {
                alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_017, 'warning');//Please select termination points!
                return false;
            }
            if ($(app.DE.tempPath).attr("d") == "") {
                alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_018, "warning");//Path can not be blank!
                return false;
            }
            var attr = $(this).data();
            if (app.checkTemplateExist(e)) {

                var strucIdVal = $(app.DE.StructureId).val();
                var lyrDetail = getLayerDetail("Cable");
                var ntkIdType = lyrDetail['network_id_type'];

                var TPA = $(app.DE.Cable_A_End + ' option:selected').data();
                var TPB = $(app.DE.Cable_B_End + ' option:selected').data();
                var _tempPathData = $(app.DE.tempPath).data();

                app.lstISPCablePt.push({ network_name: TPA.entityType, network_id: TPA.networkId, system_id: TPA.systemId, node_type: _tempPathData.aNodeType });
                app.lstISPCablePt.push({ network_name: TPB.entityType, network_id: TPB.networkId, system_id: TPB.systemId, node_type: _tempPathData.bNodeType });

                var _data = {
                    geom: '0', enType: 'Cable', cableType: 'ISP', lstTP: app.lstISPCablePt, networkIdType: ntkIdType,
                    ModelInfo: { structureid: strucIdVal }
                };
                app.addCable(_data);
            }
        });

        $(document).on("click", app.DE.ISPCableTemplate, function () {
            $(this).prev('.clsTemplateIcon').trigger("click");
        });

        $(document).on("click", app.DE.InformationTool, function () {
            app.clearCableEvents();
            $('.ISP-Tool div:not(' + app.DE.InformationTool + ')').removeClass('activeToolBar');
            app.cancelSplice();
            $(this).toggleClass('activeToolBar');
            $('.leftPanel').hide();
            if ($(this).hasClass('activeToolBar')) {
                attachUnAttachEvt($(app.DE.EntityInfo), 'click', function () { app.EntityInfoDisplay(this); });
                $(app.DE.EntityInfo).css({ 'cursor': 'url(' + baseUrl + appRoot + 'Content/images/hand.cur' + '), default' });
            } else {

                app.DisableInfoTool();
            }
        });
        $(document).on("click", app.DE.EditFloorInfo, function () {
            app.updateFloorInfo(this);
        });
        $(document).on("click", app.DE.EntityShift, app.layerActions.entity.shift)

        $(document).on("click", app.DE.SplicingTool, function () {
            app.clearCableEvents();
            $(app.DE.circleMarker).css('visibility', 'hidden');
            $('.ISP-Tool div:not(' + app.DE.SplicingTool + ')').removeClass('activeToolBar');
            $(this).toggleClass('activeToolBar', function () {
                $('.leftPanel').hide();
                if ($(this).hasClass('activeToolBar')) {

                    //$(app.DE.EntityInfo + ':not(path)')
                    attachUnAttachEvt($(app.DE.EntityInfo + '[data-is-splicer="True"]:not(.network-status-D)'), 'click', function () {
                        app.destinationCableId = null;
                        app.sourceCableId = null;
                        app.clearLineAnimation();
                        app.initiateSplicing(this);
                    });
                    $(app.DE.EntityInfo + '[data-is-splicer="True"]:not(.network-status-D)').css({ 'cursor': 'url(' + baseUrl + appRoot + 'Content/images/hand.cur' + '), default' });
                    //$('.network-status-D').unbind('click').css("cursor", "not-allowed");
                } else {
                    attachUnAttachEvt($(app.DE.EntityInfo + '[data-is-splicer="True"]:not(.network-status-D)'), 'click', function () { });
                    $(app.DE.EntityInfo).unbind('click');
                    $(app.DE.EntityInfo).css("cursor", "default");
                    $('#divSplicingWindow').hide();
                    app.destinationCableId = null;
                    app.sourceCableId = null;
                    app.clearLineAnimation();
                }
                app.layerActions.entity.resetFocus($('.entityInfo'));
            });
        });

        $(document).on("click", app.DE.liImage, function () {
            app.getElementImages();
        });

        $(document).on("change", app.DE.UploadImage, function () {
            app.uploadImageFile();
        });

        $(document).on("click", app.DE.DeleteImages, function () {
            app.deleteEntityImages();
        });

        $(document).on("click", app.DE.DownloadImages, function () {
            app.downloadEntityImages($(app.DE.liImage).data().entityType);
        });

        $(document).on("click", app.DE.liDocument, function () {
            app.getAttachmentFiles();
        });

        $(document).on("change", app.DE.UploadDocument, function () {
            app.uploadDocumentFile();
        });

        $(document).on("click", app.DE.DeleteDocuments, function () {
            app.deleteEntityDocuments();
        });

        $(document).on("click", app.DE.DownloadDocuments, function () {
            app.downloadEntityDocuments($(app.DE.liImage).data().entityType);
        });

        $(document).on("mouseover", app.DE.entityInfoList, function () {
            var cableId = 'ispCable_' + $(this).data().systemId;
            $('#' + cableId).attr("class", $('#' + cableId).attr("class") + " pathAnimation");
        });

        $(document).on("mouseout", app.DE.entityInfoList, function () {
            var cableId = 'ispCable_' + $(this).data().systemId;
            $('#' + cableId).attr("class", $('#' + cableId).attr("class").replace('pathAnimation', ''));
        });

        $(document).on("click", app.DE.EditShaftInfo, function () {
            app.updateShaftInfo(this);
            app.structureActions.bind();
            $(app.DE.ddlShaft).val('').trigger("chosen:updated");
            $(app.DE.ddlFloor).val('').trigger("chosen:updated");
        });

        $(document).on('click', app.DE.AddMoreShaftRange, app.structureActions.shaft.addRange);

        //$(document).on('change', '#ddlShaft', app.structureActions.shaft.onShaftChange);

        $(document).on('click', '#tblShaftLstInfo .icon-close', function (e) {
            app.removeShaftRangeRow(e);
        });

        $(document).on('click', ".mainlyr  input[type='checkbox']", function (e) {
            let allChecksLen = $(".mainlyr  input[type='checkbox']").length;
            let checkedLen = $(".mainlyr  input[type='checkbox']:checked").length;
            $(app.DE.ShowAllEntity).prop('checked', allChecksLen == checkedLen);

        });

        $(document).on('mouseenter', app.DE.cableDot, app.cableActions.onDotMouseEnter);
        $(document).on('mousehover', app.DE.cableDot, app.cableActions.onDotMouseEnter);
        $(document).on('mouseleave', app.DE.cableDot, app.cableActions.onDotMouseLeave);
        $(document).on('click', app.DE.EntitySplit, app.layerActions.entity.splitCable);
        $(document).on('click', '#closeModalPopup', function () {
            app.cableActions.resetCablesFocus();
        });
        $(document).on('click', app.DE.DoorPosition, function (e) {
            let $selected = $(e.currentTarget);
            $(app.DE.DoorPosition).removeClass('active');
            $selected.addClass('active');
            let doorPos = $selected.data('position');
            $(app.DE.DoorPositionVal).val(doorPos);
        });
        $(document).on('click', app.DE.EntityViewAccessories, app.layerActions.entity.accessoriesView);
    }

    //this.ddlValidation = function (index) { //    
    //    var shaftId = $('#hdnShaftId').val();
    //    var isValidRange = true;
    //    var isEntityExist = false;
    //    var startRangeVal = parseInt($('#ShaftRangelist_' + index + '__shaft_start_range').val())
    //    var endRangeVal = parseInt($('#ShaftRangelist_' + index + '__shaft_end_range').val());
    //    for (i = startRangeVal; i <= endRangeVal; i++) {
    //        if ($('#ShaftRangelist_' + index + '__shaft_start_range option[value="' + i + '"]').attr('disabled') == 'disabled') {
    //            isValidRange = false;
    //        }
    //        if ($('div_shaft_' + shaftId + '_Floor_' + i).children().length) {
    //            isEntityExist = true;
    //        }
    //    }
    //    if (isValidRange) {
    //        app.refreshFloorCombination();
    //    } else if (isEntityExist) {
    //        alert('Entity already exist !', 'warning');
    //        return false;
    //    } else {
    //        $('#ShaftRangelist_' + index + '__shaft_start_range').val('0').trigger('chosen:updated');
    //        $('#ShaftRangelist_' + index + '__shaft_end_range').val('0').trigger('chosen:updated');
    //        alert('Invalid range!', 'warning');
    //        return false;
    //    }
    //}
    //this.refreshFloorCombination = function () {
    //    $('option').attr('disabled', false).trigger("chosen:updated");
    //    $.each($('#tblShaftLstInfo tbody tr'), function (index) {
    //    var row = $('#tblShaftLstInfo tr[data-row-num="' + index + '"]');
    //    var startRange = parseInt($($(row).children()[0]).children().val());
    //    var endRange = parseInt($($(row).children()[1]).children().val());
    //        //$.each($('#tblShaftLstInfo tbody tr:eq(' + index + ') td select'), function (index,e) {
    //        //    $(e).find('.chosen-select option').attr('disabled', false).trigger("chosen:updated");
    //        //})
    //        $.each($('#tblShaftLstInfo tbody tr:not(:eq('+ index + ')) '), function (index, e) {
    //            //var ddlId = $(this).attr('id');
    //    if (startRange > 0 && endRange > 0) {
    //        for (var j = startRange; j <= endRange; j++) {
    //                    //$('#' + ddlId + ' option[value="' + j + '"]').attr('disabled', true).trigger("chosen:updated");
    //                    $(this).find('.chosen-select option[value="' + j + '"]').attr('disabled', true).trigger("chosen:updated");
    //        }
    //    }
    //        })
    //    })
    //}

    this.DisableInfoTool = function () {
        attachUnAttachEvt($(app.DE.EntityInfo), 'click', function () { });
        $(app.DE.EntityInfo).unbind('click');
        $(app.DE.EntityInfo).css("cursor", "default");
        app.layerActions.entity.resetFocus($('.entityInfo'));
        $(app.DE.circleMarker).css('visibility', 'hidden');
        //$(app.DE.SplicingTool).removeClass('activeToolBar');
        //$(app.DE.SplicingTool).removeClass('activeISPTab ');
    }
    //Function 
    this.showMessageOverlay = function () {
        $('#alertBackOverLay').fadeIn();
    }
    this.focusCableToEquipment = function (ddlId) {

        var systemID = $(ddlId).val();
        var eType = $(ddlId + ' :selected').attr('data-entity-type');
        var gType = $(ddlId + ' :selected').attr('data-geom-type');
        var cableDirection = $(ddlId + ' :selected').attr('data-cable-direction');

        if (gType == 'Point') {
            $(app.DE.Cable_B_End).val('0').trigger("chosen:updated");
            $(app.DE.Cable_A_End).val('0').trigger("chosen:updated");
            $(app.DE.Cable_B_End + ' option').attr('disabled', false).trigger("chosen:updated");
            $(app.DE.Cable_A_End + ' option').attr('disabled', false).trigger("chosen:updated");
            if (app.gMapObj.pointObj != null) { app.gMapObj.pointObj.setMap(null); app.gMapObj.pointObj = null; }
            if (app.gMapObj.sourceLineObj != null) { app.gMapObj.sourceLineObj.setMap(null); app.gMapObj.sourceLineObj = null; }
            if (app.gMapObj.destLineObj != null) { app.gMapObj.destLineObj.setMap(null); app.gMapObj.destLineObj = null; }
        }
    }
    this.focusEntity = function (ddlId) {
        $(ddlId + '_chosen a').attr('title', '');
        if ($(app.DE.Cable_B_End).val() != '0' && $(app.DE.Cable_A_End).val() == $(app.DE.Cable_B_End).val()) {
            $(app.DE.Cable_B_End).val('0').trigger("chosen:updated");
            alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_019, 'warning');//Please select different termination point!
            return false;
        }
        var systemID = $(ddlId).val();
        $(ddlId + '_chosen a').attr('title', $(ddlId + " :selected").text());
        var eType = $(ddlId + ' :selected').attr('data-entity-type');
        var gType = $(ddlId + ' :selected').attr('data-geom-type');
        var cableDirection = $(ddlId + ' :selected').attr('data-cable-direction');
        if (gType == 'Point' && app.gMapObj.pointObj != null) { app.gMapObj.pointObj.setMap(null); app.gMapObj.pointObj = null; }
        else if (gType == 'Line' && cableDirection == 'Source' && app.gMapObj.sourceLineObj != null) { app.gMapObj.sourceLineObj.setMap(null); app.gMapObj.sourceLineObj = null; }
        else if (gType == 'Line' && cableDirection == 'Destination' && app.gMapObj.destLineObj != null) { app.gMapObj.destLineObj.setMap(null); app.gMapObj.destLineObj = null; $(app.DE.Cable_A_End + ' option[value="' + $(app.DE.Cable_B_End).val() + '"]').attr('disabled', true).trigger("chosen:updated"); }

        if (cableDirection == 'Source' && systemID == '0') { $(app.DE.Cable_B_End + ' option').attr('disabled', false).trigger("chosen:updated"); return false; }
        else if (cableDirection == 'Destination' && systemID == '0') { $(app.DE.Cable_A_End + ' option').attr('disabled', false).trigger("chosen:updated"); return false; }
        if (cableDirection == 'Source') {
            $(app.DE.Cable_B_End + ' option').attr('disabled', false).trigger("chosen:updated");
            var destOpt = app.DE.Cable_B_End + ' option[value="' + $(app.DE.Cable_A_End).val() + '"]';
            $(destOpt).attr('disabled', true).trigger("chosen:updated");
        } else if (cableDirection == 'Destination') {
            $(app.DE.Cable_A_End + ' option').attr('disabled', false).trigger("chosen:updated");
            var sourceOpt = app.DE.Cable_A_End + ' option[value="' + $(app.DE.Cable_B_End).val() + '"]';
            $(sourceOpt).attr('disabled', true).trigger("chosen:updated");
        }
        if (systemID == '0') return false;
    }
    this.focusOnItem = function (ddlId) {
        if ($(app.DE.DestinationCable).val() != '0' && $(app.DE.SourceCable).val() == $(app.DE.DestinationCable).val()) {
            $(app.DE.DestinationCable).val('0').trigger("chosen:updated");
            alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_020, 'warning');//Please select different cable!
            return false;
        }

        var systemID = $(ddlId).val();
        var geomType = $(ddlId + ' :selected').attr('data-geom-type');
        var entityType = $(ddlId + ' :selected').attr('data-entity-type');
        var cableDirection = $(ddlId + ' :selected').attr('data-cable-direction');
        if (cableDirection == 'Source' && systemID != '0') {
            app.clearLineAnimation();
            var cableId = 'ispCable_' + systemID;
            app.sourceCableId = cableId;
            if (app.destinationCableId != null) {
                $('#' + app.destinationCableId).attr("class", $('#' + app.destinationCableId).attr("class") + " pathAnimation");
            }
            $('#' + app.sourceCableId).attr("class", $('#' + app.sourceCableId).attr("class") + " pathAnimation");
        }
        //var connectorEtype = $(app.DE.ConnectingEntity + ' :selected').attr('data-entity-type');
        //if (connectorEtype == 'ODF' || connectorEtype == 'FMS') {
        //    $(splicing.DE.lblRightCable).text('Cable');
        //    $(splicing.DE.divLeftCable).hide();
        //}
        //else {
        //    $(splicing.DE.divLeftCable).show();
        //    $(splicing.DE.lblRightCable).text('Right Cable');
        //}
        if (cableDirection == 'Destination' && systemID != '0') {
            app.clearLineAnimation();
            if (app.sourceCableId != null) {
                $('#' + app.sourceCableId).attr("class", $('#' + app.sourceCableId).attr("class") + " pathAnimation");
            }
            var cableId = 'ispCable_' + systemID;
            app.destinationCableId = cableId;

            $('#' + app.destinationCableId).attr("class", $('#' + app.destinationCableId).attr("class") + " pathAnimation");
        }
        if (cableDirection == 'Source' && systemID == '0') {
            $(app.DE.DestinationCable + ' option').attr('disabled', false).trigger("chosen:updated");
            $('#' + app.sourceCableId).attr("class", $('#' + app.sourceCableId).attr("class").replace('pathAnimation', ''));
            return false;
        }
        else if (cableDirection == 'Destination' && systemID == '0') {
            $(app.DE.SourceCable + ' option').attr('disabled', false).trigger("chosen:updated");
            $('#' + app.destinationCableId).attr("class", $('#' + app.destinationCableId).attr("class").replace('pathAnimation', ''));
            return false;
        }
        if (cableDirection == 'Source') {
            $(app.DE.DestinationCable + ' option').attr('disabled', false).trigger("chosen:updated");
            var destOpt = app.DE.DestinationCable + ' option[value="' + $(app.DE.SourceCable).val() + '"]';
            $(destOpt).attr('disabled', true).trigger("chosen:updated");
        } else if (cableDirection == 'Destination') {
            $(app.DE.SourceCable + ' option').attr('disabled', false).trigger("chosen:updated");
            var sourceOpt = app.DE.SourceCable + ' option[value="' + $(app.DE.DestinationCable).val() + '"]';
            $(sourceOpt).attr('disabled', true).trigger("chosen:updated");
        }
        if (systemID == '0') return false;
    }
    this.filter = function filter(values, obj, attribute, callback) {
        $(obj).removeClass('show');
        var regex = new RegExp('\\b\\w*' + values.toUpperCase() + '\\w*\\b');
        $(obj).addClass('hide').filter(function () {
            return regex.test($(this).data(attribute).toUpperCase())
        }).removeClass('hide').addClass('show');
        callback.call();
    }
    this.filterbyBaselocation = function filter(values, obj, attribute, listObj) {
        $(obj).removeClass('show');
        var regex = new RegExp('\\b\\w*' + values.toUpperCase() + '\\w*\\b');
        $(obj).addClass('hide').filter(function () {
            if ($(app.DE.ParentEntityTab).hasClass('active') && $(app.DE.BaseLocation).val() != '0') {
                if (!app.convert.toBoolean($(app.DE.BaseLocation + ' :selected').data().isShaftElement.toLowerCase())) {
                    return (regex.test($(this).data(attribute).toUpperCase()) && app.convert.toBoolean($(this).data().isFloorElement.toLowerCase()));
                } else if (app.convert.toBoolean($(app.DE.BaseLocation + ' :selected').data().isShaftElement.toLowerCase())) {
                    return (regex.test($(this).data(attribute).toUpperCase()) && app.convert.toBoolean($(this).data().isShaftElement.toLowerCase()));
                }
            } else if ($(app.DE.ChildEntityTab).hasClass('active') && $(app.DE.ddlParentEntity).val() != '0') {
                var pEntityes = $(this).data().parentEntities;
                if (pEntityes != undefined && pEntityes != '') {
                    var index = pEntityes.split('|').findIndex(function (m) { return m.toUpperCase() == $(app.DE.ddlParentEntity + ' :selected').data().entityType.toUpperCase(); });
                    return (regex.test($(this).data(attribute).toUpperCase()) && index >= 0);
                } else { return regex.test($(this).data(attribute).toUpperCase()) }
            } else {
                app.filter(values, listObj + '>.ElementboxParent', attribute, function () { });
            }
        }).removeClass('hide').addClass('show');
        $(listObj + '>#divNoRecordFound').hide();
        if ($(listObj + '>.show').length == 0) { $(listObj + ' > #divNoRecordFound').show(); }
    }
    this.filterStructureEntity = function filterStructureEntity(entityType, parentEntityType, isDisplay) {
        var _objEntity = parentEntityType != "" ? $('.table-wrap .entityInfo[data-entity-type=' + entityType + '][data-parent-entity-type=' + parentEntityType + ']') : $('.table-wrap  .entityInfo[data-entity-type=' + entityType + ']');
        var _visibility = isDisplay ? "visible" : "hidden";
        var _display = isDisplay ? "block" : "none";

        _objEntity.css('visibility', _visibility);
        if ($(app.DE.tempPath).data().isEditMode) {
            _objEntity.find('.dot').css('visibility', _visibility);
        }
        _objEntity.parent('.ParentInnerBox').css('visibility', _visibility);
        _objEntity.next('.entityboxcount').css('visibility', _visibility);
        _objEntity.next('.ParentBox.entityParentBox').css('visibility', _visibility);
        _objEntity.next('.entityboxcount').next('.ParentBox.entityParentBox').css('visibility', _visibility);
        _objEntity.next().next('.ParentBox.entityParentBox').hide();
        if (entityType == 'Customer') {
            _objEntity = $('.table-wrap  .CPEtoCustomerPath')
            _objEntity.css('visibility', _visibility);
        } else if (entityType == 'PatchCord') {
            _objEntity = $('.table-wrap  .CPEtoCPEPath')
            _objEntity.css('visibility', _visibility);
        }

    }
    this.checkTemplateExist = function (e) {
        var status = true;
        var attr = $(e.currentTarget).data();
        var isTemplateRequired = app.convert.toBoolean(attr.isTemplateRequired);
        var isChildLayer = app.convert.toBoolean(attr.isIspChildLayer);
        var isParentLayer = app.convert.toBoolean(attr.isIspParentLayer);
        var entityType = app.trim(attr.entityType);
        var templateId = attr.templateId;
        if (isTemplateRequired) {
            ajaxReq('ISP/checkTemplateExist', { enType: entityType }, false, function (data) {
                if (data.status == 'FAILED') {
                    alert(data.message, 'warning');
                    status = false;
                }
            }, true, false);
        }
        return status;
    }
    this.setElement = function (e) {

        var attr = $(e.currentTarget).data();
        var isTemplateRequired = app.convert.toBoolean(attr.isTemplateRequired);
        var isChildLayer = app.convert.toBoolean(attr.isIspChildLayer);
        var isParentLayer = app.convert.toBoolean(attr.isIspParentLayer);
        var entityType = app.trim(attr.entityType);
        var templateId = attr.templateId;
        if (isTemplateRequired) {
            ajaxReq('ISP/checkTemplateExist', { enType: entityType }, true, function (data) {
                if (data.status == 'FAILED') {
                    alert(data.message, 'warning');
                    return false;
                }
                app.addEntityInfo(e, entityType, isParentLayer, isChildLayer, templateId);
            }, true, true, false);
        } else {
            app.addEntityInfo(e, entityType, isParentLayer, isChildLayer, templateId);

        }
    }
    this.isValidParentLayer = function () {
        if ($(app.DE.BaseLocation).val() == '0') {
            alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_021, 'warning');//Please select base location first!
            return false;
        } else if ($(app.DE.BaseLocation).val() == 'Shaft') {
            if ($(app.DE.ddlShaft).val() == '') {
                alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_022, 'warning');//Please select shaft first!
                return false;
            }
            if ($(app.DE.ddlFloor).val() == '') {
                alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_023, 'warning');//Please select floor first!
                return false;
            }
            if ($('#div_shaft_' + $(app.DE.ddlShaft).val() + '_Floor_' + $(app.DE.ddlFloor).val() + '').children().length == 4) {
                alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_024, 'warning');//Maximum 4 element can be placed on this shaft!
                return false;
            }
            if ($('#div_shaft_' + $(app.DE.ddlShaft).val() + '_Floor_' + $(app.DE.ddlFloor).val() + '').parent('td').hasClass('shaftdisabled')) {
                alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_025, 'warning');//Shaft does not exist on this floor!
                return false;
            }
        } else if ($(app.DE.BaseLocation).val() == 'Floor') {
            if ($(app.DE.ddlFloor).val() == '') {
                alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_023, 'warning');//Please select floor first!
                return false;
            }
        }
        return true;
    }
    this.isValidChildLayer = function () {
        if ($(app.DE.ddlParentEntity).val() == '0') {
            alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_027, 'warning');//Please select parent entity first!
            return false;
        }
        return true;
    }
    this.isValidUnit = function (floorId) {
         
        //var floorId = $(app.DE.ddlFloor).val();
        var unitCount = $('#divFloor_' + floorId).data().totalUnit;
        var existUnitCount = $('#divFloor_' + floorId + ' div[data-entity-type="UNIT"]').length;
        if (!(parseInt(existUnitCount) < parseInt(unitCount))) {
            //Only//units are allowed on this floor!

            alert($.validator.format(MultilingualKey.SI_ISP_GBL_JQ_FRM_029, unitCount));
            return false;
        }
        return true;
    }
    this.addEntityInfo = function (e, entityType, isParentLayer, isChildLayer, templateId) {
        if (($(app.DE.ParentEntityTab).hasClass('active') && isParentLayer)) {
            if ((entityType == app.EnumEntityType.UNIT && app.isValidParentLayer() && app.isValidUnit($(app.DE.ddlFloor).val()))) {
                var layerDetails = getLayerDetail(entityType);
                if (layerDetails != null) {
                    app.saveElementInfo(e, entityType, layerDetails, templateId);
                }
            } else if (entityType != app.EnumEntityType.UNIT && app.isValidParentLayer()) {
                var layerDetails = getLayerDetail(entityType);
                if (layerDetails != null) {
                    app.saveElementInfo(e, entityType, layerDetails, templateId);
                }
            }
        } else if ($(app.DE.ChildEntityTab).hasClass('active') && isChildLayer) {
            if (app.isValidChildLayer()) {
                var layerDetails = getLayerDetail(entityType);
                if (layerDetails != null) {
                    app.saveElementInfo(e, entityType, layerDetails, templateId);
                }
            }
        }
    }
    this.saveElementInfo = function (e, entityType, layerDetails, templateId) {
        app.currentElement = e;
        var pSystemId = 0;
        var pEntityType = null;
        var pNetworkId = null;
        if ($(app.DE.ddlParentEntity).val() != '0') {
            pSystemId = $(app.DE.ddlParentEntity).val();
            pEntityType = $(app.DE.ddlParentEntity + ' :selected').data().entityType;
            pNetworkId = $(app.DE.ddlParentEntity + ' :selected').text();
        } else {
            pSystemId = $(app.DE.StructureId).val();
            pEntityType = app.EnumEntityType.Structure;
            pNetworkId = $('#hdnStructureNwId').val();
        }
        if (app.convert.toBoolean(layerDetails.is_direct_save)) {
            let data = {
                networkIdType: layerDetails.network_id_type,
                templateId: templateId,
                objIspEntityMap: {
                    floor_id: $(app.DE.ddlFloor).val(),
                    structure_id: $(app.DE.StructureId).val(),
                    shaft_id: $(app.DE.ddlShaft).val()
                },
                pSystemId: pSystemId,
                pEntityType: pEntityType,
                isDirectSave: true,
                pNetworkId: pNetworkId
            };
            ajaxReq(layerDetails.save_entity_url, data, false, function (response) {
                if (response.status == 'OK') {
                    app.refreshStructureInfo(response, false);
                    alert(response.message, 'success', 'success');
                } else { alert(response.message, "warning"); }
            }, true, false);
        }
        else {
            var pageUrl = layerDetails.layer_form_url;
            var modalClass = 'modal-lg';
            popup.LoadModalDialog(app.ParentModel, pageUrl, {
                networkIdType: layerDetails.network_id_type,
                elementType: layerDetails.layerName,
                structureid: $(app.DE.StructureId).val(),
                templateId: templateId,
                floorid: $(app.DE.ddlFloor).val(),
                shaftid: $(app.DE.ddlShaft).val(),
                systemId: 0, pSystemId: pSystemId,
                pEntityType: pEntityType,
                pNetworkId: pNetworkId
            }, layerDetails.layer_title, modalClass);
        }

    }

    //this.EntityExport = function (systemID, entityType, geomType) {
    //    var _networkstage = '';
    //    ajaxReq('Main/CheckEntityData', { systemId: systemID, entityType: entityType, networkStage: '' }, false, function (status) {
    //        if (status != null && status != undefined) {
    //            if (status)  // check if true
    //            {
    //                window.location = appRoot + 'Main/ExportInfoEntity?systemId=' + systemID + '&entityType=' + entityType + '&networkStage=' + _networkstage + '';
    //            }
    //            else {
    //                alert('No records found!', "warning");
    //            }
    //        }
    //    }, true, true);
    //}

    //this.showHistory = function (systemId, entityType) {
    //    var formURL = "Audit/GetHistory";
    //    var layerTitle = getLayerTltle(entityType);
    //    var titleText = layerTitle.toUpperCase() + " History";
    //    popup.LoadModalDialog('PARENT', formURL, { systemId: systemId, eType: entityType }, titleText, 'modal-lg');
    //}

    this.GetISPCableTubeCoreInfo = function (cableid) {
         
        ajaxReq('ISP/GetISPCableTubeCoreDetail', {
            cableId: cableid
        }, false, function (resp) {
            $("#TubeColor").html(resp);
        }, false, false);
    }
    this.GetCableFiberDetail = function (_cableid) {
         
        if ($("#FiberDetail").html().trim() == '') {
            ajaxReq('Library/GetCableFiberDetail', { cableId: _cableid, type: "ISP" }, false, function (resp) {
                $("#FiberDetail").html(resp);
                $("#FiberDetail").css('background-image', 'none');

            }, false, false);
        }
    }
    //this.ShowDetailFromInfo = function (_data) {
    //    var modelClass = getPopUpModelClass(_data.entityType);
    //    var lyrDetail = getLayerDetail(_data.entityType);
    //    var formURL = lyrDetail['layer_form_url'];
    //    var titleText = lyrDetail['layer_title'];
    //    var strucIdVal = $(app.DE.StructureId).val();
    //    var _model = { ModelInfo: { structureid: strucIdVal } }
    //    var _data = $.extend(_data, _model);
    //    popup.LoadModalDialog(app.ParentModel, formURL, _data, titleText, modelClass);
    //}

    this.EntityInfoDisplay = function (e) {

        let $marker = $(app.DE.circleMarker);
        app.layerActions.entity.resetFocus($('.entityInfo'));
        $marker.css('visibility', 'hidden');
        var attr = $(e).data();
        var ListSystemIds = [];
        let offset = $(app.DE.SVGCable).offset();

        let mousePointer = { x: event.clientX - offset.left, y: event.clientY - offset.top };
        if (attr.entityGeomtype == 'LINE') {
            ListSystemIds.push({ system_id: attr.systemId, entity_name: attr.entityType, entity_title: attr.entityTitle, geom_type: attr.entityGeomtype, network_id: attr.networkId, network_status: attr.networkStatus });
            var bufferpx = $marker.data().traceBuffer;

            let offset = $(app.DE.SVGCable).offset();
            let cables = app.cableActions.getIntersectedCableAtPoint($('.entityInfo[data-entity-geomtype="LINE"]'), mousePointer, bufferpx);
            //console.log(cables);

            $.each(cables, function (index, item) {
                var itemoffSet = $(item.element).offset();
                var attrItem = $(item.element).data();
                if (attr.systemId !== attrItem.systemId) {
                    ListSystemIds.push({
                        system_id: attrItem.systemId,
                        entity_name: attrItem.entityType,
                        entity_title: attrItem.entityTitle,
                        geom_type: attrItem.entityGeomtype,
                        network_id: attrItem.networkId,
                        network_status: attrItem.networkStatus
                    });
                }
            });
            $marker.attr('r', bufferpx);
            $marker.css('visibility', 'visible');
            $marker.attr('transform', "translate(" + mousePointer.x + "," + mousePointer.y + ")");
        }
        else { app.layerActions.entity.focus($(e)); }
        if (ListSystemIds.length > 1) {

            app.ShowEntityInformation(ListSystemIds);
        } else {
            app.ShowEntityInformationDetail(attr.systemId, attr.entityType, attr.entityTitle, attr.entityGeomtype, false, attr.networkStatus);

        }
        if (ListSystemIds.length == 1)
            app.cableActions.focus($(e));

        if ($('.clsInfoTool').css('display') == 'none') {
            $('.clsInfoTool').animate({ width: 'toggle' });
        }
        $(app.DE.leftMenuActions).removeClass('activeISPTab');
        $('#divleftMenu div.ispInfoTool').addClass('activeISPTab');
        //app.layerActions.entity.focus($(e));
    }

    this.ShowEntityInformation = function (listObj) {
        app.InfoProgress(app.DE.EntityInformationList);
        ajaxReq('ISP/GetISPEntityInformation', { listObj: JSON.stringify(listObj) }, true, function (resp) {
            $(app.DE.EntityInformationList).html(resp);
            $('#dvEntityInformation').animate({ 'margin-left': '0px' }, 700);
            $('#divinfoBack').show();
            //show hide the NoRecordFound div 
            if ($('.panel-body table tr').length == 0)
                $('#divInfoNoRecordFound').show();
            else
                $('#divInfoNoRecordFound').hide();
        }, false, false, true);
    }

    this.ShowEntityInformationDetail = function (systemID, eName, eTitle, gType, backbutton, nStatus) {
        app.InfoProgress(app.DE.EntityInformationDetail);
        ajaxReq('ISP/GetISPEntityInformationDetail', { systemId: systemID, entityName: eName, entityTitle: eTitle, geomType: gType, networkStatus: nStatus }, true, function (resp) {
            // console.log("GetISPEntityInformationDetail"+resp);
            $(app.DE.EntityInformationDetail).html(resp);
            $('#dvEntityInformation').animate({ 'margin-left': '-450px' }, 700);
            if (backbutton) { $('#divinfoBack').show(); } else { $('#divinfoBack').hide(); }
            //show hide the NoRecordFound div 
            if ($('.panel-body table tr').length == 0)
                $('#divInfoNoRecordFound').show();
            else
                $('#divInfoNoRecordFound').hide();

            app.layerActions.networkStatus.bind(systemID, eName, nStatus);
            if (eName.toUpperCase() == "CABLE") {
                app.cableActions.focus(app.layerActions.entity.select({ type: eName, systemId: systemID }));
            }
        }, false, false, true);
    }

    this.RefreshISPNetworkLayerElements = function (structureId) {
        ajaxReq('ISP/GetISPNetworkLayerElements', { structureId: structureId }, true, function (resp) {
            $(app.DE.ISPNetworkLayer).html(resp);
        }, false, false, true);
    }

    this.HideEntityInformationDetail = function () {
        $('#dvEntityInformation').animate({ 'margin-left': '0px' }, 700);
        $(app.DE.EntityCancel).trigger("click");
        app.layerActions.entity.resetFocus($('.entityInfo'));
    }

    this.updateFloorName = function (floorSystemId, structureId, floorName, obj) {
        ajaxReq('ISP/updateFloorName', { floorSystemId: floorSystemId, structureId: structureId, floorName: floorName }, false, function (response) {
            if (response) {
                app.refreshStructureInfo();
                $(app.DE.ddlFloor + ' option[value="' + floorSystemId + '"]').text(floorName).trigger("chosen:updated");
                alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_030, 'success', 'success');//Floor name has been updated successfully!
            } else {
                alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_031, 'warning', 'Information');//Something went wrong while update floor name!
            }
            $(obj).val(($(obj).val().length > 5 ? $(obj).val().substring(0, 5) + '...' : $(obj).val()))
                .removeClass('labledTextbox')
                .removeClass('tinyloading')
                .addClass('labledTextbox');
            $(obj).data().currentName = floorName;
        }, true, false);
    }
    this.updateShaftName = function (shaftSystemId, structureId, shaftName, obj) {
        ajaxReq('ISP/updateShaftName', { shaftSystemId: shaftSystemId, structureId: structureId, shaftName: shaftName }, false, function (response) {
            if (response) {
                app.refreshStructureInfo();
                $(app.DE.ddlShaft + ' option[value="' + shaftSystemId + '"]').text(shaftName).trigger("chosen:updated");
                alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_026, 'warning');//Shaft name has been updated successfully!
            } else {
                alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_001, 'warning', 'Information');//Something went wrong while update shaft name!
            }
            $(obj).val(($(obj).val().length > 5 ? $(obj).val().substring(0, 5) + '...' : $(obj).val()))
                .removeClass('labledTextbox')
                .removeClass('tinyloading')
                .addClass('labledTextbox')
            $(obj).data().currentName = shaftName;
        }, true, false);
    }
    this.trim = function (value) {
        return (value != undefined && value != '' && value != null) ? value.replace(/(\r\n|\n|\r| )/gm, "") : '';
    }
    this.clearControl = function () {
        $(app.DE.ddlParentEntity).val('0').trigger("chosen:updated");
        $('#divChildEntity  #txtSearchElement').val('');
        $('#divParentEntity #txtSearchElement').val('');
        $(app.DE.SearchNetworkEntity).val('');
        $(app.DE.SearchNetworkEntity).keyup();
        $(app.DE.BaseLocation).val('0').trigger("chosen:updated");
        $(app.DE.ddlShaft).val('').trigger("chosen:updated");
        $(app.DE.ddlFloor).val('').trigger("chosen:updated");
        $('#ddlCableAEnd').val(0).trigger('chosen:updated');
        $('#ddlCableBEnd').val(0).trigger('chosen:updated');
        $('#ddlParentEntityType').val('0').trigger('chosen:updated');
        $('.ElementboxParent').removeClass('hide');
        app.refreshParentEntityDropDown();
    }

    this.refreshStructureInfo = function () {
        app.tableScroll.top = $('.table-scroll').scrollTop();
        app.tableScroll.left = $('.table-scroll').scrollLeft();
        $(app.DE.refreshStructureInfo).trigger("click");

    }

    this.calculateCableLength = function (obj, networkStatus, objStartReading, objEndReading, objCalLength, objMeasuredLength) {
        var startReading = parseFloat($('#' + objStartReading).val());
        var endReading = parseFloat($('#' + objEndReading).val());
        if (networkStatus == 'A' && startReading == 0) {
            $('#' + objStartReading).val('');
            return false;
        }
        if (networkStatus == 'A' && endReading == 0) {
            $('#' + objEndReading).val('');
            return false;
        }
        var length = endReading - startReading;
        if (length > 0) {
            $('#' + objCalLength).val(Math.round(length * 100) / 100);
            if ($('#cable_type').val() == 'ISP') {
                $('#' + objMeasuredLength).val(Math.round(length * 100) / 100);
            }

        } else {
            $('#' + objCalLength).val(0);
            if ($('#cable_type').val() == 'ISP') {
                $('#' + objMeasuredLength).val(0);
            }

        }
    }



    this.addCable = function (data) {
        popup.LoadModalDialog(app.ParentModel, "ISP/AddCable", data, "Cable", "modal-lg", function () {
            $('#hdnLineGeom').val($("#tempPath").attr("d"));
            //app.isEditMode = false;
            //app.editCableId = '';
        });
    }
    //this.refreshStructureInfo = function (enDetails, isCallFromPageMessage) {
    //     //$(app.DE.refreshStructureInfo).trigger("click");
    //    ajaxReq('ISP/getNewEntity', { structureId:$(app.DE.StructureId).val(),systemId: enDetails.systemId, entityType: enDetails.entityType }, false, function (response) {
    //        if (response) {
    //            //var entityHtml = $.parseHTML(app.FloorElement);
    //            //$(entityHtml).find('#divBox').attr('data-entity-type', response.entity_type);
    //            //$(entityHtml).find('#divBox').attr('data-entity-title', response.layer_title);
    //            //$(entityHtml).find('#divBox').attr('data-is-isp-child-layer', response.is_isp_child_layer);
    //            //$(entityHtml).find('#divBox').attr('data-system_id', response.system_id);
    //            //$(entityHtml).find('#spnEntityName').text(response.layer_title);
    //            app.flyEntity(entityHtml, enDetails, isCallFromPageMessage);
    //        } else {
    //            alert('Something went wrong while update shaft name!');
    //        }
    //    }, true, false);
    //    app.refreshParentDropDown();
    //}
    this.validateFloorDimension = function () {
        var floorId = '#divFloor_' + $('#hdnFloorId').val();
        var attr = $(floorId).data();
        var floorHeight = parseFloat(attr.height);
        var floorWidth = parseFloat(attr.width);
        var floorLength = parseFloat(attr.length);


        var roomHeight = parseFloat($('#txtRoomHeight').val());
        var roomWidth = parseFloat($('#txtRoomWidth').val());
        var roomLength = parseFloat($('#txtRoomLength').val());

        if (roomHeight > floorHeight) {
            //UNIT height can not be greater then floor height!//Floor Height//Unit Height
            var message = MultilingualKey.SI_ISP_GBL_JQ_GBL_002;
            message += '<br/><div class="table-responsive tble_cls"><table border="1" width="100%" class="webgrid" id="tblAlertDependendEntity"><tbody>';
            message += '<tr><td>' + MultilingualKey.SI_ISP_GBL_JQ_GBL_003 + ' </td><td> ' + floorHeight + '(m)</td></tr>';
            message += '<tr><td>' + MultilingualKey.SI_ISP_GBL_JQ_GBL_004 + ' </td><td> ' + roomHeight + '(m)</td></tr>';
            message += '</tbody></table></div>';
            alert(message, "warning");
            return false;
        }
        if (roomWidth > floorWidth) {

            //UNIT width can not be greater then floor width!//Floor Width//Unit Width
            var message = MultilingualKey.SI_ISP_GBL_JQ_GBL_005;
            message += '<br/><div class="table-responsive tble_cls"><table border="1" width="100%" class="webgrid" id="tblAlertDependendEntity"><tbody>';
            message += '<tr><td>' + MultilingualKey.SI_ISP_GBL_JQ_GBL_006 + ' </td><td> ' + floorWidth + '(m)</td></tr>';
            message += '<tr><td>' + MultilingualKey.SI_ISP_GBL_JQ_GBL_007 + ' </td><td> ' + roomWidth + '(m)</td></tr>';
            message += '</tbody></table></div>';
            alert(message, "warning");
            return false;
        }
        if (roomLength > floorLength) {
            //UNIT length can not be greater then floor length!//Floor Length//Unit Length
            var message = MultilingualKey.SI_ISP_GBL_JQ_GBL_008;
            message += '<br/><div class="table-responsive tble_cls"><table border="1" width="100%" class="webgrid" id="tblAlertDependendEntity"><tbody>';
            message += '<tr><td>' + MultilingualKey.SI_ISP_GBL_JQ_GBL_009 + ' </td><td> ' + floorLength + '(m)</td></tr>';
            message += '<tr><td>' + MultilingualKey.SI_ISP_GBL_JQ_GBL_010 + '</td><td> ' + roomLength + '(m)</td></tr>';
            message += '</tbody></table></div>';
            alert(message, "warning");
            return false;
        }

        var allRoomLength = 0;
        var availableLength = 0;
        $(floorId + ' div[data-entity-type="UNIT"]').each(function () {
            allRoomLength += parseFloat($(this).attr('data-additionalattr').split('|')[2].replace('length=', ''));
        })
        availableLength = (floorLength - allRoomLength);
        if ($('#hdnUnitSystemId').val() == 0) {
            allRoomLength = allRoomLength + roomLength;
        }
        //if (availableLength == 0) {
        //    alert('Space is not available on <b> ' + $(floorId).text() + ' </b> floor!', "warning");
        //    return false;
        //} else
        if (allRoomLength > floorLength) {
            //selected UNIT can not be added into//as unit length is greater than the available!//Floor Length//Available Floor Length//Unit Length

            var message = getMultilingualStringValue($.validator.format(MultilingualKey.SI_ISP_GBL_JQ_GBL_012, $(floorId).text()));
            message += '<br/><div class="table-responsive tble_cls"><table border="1" width="100%" class="webgrid" id="tblAlertDependendEntity"><tbody>';
            message += '<tr><td>' + MultilingualKey.SI_ISP_GBL_JQ_GBL_009 + ' </td><td> ' + floorLength + '(m)</td></tr>';
            message += '<tr><td>' + MultilingualKey.SI_ISP_GBL_JQ_GBL_013 + ' </td><td> ' + availableLength + '(m)</td></tr>';
            message += '<tr><td>' + MultilingualKey.SI_ISP_GBL_JQ_GBL_010 + ' </td><td> ' + roomLength + '(m)</td></tr>';
            message += '</tbody></table></div>';
            alert(message, "warning");
            return false;
        }
        let doorType = $('#door_type').val();
        if (doorType != 'none') {
            let doorWidth = $('#door_width').val();
            if (!doorWidth || doorWidth == 0) {
                alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_014, "warning");//Door width is required!
                return false;
            }
        }
        $('#frmAddRoom').submit();
    }
    this.filterLocationBaseEntity = function (obj) {
        $('#divParentEntity #txtSearchElement').val('');
        $('#divParentEntity #divNoRecordFound').hide();
        if ($(obj).val() != '0') {
            $(app.DE.ParentEntityBox).removeClass('show');
            $(app.DE.ParentEntityBox).addClass('hide').filter(function () {
                if (!app.convert.toBoolean($(obj + ' :selected').data().isShaftElement.toLowerCase())) {
                    return app.convert.toBoolean($(this).data().isFloorElement.toLowerCase());
                } else if (app.convert.toBoolean($(obj + ' :selected').data().isShaftElement.toLowerCase())) {
                    return app.convert.toBoolean($(this).data().isShaftElement.toLowerCase());
                }
            }).removeClass('hide').addClass('show');
        } else {
            $(app.DE.ParentEntityBox).removeClass('hide');
        }
        app.structureActions.floor.resetFloorSelect();
    }
    this.filterChildEntity = function (obj) {
        $('#divChildEntity  #txtSearchElement').val('');
        $('#divChildEntity #divNoRecordFound').hide();
        if ($(obj).val() != '0') {
            $(app.DE.ChildEntityBox).removeClass('show');
            $(app.DE.ChildEntityBox).addClass('hide').filter(function () {
                var pEntityes = $(this).data().parentEntities;
                if (pEntityes != undefined && pEntityes != '') {
                    var index = pEntityes.split('|').findIndex(function (m) { return m.toUpperCase() == $(obj + ' :selected').data().entityType.toUpperCase(); });
                    return index >= 0;
                } else { return true; }
            }).removeClass('hide').addClass('show');
        } else {
            $(app.DE.ChildEntityBox).removeClass('hide');
            $('#ddlParentEntityType').val('0').trigger('chosen:updated');
        }
    }

    this.refreshParentDropDown = function () {
        var prevValue = $(app.DE.ddlParentEntity).val();
        ajaxReq('ISP/getParentEntities', { structureId: $(app.DE.StructureId).val() }, false, function (response) {
            var html = '<option value="0">-- Select --</option>'
            $.each(response, function (index, item) {
                html += '<option value="' + item.system_id + '" data-entity-type="' + item.entity_type + '">' + item.network_id + '</option>';
            })
            app.refreshParentEntityTypeDropDown(response);
            $(app.DE.ddlParentEntity).html(html).trigger('chosen:updated');
            $(app.DE.ddlParentEntity).val(prevValue).trigger('chosen:updated');
            app.parentEntities = response;
        }, true, false)

        var CableAEndprevValue = $(app.DE.Cable_A_End).val();
        var CableBEndprevValue = $(app.DE.Cable_B_End).val();
        ajaxReq('ISP/getStructureEntities', { structureId: $(app.DE.StructureId).val() }, false, function (response) {
            var html = '<option value="0">-- Select --</option>'
            $.each(response, function (index, item) {
                if (item.is_isp_tp == true)
                    html += '<option value="' + item.entity_system_id + '" data-system-id="' + item.entity_system_id + '" data-entity-type="' + item.entity_type + '" data-geom-type="Point" data-network-id="' + item.network_id + '" data-shaft_id="' + item.shaft_id + '" data-floor_id="' + item.floor_id + '">' + item.network_id + '</option>';
            })
            $(app.DE.Cable_A_End).html(html).trigger('chosen:updated');
            $(app.DE.Cable_A_End).val(CableAEndprevValue).trigger('chosen:updated');

            $(app.DE.Cable_B_End).html(html).trigger('chosen:updated');
            $(app.DE.Cable_B_End).val(CableBEndprevValue).trigger('chosen:updated');

        }, true, false)
    }

    this.flyEntity = function (html, response, isCallFromPageMessage) {
        var $counter = $('#divFloor_' + $(app.DE.ddlFloor).val());
        var $that = $(app.currentElement.currentTarget).parent().parent('.ElementboxParent');
        var $clone = $that.clone();
        var speed = 2000;
        $that.after($clone);
        var leftPosition = 0;
        var topPosition = 0
        if ($(app.DE.ParentEntityTab).hasClass('active')) {
            if ($(app.DE.ddlShaft).val() != '' && $(app.DE.ddlFloor).val() != '') {
                topPosition = $('#div_shaft_' + $(app.DE.ddlShaft).val() + '_Floor_' + $(app.DE.ddlFloor).val()).offset().top + 40;
                leftPosition = $('#div_shaft_' + $(app.DE.ddlShaft).val() + '_Floor_' + $(app.DE.ddlFloor).val()).offset().left;
                if ($('#div_shaft_' + $(app.DE.ddlShaft).val() + '_Floor_' + $(app.DE.ddlFloor).val()).children().length > 0) {
                    topPosition = $('#div_shaft_' + $(app.DE.ddlShaft).val() + '_Floor_' + $(app.DE.ddlFloor).val()).children().last().offset().top + 40;
                    leftPosition = $('#div_shaft_' + $(app.DE.ddlShaft).val() + '_Floor_' + $(app.DE.ddlFloor).val()).offset().left;
                }

            } else {
                topPosition = $('#divFloor_' + $(app.DE.ddlFloor).val()).offset().top;
                leftPosition = $('#divFloor_' + $(app.DE.ddlFloor).val()).offset().left + 40;
                if ($('#divFloor_' + $(app.DE.ddlFloor).val()).children().length > 0) {
                    topPosition = $('#divFloor_' + $(app.DE.ddlFloor).val()).offset().top;
                    leftPosition = $('#divFloor_' + $(app.DE.ddlFloor).val()).children().last().offset().left + 40;
                }

            }
        } else if ($(app.DE.ChildEntityTab).hasClass('active')) {
            if ($(app.DE.ddlShaft).val() != '' && $(app.DE.ddlFloor).val() != '') {
                topPosition = $('#divParentFirstBox_' + $(app.DE.ddlParentEntity + ' :selected').data().entityType + '_' + $(app.DE.ddlParentEntity).val()).offset().top;
                leftPosition = $('#divParentFirstBox_' + $(app.DE.ddlParentEntity + ' :selected').data().entityType + '_' + $(app.DE.ddlParentEntity).val()).offset().left + 40;
                if ($('#divParentFirstBox_' + $(app.DE.ddlParentEntity + ' :selected').data().entityType + '_' + $(app.DE.ddlParentEntity).val()).children().length > 0) {
                    topPosition = $('#divParentFirstBox_' + $(app.DE.ddlParentEntity + ' :selected').data().entityType + '_' + $(app.DE.ddlParentEntity).val()).children().last().offset().top;
                    leftPosition = $('#divParentFirstBox_' + $(app.DE.ddlParentEntity + ' :selected').data().entityType + '_' + $(app.DE.ddlParentEntity).val()).offset().left + 40;
                }
            }
            else {
                topPosition = $('#divParentFirstBox_' + $(app.DE.ddlParentEntity + ' :selected').data().entityType + '_' + $(app.DE.ddlParentEntity).val()).offset().top;
                leftPosition = $('#divParentFirstBox_' + $(app.DE.ddlParentEntity + ' :selected').data().entityType + '_' + $(app.DE.ddlParentEntity).val()).offset().left + 40;
                if ($('#divParentFirstBox_' + $(app.DE.ddlParentEntity + ' :selected').data().entityType + '_' + $(app.DE.ddlParentEntity).val()).children().length > 0) {
                    topPosition = $('#divParentFirstBox_' + $(app.DE.ddlParentEntity + ' :selected').data().entityType + '_' + $(app.DE.ddlParentEntity).val()).offset().top;
                    leftPosition = $('#divParentFirstBox_' + $(app.DE.ddlParentEntity + ' :selected').data().entityType + '_' + $(app.DE.ddlParentEntity).val()).children().last().offset().left + 40;
                }
            }
        }
        $clone.css({
            "position": 'fixed',
            "top": $that.offset().top,
            "left": $that.offset().left,
        }).animate({
            left: leftPosition, //$counter.children().last().offset().left + 40,
            top: topPosition// $counter.offset().top
        }, speed, function () {
            if ($(app.DE.ParentEntityTab).hasClass('active')) {
                $(html).find('.ParentFirstBox').attr('id', 'divParentFirstBox_' + response.entityType + '_' + response.systemId);
                if ($(app.DE.ddlShaft).val() != '' && $(app.DE.ddlFloor).val() != '') {
                    $('#div_shaft_' + $(app.DE.ddlShaft).val() + '_Floor_' + $(app.DE.ddlFloor).val()).append(html);
                } else {
                    $('#divFloor_' + $(app.DE.ddlFloor).val()).append(html);
                }
            } else if ($(app.DE.ChildEntityTab).hasClass('active')) {
                //$(html).find('.ParentBox').removeClass('hide');
                //$(html).find('.entityInfo').addClass('ChildEntity').parent().append(app.childParent);
                //$(html).find('.entityInfo').addClass('ChildEntity').parent().removeClass('EntityBox').addClass('ParentInnerBox');
                //$('#divParentFirstBox_' + $(app.DE.ddlParentEntity + ' :selected').data().entityType + '_' + $(app.DE.ddlParentEntity).val()).append(html);
            }
            // if (!isCallFromPageMessage) {
            alert(response.message, 'success', 'success');
            //}

            $clone.remove();
        });
    }

    this.ManageLibraryEvents = function (obj) {
        app.layerActions.entity.resetFocus($('.entityInfo'));
        app.clearCableEvents();
        if ($(obj).attr("data-tabName").toUpperCase() == "CABLE") {
            app.attachCableEvents();
        }

    }
    this.attachCableEvents = function () {
        //enable cable events.
        $(app.DE.ElementDots).css('visibility', 'visible');
    }
    this.clearCableEvents = function () {
        if (!$(app.DE.tempPath).data().isEditMode) {
            objCustomD3.removeTempPath();
            $(app.DE.ElementDots).css('visibility', 'hidden');
        }
        else {
            $(app.DE.EntityCancel).trigger("click");
        }
        $(app.DE.Cable_A_End).val(0).trigger("chosen:updated");
        $(app.DE.Cable_B_End).val(0).trigger("chosen:updated");
    }

    this.SaveCable = function (response) {

        if (response.objPM != undefined && response.objPM.status == "OK") {
            app.layerActions.entity.resetFocus($('.entityInfo'));
            if (response.objPM.isNewEntity) {
                d3.select('#svgCable').append('path')
                    .attr('d', d3.select(app.DE.tempPath).attr('d'))
                    .attr("pointer-events", "visibleStroke")
                    .attr("id", "ispCable_" + response.system_id)
                    .attr("class", "entityInfo P")
                    .attr("data-system-id", response.system_id)
                    .attr("data-total-core", response.total_core)
                    .attr("data-network-id", response.network_id)
                    .attr("data-cable-type", response.cable_type)
                    .attr("data-network-status", response.network_status)
                    .attr("data-a-system-id", response.a_system_id)
                    .attr("data-a-entity-type", response.a_entity_type)
                    .attr("data-a-node-type", response.a_node_type)
                    .attr("data-a-location", response.a_location)
                    .attr("data-b-system-id", response.b_system_id)
                    .attr("data-b-entity-type", response.b_entity_type)
                    .attr("data-b-node-type", response.b_node_type)
                    .attr("data-b-location", response.b_location)
                    .attr("data-entity-type", "Cable")
                    .attr("data-entity-geomtype", "LINE")
                    .attr("data-entity-title", "Cable").html('<title>' + response.network_id + '</title>');

                //refresh cable count in network layer..
                app.RefreshISPNetworkLayerElements($(app.DE.StructureId).val());

            }
            else {
                if ($(app.DE.tempPath).data().isEditMode) {
                    d3.select('#svgCable #ispCable_' + response.system_id)
                        .attr('d', d3.select(app.DE.tempPath).attr('d'))
                        .attr("pointer-events", "visibleStroke")
                        .attr("data-a-system-id", response.a_system_id)
                        .attr("data-a-entity-type", response.a_entity_type)
                        .attr("data-a-node-type", response.a_node_type)
                        .attr("data-a-location", response.a_location)
                        .attr("data-b-system-id", response.b_system_id)
                        .attr("data-b-entity-type", response.b_entity_type)
                        .attr("data-b-node-type", response.b_node_type)
                        .attr("data-b-location", response.b_location);
                    $('#svgCable #ispCable_' + response.system_id).css("display", "block");
                    $(app.DE.EntityCancel).parent().fadeOut();
                    $(app.DE.EntitySave).parent().fadeOut();
                }

                $(app.DE.ElementDots).css('visibility', 'hidden');

                app.enableInfoTool();
            }

            $(app.DE.tempPath).data().aSystemId = "";
            $(app.DE.tempPath).data().aEntityType = "";
            $(app.DE.tempPath).data().aLocation = "";
            $(app.DE.tempPath).data().bSystemId = "";
            $(app.DE.tempPath).data().bEntityType = "";
            $(app.DE.tempPath).data().bLocation = "";
            $(app.DE.tempPath).data().isEditMode = false;
            d3.select(app.DE.tempPath).attr('d', '');
            objCustomD3.removeActualMarkers();

        }

        $(popup.DE.closeModalPopup).trigger("click");
        alert(response.objPM.message, response.objPM.status == "OK" ? 'success' : 'warning', response.objPM.status == "OK" ? 'success' : 'information');
        // alert(response.objPM.message, 'success','success');
        //app.clearControl();
        $('#ddlCableAEnd').val('0').trigger('chosen:updated');
        $('#ddlCableBEnd').val('0').trigger('chosen:updated');
        //app.refreshStructureInfo();
    }
    /*
    //this.deleteISPEntity = function (systmId, eType, eTitle, gType) {
    //    var postData = { systemId: systmId, entityType: eType, geomType: gType };
    
    //    ajaxReq('Main/getDependentChildElements', postData, true, function (resp) {
    //        if (resp.status == "OK") {
    
    //            if (resp.results.ChildElements.length > 0) {
    //                var Msg = '<div class="table-responsive tble_cls"><table border="1" width="100%" class="webgrid" id="tblAlertDependendEntity"><thead><tr><td>Entity</td><td>Network Id</td></tr></thead><tbody>';
    //                $.each(resp.results.ChildElements, function (index, item) {
    //                    Msg += '<tr><td>' + item.Entity_Type + ' </td><td> ' + item.Network_Id + '</td></tr>';
    //                });
    //                Msg += '</tbody></table></div>';
    //                alert('Following are the dependent elements. You need to remove them first:<br/>' + Msg, 'warning');
    //            }
    
    //            else {
    //                var html = '<div id="dvEntityInformationDetail" class="infodiv"><div class="NoRecordInfo">Please click on any network entity for information.</div></div'
    //                var func = function () {
    //                    ajaxReq('Main/DeleteEntityFromInfo', postData, true, function (jResp) {
    //                        if (jResp.status == "OK") {
    //                            alert(jResp.message, 'success', 'success');
    //                            app.refreshStructureInfo();
    //                        } else {
    //                            alert(jResp.message, 'warning');
    //                        }
    //                    }, true, true);
    //                    $(app.DE.EntityInformationDetail).html(html);
    //                    if ($('.clsInfoTool').css('display') == 'block') {
    //                        $('.clsInfoTool').animate({ width: 'toggle' });
    //                        $(app.DE.leftMenuActions).removeClass('activeISPTab');
    //                    }
    
    //                };
    //                confirm('Are you sure you want to delete this ' + eType + ' ?', func);
    //            }
    //        }
    //        else {
    //            alert(resp.message, 'warning');
    
    //        }
    //    }, true, true);
    //}
    */
    this.getEntityReference = function (_entityId, _entityType, editPermission) {
        if ($("#dvRefrence").html().trim() == '') {
            ajaxReq('Library/GetReferenceEntity', {
                entityId: _entityId, entityType: _entityType
            }, true, function (resp) {
                $("#dvRefrence").html(resp);
                if (_entityId > 0) {
                    popup.disablebuttonWhenEditDisabled(editPermission);

                }
                $("#dvRefrence").css('background-image', 'none');
            }, false, false);
        }
    }



    this.OnOffNetworkLayers = function (isChecked) {
        $.each($(".mainlyr  input[type='checkbox']"), function (indx, itm) {
            $(this).prop('checked', isChecked);
            app.filterStructureEntity($(this).data().entityType, $(this).data().parentEntityType, isChecked);
        });
    }

    this.bindStructureEvents = function () {

        app.refreshParentDropDown();
        app.RefreshISPNetworkLayerElements($(app.DE.StructureId).val());
        objCustomD3.InitD3();
        // bind info events..
        if ($(app.DE.InformationTool).hasClass('activeToolBar')) {
            attachUnAttachEvt($(app.DE.EntityInfo), 'click', function () { app.EntityInfoDisplay(this); });
            $(app.DE.EntityInfo).css("cursor", "pointer");
        } else {
            attachUnAttachEvt($(app.DE.EntityInfo), 'click', function () { });
            $(app.DE.EntityInfo).unbind('click');
            $(app.DE.EntityInfo).css("cursor", "default");
        }
        if ($(app.DE.ChildEntityTab).hasClass('active')) {
            app.filterParentEntity($('#ddlParentEntityType').val());
        }
        app.cableActions.resize();
        $('.table-scroll').scrollTop(app.tableScroll.top).scrollLeft(app.tableScroll.left);
        app.getCPEConnections();
        app.cableActions.cableTemplateSuccess();
    }


    this.updateFloorInfo = function (obj) {
        var pageUrl = 'ISP/FloorInfo';
        var modalClass = 'modal-sm';
        popup.LoadModalDialog(app.ParentModel, pageUrl, { floorId: $(obj).data().systemId }, 'Floor Detail', modalClass);
    }

    this.isValidShiftEntity = function () {
        var floorId = $('#ddlFloorList').val();
        var isFloorNotSelected = (floorId == '' || floorId == null || floorId == undefined);
        //if (floorId == '') { alert('Please select floor first!', 'warning'); return false; }

        if ($('#hdnEntityType').val() == app.EnumEntityType.UNIT && !isFloorNotSelected && !app.isValidUnit(floorId)) {
            return false;
        }

        if (app.convert.toBoolean($('#hdnIsShaftElement').val()) && !app.convert.toBoolean($('#hdnIsFloorElement').val())) {
            let shaftId = $('#ddlShaftList').val();
            if (isFloorNotSelected) { alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_022, 'warning'); return false; }//Please select shaft first!
            else if (isFloorNotSelected) { alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_023, 'warning'); return false; }//Please select floor first!
            if (!isFloorNotSelected && !app.structureActions.floor.isVisibleFloor(shaftId, floorId)) { alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_017, 'warning'); return false; }//Selected floor is disabled!
        }
        else if (app.convert.toBoolean($('#hdnIsFloorElement').val()) && !app.convert.toBoolean($('#hdnIsShaftElement').val())) {
            if (app.convert.toBoolean($('#hdnisISPChildLayer').val()) && $('#divParentEntity').length > 0 && isFloorNotSelected && $('#ddlUnitList').length > 0 && $('#ddlUnitList').val() == '0') {
                alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_015, 'warning'); return false;//Please select floor/unit first!
            } else if (isFloorNotSelected && $('#ddlUnitList').length == 0) {
                alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_023, 'warning'); return false;//Please select floor first!
            }
        }
        else if (app.convert.toBoolean($('#hdnIsShaftElement').val()) && app.convert.toBoolean($('#hdnIsFloorElement').val())) {
            if (app.convert.toBoolean($('#hdnisISPChildLayer').val()) && $('#divParentEntity').length > 0 && isFloorNotSelected && $('#ddlUnitList').length > 0 && $('#ddlUnitList').val() == '0') {
                alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_015, 'warning'); return false;//Please select floor/unit first!
            } else if (isFloorNotSelected && $('#ddlUnitList').length == 0) {
                alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_023, 'warning'); return false;//Please select floor first!
            }
        }
        if ($('#hdnEntityType').val().toUpperCase() == app.EnumEntityType.UNIT) {
            var unitHeight = 0, unitWidth = 0, unitLength = 0;
            var floorData = $('#divFloor_' + floorId).data();
            var additionAttr = $('#div_UNIT_' + $('#hdnEntityId').val()).attr('data-additionalattr').split('|');
            if (additionAttr.length >= 3) {
                unitWidth = parseInt(additionAttr[0].split('=')[1]);
                unitHeight = parseInt(additionAttr[1].split('=')[1]);
                unitLength = parseInt(additionAttr[2].split('=')[1]);
            }
            if (unitHeight > parseInt(floorData.height)) {
                //UNIT height can not be greater then floor height//Floor Height//Unit Height
                var message = MultilingualKey.SI_ISP_GBL_JQ_GBL_002;
                message += '<br/><div class="table-responsive tble_cls"><table border="1" width="100%" class="webgrid" id="tblAlertDependendEntity"><tbody>';
                message += '<tr><td>' + MultilingualKey.SI_ISP_GBL_JQ_GBL_003 + ' </td><td> ' + parseInt(floorData.height) + '(m)</td></tr>';
                message += '<tr><td>' + MultilingualKey.SI_ISP_GBL_JQ_GBL_004 + ' </td><td> ' + unitHeight + '(m)</td></tr>';
                message += '</tbody></table></div>';
                alert(message, "warning"); return false;
            }
            if (unitWidth > parseInt(floorData.width)) {
                //UNIT length can not be greater then floor length!//Floor Width//Unit Width
                var message = MultilingualKey.SI_ISP_GBL_JQ_GBL_008;
                message += '<br/><div class="table-responsive tble_cls"><table border="1" width="100%" class="webgrid" id="tblAlertDependendEntity"><tbody>';
                message += '<tr><td>' + MultilingualKey.SI_ISP_GBL_JQ_GBL_006 + ' </td><td> ' + parseInt(floorData.width) + '(m)</td></tr>';
                message += '<tr><td>' + MultilingualKey.SI_ISP_GBL_JQ_GBL_007 + ' </td><td> ' + unitWidth + '(m)</td></tr>';
                message += '</tbody></table></div>';
                alert(message, "warning"); return false;
            }
            if (unitLength > parseInt(floorData.length)) {
                //UNIT length can not be greater then floor length!//Floor Length//Unit Length
                var message = MultilingualKey.SI_ISP_GBL_JQ_GBL_008;
                message += '<br/><div class="table-responsive tble_cls"><table border="1" width="100%" class="webgrid" id="tblAlertDependendEntity"><tbody>';
                message += '<tr><td>' + MultilingualKey.SI_ISP_GBL_JQ_GBL_009 + ' </td><td> ' + parseInt(floorData.length) + '(m)</td></tr>';
                message += '<tr><td>' + MultilingualKey.SI_ISP_GBL_JQ_GBL_010 + ' </td><td> ' + unitLength + '(m)</td></tr>';
                message += '</tbody></table></div>';
                alert(message, "warning");
                return false;
            }

            var allRoomLength = 0;
            var availableLength = 0;
            $('#divFloor_' + floorId + ' div[data-entity-type="UNIT"]').each(function () {
                allRoomLength += parseFloat($(this).attr('data-additionalattr').split('|')[2].replace('length=', ''));
            })
            availableLength = (parseInt(floorData.length) - allRoomLength);
            allRoomLength = allRoomLength + unitLength;
            //if (availableLength == 0) {
            //    alert('Space is not available on <b> ' + $('#ddlFloorList :selected').text() + ' </b> floor!', "warning");
            //    return false;
            //} else
            if (allRoomLength == parseInt(floorData.length)) {
                //selected UNIT can not be added into//as unit length is greater than the available!//Floor Length//Available Floor Length//Unit Length

                var message = getMultilingualStringValue($.validator.format(MultilingualKey.SI_ISP_GBL_JQ_GBL_012, $('#ddlFloorList :selected').text()));
                //MultilingualKey.SI_ISP_GBL_JQ_GBL_011 + '  <b> ' + $('#ddlFloorList :selected').text() + ' </b>  ' + MultilingualKey.SI_ISP_GBL_JQ_GBL_012;
                message += '<br/><div class="table-responsive tble_cls"><table border="1" width="100%" class="webgrid" id="tblAlertDependendEntity"><tbody>';
                message += '<tr><td>' + MultilingualKey.SI_ISP_GBL_JQ_GBL_009 + ' </td><td> ' + parseInt(floorData.length) + '(m)</td></tr>';
                message += '<tr><td>' + MultilingualKey.SI_ISP_GBL_JQ_GBL_013 + '</td><td> ' + availableLength + '(m)</td></tr>';
                message += '<tr><td>' + MultilingualKey.SI_ISP_GBL_JQ_GBL_010 + '</td><td> ' + unitLength + '(m)</td></tr>';
                message += '</tbody></table></div>';
                alert(message, "warning");
                return false;
            }
        }
        return true;
    }

    this.initiateSplicing = function (obj) {
        app.layerActions.entity.focus($(obj));
        var attr = $(obj).data();
        var offset = $(obj).offset();
        var titleText = 'Splicing';
        var dataURL = 'ISP/Splicing';
        ajaxReq(dataURL, { structureId: $(app.DE.StructureId).val(), systemId: attr.systemId, entityType: attr.entityType, point_x: parseInt(offset.left), point_y: parseInt(offset.top) }, true, function (resp) {
            //$(app.DE.splicingMain).html(resp);
            //$(app.DE.SplicingDiv).show();
            //$(app.DE.SplicingDiv).draggable({ scroll: false, handle: "#splicingHeader", containment: "window" });
            $('#divSplicingWindow').html(resp).show()

        }, false, true);
    }

    this.ClearSplicingRelatedObjs = function () {
        var sourceId = $(app.DE.SourceCable).val();
        var destinationId = $(app.DE.DestinationCable).val();
        if (sourceId != '0') {
            var cableId = 'ispCable_' + sourceId;
            $('#' + cableId).attr("class", $('#' + cableId).attr("class").replace('pathAnimation', ''));




        }
        if (destinationId != '0') {
            var cableId = 'ispCable_' + destinationId;
            $('#' + cableId).attr("class", $('#' + cableId).attr("class").replace('pathAnimation', ''));
        }
        $('#pointervgContainer').html('');
        $(app.DE.DestinationCable).val('0').trigger("chosen:updated");
        $(app.DE.SourceCable).val('0').trigger("chosen:updated");
        $(app.DE.ConnectingEntity).val('0').trigger("chosen:updated");
        $(app.DE.ddlcustomer).val('0').trigger("chosen:updated");
        $(app.DE.ddlCPEEntity).val('').trigger("chosen:updated");
    }

    this.cancelSplice = function (obj) {
        $('#divSplicingWindow').hide();
        let $tool = $(app.DE.SplicingTool);
        app.destinationCableId = null;
        app.sourceCableId = null;
        app.clearLineAnimation();
        app.layerActions.entity.resetFocus($('.entityInfo'));
        if ($tool.hasClass('activeToolBar')) {
            $tool.removeClass('activeToolBar');
            app.removeRedHandPointer();
        }
    }

    this.removeRedHandPointer = function () {
        let $pointer = $(app.DE.EntityInfo);
        attachUnAttachEvt($pointer, 'click', function () { });
        $pointer.unbind('click');
        $pointer.css("cursor", "default");
    }
    this.clearLineAnimation = function () {
        $('#svgCable path:not("#tempPath")').each(function () {
            $(this).attr("class", $(this).attr("class").replace('pathAnimation', ''));
        });
    }

    this.uploadImageFile = function () {
        //Get File from uploader and prepare form data to post.
        var frmData = new FormData();
        var filesize = $('#hdnMaxFileUploadSizeLimit').val();
        var Sizeinbytes = filesize * 1024;
        if ($(app.DE.UploadImage).get(0).files[0].size > Sizeinbytes) {
            var validFileSizeinMB = filesize % 1024 == 0 ? parseInt(filesize / 1024) : (filesize / 1024).toFixed(2);
            alert(getMultilingualStringValue($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_112, validFileSizeinMB)));
            //alert(MultilingualKey.SI_OSP_GBL_GBL_GBL_053 + "  <b>" + validFileSizeinMB + "<b> MB!", 'warning');//Image size is too large. Maximum image size allowed is
        }
        else {
            var uploadedfile = $(app.DE.UploadImage).get(0).files[0];
            if (uploadedfile == undefined || uploadedfile == null) {
                alert(MultilingualKey.SI_OSP_GBL_GBL_GBL_099, 'warning');//Please select a file!
                return false;
            }
            if (!app.validateImageFileType()) { return false; }
            frmData.append(uploadedfile.name, uploadedfile);
            frmData.append('system_Id', $(app.DE.liImage).data().systemId);
            frmData.append('entity_type', $(app.DE.liImage).data().entityType);
            ajaxReqforFileUpload('Main/UploadImage', frmData, true, function (resp) {
                if (resp.status == "OK") {
                    app.getElementImages();
                    alert(resp.message, 'success', 'success');
                    if ($(app.DE.UploadImage)[0] != undefined)
                        $(app.DE.UploadImage)[0].value = '';
                }
                else {
                    alert(resp.message, 'warning');
                }
            }, true);
        }
    }

    this.validateImageFileType = function () {
        var validFilesTypes = ["bmp", "gif", "png", "jpg", "jpeg"];
        var file = $(app.DE.UploadImage).val();
        var filepath = file;
        return app.ValidateFileType(validFilesTypes, filepath);
    }

    this.getElementImages = function () {
        app.InfoProgress(app.DE.divImage);
        var _system_Id = $(app.DE.liImage).data().systemId;
        var _entity_type = $(app.DE.liImage).data().entityType;
        ajaxReq('ISP/getISPEntityImages', { system_Id: _system_Id, entity_type: _entity_type }, true, function (jResp) {
            $(app.DE.divImage).html(jResp);
            $('#OrbitHolder').orbit();
        }, false, false, true);
    }

    this.ValidateFileType = function (validFilesTypes, filepath) {
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
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_059 +
                " extension:\n\n" + validFilesTypes.join(", "), 'warning');
        }
        return isValidFile;
    }

    this.InfoProgress = function (divId) {
        var infoProgress = '<div id="dvInfoProgress" style="display: block;"><div id="blur" class="infoProgresblur">&nbsp;</div><div id="infoProgressbar" style="width: 247px; height: 152px; top: 40%; left: 5%;">';
        infoProgress += '<img alt="Loading..." src="../content/images/loading_new.gif" /><br /></div></div>';
        $(divId).html(infoProgress);
    }

    this.deleteEntityImages = function () {
        //Ready the list of selected images for delete
        var ListSystemIds = [];
        $.each($(".ImagesContainer  input[type='checkbox']"), function (indx, itm) {
            if ($(this).is(':checked')) {
                ListSystemIds.push($(this).data().systemId)
            }
        });
        if (ListSystemIds.length > 0) {
            var func = function () {
                ajaxReq('Main/DeleteEntityImages', { ListSystem_Id: ListSystemIds }, true, function (j) {
                    alert(j.message, 'success', 'success');
                    app.getElementImages();
                }, true, true)
            };
            showConfirm(MultilingualKey.SI_ISP_GBL_JQ_GBL_018, func);//Are you sure you want to delete selected images?
        } else {
            alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_019, 'warning');//Please select any image!
        }
    }

    this.downloadEntityImages = function (entityType) {
        //Ready the list of selected images for download
        var listPathName = [];
        $.each($(".ImagesContainer  input[type='checkbox']"), function (indx, itm) {
            if ($(this).is(':checked')) {
                listPathName.push({ systemId: $(this).data().systemId });
                //listPathName.push({ name: $(this).data().imageName, location: $(this).data().imageLocation });
            }
        });
        if (listPathName.length > 0) {
            window.location.href = 'DownloadFiles?json=' + JSON.stringify(listPathName) + '&entity_type=' + entityType;
        }
        else
            alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_019, 'warning');//Please select any image!
    }

    this.uploadDocumentFile = function () {
        var frmData = new FormData();
        var filesize = $('#hdnMaxFileUploadSizeLimit').val();

        var Sizeinbytes = filesize * 1024;
        if ($(app.DE.UploadDocument).get(0).files[0].size > Sizeinbytes) {
            //File size is too large. Maximum file size allowed is

            alert(getMultilingualStringValue($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_FRM_109, (filesize / 1024).toFixed(2))));
            //alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_109 + "  <b>" + (filesize / 1024).toFixed(2) + "<b> MB!", 'warning');
        }
        else {
            var uploadedfile = $(app.DE.UploadDocument).get(0).files[0];
            if (!app.validateDocumentFileType()) { return false; }
            frmData.append(uploadedfile.name, uploadedfile);
            frmData.append('system_Id', $(app.DE.liDocument).data().systemId);
            frmData.append('entity_type', $(app.DE.liDocument).data().entityType);
            ajaxReqforFileUpload('Main/UploadDocument', frmData, true, function (resp) {
                if (resp.status == "OK") {
                    app.getAttachmentFiles();
                    alert(resp.message, 'success', 'success');
                    if ($(app.DE.UploadDocument)[0] != undefined)
                        $(app.DE.UploadDocument)[0].value = '';
                }
                else {
                    alert(resp.message, 'warning');
                }

            }, true);
        }
    }

    this.getAttachmentFiles = function () {
        app.InfoProgress(app.DE.divDocument);
        var _system_Id = $(app.DE.liDocument).data().systemId;
        var _entity_type = $(app.DE.liDocument).data().entityType;
        ajaxReq('ISP/getAttachmentDetails', { system_Id: _system_Id, entity_type: _entity_type }, true, function (jResp) {
            $(app.DE.divDocument).html(jResp);
        }, false, false, true);
    }

    this.validateDocumentFileType = function () {
        var validFilesTypes = ["dwg", "pdf", "jpeg", "jpg", "doc", "docx", "xls", "xlsx", "csv", "vsd", "ppt", "pptx", "png", "htm", "html"];
        var file = $(app.DE.UploadDocument).val();
        var filepath = file;
        return app.ValidateFileType(validFilesTypes, filepath);
    }

    this.deleteEntityDocuments = function (attachmentId) {
        //Ready the list of selected images for delete
        var ListSystemIds = [];
        $.each($("#ulDocumentUpload  input[type='checkbox']"), function (indx, itm) {
            if ($(this).is(':checked')) {
                ListSystemIds.push($(this).data().systemId)
            }
        });
        if (ListSystemIds.length > 0) {
            var func = function () {
                ajaxReq('Main/DeleteAttachmentFiles', { ListSystem_Id: ListSystemIds }, true, function (j) {
                    alert(j.message, 'success', 'success');
                    app.getAttachmentFiles();
                }, true, true)
            };
            showConfirm(MultilingualKey.SI_OSP_GBL_JQ_FRM_004, func);//Are you sure you want to delete this file?
        } else {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_005, 'warning');//Please select any file!
        }
    }

    this.downloadEntityDocuments = function (entityType) {
        //Ready the list of selected images for download
        var listPathName = [];
        $.each($("#ulDocumentUpload  input[type='checkbox']"), function (indx, itm) {
            if ($(this).is(':checked')) {
                listPathName.push({ systemId: $(this).data().systemId });
            }
        });
        if (listPathName.length > 0) {
            window.location.href = 'DownloadFiles?json=' + JSON.stringify(listPathName) + '&entity_type=' + entityType;
        } else {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_005, 'warning');//Please select any file!
        }
    }
    this.isUnitValid = function (systemId, obj) {
        var structureTotalUnits = parseInt($('#total_unit').val());
        var totalUsedUnits = 0;
        $.each($('.str-floor'), function () {
            var floorId = $(this).attr('id');
            if (floorId != 'divFloor_' + systemId) {
                totalUsedUnits = totalUsedUnits + parseInt($(this).data().totalUnit);
            }
        })
        if (parseInt($(obj).val()) > (structureTotalUnits - totalUsedUnits)) {
            //Maximum units are allowed in this structure!

            alert($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_FRM_079, structureTotalUnits), 'warning');
            $(obj).val($('#divFloor_' + systemId).data().totalUnit);
            return false;
        }
    }
    this.updateAllCablePath = function () {
        var ospISPCable = [];
        $.each($('path[data-cable-type="OSP"][d=""]'), function (index, item) {
            var attr = $(this).data();
            var isp_tp_system_id, isp_tp_entity_type;
            let isEndPoint = false;
            // decide isp termination point
            if (attr.aEntityType != '') {
                if ($("#div_" + attr.aEntityType.toUpperCase() + "_" + attr.aSystemId).length > 0) {
                    isp_tp_system_id = attr.aSystemId;
                    isp_tp_entity_type = attr.aEntityType;
                    isEndPoint = false;
                }
            }

            if (attr.bEntityType != '') {
                if ($("#div_" + attr.bEntityType.toUpperCase() + "_" + attr.bSystemId).length > 0) {
                    isp_tp_system_id = attr.bSystemId;
                    isp_tp_entity_type = attr.bEntityType;
                    isEndPoint = true;
                }
            }

            if (isp_tp_system_id > 0 && isp_tp_entity_type != '') {
                var path = objCustomD3.getOSPCablePath(isp_tp_system_id, isp_tp_entity_type.toUpperCase(), isEndPoint);
                $(this).attr('d', path);
                ospISPCable.push({
                    line_geom: path, StructureId: $(app.DE.StructureId).val(), system_id: parseInt(attr.systemId), geom_source: 'S'
                });
            }
        });
        $.each($('path[data-cable-type="ISP"][d=""]'), function (index, item) {
            var attr = $(this).data();


            let res = objCustomD3.getISPCablePath($("#div_" + attr.aEntityType + "_" + attr.aSystemId), $("#div_" + attr.bEntityType + "_" + attr.bSystemId));
            $(this).attr('d', res.path);
            ospISPCable.push({
                line_geom: res.path,
                StructureId: $(app.DE.StructureId).val(),
                system_id: parseInt(attr.systemId),
                geom_source: 'S',
                a_node_type: res.positionA.dot,
                b_node_type: res.positionB.dot
            });
        });
        if (ospISPCable.length > 0) {
            ajaxReq('ISP/createOSPISPCable', { ospISPCables: ospISPCable }, false, function (response) { }, true, false);
        }
    }

    this.getPointArrayFromSvgPath = function (_path, _cableType) {
        var _finalPointArray = [];
        var nodes = _path.split(/(?=[LMC])/);

        for (var i = 0; i < nodes.length; i++) {
            var point = nodes[i].slice(1, nodes[i].length).split(',');
            var _isEditAllowed = _cableType.toUpperCase() == "OSP" && i == nodes.length - 1 ? false : true;
            if (_finalPointArray.findIndex(function (m) { return m.x == point[0] && m.y == point[1] }) < 0) {
                _finalPointArray.push({ x: point[0], y: point[1], isVirtual: false, isMidPoint: false, isEditAllowed: _isEditAllowed });
            }
        }
        return _finalPointArray;
    }


    this.SnapCableEndPoints = function () {

        // get all cable paths...
        var arrCableNewPaths = [];
        $(app.DE.CablePaths).each(function () {
            var objCableData = $(this).data();
            var arrCablePoints = app.getPointArrayFromSvgPath($(this).attr('d'), objCableData.cableType);
            if (arrCablePoints.length > 0) {

                var isChangeFound = false;
                //validate start point and update path if require..
                var startPoint = arrCablePoints[0];

                var objStartElementNode = $('#div_' + objCableData.aEntityType + '_' + objCableData.aSystemId + ' > span.' + objCableData.aNodeType);
                //var objStartElementNode = $('#div_' + objCableData.bEntityType + '_' + objCableData.aSystemId + ' > span.' + objCableData.bEntityType);

                if (objStartElementNode.length > 0) {
                    var startElementPostion = getElementPosition(objStartElementNode, $(app.DE.SVGContainer));
                    var aEndGap = Math.sqrt(((startPoint.x - startElementPostion.x) * (startPoint.x - startElementPostion.x)) + ((startPoint.y - startElementPostion.y) * (startPoint.y - startElementPostion.y)));

                    if (aEndGap > 1) {
                        isChangeFound = true;

                        // cable 1st node last node distance , and 1st node and new node distance,

                        var OldCableNodeDistance = Math.sqrt(((arrCablePoints[0].x - arrCablePoints[arrCablePoints.length - 1].x)
                            * (arrCablePoints[0].x - arrCablePoints[arrCablePoints.length - 1].x))
                            + ((arrCablePoints[0].y - arrCablePoints[arrCablePoints.length - 1].y) * (arrCablePoints[0].y - arrCablePoints[arrCablePoints.length - 1].y)));


                        var newCableNodeDistance = Math.sqrt(((startElementPostion.x - arrCablePoints[arrCablePoints.length - 1].x)
                            * (startElementPostion.x - arrCablePoints[arrCablePoints.length - 1].x))
                            + ((startElementPostion.y - arrCablePoints[arrCablePoints.length - 1].y) * (startElementPostion.y - arrCablePoints[arrCablePoints.length - 1].y)));


                        var isCableShrinked = OldCableNodeDistance > newCableNodeDistance;

                        //validate 2nd point geom..   
                        var prevIndex = 0;
                        for (var index = 1; index < arrCablePoints.length - 2; index++) {
                            if ((index - prevIndex) == 1) {
                                if (arrCablePoints[index].x == arrCablePoints[0].x) {
                                    var prevIndex = 0;
                                    for (var index = 1; index < arrCablePoints.length - 2; index++) {



                                        arrCablePoints[index].x = startElementPostion.x;
                                        //index point and its previous point ->diff ,

                                        if (isCableShrinked) {
                                            if ((startElementPostion.x - startPoint.x) > 0) // forward case for x..
                                            {
                                                if (arrCablePoints[index].x - arrCablePoints[arrCablePoints.length - 2].x > 0) {
                                                    arrCablePoints.splice(index, 1);
                                                    continue;
                                                }
                                                if (arrCablePoints[index].y == arrCablePoints[0].y) {
                                                    arrCablePoints[index].y = startElementPostion.y;
                                                }
                                                else {
                                                    var xgap = Math.abs(startPoint.x - startElementPostion.x);
                                                    arrCablePoints[index].x = parseInt(arrCablePoints[index].x) + ((startElementPostion.x - startPoint.x) > 0 ? xgap : -xgap);
                                                }
                                            }
                                            else { // backward scnerio for x...

                                                prevIndex = index;
                                                if (arrCablePoints[index].x - arrCablePoints[arrCablePoints.length - 2].x < 0) {
                                                    arrCablePoints.splice(index, 1);
                                                    continue;
                                                }
                                                else {
                                                    break;
                                                    var xgap = Math.abs(startPoint.x - startElementPostion.x);
                                                    arrCablePoints[index].x = parseInt(arrCablePoints[index].x) + ((startElementPostion.x - startPoint.x) > 0 ? xgap : -xgap);
                                                }

                                            }

                                            if ((startElementPostion.y - startPoint.y) > 0) // backward case for y..
                                            {
                                                if (arrCablePoints[index].y - arrCablePoints[arrCablePoints.length - 2].y > 0) {
                                                    arrCablePoints.splice(index, 1);
                                                    continue;
                                                }
                                                else {
                                                    var ygap = Math.abs(startPoint.y - startElementPostion.y);
                                                    arrCablePoints[index].y = parseInt(arrCablePoints[index].y) + ((startElementPostion.y - startPoint.y) > 0 ? ygap : -ygap);
                                                }
                                            }
                                            else { // backward scnerio for y...

                                                if (arrCablePoints[index].y - arrCablePoints[arrCablePoints.length - 2].y < 0) {
                                                    arrCablePoints.splice(index, 1);
                                                    continue;
                                                }
                                                else {
                                                    var ygap = Math.abs(startPoint.y - startElementPostion.y);
                                                    arrCablePoints[index].y = parseInt(arrCablePoints[index].y) + ((startElementPostion.y - startPoint.y) > 0 ? ygap : -ygap);
                                                }

                                            }
                                        }




                                        // var newX = startElementPostion.x + (isCableShrinked ? xgap : -xgap);

                                        //var ygap = Math.abs(arrCablePoints[index].y - arrCablePoints[0].y);

                                        // var newY = startElementPostion.y + (isCableShrinked ? ygap : -ygap);

                                        //
                                        //arrCablePoints[index].y = newY;



                                        //var xgap = Math.abs(arrCablePoints[index].x - arrCablePoints[0].x);

                                        // var newX = startElementPostion.x + (isCableShrinked ? xgap : -xgap);

                                        //var ygap = Math.abs(arrCablePoints[index].y - arrCablePoints[0].y);

                                        // var newY = startElementPostion.y + (isCableShrinked ? ygap : -ygap);

                                        //arrCablePoints[index].x = newX;
                                        //arrCablePoints[index].y = newY;




                                    }




                                    // update first point geom
                                    arrCablePoints[0].x = startElementPostion.x;
                                    arrCablePoints[0].y = startElementPostion.y;
                                    console.log('startPointChanged : Yes');
                                }
                            }
                            else {
                                break;
                            }
                        }

                        //validate start point and update path if require..

                        var endPoint = arrCablePoints[arrCablePoints.length - 1];
                        var objEndElementNode = $('#div_' + objCableData.bEntityType + '_' + objCableData.bSystemId + ' > span.' + objCableData.bNodeType);
                        var objEndElementNode = $('#div_' + objCableData.bEntityType + '_' + objCableData.bSystemId + ' > span.' + objCableData.bNodeType);

                        if (objEndElementNode.length > 0) {

                            var endElementPostion = getElementPosition(objEndElementNode, $(app.DE.SVGContainer));
                        }

                        var bEndGap = Math.sqrt(((endPoint.x - endElementPostion.x) * (endPoint.x - endElementPostion.x)) + ((endPoint.y - endElementPostion.y) * (endPoint.y - endElementPostion.y)));

                        if (bEndGap > 1) {
                            isChangeFound = true;

                            //validate 2nd last point geom..
                            var prevIndex = arrCablePoints.length - 1;
                            for (var index = arrCablePoints.length - 2; index > 1; index--) {
                                if ((prevIndex) - index == 1) {
                                    if (arrCablePoints[index].x == arrCablePoints[arrCablePoints.length - 1].x) {

                                        arrCablePoints[index].x = endElementPostion.x;
                                    }
                                    if (arrCablePoints[index].y == arrCablePoints[arrCablePoints.length - 1].y) {
                                        arrCablePoints[index].y = endElementPostion.y;
                                    }

                                    prevIndex = index;
                                }
                                else {
                                    break;
                                }
                            }



                            arrCablePoints[arrCablePoints.length - 1].x = endElementPostion.x;
                            arrCablePoints[arrCablePoints.length - 1].y = endElementPostion.y;
                            console.log('EndPointChanged : Yes');
                        }
                    }

                    if (isChangeFound) {
                        //get point array to string ...
                        var cablePath = objCustomD3.lineFunction(arrCablePoints);
                        $(this).attr('d', cablePath);
                        arrCableNewPaths.push({ system_id: objCableData.systemId, structure_id: $(app.DE.StructureId).val(), line_geom: cablePath });
                        console.log('Path::' + cablePath);
                    }

                    prevIndex = index;
                }
            }
        });
    }
    this.updateShaftInfo = function (obj) {
        var pageUrl = 'ISP/ShaftInfo';
        var modalClass = 'modal-sm';
        popup.LoadModalDialog(app.ParentModel, pageUrl, { shaftId: $(obj).data().systemId, structureId: $(obj).data().structureId }, 'Shaft Detail', modalClass);
    }
    this.enableDisableParent = function (obj) {
        if (obj == 'ddlUnitList' && $('#ddlUnitList').val() != '0') {
            $('#ddlShaftList').val('').trigger('chosen:updated');
            $('#hdnParentEntityType').val($('#ddlUnitList option:selected').attr('data-entity-type'));
            $('#hdnParentSystemId').val($('#ddlUnitList').val());
            $('#hdnParentNetworkid').val($('#ddlUnitList option:selected').attr('data-network-id'));
        } else {
            $('#hdnParentEntityType').val(app.EnumEntityType.Structure);
            $('#hdnParentSystemId').val($(app.DE.StructureId).val());
            $('#hdnParentNetworkid').val($('#hdnStructureNwId').val());
        }
    }

    this.layerActions = {
        
        networkStatus: {
            convertStatus: function (_systemId, _entityType, _networkStatus, _oldStatus) {
                var _old = app.layerActions.networkStatus.getNetStatus(_oldStatus);
                var _new = app.layerActions.networkStatus.getNetStatus(_networkStatus);
                var layerTitle = getLayerTltle(_entityType);
                //Are you sure you want convert
                 
                confirm(getMultilingualStringValue($.validator.format(MultilingualKey.SI_ISP_GBL_JQ_GBL_020, layerTitle, _old, _new)), function () {
                    ajaxReq('Main/NetworkStage', { systemid: _systemId, entity_type: _entityType, curr_status: _networkStatus, old_status: _oldStatus }, true, function (resp) {
                        if (resp.status == 'OK') {
                            //successfully converted from


                            //var retmsg = layerTitle + " "+MultilingualKey.SI_ISP_GBL_JQ_GBL_021+" " + _old + ' to ' + _new + '.';
                            alert($.validator.format(MultilingualKey.SI_ISP_GBL_JQ_GBL_021, layerTitle, _old, _new), 'success', 'success');

                            var $e = app.layerActions.entity.select({ type: _entityType, systemId: _systemId });
                            if ($e) {
                                $e.data('networkStatus', _networkStatus);
                                app.layerActions.entity.setNetworkStatus($e, { type: _entityType, oldStatus: _oldStatus, newStatus: _networkStatus });
                                var attr = $e.data();
                                if (attr)
                                    app.ShowEntityInformationDetail(attr.systemId, attr.entityType, attr.entityTitle, attr.entityGeomtype, false, attr.networkStatus);
                            }
                        }
                        else {
                            alert(resp.results.message, 'warning', 'Information');
                        }

                    }, false, true, false);
                });
            },
            convertToPlanned: function (_systemId, _entityType, _oldStatus) {
                app.layerActions.networkStatus.convertStatus(_systemId, _entityType, 'P', _oldStatus);
            },
            convertToAsBuilt: function (_systemId, _entityType, _oldStatus) {
                app.layerActions.networkStatus.convertStatus(_systemId, _entityType, 'A', _oldStatus);
            },
            convertToDormant: function (_systemId, _entityType, _oldStatus) {
                app.layerActions.networkStatus.convertStatus(_systemId, _entityType, 'D', _oldStatus);
            },
            getNetStatus: function (Status) {
                var network = '';
                if (Status == 'P')
                    network = 'Planned';
                else if (Status == 'A')
                    network = 'As-Built';
                else
                    network = 'Dormant';
                return network;
            },
            bind: function (systemID, eName, nStatus) {
                let $doc = $(document);
                $doc.off('click', app.DE.EntityConvertToPlanned);
                $doc.off('click', app.DE.EntityConvertToAsBuilt);
                $doc.off('click', app.DE.EntityConvertToDormant);
                if (nStatus == 'P') {
                    $(app.DE.EntityConvertToPlanned).parent().hide();
                }
                else {
                    $doc.on('click', app.DE.EntityConvertToPlanned, function (e) {
                        app.layerActions.networkStatus.convertToPlanned(systemID, eName, nStatus);
                    });
                }
                if (nStatus == 'A' || nStatus == 'D') {
                    $(app.DE.EntityConvertToAsBuilt).parent().hide();
                }
                else {
                    $doc.on('click', app.DE.EntityConvertToAsBuilt, function (e) {
                        app.layerActions.networkStatus.convertToAsBuilt(systemID, eName, nStatus);
                    });
                }
                if (nStatus == 'D') {
                    $(app.DE.EntityConvertToDormant).parent().hide();
                }
                else {
                    $doc.on('click', app.DE.EntityConvertToDormant, function (e) {
                        app.layerActions.networkStatus.convertToDormant(systemID, eName, nStatus);
                    });
                }
            }
        },
        history: {
            get: function (e) {
                var attr = $(e.currentTarget).data();
                app.layerActions.history.render(attr.systemId, attr.entityType);
            },
            render: function (systemId, entityType) {
                var formURL = "Audit/GetHistory";
                var layerTitle = getLayerTltle(entityType);
                var titleText = layerTitle.toUpperCase() + " History";
                popup.LoadModalDialog('PARENT', formURL, { systemId: systemId, eType: entityType }, titleText, 'modal-lg');
            }
        },
        exportData: {
            download: function (e) {
                var attr = $(e.currentTarget).data();
                app.layerActions.exportData.render(attr.systemId, attr.entityType, attr.geomType);
            },
            downloadConnections: function (IsConnection) {
                 
                ExportRoomViewDetail(IsConnection);
            },
            render: function (systemID, entityType, geomType) {
                var _networkstage = '';
                ajaxReq('Main/CheckEntityData', { systemId: systemID, entityType: entityType, networkStage: '' }, false, function (status) {
                    if (status != null && status != undefined) {
                        if (status)  // check if true
                        {
                            window.location = appRoot + 'Main/ExportInfoEntity?systemId=' + systemID + '&entityType=' + entityType + '&networkStage=' + _networkstage + '';
                        }
                        else {
                            alert(MultilingualKey.SI_OSP_GBL_JQ_RPT_014, "warning");//No records found!
                        }
                    }
                }, true, true);

            }
        },
        exportSplicing: {
            showSplicingReport: function (e) {
                var attr = $(e.currentTarget).data();
                var formURL = "Splicing/GetSplicingReport";
                var layerTitle = getLayerTltle(attr.entityType );
                var titleText = layerTitle + " Splicing Report";
                popup.LoadModalDialog('PARENT', formURL, {
                    source_system_id: attr.systemId , source_entity_type: attr.entityType
                }, titleText, 'modal-lg');
            }
            //download: function (e) {
            //    var attr = $(e.currentTarget).data();
            //    window.location = appRoot + 'Splicing/ExportSplicing?systemId=' + attr.systemId + '&entityType=' + attr.entityType + '';
            //    //app.layerActions.exportSplicing.render(attr.systemId, attr.entityType);
            //},
            //render: function (systemID, entityType,) {

            //    window.location = appRoot + 'Splicing/ExportSplicing?systemId=' + systemID + '&entityType=' + entityType + '';

            //}
        },
        detail: {
            get: function (e) {
                 
                var attr = $(e.currentTarget).data();
                var NetworkStatus = '';
                app.layerActions.detail.render({ systemId: attr.systemId, entityType: attr.entityType, geomType: attr.geomType, networkStatus: NetworkStatus });
            },
            render: function (_data) {
                var modelClass = getPopUpModelClass(_data.entityType);
                var lyrDetail = getLayerDetail(_data.entityType);
                var formURL = lyrDetail['layer_form_url'];
                var titleText = lyrDetail['layer_title'];
                var strucIdVal = $(app.DE.StructureId).val();
                var _model = { ModelInfo: { structureid: strucIdVal } }
                var _data = $.extend(_data, _model);

                popup.LoadModalDialog(app.ParentModel, formURL, _data, titleText, modelClass);
            }
        },
        ConnectedCustomerDetails: {
            get: function (e) {
                 
                var attr = $(e.currentTarget).data();
                app.layerActions.ConnectedCustomerDetails.render({ equipment_id: attr.network_id, objFilterAttributes: { entity_type: attr.entityType, entityid: attr.systemId }, isControllEnable: false });
             },
            render: function (_data) {
                 
               // var _data = $.extend(_data);
                var formURL = 'splicing/GetConnectedCustomerDetailsInInfo';
                var titleText = 'Connected Customer Details';
                popup.LoadModalDialog('PARENT', formURL, _data, titleText, 'modal-lg');
            }
        },
        UpdateGeographicDetails: {
            get: function (e) {
                var attr = $(e.currentTarget).data();
                app.layerActions.UpdateGeographicDetails.render({ pEntityType: attr.entityType, pSystemId: attr.systemId, pGeomType: attr.geomType });
            },
            render: function (_data) {
                confirm(MultilingualKey.SI_OSP_GBL_JQ_FRM_191, function () {
                    ajaxReq('ISP/Auto_Codification', _data, false, function (resp) {
                        alert(resp.message);
                         
                        if (resp.listLog != '') {
                            var link = document.createElement('a');
                            document.body.appendChild(link);
                            link.href = "Library/ExportCodificationLogs";
                            link.target = "_blank";
                            link.trigger("click");
                        }
                    });
                });
            }
        },

       
        entity: {
            remove: function (e) {
                
                var attr = $(this).data();
                if (attr.entityType.toLowerCase() == app.EnumEntityType.Rack.toLowerCase() || attr.entityType.toLowerCase() == app.EnumEntityType.Equipment.toLowerCase()) {

                }
                else {
                    app.layerActions.entity.deleteEntity(attr.systemId, attr.entityType, attr.entityTitle, attr.geomType);
                }
            },
            deleteEntity: function (systmId, eType, eTitle, gType) {
                var postData = { systemId: systmId, entityType: eType, geomType: gType };

                ajaxReq('Main/getDependentChildElements', postData, true, function (resp) {
                    if (resp.status == "OK") {

                        if (resp.results.ChildElements.length > 0) {
                            var Msg = '<div class="table-responsive tble_cls"><table border="1" width="100%" class="webgrid" id="tblAlertDependendEntity"><thead><tr><td>Entity</td><td>Display Name</td></tr></thead><tbody>';
                            $.each(resp.results.ChildElements, function (index, item) {
                                Msg += '<tr><td>' + item.Entity_Type + ' </td><td> ' + item.display_name + '</td></tr>';
                            });
                            Msg += '</tbody></table></div>';

                            alert($.validator.format(getMultilingualStringValue(MultilingualKey.SI_OSP_SBA_JQ_FRM_003), Msg), 'warning');
                            //Following are the dependent elements. You need to remove them first
                        }

                        else {
                            //Please click on any network entity for information.
                            var html = '<div id="dvEntityInformationDetail" class="infodiv"><div class="NoRecordInfo">  ' + MultilingualKey.SI_ISP_GBL_JQ_GBL_022 + ' </div></div'
                            var func = function () {
                                ajaxReq('Main/DeleteEntityFromInfo', postData, true, function (jResp) {
                                    if (jResp.status == "OK") {
                                        app.refreshStructureInfo();
                                        //let $remove = app.partialLoad.selectContainer({ type: eType, systemId: systmId });
                                        //if ($remove) {
                                        //    $remove.remove();
                                        //}
                                        alert(jResp.message, 'success', 'success');
                                    } else {
                                        alert(jResp.message, 'warning');
                                    }
                                }, true, true);
                                $(app.DE.EntityInformationDetail).html(html);
                                if ($('.clsInfoTool').css('display') == 'block') {
                                    $('.clsInfoTool').animate({ width: 'toggle' });
                                    $(app.DE.leftMenuActions).removeClass('activeISPTab');
                                }

                            };
                            //Are you sure you want to delete this
                            var layerTitle = getLayerTltle(eType);
                            confirm($.validator.format(MultilingualKey.SI_OSP_DSA_JQ_FRM_003, layerTitle), func);
                        }
                    }
                    else {
                        alert(resp.message, 'warning');

                    }
                }, true, true);
            },
            edit: function (e) {
                var attr = $(e.currentTarget).data();
                app.DisableInfoTool();
                //edit cable..

                if (attr.entityType == app.EnumEntityType.Cable) {
                    //show save cancel actions..
                    $(app.DE.EntityCancel).parent().fadeIn();
                    $(app.DE.EntitySave).parent().fadeIn();

                    // get existing cable path..
                    var objCable = $('#ispCable_' + attr.systemId);
                    var _path = objCable.attr('d');
                    var _cableData = objCable.data();
                    objCable.hide();
                    $(app.DE.ElementDots).css('visibility', 'visible');
                    // Enable edit mode
                    let $tempPathData = $('#tempPath').data();
                    $tempPathData.isEditMode = true;
                    // copy termination point detail to temp path..
                    $tempPathData.aSystemId = objCable.attr('data-a-system-id');
                    $tempPathData.aEntityType = objCable.attr('data-a-entity-type');
                    $tempPathData.aLocation = objCable.attr('data-a-location');
                    $tempPathData.aNodeType = objCable.attr('data-a-node-type');
                    $tempPathData.bSystemId = objCable.attr('data-b-system-id');
                    $tempPathData.bEntityType = objCable.attr('data-b-entity-type');
                    $tempPathData.bLocation = objCable.attr('data-b-location');
                    $tempPathData.bNodeType = objCable.attr('data-b-node-type');
                    $tempPathData.cableType = objCable.attr('data-cable-type');
                    $tempPathData.systemId = objCable.attr('data-system-id');

                    // get point array from svg path
                    objCustomD3.tempPathData = app.getPointArrayFromSvgPath(_path, _cableData.cableType);
                    objCustomD3.drawTempPathForEdit();
                    app.cableActions.focusEndPoints($('#div_' + $tempPathData.aEntityType + '_' + $tempPathData.aSystemId), $('#div_' + $tempPathData.bEntityType + '_' + $tempPathData.bSystemId));
                }

                if (attr.entityType.toLowerCase() == app.EnumEntityType.Rack.toLowerCase() || attr.entityType.toLowerCase() == app.EnumEntityType.Equipment.toLowerCase()) {
                    $(app.DE.EntityCancel).parent().fadeIn();
                    $(app.DE.EntitySave).parent().fadeIn();
                }
                //var _data = { objIn: { cableType: 'ISP', systemId: attr.systemId } };
                //ajaxReq('ISP/GetCableInfo', { systemId: attr.systemId }, false, function (response) {
                //    if (!response) {
                //        alert('Invalid cable systemId');
                //    }
                //    else {
                //        $('.infoIcon >.icon-Save,.infoIcon >.icon-Cancel').parent().fadeIn();
                //        var systemId = attr.systemId;
                //        var cableId = 'ispCable_' + systemId;
                //        app.lstTerminationPoint[0] = response.a_location;
                //        app.lstTerminationPoint[1] = response.b_location;

                //        var path = $("#" + cableId).attr('d');
                //$("#tempPath").attr({ d: path });
                //$('#tempPath').attr('d', path);
                //    }
                //}, true, false);
            },
            save: function (e) {

                var attr = $(this).data();
                if (attr.entityType == app.EnumEntityType.Cable) {
                    app.lstISPCablePt = [];
                    var objExistingCable = $('#ispCable_' + attr.systemId);
                    var updateCablePath = $(app.DE.tempPath).attr("d");
                    var existingCablePath = objExistingCable.attr("d");
                    var updatedCableData = $(app.DE.tempPath).data();

                    if (updateCablePath == existingCablePath) {
                        alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_023, "warning");//No change found in cable path!    
                        return false;
                    }
                    else if (updateCablePath == "") {
                        alert(MultilingualKey.SI_ISP_GBL_JQ_FRM_018, "warning");//Path can not be blank!
                        return false;
                    }

                    else if (updatedCableData.aSystemId == "" || updatedCableData.aEntityType == "") {
                        alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_024, "warning");//Start termination point can not be blank!
                        return false;
                    }
                    else if (updatedCableData.bSystemId == "" || updatedCableData.bEntityType == "") {
                        alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_025, "warning");//End termination point can not be blank!
                        return false;
                    }
                    else {

                        var strucIdVal = $(app.DE.StructureId).val();
                        var lyrDetail = getLayerDetail("Cable");
                        var ntkIdType = lyrDetail['network_id_type'];

                        app.lstISPCablePt.push({ network_name: updatedCableData.aEntityType, network_id: updatedCableData.aLocation, system_id: updatedCableData.aSystemId, node_type: updatedCableData.aNodeType });
                        app.lstISPCablePt.push({ network_name: updatedCableData.bEntityType, network_id: updatedCableData.bLocation, system_id: updatedCableData.bSystemId, node_type: updatedCableData.bNodeType });

                        var _data = {
                            geom: '0', enType: 'Cable', cableType: 'ISP', lstTP: app.lstISPCablePt, networkIdType: ntkIdType, systemId: attr.systemId,
                            ModelInfo: { structureid: strucIdVal }
                        };
                        app.addCable(_data);

                    }
                }
                app.layerActions.entity.resetFocus($('.entityInfo'));
            },
            cancel: function (e) {
                var attr = $(e.currentTarget).data();
                if (attr.entityType == app.EnumEntityType.Cable) {
                    $('#ispCable_' + attr.systemId).show();
                    $(app.DE.ElementDots).css('visibility', 'hidden');

                    // Enable edit mode
                    let $tempPathData = $('#tempPath').data();
                    $tempPathData.isEditMode = false;
                    // copy termination point detail to temp path..
                    $tempPathData.aSystemId = "";
                    $tempPathData.aEntityType = "";
                    $tempPathData.aLocation = "";
                    $tempPathData.bSystemId = "";
                    $tempPathData.bEntityType = "";
                    $tempPathData.bLocation = "";
                    objCustomD3.removeTempPath();
                }
                $(app.DE.EntityCancel).parent().fadeOut();
                $(app.DE.EntitySave).parent().fadeOut();

                app.enableInfoTool();
                app.layerActions.entity.resetFocus($('.entityInfo'));
            },
            shift: function (e) {
                var attr = $(e.currentTarget).data();
                popup.LoadModalDialog(app.ParentModel, "ISP/ShiftEntityPosition", {
                    structureId: $(app.DE.StructureId).val(),
                    shaftId: parseInt($('#div_' + attr.entityType.toLocaleUpperCase() + '_' + attr.systemId).data().shaftId),
                    floorId: parseInt($('#div_' + attr.entityType.toLocaleUpperCase() + '_' + attr.systemId).data().floorId),
                    entityId: attr.systemId, entityType: attr.entityType,
                    parent_system_id: parseInt($('#div_' + attr.entityType.toLocaleUpperCase() + '_' + attr.systemId).data().parentSystemId),
                    parent_entity_type: $('#div_' + attr.entityType.toLocaleUpperCase() + '_' + attr.systemId).data().parentEntityType,
                    parent_network_id: $('#div_' + attr.entityType.toLocaleUpperCase() + '_' + attr.systemId).data().parentNetworkId
                }, ' Shift Entity', 'modal-sm', function () {
                    $('.chosen-select').chosen({ search_contains: true, width: '100%' });
                    if ($('#div_' + attr.entityType.toLocaleUpperCase() + '_' + attr.systemId).data().parentEntityType != app.EnumEntityType.Structure) {
                        $('#ddlUnitList').val(parseInt($('#div_' + attr.entityType.toLocaleUpperCase() + '_' + attr.systemId).data().parentSystemId)).trigger('chosen:updated');
                    }
                });
            },
            select: function (element) {
                let $res = null;
                element.type = element.type.toUpperCase();
                switch (element.type) {
                    case 'CABLE':
                        $res = $('#ispCable_' + element.systemId);
                        break;
                    default:
                        $res = $('#div_' + element.type.toUpperCase() + '_' + element.systemId);
                        break;

                }
                return $res;
            },
            setNetworkStatus: function ($e, element) {
                element.type = element.type.toUpperCase();
                switch (element.type) {
                    case 'CABLE':
                        let tempClass = $e.attr("class");
                        tempClass = tempClass.replace(element.oldStatus, '').trim();
                        $e.attr("class", tempClass + " " + element.newStatus);
                        //$e.removeClass(element.oldStatus);
                        //$e.addClass(element.newStatus);
                        break;
                    default:
                        $e.removeClass('network-status-' + element.oldStatus);
                        $e.addClass('network-status-' + element.newStatus);
                        break;

                }
            },
            focus: function ($e) {
                app.layerActions.entity.resetFocus($('.entityInfo'));
                $e.attr("class", $e.attr("class").trim() + " " + app.DE.EntitySelected);
            },
            resetFocus: function ($e) {
                $e.each(function (i, e) {
                    $(e).attr("class", $(e).attr("class").replace(app.DE.EntitySelected, ''));
                    $(e).attr("class", $(e).attr("class").trim().replace(app.DE.PathSelected, ''));
                });
            },
            splitCable: function (e) {
                let $marker = $(app.DE.circleMarker);
                let bufferpx = $marker.data().splitBuffer;
                let $svgCable = $(app.DE.SVGCable);
                var attr = $(this).data();

                let entityId = attr.systemId;
                let $entity = $('#div_' + attr.entityType + '_' + entityId);
                let mousePointer = {
                    x: objCustomD3.getOffsetToParent($entity, $svgCable, "left") + 8,
                    y: objCustomD3.getOffsetToParent($entity, $svgCable, "top") + 4
                };

                let cables = app.cableActions.getIntersectedCableAtPoint($('.entityInfo[data-entity-geomtype="LINE"][data-cable-type="ISP"]'), mousePointer, bufferpx);
                let nearByCables = [];
                cables.forEach(function (e) {
                    let attr = $(e.element).data();
                    if (attr.aSystemId != entityId && attr.bSystemId != entityId)
                        nearByCables.push({
                            system_id: attr.systemId,
                            network_id: attr.networkId,
                            total_core: attr.totalCore,
                            network_status: attr.networkStatus,
                            //touchingPart: e.touchingPart
                        });
                });
                //$marker.attr('r', bufferpx);
                //$marker.css('visibility', 'visible');
                //$marker.attr('transform', "translate(" + mousePointer.x + "," + mousePointer.y + ")");
                app.cableActions.split({ systemId: attr.systemId, entityType: attr.entityType, cables: nearByCables });
            },
            roomView: function (e) {
                //open room view
            },

            accessoriesView: function (e) {
                var attr = $(this).data();
                let $entity = $('#div_' + attr.entityType + '_' + attr.systemId);
                let networkId = $entity.data('networkId');
                var formURL = "Library/GetAccessories";
                var layerTitle = getLayerTltle(attr.entityType);
                var titleText = layerTitle.toUpperCase() + " Accessories";
                popup.LoadModalDialog('PARENT', formURL, { parent_systemId: attr.systemId, parent_eType: attr.entityType, parent_network_id: networkId }, titleText, 'modal-lg');
            }

        },
        customerSLD: {

            getSLD: function (e) {
                var attr = $(e.currentTarget).data();
                app.layerActions.customerSLD.render(attr.systemId, attr.entityType);
            },
            render: function (systemID, entityType,) {

                ajaxReq('main/EncryptMultiple', { entityid: systemID, entity_type: entityType, port_no: "Customer" }, false, function (resp) {
                    window.open(appRoot + 'Library/GetSLDDiagram?key=' + resp.entityid + ','
                        + resp.entity_type + ','
                        + resp.port_no, '_blank');
                }, false, false);

            }
        },
        
    };

    this.structureActions = {
        bind: function () {
            $(document).off('click', '#tblShaftLstInfo .icon-close').on('click', '#tblShaftLstInfo .icon-close', function (e) {
                app.structureActions.shaft.removeRange(e);
                app.structureActions.floor.refresh();
            });

        },
        shaft: {
            addRange: function (e) {

                var addRowInShaftRangeTbl = $(app.DE.ShaftInfoTable + ' tbody');
                var rowCount = $(app.DE.ShaftInfoTable + ' tbody tr:last').data().rowNum + 1;
                if (rowCount > 0) {
                    $(app.DE.ShaftInfoTable).show();
                }

                var shaftRowData = '<tr data-row-num="' + rowCount + '"><td><select class="chosen-select form-control" id="ShaftRangelist_' + rowCount + '__shaft_start_range" Name="ShaftRangelist[' + rowCount + '].shaft_start_range"  style="padding:0;" onchange="isp.structureActions.shaft.validateRangeSelector(this)"></td>';
                shaftRowData += '<td><select class="chosen-select form-control" id="ShaftRangelist_' + rowCount + '__shaft_end_range" Name="ShaftRangelist[' + rowCount + '].shaft_end_range"  style="padding:0;" onchange="isp.structureActions.shaft.validateRangeSelector(this)"></td><td><i class="icon-close"></i></td><td><input  id="ShaftRangelist_' + rowCount + '__system_id" name="ShaftRangelist[' + rowCount + '].system_id" type="hidden" value="0"><input type="hidden" id="ShaftRangelist_' + rowCount + '__shaft_id" name="ShaftRangelist[' + rowCount + '].shaft_id" value="' + $('#hdnShaftId').val() + '" /><input type="hidden" id="ShaftRangelist_' + rowCount + '__structure_id" name="ShaftRangelist[' + rowCount + '].structure_id" value="' + $(app.DE.StructureId).val() + '" /></td></tr>';
                addRowInShaftRangeTbl.append(shaftRowData);
                let $startRange = $('#ShaftRangelist_' + rowCount + '__shaft_start_range');
                let $endRange = $('#ShaftRangelist_' + rowCount + '__shaft_end_range');
                let options = $(app.DE.ShaftInfoTable + ' tbody tr:first td:first select:first').html();
                $startRange.html(options).val('0').trigger('chosen:updated');
                $endRange.html(options).val('0').trigger('chosen:updated');
                //$('.chosen-select').chosen({ width: '100%' });

                app.structureActions.floor.refresh();

            },
            removeRange: function (e) {
                var rowCount = $(app.DE.ShaftInfoTable + ' tbody tr').length;
                if (rowCount == 1) { alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_038); return false; }//Minimum one range is required!
                var rowIndex = parseInt($(e.currentTarget).parent('td').parent('tr').attr('data-row-num'));
                //if (rowIndex == 0) { alert('First row is required!'); return false; }

                $(app.DE.ShaftInfoTable + ' tr[data-row-num="' + rowIndex + '"]').remove();
                app.structureActions.shaft.resetRangeIndex();
            },
            resetRangeIndex: function () {
                $(app.DE.ShaftInfoTable + ' tr').each(function (index, row) {
                    let $row = $(row);
                    $row.attr('data-row-num', index);
                    $row.data('rowNum', index);
                    $($row.children()[0]).children().attr('id', 'ShaftRangelist_' + index + '__shaft_start_range').attr('name', 'ShaftRangelist[' + index + '].shaft_start_range');
                    $($row.children()[1]).children().attr('id', 'ShaftRangelist_' + index + '__shaft_end_range').attr('name', 'ShaftRangelist[' + index + '].shaft_end_range');
                    let hiddenData = '<input  id="ShaftRangelist_' + index + '__system_id" name="ShaftRangelist[' + index + '].system_id" type="hidden" value="0"><input type="hidden" id="ShaftRangelist_' + index + '__shaft_id" name="ShaftRangelist[' + index + '].shaft_id" value="' + $('#hdnShaftId').val() + '" /><input type="hidden" id="ShaftRangelist_' + index + '__structure_id" name="ShaftRangelist[' + index + '].structure_id" value="' + $(app.DE.StructureId).val() + '" />';
                    let lastCell = $row.find('td:last');
                    lastCell.html(hiddenData);
                });
            },
            saveRange: function (reponse) {
                //if (response.objPM != undefined && response.objPM.status == "OK") {
                //    $(popup.DE.closeModalPopup).trigger("click");
                //    alert(response.objPM.message, response.objPM.status == "OK" ? 'success' : 'warning', response.objPM.status == "OK" ? 'success' : 'information');
                //}
                app.refreshStructureInfo();
                $('#txtShaft_' + reponse.system_id).val((reponse.shaft_name.length > 17 ? reponse.shaft_name.substring(0, 17) + '...' : reponse.shaft_name)).removeClass('labledTextbox').removeClass('tinyloading').addClass('labledTextbox');
                $('#txtShaft_' + reponse.system_id).data().currentName = reponse.shaft_name;
                $('#txtShaft_' + reponse.system_id).parent('.ShaftNameContainer').attr('data-original-title', reponse.shaft_name);
                $('#ddlShaft option[value="' + reponse.system_id + '"]').text(reponse.shaft_name).trigger("chosen:updated");
                $(popup.DE.closeModalPopup).trigger("click");
                alert(reponse.objPM.message, reponse.objPM.status == "OK" ? 'success' : 'warning', reponse.objPM.status == "OK" ? 'success' : 'information');
            },
            refreshRange: function (rowCount) {
                let $startRange = $('#ShaftRangelist_' + rowCount + '__shaft_start_range');
                let $endRange = $('#ShaftRangelist_' + rowCount + '__shaft_end_range');
                if ($startRange && $endRange && $endRange.val() > 0 && $startRange.val() > 0) {
                    app.structureActions.floor.refresh();
                }
            },
            validateRangeSelector: function (e) { // 
                let $row = $(e).closest('tr');
                let index = $row.data().rowNum;
                if (app.structureActions.shaft.commonValidator(index, app.structureActions.shaft.rangeValidator))
                    app.structureActions.floor.refresh();
            },
            commonValidator: function (index, validator) { //    

                var isValidRange = false;

                var startRangeVal = parseInt($('#ShaftRangelist_' + index + '__shaft_start_range').val())
                var endRangeVal = parseInt($('#ShaftRangelist_' + index + '__shaft_end_range').val());
                if (isNaN(startRangeVal)) { startRangeVal = 0; }
                if (isNaN(endRangeVal)) { endRangeVal = 0; }
                let min = startRangeVal > endRangeVal ? endRangeVal : startRangeVal;
                let max = startRangeVal > endRangeVal ? startRangeVal : endRangeVal;
                if (endRangeVal > 0) {//|| (isValidRange = startRangeVal <= endRangeVal)
                    for (i = min; i <= max; i++) {
                        if (typeof validator === 'function') {
                            if (!(isValidRange = validator(index, i)))
                                return false;
                        }
                    }
                }
                return true;
            },
            rangeValidator: function (index, val) {
                var shaftId = $('#hdnShaftId').val();
                if ($('#ShaftRangelist_' + index + '__shaft_start_range option[value="' + val + '"]').attr('disabled') == 'disabled') {
                    $('#ShaftRangelist_' + index + '__shaft_start_range').val('0').trigger('chosen:updated');
                    $('#ShaftRangelist_' + index + '__shaft_end_range').val('0').trigger('chosen:updated');

                    alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_039, 'warning');//Overlapped range found.
                    return false;
                }
                return true;
            },
            IsShaftEmpty: function (index, val) {
                var shaftId = $('#hdnShaftId').val();
                if ($('#div_shaft_' + shaftId + '_Floor_' + val).children().length) {
                    alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_040, 'warning');//Shaft is not empty!
                    return false;
                }
                return true;
            },
            submit: function () {
                var isValidRange = true;
                if ($("#shaftName").val() != "") {
                    $("#shaftName").removeClass("input-validation-error");
                    if (isValidRange && $('.ShaftNameContainer .ShaftName').filter(function () {
                        return (app.trim($(this).data().currentName.toUpperCase()) == app.trim($('#shaftName').val().toUpperCase()) && parseInt($('#hdnShaftId').val()) != parseInt($(this).data().systemId))
                    }).length > 0) {
                        isValidRange = false;
                        alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_075, 'warning');//Shaft name already exist!
                        return false;
                    }
                    var range = [];
                    let countDelete = 0;
                    if ($('#ddlFloorRangeType').val() == 'Custom') {
                        let $allRange = $('#tblShaftLstInfo tbody tr');
                        let rangeLen = $allRange.length;

                        $.each($allRange, function (index, row) {
                            //var $row = $('#tblShaftLstInfo tr[data-row-num="' + index + '"]');
                            var startRange = parseInt($($(row).children()[0]).children().val());
                            var endRange = parseInt($($(row).children()[1]).children().val());

                            if (startRange == 0 || endRange == 0) {
                                if (countDelete == (rangeLen - 1)) {
                                    isValidRange = false;
                                    alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_026, 'warning');//Floor range is required!
                                }
                                else {
                                    $(row).remove();
                                    countDelete++;
                                }
                            }
                            if (isValidRange && startRange && endRange && startRange > endRange) {
                                isValidRange = false;
                                alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_027, 'warning');//Floor range must be in increasing order!

                            }
                            range.push({ start: startRange, end: endRange });
                        });
                        if (isValidRange && !app.structureActions.shaft.validateShaftFloors($('#hdnShaftId').val(), app.structureActions.floor.getAll(), range)) {
                            isValidRange = false;
                            alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_028, 'warning');//Non empty floor(s) must be included in your selected range(s)!
                            return false;
                        }

                    }

                    if (!isValidRange) return false;
                    if (isValidRange) {
                        if (countDelete) { app.structureActions.shaft.resetRangeIndex(); }
                        $('#frmShaftInfo').submit();
                    }
                }
                else {
                    $("#shaftName").addClass("input-validation-error");
                    return false;
                }
            },
            validateShaftFloors: function (shaftId, floorList, range) {

                var nonEmptyFloor = [];

                for (let i = 0; i < floorList.length; i++) {
                    if ($('#div_shaft_' + shaftId + '_Floor_' + floorList[i]).children().length) {

                        nonEmptyFloor.push(floorList[i]);
                    }
                }

                let check = true;
                for (let i = 0; i < nonEmptyFloor.length; i++) {
                    check = app.structureActions.shaft.IsShaftFloorInRange(nonEmptyFloor[i], range);
                    if (!check)
                        return false;
                }

                return check;
            },
            IsShaftFloorInRange: function (floor, range) {
                check = false;
                for (let j = 0; j < range.length; j++) {
                    if (floor >= range[j].start && floor <= range[j].end) {
                        check = true;
                        break;
                    }
                }
                return check;
            },
            onShaftChange: function (id, dependentId) {
                let shaftId = $('#' + id).val();
                $('#' + dependentId).val('').trigger("chosen:updated");
                app.structureActions.floor.renderFloorSelect(shaftId, '#' + dependentId);
            }
        },
        floor: {
            refresh: function () {
                $('#tblShaftLstInfo tr option').attr('disabled', false).trigger("chosen:updated");
                $.each($('#tblShaftLstInfo tbody tr'), function (index, row) {
                    //var row = $('#tblShaftLstInfo tr[data-row-num="' + index + '"]');
                    //var rowIndex = $(row).data().rowNum;
                    var startRange = parseInt($($(row).children()[0]).children().val());
                    var endRange = parseInt($($(row).children()[1]).children().val());

                    $.each($('#tblShaftLstInfo tbody tr:not(:eq(' + index + ')) '), function (index, e) {

                        if (startRange > 0 && endRange > 0) {
                            let min = startRange > endRange ? endRange : startRange;
                            let max = startRange > endRange ? startRange : endRange;
                            for (var j = min; j <= max; j++) {
                                if (j)
                                    $(this).find('.chosen-select option[value="' + j + '"]').attr('disabled', true).addClass("disabled-floor").trigger("chosen:updated");
                            }
                        }
                    })
                })
            },
            validate: function () {
                 
                var totalUnits = parseInt($('#hdnTotalUnits').val());
                var distributedUnitToFloor = 0;
                $('.str-floor').filter(function () { if ($(this).data().totalUnit != undefined) { distributedUnitToFloor = distributedUnitToFloor + parseInt($(this).data().totalUnit); } })
                if ((parseInt($('#no_of_units').val()) > totalUnits) || ((parseInt($('#no_of_units').val()) - ($('#divFloor_' + $('#hdnFloorId').val()).data().totalUnit)) > (totalUnits - distributedUnitToFloor))) {
                    //Number of units can not be greater than//total units//of structure!
                    //alert(MultilingualKey.


                    alert(getMultilingualStringValue($.validator.format(MultilingualKey.SI_ISP_GBL_JQ_GBL_029, totalUnits)));
                    return false;
                } else if (parseInt($('#no_of_units').val()) < $('#divFloor_' + $('#hdnFloorId').val() + ' div[data-entity-type="UNIT"]').length) {
                    //Number of units can not be less than//placed units// on floor!

                    alert(getMultilingualStringValue($.validator.format(MultilingualKey.SI_ISP_GBL_JQ_GBL_032, $('#divFloor_' + $('#hdnFloorId').val() + ' div[data-entity-type="UNIT"]').length)));
                    return false;
                }
                else if ($('.FloorInfo .FloorName').filter(function () { return ($(this).data().currentName.toUpperCase() == $('#floor_name').val().toUpperCase() && parseInt($('#hdnFloorId').val()) != parseInt($(this).data().systemId)) }).length > 0) {
                    alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_078, 'warning');//Floor name already exist!
                    return false;
                }
                var totalLength = 0;
                var maxHeight = 0;
                var maxWidth = 0;
                $('#divFloor_' + $('#hdnFloorId').val() + ' div[data-entity-type="UNIT"]').filter(function () {
                    var additionAttr = $(this).attr('data-additionalattr').split('|');
                    if (additionAttr.length >= 3) {
                        maxWidth = maxWidth < parseInt(additionAttr[0].split('=')[1]) ? parseInt(additionAttr[0].split('=')[1]) : maxWidth;
                        maxHeight = maxHeight < parseInt(additionAttr[1].split('=')[1]) ? parseInt(additionAttr[1].split('=')[1]) : maxHeight;
                        totalLength = totalLength + parseInt(additionAttr[2].split('=')[1]);
                    }
                })
                if (parseInt($('#txtFloorLength').val()) < totalLength) {

                    alert($.validator.format(MultilingualKey.SI_ISP_GBL_JQ_GBL_035, totalLength), 'warning');
                    //alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_035 + ' (' + totalLength + ')!', 'warning');//Floor length can not be less then the sum of unit length
                    return false;
                } else if (parseInt($('#txtFloorWidth').val()) < maxWidth) {

                    alert($.validator.format(MultilingualKey.SI_ISP_GBL_JQ_GBL_036, maxWidth), 'warning');
                    // alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_036 + '  (' + maxWidth + ')!', 'warning');//Floor width can not be less then unit width
                    return false;
                } else if (parseInt($('#txtFloorHeight').val()) < maxHeight) {

                    alert($.validator.format(MultilingualKey.SI_ISP_GBL_JQ_GBL_037, maxWidth), 'warning');
                    // alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_037 + ' (' + maxHeight + ')!', 'warning');//Floor height can not be less then unit height
                    return false;
                }
            },
            getAll: function () {

                var options = $('select#ddlFloor option');

                let floors = $.map(options, function (option) {
                    if (option.value)
                        return parseInt(option.value, 10);
                });
                return floors;
            },
            isVisibleFloor: function (shaftId, floorId) {
                let $e = $('#div_shaft_' + shaftId + '_Floor_' + floorId);
                return $e.closest('.str-shaft').hasClass(app.DE.DisabledFloor);
            },
            renderFloorSelect: function (shaftId, id) {
                $(id + ' option').attr('disabled', false).each(function (i, e) {
                    if ($(e).val() && app.structureActions.floor.isVisibleFloor(shaftId, $(e).val())) {
                        $(e).attr('disabled', true);
                    }
                });
                $(id).trigger("chosen:updated");
            },
            resetFloorSelect: function () {
                $('select#ddlFloor option').attr('disabled', false).trigger("chosen:updated");
                $('select#ddlFloor').trigger("chosen:updated");
            },
            success: function (reponse) {
                app.refreshStructureInfo();
                $('#txtFloor_' + reponse.system_id).val((reponse.floor_name.length > 17 ? reponse.floor_name.substring(0, 17) + '...' : reponse.floor_name)).removeClass('labledTextbox').removeClass('tinyloading').addClass('labledTextbox');
                $('#txtFloor_' + reponse.system_id).data().currentName = reponse.floor_name;
                $('#txtFloor_' + reponse.system_id).parent('.FloorInfo').attr('data-original-title', reponse.floor_name);
                $('#ddlFloor option[value="' + reponse.system_id + '"]').text(reponse.floor_name).trigger("chosen:updated");
                $(popup.DE.closeModalPopup).trigger("click");
                alert(reponse.objPM.message, reponse.objPM.status == "OK" ? 'success' : 'warning', reponse.objPM.status == "OK" ? 'success' : 'information');
            }
        },

    };

    this.OpenISPLegend = function () {
        $('.ISPlegend').slideToggle();
        var isContentExist = ($(".ISPlegend .LegendContent").html()).trim() == "" ? false : true;
        if (!isContentExist) {
            ajaxReq('ISP/getLegnedDetail', {}, true, function (resp) {
                $(".ISPlegend .LegendContent").html(resp);
                $(".ISPlegend").css('background-image', 'none');
            }, false, false);
        }

    }

    this.ClosedISPLegend = function () {
        $('.ISPlegend').hide();
        $('#chkISPLegend').attr('checked', false);
    }

    this.filterParentEntity = function (obj) {
        var prvVal = $(app.DE.ddlParentEntity).val();
        var selectedEntityType = $('#ddlParentEntityType :selected').attr('data-entity-type');
        var html = '<option value="0">-- Select --</option>'
        if ($('#ddlParentEntityType').val() == "0") {
            $.each(app.parentEntities, function (indx, itm) {
                html += '<option value="' + itm.system_id + '" data-entity-type="' + itm.entity_type + '">' + itm.network_id + '</option>';
            });
        } else {
            $.each($('.str-main-table div[data-entity-type="' + selectedEntityType + '"]'), function (indx, itm) {
                var data = $(itm).data();
                html += '<option value="' + data.systemId + '" data-entity-type="' + data.entityType + '" data-layer-title="' + data.layer_title + '">' + data.networkId + '</option>';
            });
        }
        $(app.DE.ddlParentEntity).html(html).val($(app.DE.ddlParentEntity + " option[value='" + prvVal + "']").length > 0 ? prvVal : 0).trigger('chosen:updated');
    }

    this.refreshParentEntityTypeDropDown = function (data) {
        var prevValue = $('#ddlParentEntityType').val();
        var lookup = {};
        var distinctEntity = [];
        for (var item, i = 0; item = data[i++];) {
            var entity_type = item.entity_type;
            if (!(entity_type in lookup)) {
                lookup[entity_type] = 1;
                distinctEntity.push({ eType: item.entity_type, lyrTitle: item.layer_title, systemId: item.system_id });
            }
        }
        var html = '<option value="0" >-- Select --</option>'
        $.each(distinctEntity, function (index, item) {
            html += '<option value="' + item.eType + '" data-entity-type="' + item.eType + '" data-layer-title="' + item.lyrTitle + '" >' + item.lyrTitle + '</option>';
        })
        $('#ddlParentEntityType').html(html).val(prevValue).trigger('chosen:updated');
    }

    this.refreshParentEntityDropDown = function () {
        var html = '<option value="0" >-- Select --</option>';
        $.each(app.parentEntities, function (indx, itm) {
            html += '<option value="' + itm.system_id + '" data-entity-type="' + itm.entity_type + '">' + itm.network_id + '</option>';
        });
        $(app.DE.ddlParentEntity).html(html).trigger('chosen:updated');
    }

    this.enableInfoTool = function () {

        attachUnAttachEvt($(app.DE.EntityInfo), 'click', function () { app.EntityInfoDisplay(this); });
        $(app.DE.EntityInfo).css({ 'cursor': 'url(' + baseUrl + appRoot + 'Content/images/hand.cur' + '), default' });
    }
    this.getAllParentInFloor = function () {
        ajaxReq('ISP/getAllParentInFloor', { structureId: parseInt($(app.DE.StructureId).val()), floorId: parseInt($('#ddlFloorList').val()), parentType: 'UNIT' }, false, function (resp) {
            var optHTML = '<option value="0">Select Unit</option>';
            if (resp.length > 0 && $("#ddlUnitList").length > 0) {
                for (var i = 0; i < resp.length; i++) {
                    optHTML += '<option value=' + resp[i].entity_system_id + ' data-entity-type="UNIT" data-network-id="' + resp[i].network_id + '">' + resp[i].network_id + '</option>';
                }
                $("#ddlUnitList").html(optHTML).trigger("chosen:updated");
            } else if (resp.length == 0 && $("#ddlUnitList").length > 0) {
                $("#ddlUnitList").html(optHTML).val('0').trigger("chosen:updated");
            }
        }, false, false);
    }
    this.resetSVG = function () {
        $(app.DE.SVGCable).css('left', $('.str-main-table').offset().left);
        $(app.DE.SVGPatchCable).css('left', $('.str-main-table').offset().left);
    }

    this.cableActions = {
        resize: function () {
            let updatedPaths = [];
            let $svgCable = $(app.DE.SVGCable);
            let $svgPatchCable = $(app.DE.SVGPatchCable);
            let ospChange = { xChange: 0, yChange: 0, isYChanged: false };
            $(app.DE.CablePaths).each(function (i, e) {
                let $cable = $(e);
                let cableData = $cable.data();
                //let cablePoints = app.cableActions.getPointArrayFromSvgPath($cable.attr('d'), cableData.cableType);
                let cablePoints = objCustomD3.getPointArrayFromSvgPath($cable.attr('d'), cableData.cableType);
                let cableLen = cablePoints.length;
                let startPosition, endPosition;
                let pathChanges = {
                    start: {
                        xChange: { cablePoints: [], isChanged: false },
                        yChange: { cablePoints: [], isChanged: false }
                    },
                    end: {
                        xChange: { cablePoints: [], isChanged: false },
                        yChange: { cablePoints: [], isChanged: false }
                    }
                };
                if (cableLen > 2 && cableData.aNodeType != '' && cableData.bNodeType != '') {
                    let startElementNode = $('#div_' + cableData.aEntityType + '_' + cableData.aSystemId + ' > span.' + cableData.aNodeType);
                    let endElementNode = $('#div_' + cableData.bEntityType + '_' + cableData.bSystemId + ' > span.' + cableData.bNodeType);
                    let limitPoints = undefined;
                    if (cableData.cableType.toUpperCase() == 'OSP') {
                        //limitPoints = 2;
                    }
                    if (endElementNode && endElementNode.length) {
                        endPosition = getElementPosition(endElementNode, $(app.DE.mainTable));
                        endPosition.x -= (2 * endElementNode.width() + endElementNode.parent().width()) + 8;
                        pathChanges.start.xChange = app.cableActions.snapPoints(cablePoints, endPosition, 'x', 0, limitPoints);
                        pathChanges.start.yChange = app.cableActions.snapPoints(cablePoints, endPosition, 'y', 0, limitPoints);
                        if (pathChanges.start.xChange.isChanged || pathChanges.start.yChange.isChanged) {
                            cablePoints = pathChanges.start.yChange.cablePoints;

                        }
                    }

                    if (startElementNode && startElementNode.length) {
                        startPosition = getElementPosition(startElementNode, $(app.DE.mainTable));
                        startPosition.x -= (2 * startElementNode.width() + startElementNode.parent().width()) + 8;
                        pathChanges.end.xChange = app.cableActions.snapPoints(cablePoints, startPosition, 'x', 1, limitPoints);
                        pathChanges.end.yChange = app.cableActions.snapPoints(cablePoints, startPosition, 'y', 1, limitPoints);
                        if (pathChanges.end.xChange.isChanged || pathChanges.end.yChange.isChanged) {
                            cablePoints = pathChanges.end.yChange.cablePoints;

                        }
                    }

                    if (cableData.cableType.toUpperCase() == 'OSP') {
                        let $ospCable = $("#divOSPCable");
                        if (!startPosition) {
                            startPosition = {
                                x: objCustomD3.getOffsetToParent($ospCable, $svgCable, "left"),
                                y: objCustomD3.getOffsetToParent($ospCable, $svgCable, "top")
                            };
                            if (!ospChange.isYChanged) {
                                ospChange.yChange = startPosition.y - cablePoints[0].y;
                            }
                            ospChange.isYChanged = true;
                            //startPosition.y += ospChange.yChange;
                            cablePoints[0].y += ospChange.yChange;
                            cablePoints[1].y += ospChange.yChange;
                            if (Math.abs(ospChange.yChange) > 10)
                                pathChanges.start.yChange.isChanged = true;
                        }
                        if (!endPosition) {
                            endPosition = {
                                x: objCustomD3.getOffsetToParent($ospCable, $svgCable, "left"),
                                y: objCustomD3.getOffsetToParent($ospCable, $svgCable, "top")
                            };
                            if (!ospChange.isYChanged) {

                                ospChange.yChange = endPosition.y - cablePoints[cablePoints.length - 1].y;
                            }
                            ospChange.isYChanged = true;
                            //endPosition.y += ospChange.yChange;
                            cablePoints[cablePoints.length - 1].y += ospChange.yChange;
                            cablePoints[cablePoints.length - 2].y += ospChange.yChange;
                            if (Math.abs(ospChange.yChange) > 10)
                                pathChanges.end.yChange.isChanged = true;
                        }

                    }
                    if (pathChanges.end.xChange.isChanged || pathChanges.end.yChange.isChanged || pathChanges.start.xChange.isChanged || pathChanges.start.yChange.isChanged) {
                        if (cableLen > 4 && startPosition && endPosition)
                            cablePoints = app.cableActions.pathClipping(cablePoints, startPosition, endPosition, pathChanges.end.yChange.isChanged && (cableData.cableType.toUpperCase() == 'OSP'));//
                        var cablePath = objCustomD3.lineFunction(cablePoints);
                        $cable.attr('d', cablePath);
                        updatedPaths.push({
                            entity_id: cableData.systemId, line_geom: $cable.attr('d'), structure_id: $(app.DE.StructureId).val(), geom_source: 'S'
                        });
                    }
                }

            });
            app.cableActions.update(updatedPaths);
            $svgCable.css('visibility', 'visible');
            $svgPatchCable.css('visibility', 'visible');
        },
        getPointArrayFromSvgPath: function (_path, _cableType) {
            var _finalPointArray = [];
            let offset = 1;
            var nodes = _path.split(/(?=[LMC])/);

            for (var i = 0; i < nodes.length; i++) {
                var point = nodes[i].slice(1, nodes[i].length).split(',');
                point[0] = parseFloat(point[0]);
                point[1] = parseFloat(point[1]);
                var _isEditAllowed = _cableType.toUpperCase() == "OSP" && i == nodes.length - 1 ? false : true;
                //if (_finalPointArray.findIndex(function (m) { return m.x == point[0] && m.y == point[1] }) < 0) {
                if (!_finalPointArray.filter(p => p.x == point[0] && p.y == point[1] || ((p.x <= offset + point[0] && p.x >= -offset + point[0]) && (p.y <= offset + point[1] && p.y >= -offset + point[1]))).length)
                    _finalPointArray.push({ x: point[0], y: point[1], isVirtual: false, isMidPoint: false, isEditAllowed: _isEditAllowed });
                //}
            }
            return _finalPointArray;
        },
        snapPoints: function (cablePoints, position, attr, isStartNode, limitPoint) {
            let cablePointsCopy = [];
            $.extend(true, cablePointsCopy, cablePoints);
            let offset = 2;
            let check = (attr == 'x') ? 'y' : 'x';
            let cableLen = cablePoints.length;
            let startPoint = isStartNode ? 0 : cableLen - 1;
            let sign = isStartNode ? 1 : -1;
            let diff = cablePoints[isStartNode ? 0 : cableLen - 1][attr] - position[attr];
            let i = 0;
            let limit = limitPoint || (cableLen / 2 + 1);
            let margin = 100;
            let isChanged = false;
            if (cableLen > 2 && Math.abs(diff) > 10) {

                i = 0;

                //Shifting attribute
                while (i < cableLen - 1) {

                    if (cablePointsCopy[startPoint + i * sign][check] != cablePointsCopy[startPoint + (i + 1) * sign][check]) {
                        let newVal = cablePointsCopy[startPoint + (i) * sign][attr] - diff;
                        let point1 = cablePointsCopy[startPoint + i * sign], point2 = cablePointsCopy[startPoint + (i + 1) * sign];

                        if (point1[attr] == point2[attr] || (point1[attr] >= point2[attr] - 2 && point1[attr] <= point2[attr] + 2)) {

                            if ((cablePointsCopy[startPoint + (i) * sign][attr] - diff) > (position[attr] + margin))
                                newVal = (position[attr] + margin);

                            cablePoints[startPoint + (i) * sign][attr] = newVal;
                            cablePoints[startPoint + (i + 1) * sign][attr] = newVal;
                        }

                    }
                    i++;
                    if ((cableLen - i) <= limit)
                        break;
                }
                cablePoints[startPoint][attr] = position[attr];
                if (cablePointsCopy[startPoint][check] == cablePointsCopy[startPoint + sign][check]) {
                    cablePoints[startPoint + sign][attr] = cablePointsCopy[startPoint + sign][attr] - diff;

                }
                isChanged = true;
            }
            return { cablePoints: cablePoints, isChanged: isChanged };
        },

        pathClipping: function (cablePoints, start, end, isPathClip) {
            isPathClip = isPathClip == undefined ? false : isPathClip;

            let cablePointsCopy = [];
            $.extend(true, cablePointsCopy, cablePoints);
            let i = 0;
            let cableLen = cablePoints.length;
            let offsetX = 100, offsetY = 200;
            let minX = start.x - offsetX, maxX = end.x + offsetX, minY = start.y - offsetY, maxY = end.y + offsetY;
            if (start.x > end.x) {
                minX = end.x - offsetX;
                maxX = start.x + offsetX;
            }
            if (start.y > end.y) {
                minY = end.y - offsetY;
                maxY = start.y + offsetY;
            }
            let clipPoint = [];
            while (i < cableLen - 1) {

                if (cablePointsCopy[i]['x'] > minX && cablePointsCopy[i]['x'] < maxX && cablePointsCopy[i]['y'] > minY && cablePointsCopy[i]['y'] < maxY) {

                }
                else {
                    cablePointsCopy[i]['x'] = cablePointsCopy[i]['x'] < minX ? minX : ((cablePointsCopy[i]['x'] > maxX) ? maxX : cablePointsCopy[i]['x']);
                    cablePointsCopy[i]['y'] = cablePointsCopy[i]['y'] < minY ? minY : ((cablePointsCopy[i]['y'] > maxY) ? maxY : cablePointsCopy[i]['y']);
                    if (isPathClip)
                        clipPoint.push(i);
                }
                i++;
            }
            if (isPathClip) {
                cablePoints = cablePoints.filter(function (value, index) {
                    return clipPoint.indexOf(index) == -1;
                });
                return cablePoints;
            }
            return cablePointsCopy;
        },
        update: function (ISPCableMaster) {

            if (ISPCableMaster.length > 0) {
                ajaxReq('ISP/UpdateIspCablesPath', {
                    cableDetails: ISPCableMaster
                }, false, function (response) {

                }, true, false);
            }
        },

        focusEndPoints: function ($start, $end) {

            $('.entityInfo').each(function (i, e) {
                $(e).attr("class", $(e).attr("class").replace(app.DE.EntitySelected, ''));
            });
            if ($start && $start.attr("class")) {
                $start.attr("class", $start.attr("class").trim() + " " + app.DE.EntitySelected);
            }
            if ($end && $end.attr("class")) {
                $end.attr("class", $end.attr("class").trim() + " " + app.DE.EntitySelected);
            }
        },
        focus: function ($e) {

            $('.entityInfo').each(function (i, e) {
                //$(e).attr("class", $(e).attr("class").trim().replace(app.DE.EntitySelected, ''));
                $(e).attr("class", $(e).attr("class").trim().replace(app.DE.PathSelected, ''));
            });

            if ($e && $e.attr("class")) {
                $e.attr("class", $e.attr("class").trim() + " " + app.DE.PathSelected);

            }
        },
        resetCablesFocus: function () {
            $('.entityInfo').each(function (i, e) {
                $(e).attr("class", $(e).attr("class").trim().replace(app.DE.PathSelected, ''));
            });
        },
        onDotMouseEnter: function (e) {
            let $dot = $(e.currentTarget);
            $dot.parent().tooltip("disable");

            e.stopPropagation();
        },
        onDotMouseLeave: function (e) {
            let $dot = $(e.currentTarget);
            $dot.parent().tooltip("enable");

            e.stopPropagation();
        },
        cableTemplateSuccess: function (e) {
            if ($('#liCable').hasClass('active')) {
                app.clearCableEvents();
                $(app.DE.ElementDots).css('visibility', 'visible');
            }
        },
        pointInLine: function (A, B, P, offset) {

            if (A.x == B.x) {
                let yMax = A.y, yMin = B.y;
                if (B.y > A.y) {
                    yMax = B.y + offset;
                    yMin = A.y - offset;
                }
                if (A.x + offset >= P.x && A.x - offset <= P.x)
                    return yMax >= P.y && yMin <= P.y;
            }
            if (A.y == A.y) {
                let xMax = A.x, xMin = B.x;
                if (B.x > A.x) {
                    xMax = B.x + offset;
                    xMin = A.x - offset;
                }
                if (A.y + offset >= P.y && A.y - offset <= P.y) return xMax >= P.x && xMin <= P.x;
            }
            return false;
        },
        getIntersectedCableAtPoint: function ($allCables, point, buffer) {
            let intersectedCables = [];
            $allCables.each(function (i, e) {
                let $e = $(e);
                let nodes = app.cableActions.getNodes($e.attr('d'));
                //let touchingLine = app.cableActions.isTouching(nodes, point, buffer)
                if (app.cableActions.isTouching(nodes, point, buffer)) {
                    intersectedCables.push({ element: $e });
                };
            });

            return intersectedCables;
        },
        getNodes: function (_path) {
            var nodes = _path.split(/(?=[LMC])/);
            if (!nodes || nodes == '' || nodes.length == 0) {
                return [];
            }
            let nodesLen = nodes.length;
            let tempPoints = [];
            let count = 1;
            let point = nodes[0].slice(1, nodes[0].length).split(',');
            tempPoints.push({ x: parseFloat(point[0]), y: parseFloat(point[1]) });
            point = nodes[nodesLen - 1].slice(1, nodes[nodesLen - 1].length).split(',');
            tempPoints.push({ x: parseFloat(point[0]), y: parseFloat(point[1]) });
            //Remove Duplicate
            for (let i = 0; i < nodesLen - 1; i++) {
                let point = nodes[i].slice(1, nodes[i].length).split(',');
                point[0] = parseFloat(point[0]);
                point[1] = parseFloat(point[1]);
                if (!tempPoints.filter(p => p.x == point[0] && p.y == point[1]).length) {
                    tempPoints.splice(count++, 0, { x: point[0], y: point[1] });

                }
            }
            return tempPoints;
        },
        isTouching: function (nodes, point, buffer) {
            let len = nodes.length;
            for (let i = 0; i < len - 1; i++) {
                if (app.cableActions.pointInLine(nodes[i], nodes[i + 1], point, buffer))
                    return true;
            }
            return false;
        },
        getTouchingPart: function (nodes, point, buffer) {
            let len = nodes.length;
            let start = {}, end = {}, status = false;
            let distance = Infinity;
            for (let i = 0; i < len - 1; i++) {
                if (app.cableActions.pointInLine(nodes[i], nodes[i + 1], point, buffer)) {
                    let d = Math.sqrt(((nodes[i].x - point.x) * (nodes[i].x - point.x)) + ((nodes[i].y - point.y) * (nodes[i].y - point.y)));
                    status = true;

                    if (d < distance) {
                        distance = d;
                        start = nodes[i];
                        end = nodes[i + 1];
                    }
                }

                //return { status: true, start: nodes[i], end: nodes[i + 1] };
            }
            return { status: status, start: start, end: end };
        },
        split: function (data) {

            var pageUrl = 'ISP/getSplitCable';
            var modalClass = 'modal-lg';
            popup.LoadModalDialog(app.ParentModel, pageUrl, { networkIdType: data.entityType, systemId: data.systemId, entityType: data.entityType, cables: data.cables }, 'Split Cable Details', modalClass);
        },
        setNewCableDetails: function () {
            var splitcablesystemid = $("input[name='Cable']:checked").attr('s_id');

            $('#split_cable_system_id').val(splitcablesystemid);
            var splitEnityNetworkId = $('#split_entity_networkId').val();
            var splitEntity = $('#split_entity_type').val();
            var splitEntitySystem_id = $('#split_entity_system_id').val();
            var $splitCable = $('#ispCable_' + splitcablesystemid);
            var networkStatus = $splitCable.attr('data-network-status');
            var CableValue = $("input[name='Cable']:checked").val();

            if (CableValue == undefined) {
                alert(MultilingualKey.SI_OSP_CAB_JQ_FRM_006);//Please select cable to Process!
            }
            else {
                if ($('#cable_one_start_reading').length > 0 && $('#cable_one_end_reading').length > 0) {
                    $('#cable_one_start_reading,#cable_one_end_reading').keyup(function () {
                        app.calculateCableLength('', '' + networkStatus + '', 'cable_one_start_reading', 'cable_one_end_reading', 'cable_one_calculated_length', 'cable_one_measured_length')
                    });
                    $('#cable_two_start_reading,#cable_two_end_reading').keyup(function () {
                        app.calculateCableLength('', '' + networkStatus + '', 'cable_two_start_reading', 'cable_two_end_reading', 'cable_two_calculated_length', 'cable_two_measured_length')
                    });
                    if (networkStatus == 'P') {
                        $('#cable_one_start_reading,#cable_one_end_reading,#cable_one_calculated_length,#cable_one_measured_length').val(0);
                        $('#cable_two_start_reading,#cable_two_end_reading,#cable_two_calculated_length,#cable_two_measured_length').val(0);
                    } else {
                        $('#cable_one_start_reading,#cable_one_end_reading').attr('required', true).val(null);
                        $('#cable_two_start_reading,#cable_two_end_reading').attr('required', true).val(null);
                    }
                } else if (networkStatus == 'P') {
                    $('#cable_one_calculated_length,#cable_one_measured_length').val(0);
                    $('#cable_two_calculated_length,#cable_two_measured_length').val(0);
                }
                var aLocation = $('#ispCable_' + splitcablesystemid).attr('data-a-location');
                var bLocation = $('#ispCable_' + splitcablesystemid).attr('data-b-location');
                var parentCable = app.trim(CableValue);
                var firstCableNetworkId = parentCable + '_01';
                $('#cable_one_network_id').val(firstCableNetworkId);
                $('#cable_one_name').val(firstCableNetworkId);
                $('#cable_one_a_location').val(aLocation);
                $('#cable_one_b_location').val(splitEnityNetworkId);



                var secondCableNetworkId = parentCable + '_02';
                $('#cable_two_network_id').val(secondCableNetworkId);
                $('#cable_two_name').val(secondCableNetworkId);
                $('#cable_two_a_location').val(splitEnityNetworkId);
                $('#cable_two_b_location').val(bLocation);

                if ($('#cable_one_network_id').val() == "" && $('#cable_one_network_id').val() == "") {
                    alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_041);//Please click on process cable !
                }
                else {
                    let $one = $('div[data-network-id="' + aLocation + '"]'),
                        $mid = $('div[data-network-id="' + splitEnityNetworkId + '"]'),
                        $last = $('div[data-network-id="' + bLocation + '"]');

                    let point = {
                        x: objCustomD3.getOffsetToParent($mid, $(app.DE.SVGCable), "left") + 8,
                        y: objCustomD3.getOffsetToParent($mid, $(app.DE.SVGCable), "top") + 4
                    };
                    let $marker = $(app.DE.circleMarker);
                    let bufferpx = $marker.data().splitBuffer;
                    let cablePoints = objCustomD3.getPointArrayFromSvgPath($splitCable.attr('d'), $splitCable.data('cableType'));
                    let touchingLine = app.cableActions.getTouchingPart(cablePoints, point, bufferpx);
                    let firstCablePoints = [], secondCablePoints = [];
                    let isFirst = true;
                    let touchPointOne = undefined;
                    let touchPointTwo = undefined;
                    if (touchingLine.start.x == touchingLine.end.x) {
                        touchPointOne = {};
                        touchPointOne.x = touchingLine.start.x;
                        touchPointOne.y = point.y;
                        touchPointTwo = {};
                        touchPointTwo.x = touchingLine.start.x;
                        touchPointTwo.y = point.y;
                        if (point.x - 5 <= touchingLine.start.x && point.x + 5 >= touchingLine.start.x) {
                            touchPointOne.y = point.y - 15;
                            touchPointTwo.y = point.y + 15;
                        }
                    }
                    if (touchingLine.start.y == touchingLine.end.y) {
                        touchPointOne = {};
                        touchPointOne.x = point.x;
                        touchPointOne.y = touchingLine.start.y;
                        touchPointTwo = {};
                        touchPointTwo.x = point.x;
                        touchPointTwo.y = touchingLine.start.y;
                        if (point.y - 5 <= touchingLine.start.y && point.y + 5 >= touchingLine.start.y) {
                            touchPointOne.x = point.x - 15;
                            touchPointTwo.x = point.x + 15;
                        }
                    }
                    let cableOneData = objCustomD3.getClosestJointPath($mid, touchPointOne || touchingLine.start);
                    let cableTwoData = objCustomD3.getClosestJointPath($mid, touchPointTwo || touchingLine.end);
                    //let cableOneData = objCustomD3.getClosestJointPath($mid,  touchingLine.start);
                    //let cableTwoData = objCustomD3.getClosestJointPath($mid,  touchingLine.end);
                    for (let i = 0; i < cablePoints.length; i++) {
                        if (isFirst) {

                            if (cablePoints[i].x >= touchingLine.start.x - 2 && cablePoints[i].x <= touchingLine.start.x + 2
                                && cablePoints[i].y >= touchingLine.start.y - 2 && cablePoints[i].y <= touchingLine.start.y + 2)
                                isFirst = false;
                            firstCablePoints.push(cablePoints[i]);
                        }
                        else {
                            secondCablePoints.push(cablePoints[i]);
                        }
                    }
                    if (cableTwoData.pathArray.length) {

                        secondCablePoints.splice(0, 0, cableTwoData.pathArray[1]);
                        secondCablePoints.splice(0, 0, cableTwoData.pathArray[0]);

                    }
                    if (cableOneData.pathArray.length) {
                        let pathLen = cableOneData.pathArray.length;

                        firstCablePoints.push(cableOneData.pathArray[1]);
                        firstCablePoints.push(cableOneData.pathArray[0]);
                    }
                    console.log(cablePoints, touchingLine, firstCablePoints, secondCablePoints);
                    $('#ispLineGeomCableOne').val(objCustomD3.lineFunction(firstCablePoints));
                    $('#ispLineGeomCableTwo').val(objCustomD3.lineFunction(secondCablePoints));

                    $('#cable_one_a_node_type').val($splitCable.data('aNodeType'));
                    $('#cable_one_b_node_type').val(cableOneData.position.dot);
                    $('#cable_two_a_node_type').val(cableTwoData.position.dot);
                    $('#cable_two_b_node_type').val($splitCable.data('bNodeType'));
                    var spliEntityId = 'div_' + splitEntity + '_' + splitEntitySystem_id;
                    $('#split_entity_x').val($('#' + spliEntityId).position().left);
                    $('#split_entity_y').val($('#' + spliEntityId).position().top);

                    if ((parseFloat($('#cable_one_end_reading').val()) != 0 || parseFloat($('#cable_one_start_reading').val()) != 0) && (parseFloat($('#cable_one_end_reading').val()) <= parseFloat($('#cable_one_start_reading').val()))) {
                        alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_017);//Cable 1 end reading can not be less then or equal to Cable 1 start reading!
                        return false;
                    }
                    if ((parseFloat($('#cable_two_end_reading').val()) != 0 || parseFloat($('#cable_two_start_reading').val()) != 0) && (parseFloat($('#cable_two_end_reading').val()) <= parseFloat($('#cable_two_start_reading').val()))) {
                        alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_018);//Cable 2 end reading can not be less then or equal to Cable 2 start reading!
                        return false;
                    }
                }

            }
        },
        getSplitCableFiberDetail: function (cableid) {
            var formURL = 'Library/getFiberDetail';
            cable_id = cableid;
            var modalClass = 'modal-lg';
            popup.LoadModalDialog(app.ChildModel, formURL, { cableId: cableid }, 'Fiber Details', modalClass);


        }

    }
    this.animation = {
        onAdd: function (left, top) {
            let body = '<div id="animateContent" style="position:absolute" class="ElementTemplate icon-UNIT  lib-UNIT"></div>';
            $('body').append(body);
            $('#animateContent').animate({ left: left + 'px', top: top + 'px' }, "slow");
            //$('#animateContent').remove();
        },
        show: function (req) {
            if (req) {
                let $e = app.layerActions.entity.select({ type: req.entityType, systemId: req.systemId });
                let offset = $('.ElementTemplate.lib-' + req.entityType).offset();
                if (offset) {
                    $e.css({ left: -offset.left + 'px', top: -offset.top + 'px' }, "slow");
                    $e.animate({ left: '0px', top: '0px' });
                }
            }
        }
    };
    this.partialLoad = {
        refresh: function () {
            app.refreshParentDropDown();
            app.RefreshISPNetworkLayerElements($(app.DE.StructureId).val());
            objCustomD3.InitD3();
            // bind info events..

            app.cableActions.resize();
        },
        selectContainer: function (element) {
            let $remove = app.layerActions.entity.select(element);
            let $container;
            switch (element.type) {
                case 'CABLE':
                    $container = $remove;
                    break;
                default:
                    $container = $remove.closest('div.ParentInnerBox');
                    if (!$container || !$container.length)
                        $container = $remove.closest('div.EntityBox');

                    break;

            }
            return $container;
        },
        renderNew: function (data) {
            let param = {
                structureId: data.structureId,
                systemId: data.systemId,
                entityType: data.entityType
            };
            ajaxReq('ISP/getCurrentEntity', param, true, function (response) {
                if (response) {
                    data.uiHtml = response;
                    app.partialLoad.addToPage(data);
                    app.partialLoad.refresh();
                }
            }, false, true);
        },
        addToPage: function (data) {
            let parent = {}
            parent.element = $("#divFloor_" + data.floorId);
            let $new = $(data.uiHtml);
            let parentType = data.pEntityType.toUpperCase();

            if (data.shaftId)
                parent.element = $("#div_shaft_" + data.shaftId + "_Floor_" + data.floorId);

            parent.context = parent.element;
            if (parentType != 'STRUCTURE') {
                parent = app.partialLoad.getContext(data);
            }

            if (parent.element) {
                if (parent.isSecondRow) {
                    parent.context.css('height', '35px');
                    $new.css('padding-top', '8px');
                }
                $new.appendTo(parent.context);

                let $newEntity = $('#div_' + data.entityType + '_' + data.systemId);
                $newEntity.attr('data-parent-entity-type', parentType).attr('data-parent-system-id', data.pSystemId);
                let parentData = parent.element.data();

                if (parent.isNested) {
                    let count = parent.context.find('.ParentInnerBox').length;
                    let $superParent = $('#div_' + parentData.parentEntityType + '_' + parentData.parentSystemId);
                    if (count == 1) {
                        let $left = $superParent.siblings('.ParentBox').children('.leftDiv:first');
                        if (!$left.length) {
                            $superParent.after($left = $('<div>').addClass('leftDiv'));
                        }
                        let $perentContext = parent.element.closest('.ParentInnerBox');
                        if ($perentContext)
                            $perentContext.detach().appendTo($('<div class="EntityBox" data-original-title></div>')).appendTo($left);
                        $perentContext.removeClass('ParentInnerBox').addClass('EntityBox');
                        parent.element.remove('ChildEntity').addClass('Element');
                    }
                    let $right = $superParent.siblings('.ParentBox').children('.rightDiv:first');
                    if ($right.length) {
                        if ($right.find('.ParentInnerBox').length == 0)
                            $right.empty();
                    }
                }
            }
        },
        getContext: function (data, selectedSide) {
            selectedSide = selectedSide || "rightDiv";

            let type = data.pEntityType.toUpperCase();
            let $entity = $('#div_' + type + '_' + data.pSystemId);
            let parentData = $entity.data();
            let isNested = parentData.parentSystemId != data.structureId;
            let $parentBox = $entity.siblings('.ParentBox').css('visibility', 'visible');
            if (!$parentBox.length) {
                $entity.after($parentBox = $('<div>').addClass('ParentBox').addClass('entityParentBox'));
                $parentBox.append($('<div>').addClass('rightDiv'));

            }
            let $selectedDiv = $parentBox;
            if (!isNested) {
                let $side = $parentBox.find('div.' + selectedSide + ':first');
                if (!$side.length) {
                    $parentBox.append($side = $('<div>').addClass('rightDiv'));
                }
                $selectedDiv = $side;
            }

            let childCount = $selectedDiv.find('.ParentInnerBox').length;
            let isSecondRow = !isNested && !data.shaftId && childCount % 2 == 1;
            let parentBoxRow = !isSecondRow ? 'First' : 'Second';
            let boxId = 'divParent' + parentBoxRow + 'Box_' + type + '_' + data.pSystemId;
            let innerUi = '<div class="ParentFirstBox" id="' + boxId + '" data-original-title="" title=""></div>';

            if (!$('#' + boxId).length) {
                if (!data.shaftId) {
                    $selectedDiv.append(innerUi);
                }
                else {
                    $parentBox.append($('<i>').addClass('icon-Collapse_expand')).append(innerUi);
                }
            }
            if (data.shaftId) {
                let count = $('#' + boxId).children('.ParentInnerBox').length;
                let $counter = $entity.siblings('.entityboxcount');
                if (!$counter.length) {
                    $entity.after($counter = $('<div>').addClass('entityboxcount')
                        .append($('<i>').addClass('icon-Collapse_expand')).append($('<span>')));
                }
                $counter.css('visibility', 'visible').find('span').text(count + 1);
            }
            return { element: $entity, context: $('#' + boxId), isSecondRow: isSecondRow, isNested: isNested };
        }


    }
    this.getCPEConnections = function () {
        ajaxReq('ISP/getCPEConnections', { structureId: $(app.DE.StructureId).val() }, false, function (response) {
            objCustomD3.CPEConnections.hide();
            objCustomD3.CPEConnections.show(response);
            app.RefreshISPNetworkLayerElements($(app.DE.StructureId).val());
        }, true, false);
    }

    this.GetBarCode = function (system_id, entity_type) {

        $('#BarCodeInfo').html('');
        ajaxReq('Library/GetBarCode', { system_id: system_id, entity_type: entity_type }, true, function (resp) {
            $("#BarCodeInfo").html(resp);
        }, false, false);

    }
    this.GetPortInfo = function (system_id, model_id, entity_type, _type) {
         
        ajaxReq('Library/GetPortInfo', { systemId: system_id, modelId: model_id, entity_type: entity_type, type: _type }, false, function (resp) {
            $("#PortInfo").html(resp);
        }, false, false);

    }


    this.GetAutoFiberLinkId = function (searchText) {
         
        $('#' + searchText).autocomplete({

            source: function (request, response) {
                var res = ajaxReq('main/GetAutoFiberLinkId', { SearchText: request.term }, true, function (data) {
                    if (data.geonames.length == 0) {
                        var result = [
                            {
                                label: 'No match Found'
                            }
                        ];
                        response(result);
                    }
                    else {
                        response($.map(data.geonames, function (item) {
                            return {
                                label: item.link_id, value: item.link_id
                            }//
                        }))
                    }
                }, true, false);

            },
            minLength: 2,
            select: function (event, ui) {
                if (ui.item.label == 'No match Found') {
                    // this prevents "no results" from being selected
                    event.preventDefault();
                }
                else {
                    //event.preventDefault();
                    app.gtype = ui.item.geomType;
                    if (ui.item.entityName != null) {
                        $('#txtCableFiberLinkId').val(ui.item.value + ':' + ui.item.label);
                    }
                    else {
                        $('#txtCableFiberLinkId').val(ui.item.label + ':');
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
    }

    this.GetLoopMangmentDetail = function (_latitude, _longitude, _system_id, _entity_type, _structure_id) {
        // alert("_entity_type1:" + _entity_type);
        ajaxReq('ISP/GetLoopMangmentDetail', {
            latitude: _latitude, longitude: _longitude, associated_system_id: _system_id, associated_System_Type: _entity_type, structure_id: _structure_id
        }, true, function (resp) {
            $("#LoopMangment").html(resp);
            $("#LoopMangment").css('background-image', 'none');
        }, false, false);
    }

    //##POP Association
    this.GetPODAssociationDetail = function (_geom, _system_id, _entity_type, _primary_pod_system_id, _secondary_pod_system_id) {
         
        ajaxReq('ISP/GetPODAssociationDetail', {
            geom: _geom, associated_system_id: _system_id, associated_entity_Type: _entity_type, primary_pod_system_id: _primary_pod_system_id, secondary_pod_system_id: _secondary_pod_system_id
        }, true, function (resp) {
            $("#PODAssoctn").html(resp);
            $("#PODAssoctn").css('background-image', 'none');
        }, false, false);
    }

    this.calculateLoopLength = function (index, CblCalLgth) {
        $('#lstLoopMangment_' + index + '_end_reading').css('border', '1px solid #bdb9b9');
        var objStartReading = $('#lstLoopMangment_' + index + '_start_reading');
        var objEndReading = $('#lstLoopMangment_' + index + '_end_reading');
        var startReading = parseFloat(objStartReading.val());
        var endReading = parseFloat(objEndReading.val());
        if (startReading.toString() != "NaN" && endReading.toString() != "NaN") {
            var length = Math.abs(endReading - startReading);
            if (length > 0 && CblCalLgth > length) {
                $("input[name='lstLoopMangment[" + index + "].loop_length']").val(Math.round(length * 100) / 100);
            } else {
                $("input[name='lstLoopMangment[" + index + "].loop_length']").val(0);
                $('#lstLoopMangment_' + index + '_start_reading').val('');
                $('#lstLoopMangment_' + index + '_end_reading').val('');
                alert("Loop length cannot be greater than cable calculated length!");
                return false;

            }
        }
    }

    this.DeleteLoopDetail = function (system_id) {

        confirm(MultilingualKey.SI_OSP_GBL_JQ_FRM_083, function () {
            ajaxReq('Library/DeleteLoopDetailById', {
                Loop_system_id: system_id
            }, false, function (resp) {
                if (resp.status == "OK") {
                    alert(resp.message);
                    $('#lnk_LoopMangment').trigger("click");
                    $(app.DE.refreshStructureInfo).trigger("click");

                }
                else
                    alert(resp.message);
            }, true, true);
        });
    }

    this.ShowEntityOnMap = function (systemID, eType, gType) {
        if (eType.toUpperCase() == "CABLE") {
            app.cableActions.focus(app.layerActions.entity.select({ type: eType, systemId: systemID }));
        }
    }

    this.getLoopDetailsForCable = function (_system_id) {
         
        ajaxReq('ISP/getLoopDetailsForCable', {
            cable_system_id: _system_id
        }, true, function (resp) {
            $("#LoopDetails").html(resp);
            $("#LoopDetails").css('background-image', 'none');
        }, false, false);
    }



    $(document).on("click", app.DE.liRefLink, function () {
        app.getRefLinksFiles();
    });

    //this.uploadRefLinkFile = function () {
    //    //Get File from uploader and prepare form data to post.
    //    var frmData = new FormData();
    //    var filesize = $('#hdnMaxFileUploadSizeLimit').val();
    //    var Sizeinbytes = filesize * 1024;
    //    if ($(app.DE.UploadRefLink).get(0).files[0].size > Sizeinbytes) {

    //        var validFileSizeinMB = filesize % 1024 == 0 ? parseInt(filesize / 1024) : (filesize / 1024).toFixed(2);

    //        alert(getMultilingualStringValue($.validator.format(MultilingualKey.SI_OSP_GBL_JQ_GBL_112, validFileSizeinMB)));
    //        //alert(MultilingualKey.SI_OSP_GBL_GBL_GBL_053 + "  <b>" + validFileSizeinMB + "<b> MB!", 'warning');//Image size is too large. Maximum image size allowed is
    //    }
    //    else {
    //        var uploadedfile = $(app.DE.UploadRefLink).get(0).files[0];
    //        if (uploadedfile == undefined || uploadedfile == null) {
    //            alert(MultilingualKey.SI_OSP_GBL_GBL_GBL_099, 'warning');//Please select a file!
    //            return false;
    //        }
    //        if (!app.validateImageFileType()) { return false; }
    //        frmData.append(uploadedfile.name, uploadedfile);
    //        frmData.append('system_Id', $(app.DE.liImage).data().systemId);
    //        frmData.append('entity_type', $(app.DE.liImage).data().entityType);
    //        ajaxReqforFileUpload('Main/UploadRefLink', frmData, true, function (resp) {
    //            if (resp.status == "OK") {
    //                app.getElementImages();
    //                alert(resp.message, 'success', 'success');
    //                if ($(app.DE.UploadRefLink)[0] != undefined)
    //                    $(app.DE.UploadRefLink)[0].value = '';
    //            }
    //            else {
    //                alert(resp.message, 'warning');
    //            }
    //        }, true);
    //    }
    //}

    //this.validateImageFileType = function () {
    //    var validFilesTypes = ["bmp", "gif", "png", "jpg", "jpeg"];
    //    var file = $(app.DE.UploadRefLink).val();
    //    var filepath = file;
    //    return app.ValidateFileType(validFilesTypes, filepath);
    //}

    this.uploadReferenceLink = function (_systemId, _entityType) {
        popup.LoadModalDialog('PARENT', 'ISP/uploadReferenceLinks', { system_Id: 0 }, 'Upload Reference Link', "modal-sm");
    }
    this.getRefLinksFiles = function () {
        app.InfoProgress(app.DE.dvRefLink);
        var _system_Id = $(app.DE.liRefLink).data().systemId;
        var _entity_type = $(app.DE.liRefLink).data().entityType;
        ajaxReq('ISP/getISPEntityRefLink', { system_Id: _system_Id, entity_type: _entity_type }, true, function (jResp) {
             
            $(app.DE.divRefLink).html(jResp);
            //  $("#dvEntityInformationDetail #divRefLink").html(jResp);            
             
        }, false, false, true);
    }

    this.uploadRefLink = function () {
        var isvalidFm = true;
        if ($('#DocrefLinkText').val().trim() == "") {
            $('#DocrefLinkText').addClass("notvalid notvalid input-validation-error");
            isvalidFm = false;
        }
        else {
            $('#DocrefLinkText').removeClass("notvalid notvalid input-validation-error");          
        }
        if ($('#DocrefLink').val().trim() == "") {
            $('#DocrefLink').addClass("notvalid notvalid input-validation-error");
            isvalidFm = false;
        }
        else {
            $('#DocrefLink').removeClass("notvalid notvalid input-validation-error");
           
        }

        if (isvalidFm) {
            var url = $("#DocrefLink").val().trim();
            //var pattern = /^(http|https)?:\/\/[a-zA-Z0-9-\.]+\.[a-z]{2,4}/;
            var pattern = new RegExp('^(https?:\\/\\/)?' + // protocol
                '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.?)+[a-z]{2,}|' + // domain name
                '((\\d{1,3}\\.){3}\\d{1,3}))' + // ip (v4) address
                '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*' + //port
                '(\\?[;&a-z\\d%_.~+=-]*)?' + // query string
                '(\\#[-a-z\\d_]*)?$', 'i');
            if (pattern.test(url)) {
                var frmData = new FormData();
                frmData.append('system_Id', $(app.DE.liRefLink).data().systemId);
                frmData.append('entity_type', $(app.DE.liRefLink).data().entityType);
                frmData.append('refDisplayTxt', $('#DocrefLinkText').val());
                frmData.append('refLink', $('#DocrefLink').val());
                ajaxReqforFileUpload('Main/UploadRefLink', frmData, true, function (resp) {
                     
                    if (resp.status == "OK") {
                        $('#closeModalPopup').trigger("click");
                        app.getRefLinksFiles();
                        alert(resp.message);
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
            return false;
        }
    }

    this.downloadDocsFullParams = function (ActionUrl, Parameters) {
        if (Parameters.length > 0) {
            // window.location.href = appRoot + '' + ActionUrl + '?json=' + JSON.stringify(listPathName) + '&entity_type=' + FileNameType;
            window.location.href = appRoot + '' + ActionUrl + '?' + Parameters;
        }
        else {
            alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_019, 'warning');//Please select any image!
        }
    }

    this.downloadImgDocsByID = function (PrntIdWidthSelector, ChkboxWidthSelector, ActionUrl, KeyAttrName, LocationAttr, FileNameType) {
        // KeyAttrName = KeyAttrName == null || KeyAttrName == "" || KeyAttrName == undefined ? "" : KeyAttrName;
        PrntIdWidthSelector = PrntIdWidthSelector == null || PrntIdWidthSelector == "" || PrntIdWidthSelector == undefined ? "" : PrntIdWidthSelector;
        ChkboxWidthSelector = ChkboxWidthSelector == null || ChkboxWidthSelector == "" || ChkboxWidthSelector == undefined ? "" : ChkboxWidthSelector;
        // ActionUrl = ActionUrl == null || ActionUrl == "" || ActionUrl == undefined ? "" : ActionUrl;

        //Ready the list of selected images for download
        var listPathName = [];
        listPathName.push({ systemId: KeyAttrName, location: LocationAttr });
        var idss = "" + PrntIdWidthSelector + " " + ChkboxWidthSelector + "";
        //$.each($(idss), function (indx, itm) {
        //    if ($(this).is(':checked')) {
        //        listPathName.push({ systemId: $(this).attr(KeyAttrName), location: $(this).attr(LocationAttr) });
        //    }
        //});
         
        if (listPathName.length > 0) {
            window.location.href = appRoot + '' + ActionUrl + '?json=' + JSON.stringify(listPathName) + '&entity_type=' + FileNameType;
        }
        else {
            alert(MultilingualKey.SI_ISP_GBL_JQ_GBL_019, 'warning');//Please select any image!
        }
    }
    this.deleteImgDocsByID = function (PrntIdWidthSelector, ChkboxWidthSelector, ActionUrl, KeyAttrName, ReBindFuName, Parameters, DeleteType) {
        //Ready the list of selected images for delete
         
        KeyAttrName = KeyAttrName == null || KeyAttrName == "" || KeyAttrName == undefined ? "" : KeyAttrName;
        PrntIdWidthSelector = PrntIdWidthSelector == null || PrntIdWidthSelector == "" || PrntIdWidthSelector == undefined ? "" : PrntIdWidthSelector;
        ChkboxWidthSelector = ChkboxWidthSelector == null || ChkboxWidthSelector == "" || ChkboxWidthSelector == undefined ? "" : ChkboxWidthSelector;
        ActionUrl = ActionUrl == null || ActionUrl == "" || ActionUrl == undefined ? "" : ActionUrl;
        var ListSystemIds = [];
        ListSystemIds.push(KeyAttrName)
        //var idss = "" + PrntIdWidthSelector + " " + ChkboxWidthSelector+"";
        //$.each($(idss), function (indx, itm) {
        //    if ($(this).is(':checked')) {
        //        ListSystemIds.push($(this).attr(KeyAttrName))
        //    }
        //});
        if (ListSystemIds.length > 0) {
            var func = function () {
                ajaxReq(ActionUrl, { ListSystem_Id: ListSystemIds }, true, function (j) {
                    alert(j.message, 'success', 'success');
                    if (Parameters != "" && Parameters != undefined && Parameters !== null)
                        ReBindFuName(Parameters);
                    else
                        ReBindFuName();
                }, true, true)
            };
            showConfirm(MultilingualKey.SI_OSP_GBL_JQ_FRM_004.replace('file?','') +' '+ DeleteType+'?', func);//Are you sure you want to delete this file?
        } else {
            alert(MultilingualKey.SI_OSP_GBL_JQ_FRM_005, 'warning');//Please select any file!
        }
    }

}
function CheckUnCheckAll(PntIDs, IsPntID, ChldID, IsChldID) {
    let ptid, chldid;
    ptid = IsPntID == true ? $("#" + PntIDs) : $("." + PntIDs);
    chldid = IsChldID == true ? $("#" + ChldID) : $("." + ChldID);
    $(chldid).prop("checked", $(ptid).prop("checked"));
}
function CheckUnCheckAllOnSingle(PntIDs, IsPntID, ChldID, IsChldID) {
    let ptid, chldid;
    ptid = IsPntID == true ? "#" + PntIDs : "." + PntIDs;
    chldid = IsChldID == true ? "#" + ChldID : "." + ChldID;
    if ($('' + chldid + ':checked').length == $('' + chldid + '').length)
        $('' + ptid + '').prop('checked', true);
    else
        $('' + ptid + '').prop('checked', false);
}

function ValidateFormsWithParentID(ClosetParentIDWithSelector) {
    var isvalidForm = true;
    $(ClosetParentIDWithSelector + " input[type=text], select").each(function () {
        if ($(this).val() == "" && ($(this).attr("disabled") != "disabled" || $(this).attr("disabled") != true) && ($(this).attr("readonly") != "readonly" || $(this).attr("readonly") != true) && $(this).is(":visible") && $(this).css('display') != 'none' && $(this).attr("data-val-required") != "") {
            $(this).addClass('notvalid input-validation-error');
            isvalidForm = false;
        }
        else {
            $(this).removeClass('notvalid notvalid input-validation-error');
        }
    })
    return isvalidForm;
}



