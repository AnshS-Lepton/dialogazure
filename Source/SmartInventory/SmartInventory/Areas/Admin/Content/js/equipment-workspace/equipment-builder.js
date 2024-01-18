var BuilderAPIURLS = {
    getPortImages: 'Equipment/GetPortImages',
    saveModel: 'Equipment/SaveModel',
    getModelType: 'Equipment/GetModelType',
    getWorkspaceData: 'Equipment/GetWorkspaceData',
    getLibraryData: 'Equipment/GetLibraryData',
    saveModelType: '/admin/Equipment/SaveModelType',
    getModelAllRules: 'Equipment/GetModelAllRules',
    getVendorList: '../ItemTemplate/GetVendorList',
    getSubVendorList: '../ItemTemplate/GetCatSubcatData',
    getEquipmentSpecificaitons: 'Equipment/GetEquipmentSpecificaitons',
    getModelMasterData: 'Equipment/getModelMasterData'
};

function invertColor(color) {
    color = color.substring(1);
    return '#' + (Number(`0x1${color}`) ^ 0xFFFFFF).toString(16).substr(1).toUpperCase();
}

var equipmentBuilder = (function () {
    'use strict';
    var DE = {};
    var _$workArea, _$libArea;
    var _workSpaceActions = workspace.action;
    var _workAreaData;
    var _libraryData = [];
    var _libraryAllData = [];
    var _nodeTree = [];
    var _selectedModel = {};
    var _hierarchyRules = [];
    var _selectMany = [];
    var _labelBGImage;
    var _isLabelBGRemoved = false;
    var _lastAddLibClicked = 0;
    var _modelMasterData;
    var _keyPressed = {};
    var _isMinDimension = false;

    var action = {
        addModel: function (e) {
            render.enableDimension(false);
            _workAreaData = _workSpaceActions.getWorkArea();
            let current = _workSpaceActions.add();

            current.id = _workSpaceActions.getMaxId() + 1;
            current.position = { x: 0, y: 0 };

            current.height = DE.defaultHeight;
            current.width = DE.defaultWidth;

            current.color = DE.color_code;
            current.stroke = DE.stroke;
            current.isNewEquipment = action.isNewEquipment();
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
                current.db_height = e.height;
                current.db_width = e.width;
                current.db_depth = e.depth;
                current.unit = generate.mmToUnit(current.height);
                current.rotation_angle = e.rotation_angle;
                current.border_color = e.border_color;
            }
            if (e && e.model_id) {
                let defaultSizes = action.getModelDefaults({ id: e.model_id });
                current.model_id = e.model_id;
                current.model_type_id = e.model_type_id;
                current.color = e.color || defaultSizes.color_code;
                current.stroke = e.stroke || defaultSizes.stroke;
                current.font_color = e.font_color || defaultSizes.font_color;
                current.model_color_id = e.model_color_id || defaultSizes.model_color_id;
                if (_workAreaData.length <= 1) {
                    current.height = defaultSizes.height;
                    current.width = defaultSizes.width;                   
                    current.border_width = defaultSizes.border_width;
                    $(DE.modelBorderWidth).val(current.border_width);
                }
            }
            if (_workAreaData.length <= 1) {
                current.is_static = true;//_workAreaData.length == 1;
                render.setInTop(current);
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
                            //current.position.x += _selectedModel.border_width;
                            //current.position.y += _selectedModel.border_width;
                        }
                    }

                    if (!e.isBulk)
                        render.setInCenter(current, _workSpaceActions.getAbsoluteRect(current.parent));

                }
            }


            return current;

        },
        onStartModelDrag: function (d) {
            if (d.is_editable) {
                render.showSubMenuContext(false);
                if (d3.event.sourceEvent.shiftKey && d.id > 1) {
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

                }
                //If Model is static then drag parent model if exist
                render.resetResizeControl();
                if (_libraryAllData.length && model.id == 1) {
                    //render.resetRuler();
                }
            }
        },
        onModelDrag: function (d) {
            if (d.is_editable) {
                let model = d;
                model = _workSpaceActions.getDraggable(model.id);

                if (!model.is_static) {
                    let position = { x: 0, y: 0 };
                    let rectModel = _workSpaceActions.getAbsoluteRect(model.id);

                    position.x = d3.event.x + model.offset_x + (model.id != d.id ? rectModel.x : 0);
                    position.y = d3.event.y + model.offset_y + (model.id != d.id ? rectModel.y : 0);
                    action.modelMove(model, position);


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
            if (action.isNewEquipment() && d.model_id == DE.portKey) { return false; }
            let coordinates = [];
            let defaultSizes = action.getModelDefaults({ id: d.model_id });
            if (defaultSizes.is_rotation_enabled)
            { $(DE.rotateSubMenuContext).show(); }
            else { $(DE.rotateSubMenuContext).hide(); }

            if (d.model_id == DE.labelKey)
            { $(DE.labelFormatSubMenuContext).show(); }
            else { $(DE.labelFormatSubMenuContext).hide(); }

            coordinates = [d3.event.x, d3.event.y];
            if (d.id == 1 && d.is_editable && d.model_id == DE.portKey) {
                render.subContextMenu(d, coordinates);
                action.selectModel(_workSpaceActions.select(d.id));
                render.modelFocus(d.id);
                $(DE.renameSubMenuContext).hide();
                $(DE.copySubMenuContext).hide();
                $(DE.deleteSubMenuContext).hide();
                $(DE.rotateSubMenuContext).show();
            }

            else
                if (d.id > 1 && d.is_editable) {

                    _selectMany = _workSpaceActions.selectMany();
                    let isMultiSelect = _selectMany.length > 1;
                    render.subContextMenu(d, coordinates);
                    if (_selectMany.includes(d.id) && _selectMany.length > 1) {
                        // _selectedModel = {};
                        action.selectModel({});
                    }
                    else {
                        action.resetMultiSelection();
                        action.selectModel(_workSpaceActions.select(d.id));
                        render.modelFocus(d.id);
                    }
                    $(DE.deleteSubMenuContext).show();
                    if (isMultiSelect) {
                        $(DE.renameSubMenuContext).hide();
                        $(DE.copySubMenuContext).hide();
                        $(DE.rotateSubMenuContext).hide();
                    }
                    else {
                        $(DE.renameSubMenuContext).show();
                        $(DE.copySubMenuContext).show();
                        //$(DE.rotateSubMenuContext).show();
                    }
                }
            d3.event.preventDefault();
            d3.event.stopPropagation();
        },
        onLibAddButton: function () {
             
            let timeNow = (new Date()).getTime();
            if (timeNow > (_lastAddLibClicked + 2000)) {
                // Execute the link action
                let $current = d3.select(this);
                setTimeout(function () {
                    action.libAddButton($current)
                }, 100);
            }
            else {
                alert('Multiple clicks detected. Please click one time to add model!');
            }

            _lastAddLibClicked = timeNow;

        },
        libAddButton: function ($current) {
            if (validate.isOverlappedModels()) {
                alert("Models should not be overlapped each other.Move recently added model to free space!", "Warning");
                return false;
            }
            //let $current = d3.select(this);
            let id = parseInt($current.attr(DE.libraryId));
            let img_id = parseInt($current.attr(DE.libraryImgId)) || '';
            let content = $current.attr("data-image-data");//$current.html().trim();
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
            let rotation_angle = $current.attr("data-rotation-angle");
            let border_color = $current.attr("data-border-color");
            if (row > 1 || col > 1) {
                let p_border = _selectedModel.border_width || 0;
                let defaultSizes = action.getModelDefaults({ id: _selectedModel.model_id });
                let margin = defaultSizes.margin || 0;
                let startPos = { x: p_border + margin, y: p_border + margin };
                let positions = generate.bulkModelPos(row, col, width, height, startPos);
                for (let i = 0 ; i < positions.length; i++) {

                    let param = generate.inBoundaryPos(_selectedModel.id, { height: height, width: width }, { x: positions[i].x + p_border + margin, y: positions[i].y + p_border + margin });
                    if (validate.inBoundary(_selectedModel.id, param)) {
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
                            name: name,
                            rotation_angle: rotation_angle,
                            border_color: border_color                     
                        });
                    }
                }
            }
            else {
                //Check here compatiblity
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
                    name: name,
                    rotation_angle: rotation_angle,
                    border_color: border_color
                });
            }

            _workAreaData = _workSpaceActions.getWorkArea();
            render.allModels();
            API.getWorkspaceData({ modelId: id,isLibCall:true }, render.libChildren);

        },
        onPortAddButton: function () {
            let $current = d3.select(this);
            let $selectModelType = $(DE.selectModelType);
            let id = -1; //parseInt($current.attr(DE.libraryId));
            let img_id = parseInt($current.attr(DE.libraryImgId));
            let content = $current.attr("data-image-data");//$current.html().trim();
            let model_id = $(selectModel).val();
            let model_type_id = $(selectModelType).val();
            let color = $selectModelType.find('option:selected').attr("data-color-code");
            let stroke = $selectModelType.find('option:selected').attr("data-stroke-code");
            let rotation_angle = $current.attr("data-rotation-angle");
            _workAreaData = _workSpaceActions.getWorkArea();
            if (_workAreaData.length >= 1) {

                id = _workAreaData[0].db__id;
                _workAreaData[0].image_data = content;
                _workAreaData[0].img_id = img_id;
                //_workAreaData[0].rotation_angle = rotation_angle;
            }
            else {
                action.addModel({
                    id: id, img_id: img_id,
                    content: content,
                    model_id: model_id,
                    model_type_id: model_type_id,
                    color: color,
                    stroke: stroke,
                    rotation_angle: rotation_angle
                });
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
            _selectMany = [];

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
                modelDepth: '0',
                no_of_port: '0',
                isMultiConnection: false,
                modelUnit: 0
            });
            render.setDimension({
                modelHeight: 0,
                modelWidth: 0,
                modelDepth: 0,
                modelUnit: 0
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
            let url = window.location.href.split("?")[0].replace('#', '');
            window.location = url;
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

            showConfirm('Are you sure you want to reset your workspace? You will loose your progress!', action.reload);
        },
        onModelClick: function (d) {
            //Draw outer boundery add circle in corner
            render.resizeControls(d);
        },
        onModeldblClick: function (d) {
            if (action.isNewEquipment() && d.model_id == DE.portKey) { return false; }
            //render.resetRuler();
            action.resetMultiSelection();
            //Draw outer boundery add circle in corner

            //if (!d.is_static)
            {
                if (d.parent == -1 || d.model_id == DE.labelKey) {
                    //let model = Object.assign({}, d);
                    //let rect = _workSpaceActions.getAbsoluteRect(d.id);
                    //model.position.x = rect.x;
                    //model.position.y = rect.y;
                    if (d.model_id == DE.labelKey && d.rotation_angle != 0) {
                        alert("Label can not be resized after rotation!", "Warning");
                        return false;
                    }

                    if (!action.isNewEquipment()) { render.resizeControls(d); }

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
                let limit = _workSpaceActions.getSizeLimitByChildren(d.id, true);
                let defaultSizes = action.getModelDefaults({ id: d.model_id });
                let margin = defaultSizes.margin;
                let newWidth, newHeight;
                limit.x.max = (limit.x.max + d.border_width + margin);
                limit.y.max = (limit.y.max + d.border_width + margin);
                let rect = _workSpaceActions.getAbsoluteRect(d.id);
                let sign = 1;
                let shiftlPos = { x: 0, y: 0 };
                let shiftLabelPos = { x: 0, y: 0 };
                if (isTop) {
                    newWidth = Math.max(d.width + rect.x - d3.event.x, DE.resizeWidthLimit);
                    if (limit.x.max > newWidth)
                        newWidth = (limit.x.max);


                    newHeight = Math.max(d.height + rect.y - d3.event.y, DE.resizeHeightLimit);
                    if (limit.y.max > newHeight)
                        newHeight = (limit.y.max);

                    sign = 1;
                } else {

                    newWidth = Math.max(d3.event.x - rect.x, DE.resizeWidthLimit);
                    newHeight = Math.max(d3.event.y - rect.y, DE.resizeHeightLimit);
                    if (limit.x.max > newWidth)
                        newWidth = (limit.x.max);
                    if (limit.y.max > newHeight)
                        newHeight = (limit.y.max);

                    sign = -1;
                }
                if (d.model_id == DE.labelKey) {
                    shiftLabelPos.x += sign * (d.width - newWidth) / 2;
                    shiftLabelPos.y += sign * (d.height - newHeight) / 2;
                }
                let dim = validate.modelDimension({ height: newHeight + margin, width: newWidth + margin, depth: d.depth });


                if (validate.childResize(d, dim)) {
                    if (d.model_id == DE.labelKey) {
                        d.position.x += shiftLabelPos.x;
                        d.position.y += shiftLabelPos.y;
                    }
                }


                if (d.parent == -1) {
                    render.setDimension({
                        modelHeight: d.height, modelWidth: d.width, modelDepth: d.depth
                    });
                    render.setWorkSpaceSize(Math.abs(generate.modelPositionY(_workAreaData[0])) + _workAreaData[0].height + 50, Math.abs(generate.modelPositionX(_workAreaData[0])) + _workAreaData[0].width + 50);
                    render.setInTop(_workAreaData[0]);
                }
                render.allModels();

                render.resizeControls(d);
            }
        },
        onRezsizeDragEnd: function (d, isTop) {
            if (d.is_editable) {
                if (_libraryAllData.length) {
                    render.ruler(_workAreaData[0]);
                    render.setWorkSpaceSize(Math.abs(_workAreaData[0].position.y) + _workAreaData[0].height + 50, Math.abs(_workAreaData[0].position.x) + _workAreaData[0].width + 50);
                    render.libListByValidation();
                    if (d.parent == -1) {
                        render.setInTop(_workAreaData[0]);
                        let modelGroupFound = d3.select("#" + DE.modelGroup + d.id);
                        modelGroupFound.attr("transform", function () {

                            return 'translate(' + generate.modelPositionX(d) + ',' + generate.modelPositionY(d) + ') rotate(' + d.rotation_angle + "," + d.width / 2 + "," + d.height / 2 + ")";
                        });
                    }
                }
            }
            if (_isMinDimension) {
                let defaultSize = action.getModelDefaults({ id: _selectedModel.model_id });
                _isMinDimension = false;
                alert('Minimum height is ' + defaultSize.min_height + ' & minimum width is ' + defaultSize.min_width);
            }
        },
        onSaveModelClick: function (e) {
            let errors = validate.saveModel();

             
            if (errors.isError) {
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
                    border_width: m.border_width,
                    isChangeMapping: true,
                    unit_size: m.modelUnit,
                    rotation_angle: m.rotation_angle,
                    is_multi_connection: m.isMultiConnection,
                    border_color: m.border_color                   
                };
                if (errors.isPortCountFailed) {
                    confirm("Number of port of equipment is not equal to item specification! Do you want to continue?", function () {
                         
                        console.log(param);
                        API.saveModel(param);
                    });
                }
                else { API.saveModel(param); }

            }
            else { alert("Workspace is empty! Please select model from library!", "Warning"); }
        },
        onChangeSelectModel: function (e) {
            let $selectModel = $(DE.selectModel);
            let modelId = $selectModel.val();
            render.showSpecificationTab((modelId == DE.equipmentKey));
            API.getModelType(modelId);
        },
        onChangeSelectModelType: function (e) {
            //DE.color_code = $(DE.selectModelType + " option:selected").data("colorCode");

            //let $modal = $(DE.model2);
            //if ($(DE.selectModelType + " option:selected").val() == '0') {
            //    $modal.modal('show');
            //}
            let model = $(DE.selectModel + " option:selected").text();
            if (model.toLowerCase() == 'equipment')
                action.onModelTypeChange();
        },
        onOpenSaveModel: function (e) {
            action.onModelTypeChange();
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
                    dataKey: [{ key: 'color', name: 'data-color-code' }, { key: 'stroke', name: 'data-stroke-code' }]
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
            action.deleteModel(e);
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
            action.dimensionChange();
            action.onChangeHeight();
        },
        resetMultiSelection: function (e) {
            action.selectModel({});
            _selectMany = [];
            _workSpaceActions.clear();
            _workSpaceActions.clearManySelect();
        },
        onRotateSubMenuContext: function (e) {
           
            _workAreaData = _workSpaceActions.getWorkArea();
            if (_workAreaData.length == 1 && _selectedModel && _selectedModel.id == 1 && _selectedModel.model_id == DE.portKey) {
                let rotation = parseInt(_selectedModel.rotation_angle);
                _selectedModel.rotation_angle = rotation + 90;
                if (_selectedModel.rotation_angle >= 360)
                    _selectedModel.rotation_angle = 0;
                render.allModels();
                render.modelFocus();
                render.showSubMenuContext(false);
                return;
            }
            if (!_selectedModel.is_static) {


                if (_selectedModel && _selectedModel.id > 1) {
                    let rotation = parseInt(_selectedModel.rotation_angle);
                    _selectedModel.rotation_angle = rotation + 90;
                    if (_selectedModel.rotation_angle >= 360)
                        _selectedModel.rotation_angle = 0;


                    let param = generate.inBoundaryPos(_selectedModel.parent, _selectedModel, _selectedModel.position);

                    //let collisionParam = { x: position.x , y: position.y  };
                    if (validate.inBoundary(_selectedModel.parent, param) && !validate.collision(_selectedModel.id, _selectedModel.position)) {

                        render.allModels();
                    }
                    else {
                        _selectedModel.rotation_angle = rotation;
                        alert("Model can not be rotate here! Model boundary is colliding with other boundary!", "Warning");
                    }

                }
            }
            action.resetMultiSelection();
            render.modelFocus();
            render.showSubMenuContext(false);
        },
        selectModel: function (d) {
            _selectedModel = d;
            render.libListByValidation();
        },
        deleteModel: function (e) {
            _selectMany = _workSpaceActions.selectMany();
            if (_selectMany.length) {
                showConfirm('Are you sure you want to delete this?', function () {
                    for (let i = 0 ; i < _selectMany.length; i++) {
                        if (_selectMany[i] > 1)
                            _workSpaceActions.remove(_selectMany[i]);
                    }
                    action.resetMultiSelection();
                    render.allModels();
                });

            }
            else if (!_selectedModel.is_static) {
                if (_selectedModel && _selectedModel.id > 1) {
                    showConfirm('Are you sure you want to delete this?', function () {
                        _workSpaceActions.remove(_selectedModel.id);
                        action.selectModel({});
                        render.allModels();
                    });
                }
            }
            if (_selectedModel.is_static) {
                alert("You can not delete fixed item!", "Warning");
            }
            if (_selectedModel && _selectedModel.id == 1) {
                alert("You can not delete super parent model!", "Warning");
            }
        },
        onRenameSubMenuContext: function (e) {
            render.showSubMenuContext(false);
            let $txtRename = $(DE.txtRenameModel);
            $txtRename.val(_selectedModel.name);
            render.modelFocus();
            let coordinates = [0, 0];
            _selectMany = _workSpaceActions.selectMany();
            render.renamePopUp(_selectedModel, coordinates, _selectMany.length > 1);
            //d3.event.preventDefault();
            //d3.event.stopPropagation();
            $txtRename.focus();
        },
        onChangeModelName: function (e) {
            var key = e.which;
            if (key == 13)  // the enter key code
            {
                action.onRenameModelName();
            }
        },
        onCancelRenameContext: function () {
            render.showRenameContext(false);
        },
        onRenameModelName: function () {
            let name = $(DE.txtRenameModel).val();
            if (!name || name == '') {
                alert("Model name is required", 'Warning');
                return false;
            }
            render.showRenameContext(false);
            _selectedModel.name = name;
            render.allModels();
            alert("Name changed successfully!", 'Success', 'success');
        },
        onModelTypeChange: function () {
            let type = $(DE.selectModelType).find('option:selected').data("entity-name");
            action.getModelSpecification(type);
        },
        getModelSpecification: function (typeKey) {

            API.call(BuilderAPIURLS.getEquipmentSpecificaitons, { typeKey: typeKey }, render.specification);
        },
        onChangeModelUnit: function (e) {
            let $unit = $(DE.modelUnit);
            let unit = $unit.val();
            if (unit < 0) {
                unit = 1;
                $(DE.modelUnit).val(unit);
            }
            if (unit > 100) {
                unit = 999;
                $(DE.modelUnit).val(unit);
            }

            let $height = $(DE.modelHeight);
            $height.val(generate.unitToMM(unit));
            action.dimensionChange();
            $unit.val(generate.mmToUnit(generate.pixelToMM($height.val())));
            //Change length using unit val
        },
        dimensionChange: function () {
            let $height = $(DE.modelHeight);
            let $width = $(DE.modelWidth);
            let $depth = $(DE.modelDepth);

            let height = parseInt($height.val()) || 0;
            let width = parseInt($width.val()) || 0;
            let depth = parseInt($depth.val()) || 0;

            let dim = validate.modelDimension({ height: height, width: width, depth: depth });
            $height.val(dim.height);
            $width.val(dim.width);
            $depth.val(dim.depth);
            _workAreaData = _workSpaceActions.getWorkArea();
            if (_workAreaData.length > 0) {
                let limit = _workSpaceActions.getSizeLimitByChildren(_workAreaData[0].id, true);
                let defaultSizes = action.getModelDefaults({ id: _workAreaData[0].model_id });
                let margin = defaultSizes.margin;
                _workAreaData[0].height = generate.mmToPixel(dim.height);
                _workAreaData[0].width = generate.mmToPixel(dim.width);
                _workAreaData[0].depth = generate.mmToPixel(dim.depth);
                if (limit.x.max > _workAreaData[0].width) {
                    _workAreaData[0].width = limit.x.max + margin;
                    $width.val(generate.pixelToMM(_workAreaData[0].width));
                }
                if (limit.y.max > _workAreaData[0].height) {
                    _workAreaData[0].height = limit.y.max + margin;
                    $height.val(generate.pixelToMM(_workAreaData[0].height));
                }
                render.setWorkSpaceSize(Math.abs(_workAreaData[0].position.y) + _workAreaData[0].height + 50, Math.abs(_workAreaData[0].position.x) + _workAreaData[0].width + 50);
                render.setInTop(_workAreaData[0]);
                render.allModels();
            }
            if (dim.height != height || dim.width != width) {
                alert('Minimum height is ' + dim.height + ' & minimum width is ' + dim.width);
            }
        },
        onChangeHeight: function () {
            let $unit = $(DE.modelUnit);

            let $height = $(DE.modelHeight);
            $unit.val(generate.mmToUnit($height.val()));
        },
        onLabelFormatSubMenuContext: function (e) {
            render.showSubMenuContext(false);
            //render.modelFocus();
            let $modal = $(DE.labelFormatPopUp);
            $modal.modal('show');
            render.setLabelFormatContext(_selectedModel);
        },
        onSetLabelFormat: function (e) {
            let format = generate.labelFormatData();
            _selectedModel.font_size = format.font_size;
            _selectedModel.font_color = format.font_color;
            _selectedModel.color = format.color;
            _selectedModel.stroke = format.stroke;
            _selectedModel.text_orientation = format.text_orientation;
            _selectedModel.model_color_id = format.model_color_id;
            _selectedModel.font_weight = format.font_weight;

            if (_labelBGImage)
                _selectedModel.bg_image = _labelBGImage;
            if (_isLabelBGRemoved)
                _selectedModel.bg_image = null;
            _labelBGImage = undefined;
            render.allModels();
            let $modal = $(DE.labelFormatPopUp);
            $modal.modal('hide');
            _isLabelBGRemoved = false;
        },
        onReadLabelImage: function () {

            if (this.files && this.files[0]) {

                var FR = new FileReader();

                FR.addEventListener("load", function (e) {
                    //_selectedModel.bg_image = e.target.result;
                    _labelBGImage = e.target.result;
                    $(DE.imgLBGImage).attr('src', _labelBGImage);
                    render.showLabelImageContext(true);
                });

                FR.readAsDataURL(this.files[0]);
            }

        },
        onDeleteLabelBG: function () {
            _isLabelBGRemoved = true;
            $(DE.imgLBGImage).attr('src', '');
            render.showLabelImageContext(false);
            $(DE.fileBGImage).val('');
        },
        onLabelColorChange: function (e) {
            render.modelColorSelection($(this));
        },
        initModelMaster: function (res) {
            if (res && res.length) {
                _modelMasterData = res;
                let len = DE.defaultSizes.length;

                for (let i = 0; i < len; i++) {
                    let modelMaster = res.filter(x=>x.key == DE.defaultSizes[i].modelKey);
                    if (modelMaster.length > 0) {
                        DE[modelMaster[0].key + 'Key'] = modelMaster[0].id;
                        DE.defaultSizes[i].min_height = modelMaster[0].min_height;
                        DE.defaultSizes[i].min_width = modelMaster[0].min_width;
                        DE.defaultSizes[i].is_rotation_enabled = modelMaster[0].is_rotation_enabled;
                        DE.defaultSizes[i].model_id = modelMaster[0].id;
                        DE.defaultSizes[i].model_color_id = modelMaster[0].model_color_id;
                    }
                }
            }
        },
        getModelDefaults: function (modelData) {
            let id, key;
            let result = { margin: 1 };
            if (modelData.id) {
                id = modelData.id;
                result = DE.defaultSizes.filter(x=>x.model_id == id);

            }
            if (modelData.key) {
                key = modelData.key;
                result = DE.defaultSizes.filter(x=>x.key == key);
            }
            if (result && result.length)
                return result[0];
            return result;
        },
        onModelKeyPress: function () {
            _selectMany = _workSpaceActions.selectMany();
            if (_selectMany.length) {
                _workAreaData = _workSpaceActions.getWorkArea();
                for (let i = 0 ; i < _selectMany.length; i++) {
                    if (_selectMany[i] > 1) {
                        let index = _workSpaceActions.getIndex(_selectMany[i]);
                        if (index > -1)
                            action.selectedModelMove(_workAreaData[index]);
                    }
                }
            }
            else
                if (_selectedModel) {
                    action.selectedModelMove(_selectedModel);
                }
        },
        onDocumentKeyPress: function () {
            //let body = d3.select("document");
            //body.on("keydown", action.onModelKeyPress);
            d3.select('body')
                .on('keydown', function () {
                    _keyPressed[d3.event.keyCode] = true;
                    if (_selectedModel && (d3.event.keyCode === 37 || d3.event.keyCode === 38 || d3.event.keyCode === 39 || d3.event.keyCode === 40)) {
                        d3.event.preventDefault();
                        d3.event.stopPropagation();
                    }
                    //console.log(d3.event, d3.event.keyCode);
                })
                .on('keyup', function () {
                    _keyPressed[d3.event.keyCode] = false;
                    if (_selectedModel && (d3.event.keyCode === 37 || d3.event.keyCode === 38 || d3.event.keyCode === 39 || d3.event.keyCode === 40)) {
                        d3.event.preventDefault();
                        d3.event.stopPropagation();
                    }
                });
            d3.timer(action.onModelKeyPress);
        },
        modelMove: function (model, position) {
            let modelFound = d3.select("#" + DE.modelElementId + model.id);
            let modelGroupFound = d3.select("#" + DE.modelGroup + model.id);
            if ((model.parent == -1)) {
                if (position.x > 0 && position.y > 0) {
                    model.position.x = position.x;
                    model.position.y = position.y;

                    modelGroupFound.attr("transform", function () {

                        return 'translate(' + generate.modelPositionX(model) + ',' + generate.modelPositionY(model) + ') rotate(' + model.rotation_angle + "," + model.width / 2 + "," + model.height / 2 + ")";
                    });
                }
            }
            else {

                let param = generate.inBoundaryPos(model.parent, model, position);
                //Calculate param using rotation

                let collisionParam = { x: position.x, y: position.y };
                if (validate.inBoundary(model.parent, param) && !validate.collision(model.id, collisionParam)) {
                    model.position.x = position.x;
                    model.position.y = position.y;

                    modelGroupFound.attr("transform", 'translate(' + generate.modelPositionX(model) + ',' + generate.modelPositionY(model) + ') rotate(' + model.rotation_angle + "," + model.width / 2 + "," + model.height / 2 + ")");

                }

                let rect = _workSpaceActions.getAbsoluteRect(model.parent);
                render.tempModel({

                    position: { x: position.x, y: position.y },
                    height: model.height,
                    width: model.width,
                    content: modelFound.html(),
                    rotation_angle: model.rotation_angle,
                    id: model.id,
                    parentPosition: { x: rect.x, y: rect.y },
                    parent: model.parent
                });
            }
        },
        selectedModelMove: function (model) {
            if (model.is_editable) {

                model = _workSpaceActions.getDraggable(model.id);
                if (model && !model.is_static) {
                    let position = { x: 0, y: 0 };

                    let shift = generate.positionShift();
                    position.x = shift.x + model.position.x;
                    position.y = shift.y + model.position.y;
                    if (Math.abs(shift.x) > 0 || Math.abs(shift.y) > 0)
                    { action.modelMove(model, position); }
                    else { render.resetTempModel(); }
                }
            }
        },
        onChangeSelectBorderColor: function () {
            _workAreaData = _workSpaceActions.getWorkArea();
            if (_workAreaData.length > 0) {
                _workAreaData[0].border_color = $(DE.borderColor).val();
                render.allModels();
            }
        },
        isNewEquipment: function () {
            return DE.isEquipmentEditorView;
        },
        getSelectModel: function () { return _selectedModel;}
    };

    var API = {
        call: function (url, param, callback) {
            ajaxReq(url, param, true, function (res) {
                if (callback && typeof callback == 'function')
                    callback(res);
            }, false, false, true);
        },
        getPortImages: function () {
            ajaxReq(BuilderAPIURLS.getPortImages, {}, true, function (res) {
                generate.portList(res);
                render.port();
            }, false, false, true);
        },
        saveModel: function (input) {
            ajaxReq(BuilderAPIURLS.saveModel, input, true, function (res) {
                //_workAreaData = _workSpaceActions.getWorkArea();
                //_workAreaData[0].db_id = res.id;

                if (res) {
                    if (res.page_message && res.page_message.status == "OK") {
                        render.modelDetailModal(false);
                        action.reset();
                        alert(res.page_message.message, 'Success', 'success');
                    }
                    else {
                        alert(res.page_message.message);
                        render.modelDetailModal(false);
                    }
                }
            }, true, false, true);
        },
        getModelType: function (modelId) {
            ajaxReq(BuilderAPIURLS.getModelType, { modelId: modelId }, true, function (res) {
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
                    dataKey: [{ key: 'color_code', name: 'data-color-code' }, { key: 'stroke_code', name: 'data-stroke-code' }, { key: 'key', name: 'data-entity-name' }],
                    disabled: !res.has_types
                })
                $(DE.selectModelType).val('').trigger("chosen:updated");
            }, false, false, true);
        },
        getWorkspaceData: function (param, callback) {
            ajaxReq(BuilderAPIURLS.getWorkspaceData, param, true, function (res) {
                if (callback && typeof callback == 'function')
                    callback(res);
            }, false, false, true);
        },
        getLibraryData: function (param) {
            ajaxReq(BuilderAPIURLS.getLibraryData, param, true, function (res) {

                if (res && res.length) {
                    render.showlibraryTitle(true);
                    render.showLibFilterContext(true);

                    render.libraryFilter(generate.libFilterList(res));
                    _libraryAllData = res;
                    if (_libraryAllData.length) {
                        render.ruler(_workAreaData[0]);
                    }
                    render.selectLibFilterFirst();
                }
            }, false, false, true);
        },
        saveModelType: function (ISPModelTypeMaster) {

            if (ISPModelTypeMaster.key == "") {
                alert('Enter Key!');
            }
            else if (ISPModelTypeMaster.value == "") {
                alert('Enter Value!');
            }
            else if (ISPModelTypeMaster.color_code == "") {
                alert('Enter Color Code!');
            }
            else if (ISPModelTypeMaster.stroke_code == "") {
                alert('Enter Stroke Code!');
            }
            else {
                $.ajax({
                    url: BuilderAPIURLS.saveModelType,
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
                    model_type_id: req[i].model_type_id,
                    border_color: req[i].border_color == undefined ? DE.defaultBorderColor : req[i].border_color
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
                    model_type_id: req[i].model_type_id,
                    rotation_angle: req[i].rotation_angle,
                    enabled: req[i].enabled == undefined ? true : req[i].enabled,
                    border_color: req[i].border_color == undefined ? DE.defaultBorderColor : req[i].border_color,
                });
                pos_x += xOffset + margin_x;

            }
        },

        modelDetails: function () {
            let dbId = 0;
            let rotation_angle = 0;
            _workAreaData = _workSpaceActions.getWorkArea();
            let itemId = $(DE.selectVender + " option:selected").data("itemId");
            if (_workAreaData && _workAreaData.length) {
                dbId = _workAreaData[0].db_id;
                rotation_angle = _workAreaData[0].rotation_angle;
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
                itemTemplateId: itemId,
                modelUnit: $(DE.modelUnit).val(),
                rotation_angle: rotation_angle,
                isMultiConnection: $(DE.isMultiConnection).is(':checked'),
                border_color: $(DE.borderColor).val()
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
                        model_name: children[j].name,
                        font_size: children[j].font_size,
                        font_color: children[j].font_color,
                        text_orientation: children[j].text_orientation,
                        background_color: children[j].color,
                        stroke_color: children[j].stroke,
                        height: children[j].height,
                        width: children[j].width,
                        background_image: children[j].bg_image,
                        model_color_id: children[j].model_color_id,
                        font_weight: children[j].font_weight,
                        isNewEquipment: children[j].isNewEquipment,
                        ref_id: children[j].ref_id
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
            let xStart = start && start.x || 0, yStart = start && start.y || 0;
            let x = xStart, y = yStart;
            for (let i = 0; i < row; i++) {
                x = xStart;
                for (let j = 0; j < col; j++) {
                    positions.push({
                        x: x + (width / 2),
                        y: y + (height / 2)
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
            res.push({
                id: 'all',
                name: 'All',
                color: '#ffffff',
                stroke: '#000000'
            });
            types.map(function (t) {
                if (t.model_type_id && res.filter(x=>x.id == t.model_type_id).length == 0) {
                    res.push({
                        id: t.model_type_id,
                        name: t.model_type_master_name,
                        color: t.color_code,
                        stroke: t.stroke_code
                    });
                }
                return {
                    id: t.model_type_id,
                    name: t.model_type_master_name,
                    color: t.color_code,
                    stroke: t.stroke_code
                };
            });
            return res;
        },
        modelRealPos: function (d, angle) {
            let center = { x: d.position.x + d.width / 2, y: d.position.y + d.height / 2 };

            return center;
        },
        inBoundaryPos: function (parentId, model, position) {

            let rect = _workSpaceActions.getAbsoluteRect(parentId);
            let parent = _workSpaceActions.select(parentId);
            let border_width = parent.border_width || 0;
            let defaultSizes = action.getModelDefaults({ id: parent.model_id });
            let margin = defaultSizes.margin || 0;

            if (border_width) {
                rect.width -= border_width;
                rect.height -= border_width;
            }
            if (margin) {
                rect.width -= margin;
                rect.height -= margin;
            }
            let param = {
                x: position.x + rect.x - (model.width / 2) + ((position.x > (rect.width / 2)) ? model.width + border_width + margin : -(border_width + margin)),
                y: position.y + rect.y - (model.height / 2) + ((position.y > (rect.height / 2)) ? model.height + border_width + margin : -(border_width + margin))
            };
            switch (model.rotation_angle) {
                case 90:
                case 270:
                    param = {
                        x: position.x + rect.x - (model.height / 2) + ((position.x > (rect.width / 2)) ? model.height + border_width + margin : -(border_width + margin)),
                        y: position.y + rect.y - (model.width / 2) + ((position.y > (rect.height / 2)) ? model.width + border_width + margin : -(border_width + margin))
                    };
                    break;
            }

            return param;
        },
        unitToMM: function (value) {
            return parseFloat((value * (DE.rackUnitValue)).toFixed(2));
        },
        mmToUnit: function (value) {
            return parseFloat((value / (DE.rackUnitValue)).toFixed(2));
        },
        noOfPorts: function () {
            let noOfPorts = 0;
            _workAreaData = _workSpaceActions.getWorkArea();
            let totalPorts = _workAreaData.filter(x=>x.model_id == DE.portKey);
            noOfPorts = totalPorts.length;
            return noOfPorts;
        },
        modelPositionX: function (d) {
            //return d.position.x ;
            return d.position.x - (d.width / 2);
        },
        modelPositionY: function (d) {
            //return d.position.y;
            return d.position.y - (d.height / 2);
        },
        renderOperation: function (id) {
            let index = _workSpaceActions.getIndex(id);
            if (index == -1)
                return '';

            return generate.renderOperation(_workAreaData[index].parent) + generate.transformText(_workAreaData[index]);
        },
        transformText: function (d) {
            return ' translate(' + (generate.modelPositionX(d)) + ',' + (generate.modelPositionY(d)) + ') rotate(' + d.rotation_angle + "," + d.width / 2 + "," + d.height / 2 + ")";
        },
        actualAngle: function (angle) {
            while (angle < 0) {
                d.rotation_angle += 360.0;
            }
            if (angle >= 360)
                angle = angle % 360;
        },
        labelFormatData: function () {
            let $selectLabelColor = $(DE.selectLabelColor + ' option:selected');
            //let $selectLabelFontColor = $(DE.selectLabelFontColor);
            let $txtLabelFontSize = $(DE.txtLabelFontSize);
            //let $selectLabelBGColor = $(DE.selectLabelBGColor);
            //let $selectLabelStrokeColor = $(DE.selectLabelStrokeColor);
            let $selectLabelOrientation = $(DE.selectLabelOrientation);
            let $fileBGImage = $(DE.fileBGImage);
            let $txtLabelFontWeight = $(DE.txtLabelFontWeight);

            let fontColor = $selectLabelColor.data('fontColor');
            let fontBGColor = $selectLabelColor.data('bgColor');
            let stroke = 'transparent';
            let modelColorId = $selectLabelColor.val();

            let imgFile = $fileBGImage.get(0).files[0];
            return {
                font_color: fontColor,
                font_size: $txtLabelFontSize.val(),
                color: fontBGColor,
                stroke: stroke,
                text_orientation: $selectLabelOrientation.val(),
                bg_image: $fileBGImage.val(),
                model_color_id: modelColorId,
                font_weight: $txtLabelFontWeight.val()
            }
        },
        positionShift: function () {
            let shiftX = 0, shiftY = 0;
            if (_keyPressed['37']) {
                shiftX = -1;
                shiftY = 0;
            }
            if (_keyPressed['38']) {
                shiftX = 0;
                shiftY = -1;
            }
            if (_keyPressed['39']) {
                shiftX = 1;
                shiftY = 0;
            }
            if (_keyPressed['40']) {
                shiftX = 0;
                shiftY = 1;
            }
            return { x: shiftX, y: shiftY };
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
                .attr("data-rotation-angle", function (d) { return 0; })
                .attr("data-border-color", function (d) { return d.border_color; })
                .attr("data-image-data", function (d) { return d.image_data; })
                .html(function (d) {
                    let content = d.image_data;
                    
                        content = content.replaceAll('[STATUS_COLOR]', DE.portColor);
                        content = content.replaceAll('[BORDER_COLOR]', d.border_color)
                   
                    return content;
                })
                .on('click', action.onPortAddButton).exit();
            libData.enter().append("g").append("rect")
                               .attr("x", function (d) { return d.position.x; })
                                .attr("pointer-events", "none")
                                .attr(DE.libraryId, function (d) {
                                    return d.id;
                                }).attr(DE.libraryImgId, function (d) {
                                    return d.img_id || d.id;
                                }).attr(DE.dataModelId, function (d) {
                                    return d.model_id;
                                }).attr(DE.dataModelTypeId, function (d) {
                                    return d.model_type_id;
                                })
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

                        //.attr("x", function (d) { return generate.modelPositionX(d); })
                        //.attr("y", function (d) { return generate.modelPositionY(d); })

                        .attr("height", function (d) {
                            let height = d.height;
                            //if (d.rotation_angle == 90 || d.rotation_angle == 270) {
                            //    height = d.height + d.width;
                            //}
                            return height;
                        })
                        .attr("width", function (d) {
                            let width = d.width;
                            //if (d.rotation_angle == 90 || d.rotation_angle == 270) {
                            //    width = d.height + d.width;
                            //}
                            return width;
                        })


                        //.call(d3.drag()
                        //.on("start", action.onStartModelDrag)
                        //.on("drag", action.onModelDrag)
                        //.on("end", action.onEndModelDrag))
                        //.on("contextmenu", action.onModelContextMenu)
                        .on("dblclick", action.onModeldblClick);
            let content = svgMain;
            //let content = svgMain.append("g")
            //       .attr("transform", function (d) {
            //           let x = d.width / 2;
            //           let y = d.height / 2;
            //           return "rotate(" + d.rotation_angle + "," + x + "," + y + ")";
            //       });
            //let content = group.append("svg").attr("height", function (d) { return d.height; })
            //            .attr("width", function (d) { return d.width; });

            ////Add background rectangle
            let background = content.append("g");
            background.append("g").append("rect")
                 .attr("id", function (d) { return DE.elementRect + d.id; })
                 //.attr("class", DE.elementRect)
                .attr("class", function (d) {
                    let cl = DE.elementRect;
                    if (d.model_id == DE.equipmentKey)
                        cl += ' equipment';
                    if (d.model_id == DE.portKey)
                        cl += ' port';
                    return cl;
                })
            .attr("x", function (d) {
                let x = 0;

                return x;
            })
            .attr("y", function (d) {
                let y = 0;

                return y;
            })
            .attr("height", function (d) { return d.height; })
            .attr("width", function (d) { return d.width; })
            .style("fill", function (d) {
                //let color = d.model_id == DE.chessisKey ? DE.chassisColor : d.color;
                let color = d.color;
                if (!color || color == null)
                { color = DE.color_code; }
                if (d.model_id == DE.portKey) {
                    color = DE.portColor;
                }
                if (d.model_id == DE.trayKey) {
                    color = DE.trayColor;
                }
                //if (d.model_id == DE.cardKey) {
                //    color = DE.cardHatch;
                //}
                return color;
            })
             .style("opacity", function (d) {
                 //let o = 1;
                 //if (d.model_id == DE.portKey) {
                 //    o = DE.portOpacity;
                 //}
                 //return o;
             })
            .style("stroke", function (d) {
                let stroke = d.stroke;
                if (!stroke || stroke == null)
                { stroke = DE.stroke; }
                //if (d.model_id == DE.chessisKey) {
                //    stroke = DE.chassisStroke;
                //}
                return stroke;
            })
            .style("stroke-width", function (d) { return '3'; }).append('title').text(function (d) {
                let name = d.name;
                if (d.id > 1) {
                    name = d.name + ' (' + d.db_height + ' X ' + d.db_width + ')';
                }
                return name;
            });
            background.filter(function (d) {
                return d.model_id == DE.cardKey;
            }).append('rect').attr("pointer-events", "none")
            .attr("height", function (d) { return d.height; })
            .attr("width", function (d) { return d.width; })
            .style("fill", function (d) {
                let color = DE.cardHatch;

                //if (d.model_id == DE.cardKey) {
                //    color = DE.cardHatch;
                //}
                return color;
            });
            ////Chassis inner container
            background.filter(function (d) {
                return d.model_id == DE.chessisKey;
            })
            .append("svg").attr("pointer-events", "none")
              .attr("x", function (d) { return d.border_width; })
              .attr("y", function (d) { return d.border_width; })
                .attr("height", function (d) { return d.height - (2 * d.border_width); })
              .attr("width", function (d) { return d.width - (2 * d.border_width); })
                .append("rect")
                //.attr("x", function (d) { return d.border_width; })
                //.attr("y", function (d) { return d.border_width; })

              .attr("x", function (d) { return 0; })
              .attr("y", function (d) { return 0; })
              .attr("height", function (d) { return d.height - (2 * d.border_width); })
              .attr("width", function (d) { return d.width - (2 * d.border_width); })
              .style("fill", function (d) {
                  return DE.chassisInnerColor;//d.color;
              })
              .style("stroke", function (d) {
                  let stroke = d.stroke;
                  if (!stroke || stroke == null)
                  { stroke = DE.chassisStroke; }
                  return stroke;
              })
              .style("stroke-width", function (d) { return '3'; }).append('title').text(function (d) { return d.name; });

            //Add label data
            let labelArea = content.append("g");
            labelArea.filter(function (d) {
                return d.model_id == DE.labelKey;
            }).append("image")
            .attr('href', function (d) { return d.bg_image; })
            .attr('height', function (d) { return d.height; })
            .attr('width', function (d) { return d.width; });

            labelArea.filter(function (d) {
                return d.model_id != DE.portKey && d.model_id != DE.trayKey;
            }).append("text")
                    .attr("pointer-events", "none")
                    .attr("x", function (d) { return d.width / 2; })
                    .attr("y", function (d) {
                        let y = d.height / 2;
                        if (d.model_id == DE.chessisKey) {
                            y = 16;
                        }
                        if (d.model_id == DE.labelKey) {
                            y += d.font_size / 4;
                        }
                        return y;
                    })
                    .style('fill', function (d) {
                        let color = '#000000';
                        if (d.model_id == DE.labelKey) {
                            color = d.font_color;
                        }
                        return color;
                    })
                    .style('font-size', function (d) {
                        let size = DE.defaultFontSize;
                        if (d.model_id == DE.labelKey) {
                            size = d.font_size;
                        }
                        return size + 'px';
                    })
                    .style('font-weight', function (d) {
                        let size = '';
                        if (d.model_id == DE.labelKey) {
                            size = d.font_weight;
                        }
                        return size;
                    })
                    .style("text-anchor", "middle")
                    .style('writing-mode', function (d) {
                        let mode = '';
                        if (d.text_orientation)
                            mode = d.text_orientation;
                        return mode;
                    })
                    .text(function (d) {
                        if (d.model_id == DE.labelKey || d.model_id == DE.chessisKey) {
                            return d.name;
                        }
                    });

            ////Add image svg data 
            let imgContainer = content.append("g");


            //Filter by image data and id; add viewBox
            imgContainer
                .filter(function (d) {
                    return !isNaN(d.img_id) && d.image_data && d.image_data != '';
                })
                .append("svg")
                .attr("pointer-events", "none")
                .attr("preserveAspectRatio", "xMinYMin meet")

                .attr("viewBox", function (d) { return "0 0 50 50"; })
                .attr("height", function (d) { return d.height * 0.9; })
                .attr("width", function (d) { return d.width * 0.9; })
                 .attr("x", function (d) { return d.width * 0.05; })
                .attr("y", function (d) { return d.height * 0.05; })
                .html(function (d) {
                    let content = d.image_data;
                    if (d.model_id == DE.portKey) {
                        content = content.replaceAll('[STATUS_COLOR]', DE.portColor);
                        content = content.replaceAll('[BORDER_COLOR]', (d.border_color == undefined ? DE.defaultBorderColor : d.border_color))
                    }
                    return content;
                });
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
                .append("g").attr("transform", function (d) {
                    let x = d.position.x + (d.width * 0.17);
                    if (d.model_id == DE.portKey) {
                        x = d.position.x + (d.width * 0.25);
                    }
                    let y = d.position.y + 5;
                    return "translate(" + x + "," + y + ")rotate(" + d.rotation_angle + "," + d.width / 4 + "," + d.height / 4 + ")";
                })
                 .append("svg")
                 .attr("pointer-events", "none")
                 .attr("class", function (d) { return DE.libAddButtonClass; })
                 .attr("x", function (d) {
                     //let x = d.position.x + (d.width * 0.17);
                     //if (d.model_id == DE.portKey) {
                     //    x = d.position.x + (d.width * 0.25);
                     //}
                     //return x;
                     return 0;
                 })
                 .attr("y", function (d) {
                     return 0;//d.position.y + 5;
                 })
                 .attr("height", function (d) { return d.height / 2; })
                 .attr("width", function (d) { return (d.width * 0.7); })
                 .html(function (d) {
                    let content = d.image_data;
                     if (d.model_id == DE.portKey) {
                         content = content.replaceAll('[STATUS_COLOR]', DE.portColor);
                         content = content.replaceAll('[BORDER_COLOR]', d.border_color)
                     }
                     return content;
                 });

            //Main library button
            libData.enter().append("g").append("rect")
                            .attr("x", function (d) { return d.position.x; })
                             .attr("pointer-events", function (d) {
                                 let show = "visible";
                                 if (d.enabled)
                                 { show = "visible"; }
                                 else { show = "none"; }
                                 return show;
                             })
                            .attr("y", function (d) { return d.position.y; })
                            .attr("height", function (d) { return d.height; })
                            .attr("width", function (d) { return d.width; })
                            .style("fill", function (d) {
                                let color = "transparent";
                                if (d.enabled)
                                { color = "transparent"; }
                                else { color = "#c1bdbd"; }
                                return color;
                            })
                            .style("fill-opacity", function (d) {
                                let o = 1;
                                if (d.enabled)
                                { o = 1; }
                                else { o = 0.7; }
                                return o;
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
                            .attr("data-rotation-angle", function (d) { return d.rotation_angle; })
                            .attr("data-border-color", function (d) { return d.border_color; })
                            .attr("data-image-data", function (d) {
                                return d.image_data;
                            })

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
                                res = DE.libChassisGrid;
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

            libMain.filter(function (d) {
                return d.model_id == DE.cardKey;
            }).append('rect')
                .attr("x", function (d) {
                    let x = 5;
                    return x;
                })
                .attr("y", function (d) {
                    let h = 5;

                    return h;
                })
            .attr("height", function (d) { return d.height / 2 - 20; })
            .attr("width", function (d) { return (d.width * 0.7) - 10; })
            .style("fill", function (d) {
                let color = DE.cardHatch;

                //if (d.model_id == DE.cardKey) {
                //    color = DE.cardHatch;
                //}
                return color;
            });
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
                    .text(function (d) {
                        return "" + parseInt(generate.pixelToMM(d.db_height)) + " X " + parseInt(generate.pixelToMM(d.db_width)); //+ " X " + parseInt(generate.pixelToMM(d.db_depth))
                    });


            libData.enter().append("g").append("text")
                            .attr("pointer-events", "none")
                            .attr("x", function (d) { return d.position.x + d.width / 2; })
                            .attr("y", function (d) { return d.position.y + d.height - 5; })
                            .style("text-anchor", "middle")
                            .html(function (d) {
                                let name = d.name;
                                if (d.name && d.name.length > 12) {
                                    name = d.name.substring(0, 9) + '...';
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
                $selectModel.val('Select Model');
                $selectModelType.val('');
            }
        },
        setInCenter: function (model, parent) {
            let rect = parent || _$workArea.node().getBoundingClientRect()
            //let pos_x = rect.width / 2 - model.width / 2, pos_y = rect.height / 2 - model.height / 2;
            let pos_x = rect.width / 2, pos_y = rect.height / 2;
            model.position.x = pos_x < 0 ? 0 : pos_x;
            model.position.y = pos_y < 0 ? 0 : pos_y;
        },
        setInTop: function (model, parent) {
            //let rect = parent || _$workArea.node().getBoundingClientRect()

            let pos_x = (model.width / 2) + 50, pos_y = (model.height / 2) + 50;
            model.position.x = pos_x;
            model.position.y = pos_y;
        },
        resizeControls: function (e) {
            let rect = _workSpaceActions.getAbsoluteRect(e.id);

            d3.select("#" + DE.resizeBoundry).remove();
            _$workArea.append("rect")
                .attr("pointer-events", "none")
                    .attr("id", DE.resizeBoundry)
                    .attr("x", rect.x)
                    .attr("y", rect.y)
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
                   .attr("cx", rect.x)
                   .attr("cy", rect.y)
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
                    .attr("cx", rect.x + e.width)
                    .attr("cy", rect.y + e.height)
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
            let modelGroupFound = d3.select("#" + DE.modelGroup + e.id);

            d3.select("#" + DE.tempModel).remove();

            _$workArea.append("svg").attr('pointer-events', 'none').attr("id", DE.tempModel)
                        //.attr("x", e.parentPosition.x)
                        //.attr("y", e.parentPosition.y)
                    .append("g")
                .attr("transform", function () {
                    //if (modelGroupFound.attr("transform"))
                    //    return modelGroupFound.attr("transform");
                    //return 'translate(' + (generate.modelPositionX(e) + e.parentPosition.x) + ',' + (generate.modelPositionY(e) + e.parentPosition.y) + ') rotate(' + e.rotation_angle + "," + e.width / 2 + "," + e.height / 2 + ")";

                    let operation = generate.renderOperation(e.parent) + generate.transformText(e);

                    return operation;
                })

                    .append("svg")
                    .attr("pointer-events", "none")
                        //.attr("id", DE.tempModel)
                        //.attr("x", generate.modelPositionX(e))
                        //.attr("y", generate.modelPositionY(e))
                        .attr("x", 0)
                        .attr("y", 0)
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
            $(DE.txtModelUnit).val(d.modelUnit);
            $(DE.isMultiConnection).prop('checked', d.isMultiConnection);
            //$(DE.no_of_port).val(d.no_of_port);
        },
        setModelSpecification: function (d) {
            $(DE.category).val(d.category);
            $(DE.itemCode).val(d.code);

            $(DE.subCategory_1).val(d.subCategory_1);
            $(DE.subCategory_2).val(d.subCategory_2);
            $(DE.subCategory_3).val(d.subCategory_3);
            $(DE.no_of_port).val(d.no_of_port);
        },
        setDimension: function (d) {
            let $height = $(DE.modelHeight);
            let $width = $(DE.modelWidth);
            let $depth = $(DE.modelDepth);
            let $unit = $(DE.modelUnit);
            $height.val(generate.pixelToMM(d.modelHeight));
            $width.val(generate.pixelToMM(d.modelWidth));
            $depth.val(generate.pixelToMM(d.modelDepth));
            if (d.modelUnit)
            { $unit.val(d.modelUnit); }
            else { $unit.val(generate.mmToUnit(generate.pixelToMM(d.modelHeight))); }
            if (d.is_editable != undefined) {
                $height.attr('disabled', !d.is_editable);
                $width.attr('disabled', !d.is_editable);
                $depth.attr('disabled', !d.is_editable);
                $unit.attr('disabled', !d.is_editable);
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
            render.showUnitContext(flag);
            render.showBorderColorContext(flag);
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
            render.showUnitContext($selectModel.val() == DE.equipmentKey);
            render.showMultiConnectionContext(false);
            render.modelText($selectModel.find('option:selected').text());
            if ($selectModel.val() == DE.portKey) {
                if ($isEditable && $isEditable.val().toLowerCase() == 'true') {
                    render.showMultiConnectionContext(true);
                    render.showlibraryTitle(true);
                    render.showLibContext(true);
                    render.showSearchLibraryData(false);
                    render.showBorderColorContext(true);
                    API.getPortImages();
                }
            }
            else {
                //render.showLibContext(true);
                let model_id = $selectModel.val();
                let model_type_id = $selectModelType.val();
                let color = $selectModelType.find('option:selected').attr("data-color-code");
                let stroke = $selectModelType.find('option:selected').attr("data-stroke-code");
                action.selectModel(action.addModel({
                    id: 0,
                    model_id: model_id,
                    model_type_id: model_type_id,
                    color: color,
                    stroke: stroke,
                }));

                _workAreaData = _workSpaceActions.getWorkArea();
                render.allModels();
                //render.modelFocus(_selectedModel.id);&& DE.slotKey != model_id Removed from below condition to allow the card in slot
                if ($isEditable && $isEditable.val().toLowerCase() == 'true' ) {
                    API.getLibraryData({
                        modelId: $selectModel.val(),
                        modelType: $selectModelType.val()
                    });
                }

            }
        },
        libraryFilter: function (d) {
            $(DE.libFilterBox).html('');
            let $libBox = d3.select(DE.libFilterBox);
             
            $libBox.selectAll(DE.filterOptionClass).remove();
            let libFilterData = $libBox.selectAll(DE.filterOptionClass).data(d);
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
                let parentData = root.selectAll('.' + DE.modelClass)
                                      .data(nodes)
                                      .enter();
                let parent = parentData.append("g").attr('id', function (d) {
                    return DE.modelGroup + d.id;
                }).attr("transform", function (d) {
                    let x = d.position.x;
                    let y = d.position.y;
                    return 'translate(' + generate.modelPositionX(d) + ',' + generate.modelPositionY(d) + ") rotate(" + d.rotation_angle + "," + d.width / 2 + "," + d.height / 2 + ")";
                }).call(d3.drag()
                        .on("start", action.onStartModelDrag)
                        .on("drag", action.onModelDrag)
                        .on("end", action.onEndModelDrag))
                        .on("contextmenu", action.onModelContextMenu);


                parent = parent.append("svg");
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

                action.selectModel(_workAreaData[0]);
                render.setInTop(_workAreaData[0]);
                render.allModels();
                render.setDimension({ modelHeight: _workAreaData[0].height, modelWidth: _workAreaData[0].width, modelDepth: _workAreaData[0].depth, is_editable: _workAreaData[0].is_editable });
                render.showSpecificationTab((_workAreaData[0].model_id == DE.equipmentKey));
                render.setBtnSaveText("Update");
                render.setWorkSpaceSize(Math.abs(generate.modelPositionY(_workAreaData[0])) + _workAreaData[0].height + 50, Math.abs(generate.modelPositionX(_workAreaData[0])) + _workAreaData[0].width + 50);
                //render.showLibFilterContext(!_workAreaData[0].is_editable);

            } else {

            }
        },
        libChildren: function (res) {

            if (res.length > 1) {
                _workAreaData = _workSpaceActions.getWorkArea();
                res.splice(0, 1);
                let selected = _workSpaceActions.getCurrent();
                //Remove Cyclic reference 
                let maxId = _workSpaceActions.getMaxId();

                res.map(function (obj) {
                    if (obj.id > maxId) maxId = obj.id;
                });
                maxId++;
                for (let i = 0; i < res.length; i++) {
                    let children = res.filter(x=>x.parent == res[i].id);
                    for (let j = 0; j < children.length; j++) {
                        children[j].parent = maxId;
                    }
                    res[i].id = maxId;
                    maxId++;
                }

                for (let i = 0; i < res.length; i++) {
                    if (0 == res[i].parent) {
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
                    .attr("x", generate.modelPositionX(e))
                    .attr("y", generate.modelPositionY(e) - 50)
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
            let labels = container.selectAll('.h-scaler').data(rulerData).enter();
            labels.append("rect")
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
            labels.filter(function (d, i) {
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
                    .attr("x", generate.modelPositionX(e) - 50)
                    .attr("y", generate.modelPositionY(e))
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
            let labels = container.selectAll('.v-scaler').data(rulerData).enter();
            labels.append("rect")
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
            labels.filter(function (d, i) {
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
                let check = false;
                if (options.length) {
                    options[0].Types = options[0].Types.filter(x=> x.id != 0);
                    for (var i = 0; i <= options[0].Types.length - 1; i++) {
                        options[0].Types[i].hidden = false;
                        check = false;
                        if (!check && (!searchText || options[0].Types[i].model_name.toLowerCase().includes(searchText))) {
                            check = true;
                        }
                        if ((check && (!libTypes || libTypes == 'all' || options[0].Types[i].model_type_id == libTypes))
                            && (libTypes == 'all' || !options[0].Types[i].model_type_master_name || options[0].Types[i].model_type_master_name == $(DE.selectLibTypes + " option:selected").text())) {
                            check = true;
                        }
                        else {
                            check = true;
                            options[0].Types[i].hidden = true;
                        }

                        let param = {
                            parent_model_id: _selectedModel.model_id,
                            parent_model_type_id: _selectedModel.model_type_id || null,
                            child_model_id: options[0].Types[i].model_id,
                            child_model_type_id: isNaN(options[0].Types[i].model_type_id) ? null : options[0].Types[i].model_type_id
                        }
                        if (check && validate.modelDrop(param)) {
                            check = true;
                        } else {
                            check = false;
                            options[0].Types[i].hidden = true;
                        }

                        let defaultSizes = action.getModelDefaults({ id: _selectedModel.model_id });
                        let margin = defaultSizes.margin;
                        //dimension check
                        //margin check
                        if (check && (_selectedModel.height - _selectedModel.border_width * 2 - margin * 2) >= generate.mmToPixel(options[0].Types[i].height)
                            && (_selectedModel.width - _selectedModel.border_width * 2 - margin * 2) >= generate.mmToPixel(options[0].Types[i].width)) {
                            check = true;
                            //arr.push(options[0].Types[i]);
                        }
                        else {
                            check = false;
                        }

                        if (searchText && check) {
                            options[0].Types[i].enabled = check;
                            if (!options[0].Types[i].hidden)
                                arr.push(options[0].Types[i]);
                        }
                        else if (!searchText) {
                            options[0].Types[i].enabled = check;
                            if (!options[0].Types[i].hidden)
                                arr.push(options[0].Types[i]);
                        }
                    }
                }
                $(DE.lblmsg).html('');
                if (arr.length == 0) {
                    $(DE.lblmsg).html('No record found!');
                }
                generate.libraryList(arr);
                render.library();
            }
        },
        subContextMenu: function (d, coordinates) {
            //let rect = _workSpaceActions.getAbsoluteRect(d.id);
            render.showSubMenuContext(true);
            let $menuContext = $(DE.subMenuContext);
            let $grid = $(DE.workAreaContext);
            //let x = coordinates[0] + 480 - $grid.scrollLeft();
            //let y = coordinates[1] + 115 - $grid.scrollTop();
            //if (rect) {
            //    x = x + rect.x;
            //    y = y + rect.y;
            //}

            let x = coordinates[0] - 25;//- $grid.scrollLeft();
            let y = coordinates[1] - 25;// - $grid.scrollTop();
            $menuContext.css('left', x + 'px').css('top', y + 'px');

        },
        libTypeOption: function () {
            let bgColor = $(DE.selectLibTypes + " option:selected").data('colorCode');
            if (bgColor) {
                let colorCode = invertColor(bgColor);
                $(DE.selectLibTypes).css('background-color', bgColor).css('color', colorCode);
                $(DE.selectLibTypes + " option").css('background-color', '#FFFFFF').css('color', '#000000');
                $(DE.selectLibTypes + " option:selected").css('background-color', bgColor).css('color', colorCode);
                $(DE.libContext).css('border-color', bgColor);
            }
        },
        renamePopUp: function (d, coordinates, isMultiSelect) {
            let rect = _workSpaceActions.getAbsoluteRect(d.id);
            render.showRenameContext(true);
            let $menuContext = $(DE.renameContext);
            let $grid = $(DE.workAreaContext);
            let x = coordinates[0] + 480 - $grid.scrollLeft();
            let y = coordinates[1] + 115 - $grid.scrollTop();
            if (rect) {
                x = x + rect.x;
                y = y + rect.y;
            }
            $menuContext.css('left', x + 'px').css('top', y + 'px');
            //if (isMultiSelect) {
            //    $(DE.copySubMenuContext).hide();
            //}
            //else {
            //    $(DE.copySubMenuContext).show();
            //}
        },
        showRenameContext: function (flag) {
            if (flag)
                $(DE.renameContext).show();
            else
                $(DE.renameContext).hide();
        },
        specification: function (res) {
            DE.entityType = res ? res.entityType : DE.entityType;
            render.valEntityType(DE.entityType);
            let $spec = $(DE.selectSpecification);
            let selectedSpec = $spec.val();
            render.selector({
                data: res.lstSpecification,
                element: $spec,
                value: 'value',
                text: 'key',
                defaultText: 'Select Specification',
                dataKey: [{}],
                disabled: false
            });
            $spec.val(selectedSpec);
        },
        valEntityType: function (type) {
            $(DE.hdnEntityType).val(type);
        },
        showUnitContext: function (flag) {
            if (flag)
                $(DE.unitContext).show();
            else
                $(DE.unitContext).hide();
        },
        setLabelFormatContext: function (labelData) {

            let $selectLabelFontColor = $(DE.selectLabelFontColor);
            let $txtLabelFontSize = $(DE.txtLabelFontSize);
            let $selectLabelBGColor = $(DE.selectLabelBGColor);
            let $selectLabelStrokeColor = $(DE.selectLabelStrokeColor);
            let $selectLabelOrientation = $(DE.selectLabelOrientation);
            let $imgLBGImage = $(DE.imgLBGImage);
            let $selectLabelColor = $(DE.selectLabelColor);
            let $txtLabelFontWeight = $(DE.txtLabelFontWeight);

            if (labelData) {
                $selectLabelFontColor.val(labelData.font_color);
                $txtLabelFontSize.val(labelData.font_size);
                $selectLabelBGColor.val(labelData.color);
                $selectLabelStrokeColor.val(labelData.stroke);
                $selectLabelOrientation.val(labelData.text_orientation);
                $imgLBGImage.attr('src', labelData.bg_image);
                $selectLabelColor.val(labelData.model_color_id);
                $txtLabelFontWeight.val(labelData.font_weight);
            }
            render.showLabelImageContext(labelData.bg_image);
            render.modelColorSelection($selectLabelColor);
        },
        showLabelImageContext: function (flag) {
            if (flag)
                $(DE.labelImageContainer).show();
            else
                $(DE.labelImageContainer).hide();
        },
        showMultiConnectionContext: function (flag) {
            if (flag)
                $(DE.multiConnectionContainer).show();
            else
                $(DE.multiConnectionContainer).hide();
        },
        modelColorSelection: function ($e) {
            $e.attr("style", $('option:selected', $e).attr('style'));
        },
        chosenSelector: function () {
            $(DE.selectModel).trigger('chosen:updated');
            $(DE.selectModelType).trigger('chosen:updated');
            $(DE.selectLibTypes).trigger('chosen:updated');
            $(DE.selectSpecification).trigger('chosen:updated');
            $(DE.selectVender).trigger('chosen:updated');
        },
        modelText: function (text) {
            $(DE.modelText).text(text);
        },
        selectLibFilterFirst: function () {
            $(DE.libFilterBox + " input:radio:first").attr('checked', true);
            action.onLibFilterOptionClick();
        },
        showBorderColorContext: function (flag) {
            if (flag) {
                $(DE.borderColorContext).show();

            }
            else {
                $(DE.borderColorContext).hide();

            }


        },
        enableDimension: function (flag) {
            $(DE.modelHeight).attr('disabled', flag);
            $(DE.modelWidth).attr('disabled', flag);
        }
    };

    var validate = {
        initCreate: function () {
            let chk = false;
            let $selectModel = $(DE.selectModel);
            let $selectModelType = $(DE.selectModelType);
            let $optionsModelType = $(DE.selectModelType + " option");
            let $modelOption = $(DE.selectModel + ' option:selected');
            if ($selectModel.val() == "") {
                alert("Please select model!");
                return true;
            }
            if ($modelOption.data('hasType')!=null && $modelOption.data('hasType').toLowerCase() == 'false') { return false; }
            //$optionsModelType.length > 1 &&
            if (!$selectModelType.val()) {
                alert("Please select model type!");
                return true;
            }
            return chk;
        },
        saveModel: function () {
            let isError = false;
            let isPortCountFailed = false;
            let $name = $(DE.txtModelName);
            let $status = $(DE.selectStatus);
            let $specification = $(DE.selectSpecification);
            let $vendor = $(DE.selectVender);
            let modelId = $(DE.selectModel).val();
            let $no_of_port = $(DE.no_of_port);
            //validate name and status
            render.errorFocus($name, false);
            render.errorFocus($status, false);
            render.errorFocus($specification, false);
            render.errorFocus($vendor, false);
            render.errorFocus($no_of_port, false);
            if (!$name.val().trim()) {
                render.errorFocus($name, true);
                isError = true;
            }
            if (!$status.val().trim()) {
                render.errorFocus($status, true);
                isError = true;
            }
            if (modelId == DE.equipmentKey && $status.val().trim() == 1) {

                render.activeSpecificationTab(!isError);

                if (!$specification.val()) {
                    render.errorFocus($specification, true);
                    isError = true;
                }
                if (!$vendor.val()) {
                    render.errorFocus($vendor, true);
                    isError = true;
                }
                if (!isError && $no_of_port.val() != generate.noOfPorts()) {
                    render.errorFocus($no_of_port, true);
                    isPortCountFailed = true;

                }

            }
            if (validate.isOverlappedModels()) {
                isError = true;
                alert("Models should not be overlapped each other.Move recently added model to free space!", "Error");
            }
            return { isError: isError, isPortCountFailed: isPortCountFailed };

        },
        whereDropped: function (d) {
            let id = 0;
            if (d.parent && d.parent != -1) {
                let prevParent = d.parent;
                let rect = _workSpaceActions.getAbsoluteRect(d.parent, true);
                //console.log("event.x:=" + d3.event.x + " event.y:=" + d3.event.y);
                //console.log("position.x:=" + (d3.event.x + rect.x) + " position.y:=" + (d3.event.y + rect.y));
                let parent = generate.boundingRect(d.id, { x: d3.event.x + rect.x, y: d3.event.y + rect.y }); // Bounding Rect returns id of model where target is dropped.
                //console.log("Parent:=" + parent);

                if (parent != d.parent && parent != -1 && parent != prevParent) {
                    let parentData = _workAreaData[_workSpaceActions.getIndex(parent)];
                    let oldParentData = undefined;
                    if (d.parent > -1)
                        oldParentData = _workAreaData[_workSpaceActions.getIndex(d.parent)];
                    let param = {
                        parent_model_id: parentData.model_id,
                        parent_model_type_id: parentData.model_type_id || null,
                        child_model_id: d.model_id,
                        child_model_type_id: isNaN(d.model_type_id) ? null : d.model_type_id
                    }
                    if (validate.modelDrop(param)) {

                        let parentHeight = (parentData.rotation_angle == 90 || parentData.rotation_angle == 270) ? parentData.width : parentData.height;
                        let parentWidth = (parentData.rotation_angle == 90 || parentData.rotation_angle == 270) ? parentData.height : parentData.width;

                        let childHeight = (d.rotation_angle == 90 || d.rotation_angle == 270) ? d.width : d.height;
                        let childWidth = (d.rotation_angle == 90 || d.rotation_angle == 270) ? d.height : d.width;

                        console.log('pHeight := ' + parentHeight + ' pWidth := ' + parentWidth);
                        console.log('cHeight := ' + childHeight + ' cWidth := ' + childWidth);
                        let defaultSizes = action.getModelDefaults({ id: parentData.model_id });
                        let margin = defaultSizes.margin;
                        if ((parentHeight - parentData.border_width * 2 - margin * 2) >= childHeight
                                    && (parentWidth - parentData.border_width * 2 - margin * 2) >= childWidth) {
                            d.parent = parent;
                            d.ref_parent = parentData.ref_id;
                            rect = _workSpaceActions.getAbsoluteRect(d.parent);
                            d.position.x = (rect.width / 2);
                            d.position.y = (rect.height / 2);
                            d.rotation_angle -= parentData.rotation_angle - (oldParentData ? oldParentData.rotation_angle : 0);
                            while (d.rotation_angle < 0) {
                                d.rotation_angle += 360.0;
                            }
                            if (d.rotation_angle >= 360)
                                d.rotation_angle = d.rotation_angle % 360;
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
        },
        collision: function (id, param) {
            _workAreaData = _workSpaceActions.getWorkArea();
            let current = _workSpaceActions.select(id);

            let rect = _workSpaceActions.getAbsoluteRect(current.parent);

            let tempRect = { x: rect.x + param.x - current.width / 2, y: rect.y + param.y - current.height / 2, height: current.height, width: current.width };
            switch (current.rotation_angle) {
                case 90:
                case 270:
                    tempRect = {
                        x: rect.x + param.x - current.height / 2,
                        y: rect.y + param.y - current.width / 2,
                        height: current.width,
                        width: current.height
                    };
                    break;
            }
            let sibling = _workAreaData.filter(x=> x.id != id && x.id > 1 && x.parent == current.parent);
            let len = sibling.length;

            for (let i = 0 ; i < len; i++) {
                let sRect = _workSpaceActions.getAbsoluteRect(sibling[i].id, true);
                if (_workSpaceActions.isRectOverlapped(tempRect, sRect)) {
                    return true;
                }
            }
            //check for door
            //let $doorRect = $('#' + DE.roomDoorRect);
            //if ($doorRect.length > 0) {
            //    let sRect = { x: parseFloat($doorRect.attr('x')), y: parseFloat($doorRect.attr('y')), width: parseFloat($doorRect.attr('width')), height: parseFloat($doorRect.attr('height')) }
            //    if (_workSpaceActions.isRectOverlapped(tempRect, sRect)) {
            //        return true;
            //    }
            //}
            return false;
        },
        modelDimension: function (dim) {
            let defaultSize = action.getModelDefaults({ id: _selectedModel.model_id });
            let min = 1, max = 9999;
            let min_height = defaultSize.min_height, min_width = defaultSize.min_width;
            _isMinDimension = false;
            if (dim.height < min_height) {
                dim.height = min_height;
                _isMinDimension = true;
            }
            if (dim.width < min_width) {
                dim.width = min_width;
                _isMinDimension = true;
            }
            if (dim.depth < min) {
                dim.depth = min;

            }

            if (dim.height > max) {
                dim.height = max;

            }
            if (dim.width > max) {
                dim.width = max;

            }
            if (dim.depth > max) {
                dim.depth = max;

            }
            return dim;
        },

        isOverlappedModels: function () {
            //check if any model is overlapped then return true
            _workAreaData = _workSpaceActions.getWorkArea();
            let len = _workAreaData.length;
            for (let i = 1 ; i < len; i++) {
                //Get current model
                let current = _workAreaData[i];
                let rect = _workSpaceActions.getAbsoluteRect(current.id, true);

                //let tempRect = { x: rect.x , y: rect.y , height: current.height, width: current.width };

                //Check collision with sibling
                let sibling = _workAreaData.filter(x=> x.id != current.id && x.id > 1 && x.parent == current.parent).sort((a, b) => a.id - b.id);
                let sibLen = sibling.length;
                for (let i = 0 ; i < sibLen; i++) {
                    let sRect = _workSpaceActions.getAbsoluteRect(sibling[i].id, true);
                    if (_workSpaceActions.isRectOverlapped(rect, sRect)) {
                        let focusId = sibling[i].id > current.id ? sibling[i].id : current.id;
                        action.selectModel(focusId);
                        render.modelFocus(focusId);
                        return true;
                    }
                }
            }
            return false;
        },
        childResize: function (d, dim) {
            let flag = true;
            let oldW = d.width, oldH = d.height;
            d.width = dim.width;
            d.height = dim.height;
            if (d.parent > 0) {
                let sRect = _workSpaceActions.getAbsoluteRect(d.id, true);

                if (!_workSpaceActions.contains(d.parent, { x: sRect.x, y: sRect.y })
                    || !_workSpaceActions.contains(d.parent, { x: sRect.x + sRect.width, y: sRect.y })
                    || !_workSpaceActions.contains(d.parent, { x: sRect.x, y: sRect.y + sRect.height })
                    || !_workSpaceActions.contains(d.parent, { x: sRect.x + sRect.width, y: sRect.y + sRect.height })
                    ) {
                    flag = false;
                }
                if (flag) {
                    let param = generate.inBoundaryPos(d.parent, d, d.position);
                    if (validate.collision(d.id, d.position))
                        flag = false;
                }

            }
            if (!flag) {
                d.width = oldW;
                d.height = oldH;
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

            let $modelUnit = $(DE.modelUnit);
            $modelUnit.on('change', action.onChangeModelUnit);

            let $modelBorderColor = $(DE.borderColor);
            $modelBorderColor.on('change', action.onChangeSelectBorderColor);
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

            let $btnChangeLabelFormat = $(DE.btnChangeLabelFormat);
            $btnChangeLabelFormat.on('click', action.onSetLabelFormat);

            let $fileBGImage = $(DE.fileBGImage);
            $fileBGImage.on('change', action.onReadLabelImage);

            let $deleteLBGImg = $(DE.deleteLBGImg);
            $deleteLBGImg.on('click', action.onDeleteLabelBG);

            let $selectLabelColor = $(DE.selectLabelColor);
            $selectLabelColor.on('change', action.onLabelColorChange);

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


            let $renameSubMenuContext = $(DE.renameSubMenuContext);
            $renameSubMenuContext.on('click', action.onRenameSubMenuContext);

            let $txtRenameModel = $(DE.txtRenameModel);
            $txtRenameModel.on('keyup', action.onChangeModelName);

            let $cancelRename = $(DE.cancelRename);
            $cancelRename.on('click', action.onCancelRenameContext);

            let $renameModel = $(DE.renameModel);
            $renameModel.on('click', action.onRenameModelName);

            let $labelFormat = $(DE.labelFormatSubMenuContext);
            $labelFormat.on('click', action.onLabelFormatSubMenuContext);
        },
        documentEvent: function () {
            action.onDocumentKeyPress();
        }
    };

    var load = {
        leftPanel: function () {

            render.showLeftPanel(false);
            render.enableDimension(true);
      
        },
        workArea: function () {
            _$workArea = d3.select(DE.svgWorkArea).on("contextmenu", action.onWorkspaceContextMenu);
            _$libArea = d3.select(DE.svgLibArea).on("contextmenu", action.onWorkspaceContextMenu);
            _workAreaData = _workSpaceActions.getWorkArea();
            API.call(BuilderAPIURLS.getModelMasterData, {}, action.initModelMaster);
            if (DE.element_id && DE.element_id != '' && DE.element_id != '0') {
                $(DE.selectModel).val($(DE.hdnModelId).val());
                $(DE.selectModelType).val($(DE.hdnModelTypeId).val());
                API.getWorkspaceData({ modelId: DE.element_id, isStatic: false }, render.modelLoad);
            }
        },
        library: function () {
            API.call(BuilderAPIURLS.getModelAllRules, {}, function (res) { _hierarchyRules = res; });
        },
        chosenUpdate: function () {
            render.chosenSelector();
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
        init: init,
        generate: generate,
        action: action,
        render: render
    };
})();

var specificationManager = (function () {
    'use strict';
    var DE = {};
    var action = {
        onSpecificationChange: function (e) {
             
            render.setModelSpecification({});
            let specification = $(DE.selectSpecification + " option:selected").text();
            API.call(BuilderAPIURLS.getVendorList, { specification: specification }, function (res) {
                renderSelector({
                    data: res.result,
                    element: $(DE.selectVender),
                    value: 'key',
                    text: 'value',
                    defaultText: 'Select Vendor',
                    dataKey: [{ key: 'item_template_id', name: 'data-item-id' }],
                    disabled: false
                });

            });
        },
        onSubSpecificationChange: function (e) {
            render.setModelSpecification({
                category: '',
                code: "",
                subCategory_1: "",
                subCategory_2: "",
                subCategory_3: ""
            });
            let specification = $(DE.selectSpecification + " option:selected").text();
            let vendorId = $(DE.selectVender + " option:selected").val();
            let entityType = $(DE.hdnEntityType).val();
            if (entityType) { DE.entityType = entityType; }
            let param = {
                entitytype: DE.entityType,
                specification: specification,
                vendorId: vendorId
            };
            API.call(BuilderAPIURLS.getSubVendorList, param, function (res) {

                if (res.result && res.result.length) {
                    render.setModelSpecification(res.result[0]);
                    $(DE.selectVender + " option:selected").data("itemId", res.result[0]["item_template_id"]);
                    if (DE.itemTemplateId) {
                        $(DE.itemTemplateId).val(res.result[0]["item_template_id"]);
                    }
                }
            });
        }
    };
    var API = {
        call: function (url, param, callback) {
            ajaxReq(url, param, true, function (res) {
                if (callback && typeof callback == 'function')
                    callback(res);
            }, false, false, true);
        }
    };
    var render = {
        setModelSpecification: function (d) {
            $(DE.category).val(d.category);
            $(DE.itemCode).val(d.code);

            $(DE.subCategory_1).val(d.subCategory_1);
            $(DE.subCategory_2).val(d.subCategory_2);
            $(DE.subCategory_3).val(d.subCategory_3);
            $(DE.no_of_port).val(d.no_of_port);
        },
    };
    var bind = {
        modelEvent: function () {
            let $selectSpecification = $(DE.selectSpecification).unbind();
            $selectSpecification.on('change', action.onSpecificationChange);

            let $selectVender = $(DE.selectVender).unbind();
            $selectVender.on('change', action.onSubSpecificationChange);
        }
    };
    var load = {};
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
        init: init,
        API_URL: BuilderAPIURLS,
        
    };
})();



