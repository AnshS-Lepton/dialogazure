const urls = {
    getPortImages: 'Equipment/GetPortImages',
    saveModel: 'Equipment/SaveModel',
    getModelType: 'Equipment/GetModelType',
    getWorkspaceData: 'Equipment/GetWorkspaceData',
    getLibraryData: 'Equipment/GetLibraryData',
    saveModelType: '/admin/Equipment/SaveModelType',
    getModelAllRules: 'Equipment/GetModelAllRules',
    getVendorList: '../ItemTemplate/GetVendorList',
    getSubVendorList: '../ItemTemplate/GetCatSubcatData'
};

function invertColor(color) {
    color = color.substring(1);
    return '#' + (Number(`0x1${color}`) ^ 0xFFFFFF).toString(16).substr(1).toUpperCase();
}

var rackManager = (function () {
    'use strict';
    var DE = {};
    var _$workArea, _$libArea;
    var _workSpaceActions = container.action;
    var _workAreaData;
    var _libraryData = [];
    var _libraryAllData = [];
    var _nodeTree = [];
    var _selectedModel = {};
    var _hierarchyRules = [];
    var _selectMany = [];


    var action = {
        addModel: function (e) {
            _workAreaData = _workSpaceActions.getWorkArea();
            let current = _workSpaceActions.add();

            current.id = _workSpaceActions.getMaxId() + 1;
            current.position = { x: 0, y: 0 };

            current.height = DE.defaultHeight;
            current.width = DE.defaultWidth;

            current.color = DE.color_code;
            current.stroke = DE.stroke;
            if (e && e.id && !isNaN(e.id)) {
                current.color = e.color;
                current.img_id = isNaN(e.img_id) ? '' : e.img_id;
                current.db_id = e.id;
                current.stroke = e.stroke;
                current.image_data = e.content;
                current.height = e.height || DE.defaultHeight;
                current.width = e.width || DE.defaultWidth;
                current.color = e.color || DE.color_code;
                current.stroke = e.stroke || DE.stroke;
                current.position = {
                    x: e.position && e.position.x || 0,
                    y: e.position && e.position.y || 0
                };
                current.border_width = e.border_width || 0;
                current.name = e.name || '';
            }
            if (e && e.model_id) {
                current.model_id = e.model_id;
                current.model_type_id = e.model_type_id;
                if (_workAreaData.length <= 1) {
                    current.height = DE.defaultSizes[current.model_id - 1].height;
                    current.width = DE.defaultSizes[current.model_id - 1].width;
                    current.border_width = DE.defaultSizes[current.model_id - 1].border_width;

                }
            }
            if (_workAreaData.length <= 1) {
                current.is_static = false;//_workAreaData.length == 1;
                render.setInCenter(current);
                render.setDimension({ modelHeight: current.height, modelWidth: current.width, modelDepth: current.depth });
            }
            else {
                if (_libraryAllData.length) {
                    current.parent = _workAreaData[0].id;
                    let param = {
                        parent_model_id: _selectedModel.model_id,
                        parent_model_type_id: _selectedModel.model_type_id || null,
                        child_model_id: current.model_id,
                        child_model_type_id: isNaN(current.model_type_id) ? null : current.model_type_id
                    }
                    if (validate.modelDrop(param)) {
                        if (!_selectedModel.id)
                            current.parent = _workAreaData[0].id;
                        else
                            current.parent = _selectedModel.id;
                        if (_selectedModel.border_width) {
                            current.position.x += _selectedModel.border_width;
                            current.position.y += _selectedModel.border_width;
                        }
                    }

                    if (!e.isBulk)
                        render.setInCenter(current, _workSpaceActions.getAbsoluteRect(current.parent));

                }
            }

            $(DE.modelBorderWidth).val(current.border_width);
            return current;

        },
        onStartModelDrag: function (d) {
            if (d.is_editable) {
                render.showSubMenuContext(false);
                if (d3.event.sourceEvent.shiftKey && d.id > 1) {
                    //console.log(d.id);
                    if (_selectedModel.id && _selectedModel.id > 1) {
                        if (!_selectMany.includes(_selectedModel.id))
                            _selectMany = _workSpaceActions.selectMany(_selectedModel.id);

                    }
                    action.selectModel({});
                    if (_selectMany.length == 0) {
                        render.modelFocus();
                    }
                    if (!_selectMany.includes(d.id)) {
                        _selectMany = _workSpaceActions.selectMany(d.id);
                        render.modelFocus(d.id, false);
                    }
                    else {
                        _selectMany.splice(_selectMany.indexOf(d.id), 1);
                        render.modelUnFocus(d.id);
                    }
                    if (_selectMany.length == 1) {
                        action.selectModel(_workSpaceActions.select(_selectMany[0]));
                    }
                }
                let model = d;
                //Get draggable Parent if model is in static mode
                model = _workSpaceActions.getDraggable(model.id);
                if (!model.is_static) {
                    let rect = _workSpaceActions.getAbsoluteRect(model.id);
                    model.offset_x = model.position.x - d3.event.x - (model.id != d.id ? rect.x : 0);
                    model.offset_y = model.position.y - d3.event.y - (model.id != d.id ? rect.y : 0);
                    //console.log("x:=" + d3.event.x + " y:=" + d3.event.y);
                }
                //If Model is static then drag parent model if exist
                render.resetResizeControl();
                if (_libraryAllData.length && model.id == 1) {
                    render.resetRuler();
                }
            }
        },
        onModelDrag: function (d) {
            if (d.is_editable) {
                let model = d;
                model = _workSpaceActions.getDraggable(model.id);
                let modelFound = d3.select("#" + DE.modelElementId + model.id);
                //let id = parseInt(modelFound.attr(DE.modelId));
                //let index = _workSpaceActions.getIndex(id);

                if (!model.is_static) {
                    let position = { x: 0, y: 0 };
                    let rectModel = _workSpaceActions.getAbsoluteRect(model.id);
                    position.x = d3.event.x + model.offset_x + (model.id != d.id ? rectModel.x : 0);
                    position.y = d3.event.y + model.offset_y + (model.id != d.id ? rectModel.y : 0);
                    //console.log("x:="+d3.event.x + " y:=" + d3.event.y);
                    if ((model.parent == -1)) {
                        if (position.x > 0 && position.y > 0) {
                            model.position.x = position.x;
                            model.position.y = position.y;
                            modelFound.attr("x", model.position.x).attr("y", model.position.y);
                        }
                    }
                    else {
                        let rect = _workSpaceActions.getAbsoluteRect(model.parent);
                        let parent = _workSpaceActions.select(model.parent);
                        let border_width = parent.border_width || 0;


                        if (border_width) {
                            rect.width -= border_width;
                            rect.height -= border_width;
                        }
                        let param = {
                            x: position.x + rect.x + ((position.x > (rect.width - model.width)) ? model.width + border_width : -border_width),
                            y: position.y + rect.y + ((position.y > (rect.height - model.height)) ? model.height + border_width : -border_width)
                        };
                        if (validate.inBoundary(model.parent, param)) {
                            model.position.x = position.x;
                            model.position.y = position.y;
                            modelFound.attr("x", model.position.x).attr("y", model.position.y);
                        }
                        //let reLPos = generate.modelRealPos(model, model.rotation_angle);
                        render.tempModel({
                            //position: { x: reLPos.x + rect.x, y: reLPos.y + rect.y },
                            position: { x: position.x + rect.x, y: position.y + rect.y },
                            height: model.height,
                            width: model.width,
                            content: modelFound.html(),
                            rotation_angle: model.rotation_angle
                        });
                    }
                }
            }
        },
        onEndModelDrag: function (d) {
            if (d.is_editable) {
                let model = d;
                model = _workSpaceActions.getDraggable(model.id);
                if (!model.is_static) {
                    model.offset_x = 0;
                    model.offset_y = 0;

                    //Check drop container model and attached to it
                    validate.whereDropped(model);
                    render.resetTempModel();

                    if (_libraryAllData.length) {
                        render.ruler(_workAreaData[0]);

                    }
                }
            }
        },
        onModelContextMenu: function (d) {
            if (d.is_editable) {
                //console.log(d);
                //_selectedModel = _workSpaceActions.select(d.id);
                let coordinates = d3.mouse(this);
                _selectMany = _workSpaceActions.selectMany();
                render.subContextMenu(d, coordinates, _selectMany.length > 1);
                if (_selectMany.includes(d.id) && _selectMany.length > 1) {
                    // _selectedModel = {};
                    action.selectModel({});
                }
                else {
                    action.resetMultiSelection();
                    action.selectModel(_workSpaceActions.select(d.id));
                    render.modelFocus(d.id);
                }
            }
            d3.event.preventDefault();
            d3.event.stopPropagation();
        },
        onLibAddButton: function () {
            let $current = d3.select(this);
            let id = parseInt($current.attr(DE.libraryId));
            let img_id = parseInt($current.attr(DE.libraryImgId)) || '';
            let content = $current.html().trim();
            let height = parseInt($current.attr("data-height"));
            let width = parseInt($current.attr("data-width"));
            let model_id = parseInt($current.attr(DE.dataModelId));
            let model_type_id = parseInt($current.attr(DE.dataModelTypeId));
            let color = $current.attr(DE.dataColorCode);
            let stroke = $current.attr(DE.dataStrokeCode);
            let row = parseInt($(DE.rowCount).val()) || 0;
            let col = parseInt($(DE.colCount).val()) || 0;
            let border_width = parseInt($current.attr("data-border-width"));
            let name = $current.attr("data-name");
            if (row > 1 || col > 1) {
                let positions = generate.bulkModelPos(row, col, width, height);
                for (let i = 0 ; i < positions.length; i++) {
                    action.addModel({
                        id: id,
                        img_id: img_id,
                        content: model_id == DE.portKey ? content : '',//content,
                        height: height,
                        width: width,
                        model_id: model_id,
                        model_type_id: model_type_id,
                        color: color,
                        stroke: stroke,
                        position: positions[i],
                        isBulk: true,
                        name: name
                    });
                }
            }
            else {
                action.addModel({
                    id: id,
                    img_id: img_id,
                    content: model_id == DE.portKey ? content : '',//content,
                    height: height,
                    width: width,
                    model_id: model_id,
                    model_type_id: model_type_id,
                    color: color,
                    stroke: stroke,
                    border_width: border_width,
                    name: name
                });
            }

            _workAreaData = _workSpaceActions.getWorkArea();
            render.allModels();
            API.getWorkspaceData({ modelId: id }, render.libChildren);

        },
        onPortAddButton: function () {
            let $current = d3.select(this);
            let id = -1; //parseInt($current.attr(DE.libraryId));
            let img_id = parseInt($current.attr(DE.libraryImgId));
            let content = $current.html().trim();
            let model_id = $(selectModel).val();
            let model_type_id = $(selectModelType).val();
            _workAreaData = _workSpaceActions.getWorkArea();
            if (_workAreaData.length >= 1) {

                id = _workAreaData[0].db__id;
                _workAreaData[0].image_data = content;
                _workAreaData[0].img_id = img_id;
            }
            else {
                action.addModel({ id: id, img_id: img_id, content: content, model_id: model_id, model_type_id: model_type_id });
            }
            _workAreaData = _workSpaceActions.getWorkArea();
            render.allModels();

        },
        onChangeGridColor: function (e) {
            let radioValue = $(DE.gridColorOptions + ":checked").val();
            render.gridColor(radioValue);
        },
        reset: function () {


            _selectedModel = {};
            _libraryData = [];
            _libraryAllData = [];
            _nodeTree = [];

            render.lockCreateModel(false);
            _workSpaceActions.reset();
            _workAreaData = _workSpaceActions.getWorkArea();
            render.allModels();
            render.resetResizeControl();
            render.showLeftPanel(false);
            render.setModelDetail({
                db_id: 0,
                modelMasterName: '',
                modelId: '',

                modelTypeMasterName: '',
                modelTypeId: '',
                modelName: '',
                statusId: 1,

                modelHeight: '0',
                modelWidth: '0',
                modelDepth: '0'
            });
            render.setDimension({
                modelHeight: 0,
                modelWidth: 0,
                modelDepth: 0
            });
            render.selector({
                data: [],
                element: $(DE.selectModelType),
                defaultText: 'Select Type',
                disabled: false
            });
            render.setModelSpecification({});
            render.resetModelTab();
            render.setBtnSaveText("Save");
            render.setWorkSpaceSize(0, 0);
            render.showSubMenuContext(false);
            $(DE.isEditable).val(true);
        },
        reload: function () {
            window.location = window.location.href.split("?")[0];
        },
        initCreate: function (e) {

            //Validate model selection here
            if (validate.initCreate()) {

                return false;
            }
            //Lock model selection
            render.lockCreateModel(true);

            render.showLeftPanel(false);

            //add here left panel action and model operations
            render.modelOperation();
        },
        onResetWorkspace: function (e) {

            showConfirm('Are you sure you want to reset your workspace? You will loose your progress.', action.reload);
        },
        onModelClick: function (d) {
            //Draw outer boundery add circle in corner
            render.resizeControls(d);
        },
        onModeldblClick: function (d) {
            //render.resetRuler();
            action.resetMultiSelection();
            //Draw outer boundery add circle in corner

            if (!d.is_static) {
                if (d.parent == -1) {
                    render.resizeControls(d);

                }
                if (_libraryAllData.length) {
                    render.ruler(_workAreaData[0]);
                }


                action.selectModel(d);
                render.modelFocus(d.id);

                //render library list with validation of dimensions and model rules
                //render.libListByValidation();

                //Prevent all default events
                d3.event.preventDefault();
                d3.event.stopPropagation();
            }
        },
        onResizeDragStart: function (d, isTop) {

        },
        onResizeDrag: function (d, isTop) {
            if (d.is_editable) {
                let limit = _workSpaceActions.getSizeLimitByChildren(d.id);
                let newWidth, newHeight;
                if (isTop) {
                    newWidth = Math.max(d.width + d.position.x - d3.event.x, DE.resizeWidthLimit);
                    if (limit.x.max > newWidth)
                        newWidth = limit.x.max;
                    d.position.x += d.width - newWidth;

                    newHeight = Math.max(d.height + d.position.y - d3.event.y, DE.resizeHeightLimit);
                    if (limit.y.max > newHeight)
                        newHeight = limit.y.max;
                    d.position.y += d.height - newHeight;
                } else {

                    newWidth = Math.max(d3.event.x - d.position.x, DE.resizeWidthLimit);
                    newHeight = Math.max(d3.event.y - d.position.y, DE.resizeHeightLimit);
                    if (limit.x.max > newWidth)
                        newWidth = limit.x.max;
                    if (limit.y.max > newHeight)
                        newHeight = limit.y.max;
                }
                d.width = newWidth;
                d.height = newHeight;
                render.setDimension({ modelHeight: d.height, modelWidth: d.width, modelDepth: d.depth });
                render.allModels();
                render.resizeControls(d);
                render.setWorkSpaceSize(Math.abs(_workAreaData[0].position.y) + _workAreaData[0].height + 50, Math.abs(_workAreaData[0].position.x) + _workAreaData[0].width + 50);
            }
        },
        onRezsizeDragEnd: function (d, isTop) {
            if (d.is_editable) {
                if (_libraryAllData.length) {
                    render.ruler(_workAreaData[0]);
                    render.setWorkSpaceSize(Math.abs(_workAreaData[0].position.y) + _workAreaData[0].height + 50, Math.abs(_workAreaData[0].position.x) + _workAreaData[0].width + 50);
                    render.libListByValidation();
                }
            }
        },
        onSaveModelClick: function (e) {
            if (validate.saveModel()) {
                return false;
                e.stopPropagation();
            }
            let m = generate.modelDetails();
            _workAreaData = _workSpaceActions.getWorkArea();
            if (_workAreaData.length) {
                let param = {
                    id: m.db_id,
                    model_name: m.modelName,
                    model_id: m.modelId,
                    model_type_id: m.modelTypeId,
                    model_image_id: _workAreaData[0].img_id,
                    height: m.modelHeight,
                    width: m.modelWidth,
                    depth: m.modelDepth,
                    item_template_id: m.itemTemplateId || 0,
                    status_id: m.statusId,
                    is_active: true,
                    lstModelChildren: generate.modelMapping(),
                    model_master_name: m.modelMasterName,
                    border_width: m.border_width

                };
                API.saveModel(param);
            }
            else { alert("Workspace is empty! Please select model from library.", "warning"); }
        },
        onChangeSelectModel: function (e) {
            let $selectModel = $(DE.selectModel);
            let modelId = $selectModel.val();
            render.showSpecificationTab((modelId == DE.equipmentKey));
            API.getModelType(modelId);
        },
        onChangeSelectModelType: function (e) {
            DE.color_code = $(DE.selectModelType + " option:selected").data("colorCode");

            let $modal = $(DE.model2);
            if ($(DE.selectModelType + " option:selected").val() == '0') {
                $modal.modal('show');
            }
        },
        onOpenSaveModel: function (e) {
            render.setModelDetail(generate.modelDetails());
        },
        onLibFilterOptionClick: function (e) {
            let radioValue = $(DE.libFilterOptions + ":checked").val();
            let options = _libraryAllData.filter(x=> x.ModelId == radioValue);
            _libraryData = [];

            if (options && options.length) {
                options[0].Types = options[0].Types.filter(x=> x.id != 0);
                render.showLibContext(true);
                render.showLibTypesContext(true);
                render.showSearchLibraryData(true);

                render.showSpecifyContext(radioValue == DE.portKey);
                let libData = generate.libTypes(options[0].Types);

                render.selector({
                    data: libData,
                    element: $(DE.selectLibTypes),
                    disabled: libData.length == 0,
                    value: 'id',
                    text: 'name',
                    dataKey: [{ key: 'color', name: 'data-color-code' }]
                })
                render.showLibTypesContext(libData.length > 0);
            }

            action.onChangeSelectLibTypes();
        },
        onSearchLibraryData: function (e) {
            render.libListByValidation();

        },
        onChangeSelectLibTypes: function (e) {
            $(DE.txtSearchLibraryData).val('');
            render.libTypeOption();
            render.libListByValidation();

        },
        onSaveModleType: function (e) {
            let ISPModelTypeMaster = {};
            ISPModelTypeMaster.value = $(DE.txtModelTypeValue).val();
            ISPModelTypeMaster.key = $(DE.txtModelTypeKey).val();
            ISPModelTypeMaster.color_code = $(DE.txtColorCode).val();
            ISPModelTypeMaster.stroke_code = $(DE.txtStrokeCode).val();
            ISPModelTypeMaster.model_id = $(DE.selectModel + ' option:selected').val();
            API.saveModelType(ISPModelTypeMaster);
        },
        onCancelSubMenuContext: function (e) {
            action.resetMultiSelection();
            render.modelFocus();
            render.showSubMenuContext(false);
        },
        onCopySubMenuContext: function (e) {

            render.showSubMenuContext(false);
            render.modelFocus();
            if (!_selectedModel.is_static) {
                if (_selectedModel && _selectedModel.id > 1) {
                    _workSpaceActions.copy(_selectedModel.id);
                    action.selectModel(_workSpaceActions.getCurrent());
                    render.allModels();
                }
            }
        },
        onDeleteSubMenuContext: function (e) {

            render.showSubMenuContext(false);
            render.modelFocus();
            _selectMany = _workSpaceActions.selectMany();
            if (_selectMany.length) {
                for (let i = 0 ; i < _selectMany.length; i++) {
                    if (_selectMany[i] > 1)
                        _workSpaceActions.remove(_selectMany[i]);
                }
                action.resetMultiSelection();
                render.allModels();
            }
            else if (!_selectedModel.is_static) {
                if (_selectedModel && _selectedModel.id > 1) {
                    _workSpaceActions.remove(_selectedModel.id);
                    action.selectModel({});
                    render.allModels();

                }
            }

        },
        onWorkspaceContextMenu: function (e) {
            render.showSubMenuContext(false);
            action.resetMultiSelection();
            render.modelFocus();
            d3.event.preventDefault();
            d3.event.stopPropagation();
        },
        onSpecifyChanges: function (e) {
            let $row = $(DE.rowCount);
            let $col = $(DE.colCount);
            let row = parseInt($row.val()) || 0;
            let col = parseInt($col.val()) || 0;
            if (row < 1) {
                row = 1;
                $row.val(1);
            }
            if (col < 1) {
                col = 1;
                $col.val(1);
            }
            $(DE.totalCount).val(row * col);
        },
        onBorderThicknessChange: function (e) {
            let $thickness = $(DE.modelBorderWidth);
            let thickness = parseInt($thickness.val()) || 0;
            if (thickness < 1) {
                thickness = 1;
                $thickness.val(1);
            }
            _workAreaData = _workSpaceActions.getWorkArea();
            if (_workAreaData.length > 0) {
                _workAreaData[0].border_width = generate.mmToPixel(thickness);
                render.allModels();
            }
        },
        onDimensionChange: function (e) {
            let $height = $(DE.modelHeight);
            let $width = $(DE.modelWidth);
            let $depth = $(DE.modelDepth);

            let height = parseInt($height.val()) || 0;
            let width = parseInt($width.val()) || 0;
            let depth = parseInt($depth.val()) || 0;
            if (height < 1) {
                height = 1;
                $height.val(1);
            }
            if (width < 1) {
                width = 1;
                $width.val(1);
            }
            if (depth < 1) {
                depth = 1;
                $depth.val(1);
            }
            _workAreaData = _workSpaceActions.getWorkArea();
            if (_workAreaData.length > 0) {
                _workAreaData[0].height = generate.mmToPixel(height);
                _workAreaData[0].width = generate.mmToPixel(width);
                _workAreaData[0].depth = generate.mmToPixel(depth);
                render.allModels();
            }
        },
        resetMultiSelection: function (e) {
            action.selectModel({});
            _selectMany = [];
            _workSpaceActions.clear();
            _workSpaceActions.clearManySelect();
        },
        onRotateSubMenuContext: function (e) {

            if (!_selectedModel.is_static) {
                if (_selectedModel && _selectedModel.id > 1) {
                    _selectedModel.rotation_angle += 90;
                    if (_selectedModel.rotation_angle >= 360)
                        _selectedModel.rotation_angle = 0;
                    render.allModels();
                }
            }
            action.resetMultiSelection();
            render.modelFocus();
            render.showSubMenuContext(false);
        },
        selectModel: function (d) {
            _selectedModel = d;
            render.libListByValidation();
        }
    };

    var API = {
        call: function (url, param, callback) {
            ajaxReq(url, param, true, function (res) {
                if (callback && typeof callback == 'function')
                    callback(res);
            }, false, false, true);
        },
        getPortImages: function () {
            ajaxReq(urls.getPortImages, {}, true, function (res) {
                generate.portList(res);
                render.port();
            }, false, false, true);
        },
        saveModel: function (input) {
            ajaxReq(urls.saveModel, input, true, function (res) {
                //_workAreaData = _workSpaceActions.getWorkArea();
                //_workAreaData[0].db_id = res.id;
                //console.log(res);
                if (res) {
                    action.reset();
                    alert(res.page_message.message);
                    render.modelDetailModal(false);
                }
            }, true, false, true);
        },
        getModelType: function (modelId) {
            ajaxReq(urls.getModelType, { modelId: modelId }, true, function (res) {
                //Render model types
                let dataFound;
                if (res.has_types) {
                    dataFound = res.result;
                }
                render.selector({
                    data: dataFound,
                    element: $(DE.selectModelType),
                    value: 'id',
                    text: 'value',
                    defaultText: 'Select Type',
                    dataKey: [{ key: 'color_code', name: 'data-color-code' }],
                    disabled: !res.has_types
                })

            }, false, false, true);
        },
        getWorkspaceData: function (param, callback) {
            ajaxReq(urls.getWorkspaceData, param, true, function (res) {
                if (callback && typeof callback == 'function')
                    callback(res);
            }, false, false, true);
        },
        getLibraryData: function (param) {
            ajaxReq(urls.getLibraryData, param, true, function (res) {
                //console.log(res);
                if (res && res.length) {
                    render.showlibraryTitle(true);
                    render.showLibFilterContext(true);

                    render.libraryFilter(generate.libFilterList(res));
                    _libraryAllData = res;
                    if (_libraryAllData.length) {
                        render.ruler(_workAreaData[0]);
                    }
                }
            }, false, false, true);
        },
        saveModelType: function (ISPModelTypeMaster) {

            $.ajax({
                url: urls.saveModelType,
                type: 'GET',
                data: ISPModelTypeMaster,
                success: function (res) {
                    if (res.status == "OK") {
                        alert(res.message);
                        $(DE.txtModelTypeValue).val('');
                        $(DE.txtModelTypeKey).val('');
                        $(DE.txtColorCode).val('');
                        $(DE.txtStrokeCode).val('');
                        $('#btnCancelPopup').trigger('click');
                        $(DE.selectModel).val(ISPModelTypeMaster.model_id);
                        $(DE.selectModel).trigger('change');
                    }
                    else {
                        alert(res.message);
                    }

                }
            });

        }
    };

    var generate = {
        portList: function (req) {
            _libraryData = [];
            let len = req.length;
            let xOffset = 100, yOffset = 100;
            let pos_x = 0, pos_y = -yOffset, h = 100, w = 100, margin_x = 10, margin_y = 10;
            for (let i = 0; i < len; i++) {
                if (i % 2 === 0) {
                    pos_x = margin_x;
                    pos_y += yOffset + margin_y;
                }
                _libraryData.push({
                    id: req[i].id,
                    position: { x: pos_x, y: pos_y },
                    height: h,
                    width: w,
                    image_data: req[i].image_data,
                    db_height: generate.mmToPixel(req[i].height),
                    db_width: generate.mmToPixel(req[i].width),
                    db_depth: generate.mmToPixel(req[i].depth),
                    model_id: req[i].model_id,
                    model_type_id: req[i].model_type_id
                });
                pos_x += xOffset + margin_x;

            }
        },
        libraryList: function (req) {
            _libraryData = [];
            let len = req.length;
            let xOffset = 100, yOffset = 100;
            let pos_x = 0, pos_y = -yOffset, h = 100, w = 100, margin_x = 5, margin_y = 5;
            for (let i = 0; i < len; i++) {
                if (i % 2 === 0) {
                    pos_x = margin_x;
                    pos_y += yOffset + margin_y;
                }
                _libraryData.push({
                    id: req[i].id,
                    position: { x: pos_x, y: pos_y },
                    height: h,
                    width: w,
                    image_data: req[i].image_data,
                    color: req[i].color_code,
                    stroke: req[i].stroke_code,
                    db_height: generate.mmToPixel(req[i].height),
                    db_width: generate.mmToPixel(req[i].width),
                    db_depth: generate.mmToPixel(req[i].depth),
                    db_border_width: generate.mmToPixel(req[i].border_width),
                    name: req[i].model_name,
                    db_id: req[i].id,
                    img_id: req[i].model_image_id,
                    model_id: req[i].model_id,
                    model_type_id: req[i].model_type_id
                });
                pos_x += xOffset + margin_x;

            }
        },

        modelDetails: function () {
            let dbId = 0;
            _workAreaData = _workSpaceActions.getWorkArea();
            let itemId = $(DE.selectVender + " option:selected").data("itemId");
            if (_workAreaData && _workAreaData.length) {
                dbId = _workAreaData[0].db_id;
            }
            return {
                db_id: dbId,
                modelMasterName: $(DE.selectModel + " option:selected").text(),
                modelId: $(DE.selectModel).val(),

                modelTypeMasterName: $(DE.selectModelType + " option:selected").text(),
                modelTypeId: $(DE.selectModelType).val(),
                modelName: $(DE.txtModelName).val(),
                statusId: $(DE.selectStatus).val(),

                modelHeight: $(DE.modelHeight).val(),
                modelWidth: $(DE.modelWidth).val(),
                modelDepth: $(DE.modelDepth).val(),
                border_width: $(DE.modelBorderWidth).val(),
                itemTemplateId: itemId
            }
        },
        libFilterList: function (res) {
            let libData = [];
            for (let i = 0; i < res.length; i++) {
                if (res[i].Types && res[i].Types.length) {
                    libData.push({ model_id: res[i].ModelId, model_name: res[i].Types[0].model_master_name })
                };
            }
            return libData;
        },
        modelTree: function (id, data, selected) {
            if (id == -1)
                return null;
            let children = data.filter(x=>x.parent == id);
            selected.children = [];
            for (let i = 0 ; i < children.length; i++) {
                //console.log(children[i]);
                selected.children.push(children[i]);
                generate.modelTree(children[i].id, data, children[i]);
            }

        },
        modelMapping: function () {
            let mapping = [];
            _workAreaData = _workSpaceActions.getWorkArea();

            for (let i = 0; i < _workAreaData.length; i++) {
                let children = _workAreaData.filter(x=>x.parent == _workAreaData[i].id);
                for (let j = 0; j < children.length; j++) {
                    mapping.push({
                        id: children[j].id,
                        parent_model_info_id: _workAreaData[i].db_id,
                        child_model_info_id: children[j].db_id,
                        child_x_pos: children[j].position.x,
                        child_y_pos: children[j].position.y,
                        rotation_angle: children[j].rotation_angle,
                        model_view_id: children[j].model_view_id,
                        parent_id: children[j].parent,
                    });
                }
            }
            return mapping;
        },
        boundingRect: function (targetId, point) {
            let containers = [];
            _workAreaData = _workSpaceActions.getWorkArea();
            let modelList = [];
            $.extend(true, modelList, _workAreaData);
            modelList.sort(function (a, b) { return a.parent - b.parent; });
            let max = -1, parent = -1;
            for (let i = 0; i < modelList.length; i++) {
                if (targetId != modelList[i].id && targetId != modelList[i].parent) {
                    if (_workSpaceActions.contains(modelList[i].id, point)) {
                        containers.push(i);
                        if (parent == modelList[i].parent) {
                            if (max < modelList[i].id)
                                max = modelList[i].id;
                        }
                        else { max = modelList[i].id; }
                    }
                }
            }
            return max;
        },
        pixelToMM: function (value) {
            return parseFloat((value * (DE.grid_scale / DE.gridCellSize)).toFixed(2));
        },
        mmToPixel: function (value) {
            return parseFloat((value * (DE.gridCellSize / DE.grid_scale)).toFixed(2));
        },
        bulkModelPos: function (row, col, width, height, start) {
            let positions = [];
            let x = start && start.x || 0, y = start && start.y || 0;
            for (let i = 0; i < row; i++) {
                x = 0;
                for (let j = 0; j < col; j++) {
                    positions.push({
                        x: x,
                        y: y
                    });
                    x += width;
                }
                y += height;
            }
            return positions;
        },
        rulerData: function (limit, offset) {
            let res = [];

            for (let i = 0; i <= limit; i += offset) {
                res.push(i);
            }
            return res;
        },
        libTypes: function (types) {
            let res = [];
            types.map(function (t) {
                if (t.model_type_id && res.filter(x=>x.id == t.model_type_id).length == 0) {
                    res.push({
                        id: t.model_type_id,
                        name: t.model_type_master_name,
                        color: t.color_code
                    });
                }
                return { id: t.model_type_id, name: t.model_type_master_name };
            });
            return res;
        },
        modelRealPos: function (d, angle) {
            let center = { x: d.position.x + d.width / 2, y: d.position.y + d.height / 2 };
            if (angle == 90) {
                center = { x: d.position.x + d.width / 2 - d.height / 2, y: d.position.y + d.height / 2 - d.width / 2 };
                console.log("x:" + center.x, " y:" + center.y);
            }
            if (angle == 180) {
                center = { x: d.position.x + d.width / 2 - d.height / 2, y: d.position.y + d.height / 2 - d.width / 2 };
                console.log("x:" + center.x, " y:" + center.y);
            }
            if (angle == 270) {
                center = { x: d.position.x + d.width / 2 - d.height / 2, y: d.position.y + d.height / 2 - d.width / 2 };
                console.log("x:" + center.x, " y:" + center.y);
            }
            if (angle == 0) {
                center = { x: d.position.x, y: d.position.y };
                console.log("x:" + center.x, " y:" + center.y);
            }
            return center;
        }
    };

    var render = {
        port: function () {
            let libCount = _libraryData.length;
            _$libArea.attr('height', Math.ceil(libCount / 2) * DE.librarySize);
            _$libArea.selectAll("*").remove();
            let libData = _$libArea.selectAll(DE.btnLibAdd)
                .data(_libraryData);
            libData.exit().remove();
            let libMain = libData.enter()
                .append("svg")
                .attr("pointer-events", "visible")
                .attr(DE.libraryId, function (d) {
                    return d.id;
                }).attr(DE.libraryImgId, function (d) {
                    return d.img_id || d.id;
                }).attr(DE.dataModelId, function (d) {
                    return d.model_id;
                }).attr(DE.dataModelTypeId, function (d) {
                    return d.model_type_id;
                })
                .attr("class", function (d) { return DE.libAddButtonClass; })
                .attr("x", function (d) { return d.position.x + d.width / 4; })
                .attr("y", function (d) { return d.position.y + d.height / 4; })
                .attr("height", function (d) { return d.height; })
                .attr("width", function (d) { return d.width; })
                .attr("data-height", function (d) { return d.db_height; })
                .attr("data-width", function (d) { return d.db_width; })
                .html(function (d) { return d.image_data; })
                .on('click', action.onPortAddButton).exit();
            libData.enter().append("g").append("rect")
                               .attr("x", function (d) { return d.position.x; })
                                .attr("pointer-events", "none")
                               .attr("y", function (d) { return d.position.y; })
                               .attr("height", function (d) { return d.height; })
                               .attr("width", function (d) { return d.width; })
                               .style("fill", function (d) {
                                   return "transparent";
                               })
                               .style("stroke", function (d) { return "#a7a4a7"; })
                               .style("stroke-width", function (d) { return '1'; });
        },
        model: function (svgMain) {

            svgMain.attr("pointer-events", "visible")
                        .attr("id", function (d) { return DE.modelElementId + d.id; })
                        .attr(DE.elementId, function (d) { return d.db_id; })
                         .attr(DE.dataModelId, function (d) {
                             return d.model_id;
                         }).attr(DE.dataModelTypeId, function (d) {
                             return d.model_type_id;
                         })
                        .attr(DE.libraryImgId, function (d) { return d.img_id; })
                        .attr("class", function (d) { return DE.modelClass })
                        .attr("x", function (d) { return d.position.x; })
                        .attr("y", function (d) { return d.position.y; })

                        .attr("height", function (d) {
                            let height = d.height;
                            if (d.rotation_angle == 90 || d.rotation_angle == 270) {
                                height = d.height + d.width;
                            }
                            return height;
                        })
                        .attr("width", function (d) {
                            let width = d.width;
                            if (d.rotation_angle == 90 || d.rotation_angle == 270) {
                                width = d.height + d.width;
                            }
                            return width;
                        })


                        .call(d3.drag()
                        .on("start", action.onStartModelDrag)
                        .on("drag", action.onModelDrag)
                        .on("end", action.onEndModelDrag))
                        .on("contextmenu", action.onModelContextMenu)
                        .on("dblclick", action.onModeldblClick);
            let content = svgMain.append("g")
                   .attr("transform", function (d) {
                       let x = d.width / 2;
                       let y = d.height / 2;
                       return "rotate(" + d.rotation_angle + "," + x + "," + y + ")";
                   });
            //let content = group.append("svg").attr("height", function (d) { return d.height; })
            //            .attr("width", function (d) { return d.width; });

            ////Add background rectangle
            let background = content.append("g");
            background.append("g").append("rect")
                 .attr("id", function (d) { return DE.elementRect + d.id; })
                 .attr("class", DE.elementRect)
            .attr("x", function (d) {
                let x = 0;
                if (d.rotation_angle == 90) {
                    if (d.width > d.height) {
                        x = (d.width - d.height) / 2;
                    }

                }
                if (d.rotation_angle == 270) {
                    if (d.width > d.height) {
                        x = -(d.width - d.height) / 2;
                    }
                }
                return x;
            })
            .attr("y", function (d) {
                let y = 0;
                if (d.rotation_angle == 90) {
                    if (d.height > d.width) {
                        y = -(d.height - d.width) / 2;
                    }

                }
                if (d.rotation_angle == 270) {
                    if (d.height > d.width) {
                        y = (d.height - d.width) / 2;
                    }
                }
                return y;
            })
            .attr("height", function (d) { return d.height; })
            .attr("width", function (d) { return d.width; })
            .style("fill", function (d) {
                let color = d.model_id == DE.chessisKey ? DE.chassisColor : d.color;
                if (!color || color == null)
                { color = DE.color_code; }
                if (d.model_id == DE.portKey) {
                    color = DE.portColor;
                }
                return color;
            })
            .style("stroke", function (d) {
                let stroke = d.stroke;
                if (!stroke || stroke == null)
                { stroke = DE.stroke; }
                return stroke;
            })
            .style("stroke-width", function (d) { return '1'; }).append('title').text(function (d) { return d.name; });

            ////Chassis inner container
            background.filter(function (d) {
                return d.model_id == DE.chessisKey;
            }).append("rect")
              .attr("x", function (d) { return d.border_width; })
              .attr("y", function (d) { return d.border_width; })
              .attr("height", function (d) { return d.height - (2 * d.border_width); })
              .attr("width", function (d) { return d.width - (2 * d.border_width); })
              .style("fill", function (d) {
                  return DE.chassisInnerColor;//d.color;
              })
              .style("stroke", function (d) { return d.stroke; })
              .style("stroke-width", function (d) { return '1'; }).append('title').text(function (d) { return d.name; });

            ////Add image svg data 
            let imgContainer = content.append("g");


            //Filter by image data and id; add viewBox
            imgContainer
                .filter(function (d) {
                    return !isNaN(d.img_id) && d.image_data && d.image_data != '';
                })
                .append("svg")
                .attr("preserveAspectRatio", "xMinYMin meet")

                .attr("viewBox", function (d) { return "0 0 50 50"; })
                .attr("height", function (d) { return d.height - 8; })
                .attr("width", function (d) { return d.width - 8; })
                 .attr("x", function (d) { return 4; })
                .attr("y", function (d) { return 4; })
                .html(function (d) { return d.image_data; });
        },
        allModels: function () {
            render.nodeTree();
            if (_libraryAllData.length) {
                render.ruler(_workAreaData[0]);
            }
            render.modelFocus(_selectedModel.id);
        },
        library: function () {
            let libCount = _libraryData.length;
            _$libArea.attr('height', Math.ceil(libCount / 2) * DE.librarySize);
            _$libArea.selectAll("*").remove();
            let libData = _$libArea.selectAll(DE.btnLibAdd)
                .data(_libraryData);
            libData.exit().remove();
            let libMain = libData.enter()
                 .append("svg")
                 .attr("pointer-events", "none")
                 .attr("class", function (d) { return DE.libAddButtonClass; })
                 .attr("x", function (d) {
                     let x = d.position.x + (d.width * 0.17);
                     if (d.model_id == DE.portKey) {
                         x = d.position.x + (d.width * 0.25);
                     }
                     return x;
                 })
                 .attr("y", function (d) { return d.position.y + 5; })
                 .attr("height", function (d) { return d.height / 2; })
                 .attr("width", function (d) { return (d.width * 0.7); })
                 .html(function (d) { return d.image_data; });

            //Main library button
            libData.enter().append("g").append("rect")
                            .attr("x", function (d) { return d.position.x; })
                             .attr("pointer-events", "visible")
                            .attr("y", function (d) { return d.position.y; })
                            .attr("height", function (d) { return d.height; })
                            .attr("width", function (d) { return d.width; })
                            .style("fill", function (d) {
                                return "transparent";
                            })
                            .style("stroke", function (d) { return "#a7a4a7"; })
                            .style("stroke-width", function (d) { return '1'; })
                            .attr(DE.libraryId, function (d) {
                                return d.db_id;
                            }).attr(DE.libraryImgId, function (d) {
                                return d.img_id;
                            })
                            .attr(DE.dataModelId, function (d) {
                                return d.model_id;
                            }).attr(DE.dataModelTypeId, function (d) {
                                return d.model_type_id;
                            })
                            .attr(DE.dataColorCode, function (d) {
                                return d.color;
                            }).attr(DE.dataStrokeCode, function (d) {
                                return d.stroke;
                            }).attr("data-height", function (d) { return d.db_height; })
                            .attr("data-width", function (d) { return d.db_width; })
                            .attr("data-depth", function (d) { return d.db_depth; })
                            .attr("data-border-width", function (d) { return d.db_border_width; })
                            .attr("data-name", function (d) { return d.name; })
                            .attr("class", function (d) { return DE.libAddButtonClass; })
                            .html(function (d) { return d.image_data; })
                            .on('click', action.onLibAddButton)
                            .append('title').text(function (d) { return d.name; });

            libMain.filter(function (d) {
                return d.image_data && d.image_data != '';
            }).attr("preserveAspectRatio", "xMinYMin meet")
                .attr("viewBox", function (d) { return "0 0 50 50"; });
            //Library display rect
            libMain.filter(function (d) {
                return d.image_data == '' || !d.image_data;
            }).append("g").append("rect")
                        .attr("pointer-events", "none")
                        .attr("x", function (d) {
                            let x = 5;
                            if (d.model_id == DE.trayKey) {
                                x = -5;
                            }
                            return x;
                        })
                        .attr("y", function (d) {
                            let h = 5;
                            if (d.model_id == DE.trayKey) {
                                h = 25;
                            }
                            return h;
                        })
                        .attr("height", function (d) {
                            let h = d.height / 2 - 20;
                            if (d.model_id == DE.trayKey) {
                                h = 1;
                            }
                            return h;
                        })
                        .attr("width", function (d) {
                            let w = (d.width * 0.7) - 10;
                            if (d.model_id == DE.trayKey) {
                                w = (d.width * 0.7) + 10;
                            }
                            return w;
                        })
                        .style("fill", function (d) {
                            let res = d.color;
                            if (d.model_id == DE.trayKey) {
                                res = 'transparent';
                            }
                            if (d.model_id == DE.chessisKey) {
                                res = DE.chassisInnerColor;
                            }
                            return res;
                        })
                        .style("stroke", function (d) {
                            let res = d.stroke;
                            if (d.model_id == DE.trayKey) {
                                res = DE.portColor;
                            }
                            if (d.model_id == DE.chessisKey) {
                                res = DE.chassisColor;
                            }
                            return res;
                        })
                        .style("stroke-width", function (d) { return '3'; });

            //Dimension and label part
            let dimension = libData.enter().append("g");
            dimension.append("rect")
                    .attr("pointer-events", "none")
                    .attr("x", function (d) { return d.position.x + d.width / 4 - 25; })
                    .attr("y", function (d) { return d.position.y + d.height - 40; })
                    .attr("height", function (d) { return 20; })
                    .attr("width", function (d) { return (d.width); })
                    .style('fill', DE.portColor);
            dimension.append("text")
                    .attr("pointer-events", "none")
                    .attr("x", function (d) { return d.position.x + d.width / 2; })
                    .attr("y", function (d) { return d.position.y + d.height - 25; })
                    .style('fill', '#FFFFFF')
                    .style("text-anchor", "middle")
                    .text(function (d) { return "" + parseInt(generate.pixelToMM(d.db_height)) + " X " + parseInt(generate.pixelToMM(d.db_width)) + " X " + parseInt(generate.pixelToMM(d.db_depth)); });


            libData.enter().append("g").append("text")
                            .attr("pointer-events", "none")
                            .attr("x", function (d) { return d.position.x + d.width / 2; })
                            .attr("y", function (d) { return d.position.y + d.height - 5; })
                            .style("text-anchor", "middle")
                            .html(function (d) {
                                let name = d.name;
                                if (d.name.length > 16) {
                                    name = d.name.substring(0, 12) + '...';
                                }

                                return name;
                            });
        },
        gridColor: function (color) {
            let $grid = $(DE.workAreaContext);
            if ($grid) {
                let prevColor = $grid.data('color');
                $grid.removeClass(prevColor);
                if (color !== DE.defaultColor)
                    $grid.addClass(color);
                $grid.data('color', color);
            }
        },
        lockCreateModel: function (isLocked) {
            let $selectModel = $(DE.selectModel);
            let $selectModelType = $(DE.selectModelType);
            let $lockModel = $(DE.lockModel);
            let $create = $(DE.createModel);
            let $reset = $(DE.resetWorkspace);
            let $openSave = $(DE.openSaveModel);

            $create.attr('disabled', isLocked);
            $reset.attr('disabled', !isLocked);
            $selectModel.attr('disabled', isLocked);
            $selectModelType.attr('disabled', isLocked);
            $openSave.attr('disabled', !isLocked);
            if (isLocked) {
                $lockModel.css('visibility', 'visible');
                $reset.addClass(DE.focusRed);
            }
            else {
                $lockModel.css('visibility', 'hidden');
                $reset.removeClass(DE.focusRed);
            }
            if (!isLocked) {
                $selectModel.val('');
                $selectModelType.val('');
            }
        },
        setInCenter: function (model, parent) {
            let rect = parent || _$workArea.node().getBoundingClientRect()
            let pos_x = rect.width / 2 - model.width / 2, pos_y = rect.height / 2 - model.height / 2;
            model.position.x = pos_x < 0 ? 0 : pos_x;
            model.position.y = pos_y < 0 ? 0 : pos_y;
        },
        resizeControls: function (e) {
            d3.select("#" + DE.resizeBoundry).remove();
            _$workArea.append("rect")
                .attr("pointer-events", "none")
                    .attr("id", DE.resizeBoundry)
                    .attr("x", e.position.x)
                    .attr("y", e.position.y)
                    .attr("height", e.height)
                    .attr("width", e.width)
                    .style("fill", "transparent")
                    .style("stroke", "#787474");

            d3.select("#" + DE.resizeTopLeft).remove();
            _$workArea.append("circle")
                .attr("id", DE.resizeTopLeft)
                    .attr("r", 4)
                   .style("stroke", "#000")
                   .style("fill", "#fff")
                   .attr("cx", e.position.x)
                   .attr("cy", e.position.y)
                   .call(d3.drag().on("start", function () {
                       action.onResizeDragStart(e, true);
                   }).on("drag", function () {
                       action.onResizeDrag(e, true);
                   })
            .on("end", function () {
                action.onRezsizeDragEnd(e, true);
            }));
            d3.select("#" + DE.resizeBottomRight).remove();
            _$workArea.append("circle")
                    .attr("id", DE.resizeBottomRight)
                    .attr("r", 4)
                    .style("stroke", "#000").style("fill", "#fff")
                    .attr("cx", e.position.x + e.width)
                    .attr("cy", e.position.y + e.height)
                .call(d3.drag().on("start", function () {
                    action.onResizeDragStart(e, false);
                }).on("drag", function () {
                    action.onResizeDrag(e, false);
                }).on("end", function () {
                    action.onRezsizeDragEnd(e, true);
                }));


        },
        resetResizeControl: function () {
            d3.select("#" + DE.resizeTopLeft).attr("r", 0)
                                    .attr("cx", 0)
                                    .attr("cy", 0);

            d3.select("#" + DE.resizeBottomRight).attr("r", 0)
                                        .attr("cx", 0)
                                        .attr("cy", 0);
            d3.select("#" + DE.resizeBoundry).attr("x", 0)
                                    .attr("y", 0)
                                    .attr("height", 0)
                                    .attr("width", 0);
        },
        resetTempModel: function () {
            d3.select("#" + DE.tempModel).attr("x", 0)
                                   .attr("y", 0)
                                   .attr("height", 0)
                                   .attr("width", 0)
                                    .html('');


        },
        tempModel: function (e) {
            d3.select("#" + DE.tempModel).remove();

            _$workArea.append("svg").attr("id", DE.tempModel)
                    .append("g")

                .append("svg")
                .attr("pointer-events", "none")
                    //.attr("id", DE.tempModel)
                    .attr("x", e.position.x)
                    .attr("y", e.position.y)
                    //.attr("height", e.height)
                    //.attr("width", e.width)
                .style("fill-opacity", 0.3)

                    .html(e.content);
        },
        setModelDetail: function (d) {
            $(DE.txtModelMasterName).val(d.modelMasterName);
            $(DE.txtModelId).val(d.modelId);

            $(DE.txtModelTypeMasterName).val(d.modelTypeMasterName);
            $(DE.txtModelTypeId).val(d.modelTypeId);
            $(DE.txtModelName).val(d.modelName);
            $(DE.selectStatus).val(d.statusId);

            $(DE.txtModelHeight).val(d.modelHeight);
            $(DE.txtModelWidth).val(d.modelWidth);
            $(DE.txtModelDepth).val(d.modelDepth);

        },
        setModelSpecification: function (d) {
            $(DE.category).val(d.category);
            $(DE.itemCode).val(d.code);

            $(DE.subCategory_1).val(d.subCategory_1);
            $(DE.subCategory_2).val(d.subCategory_2);
        },
        setDimension: function (d) {
            let $height = $(DE.modelHeight);
            let $width = $(DE.modelWidth);
            let $depth = $(DE.modelDepth);

            $height.val(generate.pixelToMM(d.modelHeight));
            $width.val(generate.pixelToMM(d.modelWidth));
            $depth.val(generate.pixelToMM(d.modelDepth));
            if (d.is_editable != undefined) {
                $height.attr('disabled', !d.is_editable);
                $width.attr('disabled', !d.is_editable);
                $depth.attr('disabled', !d.is_editable);
            }

        },

        selector: function (d) {

            let option = "";
            if (d.defaultText)
                option = "<option value disabled selected hidden>" + d.defaultText + "</option>";
            if (d && d.data) {
                let len = d.data.length;
                let keys = "";
                if (len > 0) {
                    for (let i = 0; i < len; i++) {
                        keys = "";
                        if (d.dataKey && d.dataKey.length) {
                            for (let j = 0; j < d.dataKey.length; j++) {
                                keys += " " + d.dataKey[j].name + "=" + d.data[i][d.dataKey[j].key];
                            }
                        }
                        option += "<option value='" + d.data[i][d.value] + "' " + keys + " >" + d.data[i][d.text] + "</option>";
                    }
                }
            }
            d.element.attr('disabled', d.disabled);
            d.element.html(option);
        },


        showDimensionContext: function (flag) {
            if (flag)
                $(DE.dimensionContext).show();
            else
                $(DE.dimensionContext).hide();
            render.showEmptyPage(!flag);
        },
        showLibFilterContext: function (flag) {
            if (flag)
                $(DE.libFilterContext).show();
            else
                $(DE.libFilterContext).hide();
        },
        showLibTypesContext: function (flag) {
            if (flag)
                $(DE.selectLibTypes).show();
            else
                $(DE.selectLibTypes).hide();
        },
        showSpecifyContext: function (flag) {
            if (flag)
                $(DE.specifyNoContext).show();
            else
                $(DE.specifyNoContext).hide();
            render.resetSpecifyContent();
        },
        showLibContext: function (flag) {
            if (flag)
                $(DE.libContext).show();
            else
                $(DE.libContext).hide();
        },
        showEmptyPage: function (flag) {
            if (flag)
                $(DE.emptyPage).show();
            else
                $(DE.emptyPage).hide();
        },
        showLeftPanel: function (flag) {
            render.showDimensionContext(flag);
            render.showLibFilterContext(flag);
            render.showLibTypesContext(flag);
            render.showSpecifyContext(flag);
            render.showLibContext(flag);
            render.showlibraryTitle(flag);
            render.showBorderWidthContext(flag);
        },
        showlibraryTitle: function (flag) {
            if (flag)
                $(DE.libraryTitle).show();
            else
                $(DE.libraryTitle).hide();
        },
        showSpecificationTab: function (flag) {
            if (flag) {
                $(DE.specificationTab).show();

            }
            else {
                $(DE.specificationTab).hide();

            }


        },
        activeSpecificationTab: function (flag) {
            if (flag) {

                $(DE.anchorSpecification).addClass('active').addClass('show');
                $(DE.anchorModelDetails).removeClass('active').removeClass('show');
                $(DE.tabSpecification).addClass('active').addClass('show');
                $(DE.tabModelDetails).removeClass('active').removeClass('show');
            }
            else {

                $(DE.anchorModelDetails).addClass('active').addClass('show');
                $(DE.anchorSpecification).removeClass('active').removeClass('show');
                $(DE.tabModelDetails).addClass('active').addClass('show');
                $(DE.tabSpecification).removeClass('active').removeClass('show');
            }


        },
        showSearchLibraryData: function (flag) {
            if (flag)
                $(DE.searchModelElements).show();
            else
                $(DE.searchModelElements).hide();
        },
        showBorderWidthContext: function (flag) {
            if (flag)
                $(DE.borderWidthContext).show();
            else
                $(DE.borderWidthContext).hide();
            render.resetSpecifyContent();
        },
        modelOperation: function (d) {
            let $selectModel = $(DE.selectModel);
            let $selectModelType = $(DE.selectModelType);
            let $isEditable = $(DE.isEditable);
            render.showDimensionContext(true);
            render.showBorderWidthContext($selectModel.val() == DE.chessisKey);

            if ($selectModel.val() == DE.portKey) {
                if ($isEditable && $isEditable.val().toLowerCase() == 'true') {
                    render.showlibraryTitle(true);
                    render.showLibContext(true);
                    render.showSearchLibraryData(false);
                    API.getPortImages();
                }
            }
            else {
                //render.showLibContext(true);
                let model_id = $selectModel.val();
                let model_type_id = $selectModelType.val();
                action.selectModel(action.addModel({
                    id: 0,
                    model_id: model_id,
                    model_type_id: model_type_id,
                }));

                _workAreaData = _workSpaceActions.getWorkArea();
                render.allModels();
                //render.modelFocus(_selectedModel.id);
                if ($isEditable && $isEditable.val().toLowerCase() == 'true' && DE.slotKey != model_id) {
                    API.getLibraryData({
                        modelId: $selectModel.val(),
                        modelType: $selectModelType.val()
                    });
                }

            }
        },
        libraryFilter: function (d) {
            let $libBox = d3.select(DE.libFilterBox);

            $libBox.selectAll('.' + DE.filterOptionClass).remove();
            let libFilterData = $libBox.selectAll('.' + DE.filterOptionClass).data(d);
            let radioFunction = libFilterData.enter().append('div').attr('class', DE.filterOptionClass);
            let radio = radioFunction.append('div').attr('class', DE.radioClass);
            let radioLabel = radio.append("label").attr('class', DE.radioLabelClass).text(function (d) { return d.model_name });
            radioLabel.append("input").attr("type", "radio").attr("name", DE.radioName).attr("value", function (d) { return d.model_id });
            radioLabel.append("span").attr('class', DE.radioSpanClass);
        },
        modelChildren: function (root, nodes) {
            if (!nodes || (nodes && nodes.length == 0)) {
                return null;
            }
            else {
                let parentData = root.selectAll("svg")
                                      .data(nodes)
                                      .enter();
                //let parent = parentData.append("g")
                //    //.attr("id", function (d) { return DE.modelElementId + d.id; })
                //    .attr("transform", function (d) {
                //        let x =  d.width / 2;
                //        let y =  d.height / 2;
                //        return "rotate(" + d.rotation_angle + "," + x + "," + y + ")";
                //    }).append("svg");

                let parent = parentData.append("svg");
                render.model(parent);

                parent.each(function (d) {
                    render.modelChildren(d3.select(this), d.children);
                });

            }
        },
        nodeTree: function () {
            _workAreaData = _workSpaceActions.getWorkArea();
            // _$workArea.selectAll(DE.model).remove();
            _$workArea.selectAll('*').remove();
            if (_workAreaData.length) {
                _nodeTree = [];
                _nodeTree.push(_workAreaData[0]);
                generate.modelTree(_workAreaData[0].id, _workAreaData, _workAreaData[0]);
                render.modelChildren(_$workArea, _nodeTree);
            }

        },
        modelLoad: function (res) {
            action.initCreate();
            if (res.length) {
                _workSpaceActions.setWorkArea(res);
                _workAreaData = _workSpaceActions.getWorkArea();

                render.setInCenter(_workAreaData[0]);
                render.allModels();
                render.setDimension({ modelHeight: _workAreaData[0].height, modelWidth: _workAreaData[0].width, modelDepth: _workAreaData[0].depth, is_editable: _workAreaData[0].is_editable });
                render.showSpecificationTab((_workAreaData[0].model_id == DE.equipmentKey));
                render.setBtnSaveText("Update");
                render.setWorkSpaceSize(Math.abs(_workAreaData[0].position.y) + _workAreaData[0].height + 50, Math.abs(_workAreaData[0].position.x) + _workAreaData[0].width + 50);
                //render.showLibFilterContext(!_workAreaData[0].is_editable);
            } else {

            }
        },
        libChildren: function (res) {

            if (res.length > 1) {
                _workAreaData = _workSpaceActions.getWorkArea();
                res.splice(0, 1);
                let selected = _workSpaceActions.getCurrent();

                for (let i = 0; i < res.length; i++) {
                    if (selected.db_id == res[i].parent) {
                        res[i].parent = selected.id;
                        res[i].is_static = true;
                    }
                    _workAreaData.push(res[i]);
                }

            } else {

            }
            render.allModels();

        },
        showSubMenuContext: function (flag) {
            if (flag)
                $(DE.subMenuContext).show();
            else
                $(DE.subMenuContext).hide();
        },
        modelFocus: function (id, isReset) {
            isReset = isReset !== undefined ? isReset : true;
            if (isReset) {
                d3.selectAll('.' + DE.elementRect).classed(DE.selectedClass, false);
            }
            if (id)
                d3.select('#' + DE.elementRect + id).classed(DE.selectedClass, true);
        },
        modelUnFocus: function (id) {
            if (id)
                d3.select('#' + DE.elementRect + id).classed(DE.selectedClass, false);
        },
        errorFocus: function ($e, flag) {
            if (flag)
                $e.css('border', '1px solid red').css('border-radius', '0.25rem');
            else {
                $e.css('border', '').css('border-radius', '');
            }
        },
        modelDetailModal: function (flag) {
            if (flag)
                $(DE.modelDetailModal).modal('show');
            else
                $(DE.modelDetailModal).modal('hide');
        },
        resetSpecifyContent: function () {
            $(DE.rowCount).val(1);
            $(DE.colCount).val(1);
            $(DE.totalCount).val(1);
        },
        horizontalRuler: function (e) {
            d3.select("#" + DE.horizontalRuler).remove();
            let container = _$workArea.append("svg")
                    .attr("pointer-events", "none")
                    .attr("id", DE.horizontalRuler)
                    .attr("x", e.position.x)
                    .attr("y", e.position.y - 50)
                    .attr("height", 30)
                    .attr("width", e.width);
            container.append("rect")
                .attr("pointer-events", "none")
                    .attr("id", "h-ruler-bg")
                    .attr("x", 0)
                    .attr("y", 0)
                    .attr("height", 30)
                    .attr("width", e.width)
                    .style('fill', '#ffffff')
                    .style("fill-opacity", 1)
                    .style("stroke", "#000").style("stroke-width", "0.4");
            let rulerData = generate.rulerData(e.width, DE.gridCellSize / 5);
            let lebels = container.selectAll('.h-scaler').data(rulerData).enter();
            lebels.append("rect")
                .attr("class", "h-scaler")

                .attr("x", function (d, i) {
                    let h = d + 1;
                    if (i % 5 == 0) {
                        h = d;
                    }
                    return h;
                })
                .attr("y", function (d, i) {
                    let h = 20;
                    if (i % 5 == 0) {
                        h = 0;
                    }
                    return h;
                })
                .attr("height", function (d, i) {
                    let h = 10;
                    if (i % 5 == 0) {
                        h = 30;
                    }
                    return h;
                })
                .attr("width", 0.5)
                 .style('fill', '#000')
                 .style("fill-opacity", 1)
                 .style("stroke", "#000").style("stroke-width", "0.1");
            lebels.filter(function (d, i) {
                return i % 5 == 0;
            }).append("text")
                 .style("font-size", '9px')
                 .attr("x", function (d) { return d + 2; })
                 .attr("y", function (d) { return 15; })
             .text(function (d, i) { return generate.pixelToMM(i * DE.gridCellSize); })
        },
        resetHorizontalRuler: function () {
            d3.select("#" + DE.horizontalRuler).attr("x", 0)
                                 .attr("y", 0)
                                 .attr("height", 0)
                                 .attr("width", 0)
                                  .html('');
        },
        verticalRuler: function (e) {


            d3.select("#" + DE.verticalRuler).remove();
            let container = _$workArea.append("svg")
                    .attr("pointer-events", "none")
                    .attr("id", DE.verticalRuler)
                    .attr("x", e.position.x - 50)
                    .attr("y", e.position.y)
                    .attr("height", e.height)
                    .attr("width", 30);
            container.append("rect")
                .attr("pointer-events", "none")
                    .attr("id", "v-ruler-bg")
                    .attr("x", 0)
                    .attr("y", 0)
                    .attr("height", e.height)
                    .attr("width", 30)
                    .style('fill', '#ffffff')
                    .style("fill-opacity", 1)
                    .style("stroke", "#000")
                    .style("stroke-width", "0.4");
            let rulerData = generate.rulerData(e.height, DE.gridCellSize / 5);
            let lebels = container.selectAll('.v-scaler').data(rulerData).enter();
            lebels.append("rect")
                .attr("class", "v-scaler")

                .attr("y", function (d, i) {
                    let h = d + 1;
                    if (i % 5 == 0) {
                        h = d;
                    }
                    return h;

                })
                .attr("x", function (d, i) {
                    let h = 20;
                    if (i % 5 == 0) {
                        h = 0;
                    }
                    return h;
                })
                .attr("width", function (d, i) {
                    let h = 10;
                    if (i % 5 == 0) {
                        h = 30;
                    }
                    return h;
                })
                .attr("height", 0.5)
                .style('fill', '#000')
                .style("fill-opacity", 1)
                .style("stroke", "#000").style("stroke-width", "0.1");;
            lebels.filter(function (d, i) {
                return i % 5 == 0;
            }).append("text")
                .style("font-size", '9px')
                .attr("x", 2)
                .attr("y", function (d) { return d - 2; })
             .text(function (d, i) { return generate.pixelToMM(i * DE.gridCellSize); })

        },
        resetVerticalRuler: function () {
            d3.select("#" + DE.verticalRuler).attr("x", 0)
                                 .attr("y", 0)
                                 .attr("height", 0)
                                 .attr("width", 0)
                                  .html('');
        },
        ruler: function (e) {
            render.verticalRuler(e);
            render.horizontalRuler(e);
        },
        resetRuler: function (e) {
            render.resetVerticalRuler();
            render.resetHorizontalRuler();
        },
        resetModelTab: function (e) {
            $(DE.anchorModelDetails).addClass('active').addClass('show');
            $(DE.anchorSpecification).removeClass('active').removeClass('show');
            $(DE.tabModelDetails).addClass('active').addClass('show');
            $(DE.tabSpecification).removeClass('active').removeClass('show');
        },
        setBtnSaveText: function (text) {
            $(DE.btnSaveModel).text(text);
            $(DE.openSaveModel).val(text + " Model");
        },
        setWorkSpaceSize: function (height, width) {
            let $grid = $(DE.workAreaContext);
            let minHeight = $grid.height();
            let minWidth = $grid.width();
            if (height < minHeight) {
                height = minHeight;
            }
            if (width < minWidth) {
                width = minWidth;
            }
            _$workArea.style('height', height).style('width', width);
        },
        libListByValidation: function (e) {
            let radioValue = $(DE.libFilterOptions + ":checked").val();
            if (radioValue) {
                let options = _libraryAllData.filter(x=> x.ModelId == radioValue);

                let searchText = $(DE.txtSearchLibraryData).val().toLowerCase();
                let libTypes = $(DE.selectLibTypes + " option:selected").val();
                let arr = [];
                if (options.length) {
                    options[0].Types = options[0].Types.filter(x=> x.id != 0);
                    for (var i = 0; i <= options[0].Types.length - 1; i++) {
                        if ((options[0].Types[i].model_name.toLowerCase().includes(searchText) && (!libTypes || options[0].Types[i].model_type_id == libTypes))
                            && (!options[0].Types[i].model_type_master_name || options[0].Types[i].model_type_master_name == $(DE.selectLibTypes + " option:selected").text())) {
                            let param = {
                                parent_model_id: _selectedModel.model_id,
                                parent_model_type_id: _selectedModel.model_type_id || null,
                                child_model_id: options[0].Types[i].model_id,
                                child_model_type_id: isNaN(options[0].Types[i].model_type_id) ? null : options[0].Types[i].model_type_id
                            }
                            if (validate.modelDrop(param)) {
                                //dimension check
                                if ((_selectedModel.height - _selectedModel.border_width * 2) >= generate.mmToPixel(options[0].Types[i].height)
                                    && (_selectedModel.width - _selectedModel.border_width * 2) >= generate.mmToPixel(options[0].Types[i].width))
                                { arr.push(options[0].Types[i]); }
                            }

                        }

                    }
                }
                $(DE.lblmsg).html('');
                if (arr.length == 0) {
                    $(DE.lblmsg).html('No matches found!');
                }
                generate.libraryList(arr);
                render.library();
            }
        },
        subContextMenu: function (d, coordinates, isMultiSelect) {
            let rect = _workSpaceActions.getAbsoluteRect(d.id);
            render.showSubMenuContext(true);
            let $menuContext = $(DE.subMenuContext);
            let $grid = $(DE.workAreaContext);
            let x = coordinates[0] - 25 - $grid.scrollLeft();
            let y = coordinates[1] + 50 - $grid.scrollTop();
            if (rect) {
                x = x + rect.x;
                y = y + rect.y;
            }
            $menuContext.css('left', x + 'px').css('top', y + 'px');
            if (isMultiSelect) {
                $(DE.copySubMenuContext).hide();
            }
            else {
                $(DE.copySubMenuContext).show();
            }
        },
        libTypeOption: function () {
            let bgColor = $(DE.selectLibTypes + " option:selected").data('colorCode');
            if (bgColor) {
                let colorCode = invertColor(bgColor);
                $(DE.selectLibTypes).css('background-color', bgColor).css('color', colorCode);
                $(DE.selectLibTypes + " option").css('background-color', '#FFFFFF').css('color', '#000000');
                $(DE.selectLibTypes + " option:selected").css('background-color', bgColor).css('color', colorCode);
            }
        }
    };

    var validate = {
        initCreate: function () {
            let chk = false;
            let $selectModel = $(DE.selectModel);
            let $selectModelType = $(DE.selectModelType);
            let $optionsModelType = $(DE.selectModelType + " option");
            if ($selectModel.val() == "") {
                alert("Please select model.");
                return true;
            }
            if ($selectModelType.val() == '0') { return true; }
            if ($optionsModelType.length > 1 && !$selectModelType.val()) {
                alert("Please select model type.");
                return true;
            }
            return chk;
        },
        saveModel: function () {
            let isError = false;
            let $name = $(DE.txtModelName);
            let $status = $(DE.selectStatus);
            let $specification = $(DE.selectSpecification);
            let $vendor = $(DE.selectVender);
            let modelId = $(DE.selectModel).val();
            //validate name and status
            render.errorFocus($name, false);
            render.errorFocus($status, false);
            render.errorFocus($specification, false);
            render.errorFocus($vendor, false);
            if (!$name.val().trim()) {
                render.errorFocus($name, true);
                isError = true;
            }
            if (!$status.val().trim()) {
                render.errorFocus($status, true);
                isError = true;
            }
            if (modelId == DE.equipmentKey) {

                render.activeSpecificationTab(!isError);

                if (!$specification.val()) {
                    render.errorFocus($specification, true);
                    isError = true;
                }
                if (!$vendor.val()) {
                    render.errorFocus($vendor, true);
                    isError = true;
                }

            }
            return isError;

        },
        whereDropped: function (d) {
            let id = 0;
            if (d.parent && d.parent != -1) {
                let prevParent = d.parent;
                let rect = _workSpaceActions.getAbsoluteRect(d.parent);
                let parent = generate.boundingRect(d.id, { x: d3.event.x + rect.x, y: d3.event.y + rect.y });

                if (parent != d.parent && parent != -1 && parent != prevParent) {
                    let parentData = _workAreaData[_workSpaceActions.getIndex(parent)];
                    let param = {
                        parent_model_id: parentData.model_id,
                        parent_model_type_id: parentData.model_type_id || null,
                        child_model_id: d.model_id,
                        child_model_type_id: isNaN(d.model_type_id) ? null : d.model_type_id
                    }
                    if (validate.modelDrop(param)) {
                        if ((parentData.height - parentData.border_width * 2) >= d.height
                                    && (parentData.width - parentData.border_width * 2) >= d.width) {
                            d.parent = parent;
                            rect = _workSpaceActions.getAbsoluteRect(d.parent);
                            d.position.x = (rect.width / 2) - (d.width / 2);
                            d.position.y = (rect.height / 2) - (d.height / 2);
                            render.allModels();
                        }
                    }
                }

            }
            return id;
        },
        modelDrop: function (param) {
            let count = _hierarchyRules.filter(x=> x.parent_model_id == param.parent_model_id
                                    && x.parent_model_type_id == param.parent_model_type_id
                                    && x.child_model_id == param.child_model_id
                                    && x.child_model_type_id == param.child_model_type_id);
            return count.length;
        },
        inBoundary: function (id, point) {
            let flag = false;
            //let rect = _workSpaceActions.getAbsoluteRect(id);

            // _workAreaData = _workSpaceActions.getWorkArea();
            if (_workSpaceActions.contains(id, point)) {
                flag = true;
            }
            return flag;
        }
    };

    var bind = {
        libraryEvents: function () {
            _$libArea.selectAll(DE.btnLibAdd).on('click', action.onLibAddButton);

            let $libFilterContext = $(DE.libFilterContext);
            $libFilterContext.on('change', DE.libFilterOptions, action.onLibFilterOptionClick);

            let $specifyNoContext = $(DE.specifyNoContext);
            $specifyNoContext.on('change', DE.rowCount, action.onSpecifyChanges);
            $specifyNoContext.on('change', DE.colCount, action.onSpecifyChanges);

            let $modelBorderWidth = $(DE.modelBorderWidth);
            $modelBorderWidth.on('change', action.onBorderThicknessChange);

            let $dimensionContext = $(DE.dimensionContext);
            $dimensionContext.on('change', DE.dimensionElement, action.onDimensionChange);

        },
        gridColorEvents: function () {
            let $gridColorOptions = $(DE.gridColorOptions);
            $gridColorOptions.on('change', action.onChangeGridColor);
        },
        modelEvent: function () {
            let $resetWorkspace = $(DE.resetWorkspace);
            $resetWorkspace.on('click', action.onResetWorkspace);

            let $createModel = $(DE.createModel);
            $createModel.on('click', action.initCreate);

            let $selectModel = $(DE.selectModel);
            $selectModel.on('change', action.onChangeSelectModel);
            $selectModel.trigger("chosen:updated");

            let $selectModelType = $(DE.selectModelType);
            $selectModelType.on('change', action.onChangeSelectModelType);
            $selectModelType.trigger("chosen:updated");

            let $openSaveModel = $(DE.openSaveModel);
            $openSaveModel.on('click', action.onOpenSaveModel);

            let $saveModel = $(DE.btnSaveModel);
            $saveModel.on('click', action.onSaveModelClick);

            let $btnSaveModelType = $(DE.btnSaveModelType);
            $btnSaveModelType.on('click', action.onSaveModleType);

            let $txtSearchLibraryData = $(DE.txtSearchLibraryData);
            $txtSearchLibraryData.on('keyup', action.onSearchLibraryData);

            let $selectLibTypes = $(DE.selectLibTypes);
            $selectLibTypes.on('change', action.onChangeSelectLibTypes);

            //let $specificationContext = $(DE.specificationContext).unbind();
            //$specificationContext.on('change', $(DE.selectSpecification), action.onSpecificationChange);

            //let $selectSpecification = $(DE.selectSpecification).unbind();
            //$selectSpecification.on('change', action.onSpecificationChange);

            //let $selectVender = $(DE.selectVender).unbind();
            //$selectVender.on('change', action.onSubSpecificationChange);
        },
        subMenuContext: function () {
            let $cancelSubMenuContext = $(DE.cancelSubMenuContext);
            $cancelSubMenuContext.on('click', action.onCancelSubMenuContext);

            let $deleteSubMenuContext = $(DE.deleteSubMenuContext);
            $deleteSubMenuContext.on('click', action.onDeleteSubMenuContext);

            let $copySubMenuContext = $(DE.copySubMenuContext);
            $copySubMenuContext.on('click', action.onCopySubMenuContext);

            let $rotateSubMenuContext = $(DE.rotateSubMenuContext);
            $rotateSubMenuContext.on('click', action.onRotateSubMenuContext);

        }
    };

    var load = {
        leftPanel: function () {

            render.showLeftPanel(false);

        },
        workArea: function () {
            _$workArea = d3.select(DE.svgWorkArea).on("contextmenu", action.onWorkspaceContextMenu);
            _$libArea = d3.select(DE.svgLibArea).on("contextmenu", action.onWorkspaceContextMenu);
            _workAreaData = _workSpaceActions.getWorkArea();
            if (DE.element_id && DE.element_id != '' && DE.element_id != '0') {

                API.getWorkspaceData({ modelId: DE.element_id }, render.modelLoad);
            }
        },
        library: function () {
            API.call(urls.getModelAllRules, {}, function (res) { _hierarchyRules = res; });
        }

    };


    var setup = function () {

        //Load Data
        for (let e in load) {
            if (typeof load[e] == 'function')
                load[e]();
        }
        //Bind events
        for (let e in bind) {
            if (typeof bind[e] == 'function')
                bind[e]();
        }
    };
    var init = function (configs) {
        $.extend(true, DE, configs);
        setup();
    };
    return {
        init: init
    };




})();