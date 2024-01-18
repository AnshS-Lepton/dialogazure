var APIURLS = {
    deleteEquipment: 'ISP/DeleteChildEquipment',
    resetEquipment: 'ISP/CreateModel'
};
var API = {
    call: function (url, param, callback) {
        ajaxReq(url, param, true, function (res) {
            if (callback && typeof callback == 'function')
                callback(res);
        }, false, true, true);
    }
}
var EquipmentEditor = (function () {
    var DE = {};
    var _EquipmentBuilder = equipmentBuilder;
    var _workSpaceActions = workspace.action;
    var _selectedModel = {};
    var render = {
        equipmentView: function (flag) {
            let $tabEquipmentView = $(DE.divEquipmentView);
            if (flag) {
                $tabEquipmentView.show();
            } else {
                $tabEquipmentView.hide();
            }
        },
        roomView: function (flag) {
            let $tabRoomView = $(DE.divRoomView);
            if (flag) {
                $tabRoomView.show();
            } else {
                $tabRoomView.hide();
            }
        },
        hideRackViewMenuContext: function (flag) {
            let $rackViewSubMenuContext = $(DE.rackViewSubMenuContext);
            $rackViewSubMenuContext.hide();
        
        },
        resetEquipment: function (resp) {
            $("#divEquipmentView").html(resp).show();                   
        }

    }
    var action = {
        onUpdateEquipment: function (e) {
            if (action.validate()) {
                confirm('Are you sure do you want to update this equipment!', function () {
                    _selectedModel = _workSpaceActions.select(1);
                    _workAreaData = _workSpaceActions.getWorkArea();
                    if (_workAreaData.length) {
                        let param = {
                            system_id: parseInt(DE.system_id),
                            // model_name: m.modelName,
                            // model_id: m.modelId,
                            // model_type_id: m.modelTypeId,
                            // model_image_id: _workAreaData[0].img_id,
                            // pos_X: _EquipmentBuilder.generate.pixelToMM(_EquipmentBuilder._selectedModel.position.x),
                            // pos_Y: _EquipmentBuilder.generate.pixelToMM(_EquipmentBuilder._selectedModel.position.y),
                            height: _selectedModel.height,
                            width: _selectedModel.width,
                            //depth: m.modelDepth,
                            //item_template_id: m.itemTemplateId || 0,
                            //status_id: m.statusId,
                            //is_active: true,
                            lstModelChildren: action.updateRefId(_EquipmentBuilder.generate.modelMapping()),
                            //model_master_name: m.modelMasterName,
                            //border_width: m.border_width,
                            //isChangeMapping: true,
                            //unit_size: m.modelUnit,
                            //rotation_angle: _selectedModel.rotation_angle,
                            //is_multi_connection: m.isMultiConnection,
                            //border_color: m.border_color                      
                        };
                        action.saveModel(param);
                    }
                });
            }
        },
        saveModel: function (input) {
            ajaxReq(BuilderAPIURLS.saveModel, input, true, function (res) {
                if (res) {
                    if (res.message && res.status == "OK") {
                        //render.modelDetailModal(false);
                        action.reset();
                        alert(res.message, 'Success', 'success');
                    }
                    else {
                        alert(res.message);
                        //render.modelDetailModal(false);
                    }
                }
            }, true, true, true);
        },
        onDeleteSubMenuContext: function (e) {
            _selectedModel = _EquipmentBuilder.action.getSelectModel();
            showConfirm('Are you sure you want to delete this?', function () {
                if (_selectedModel.ref_id > 0) {
                    API.call(APIURLS.deleteEquipment, { id: _selectedModel.ref_id }, function (resp) {
                        if (resp.status) { action.deleteEquipment(_selectedModel.id); }
                        _EquipmentBuilder.render.showSubMenuContext(false);
                        alert(resp.message);
                    });
                } else { action.deleteEquipment(_selectedModel.id); }
            });
        },
        updateRefId: function (mapping) {
            for (let i = 0; i < mapping.length; i++) {
                let children = mapping.filter(x => x.parent_id == mapping[i].id && mapping[i].ref_id == 0);
                for (let j = 0; j < children.length; j++) {
                    var index = mapping.findIndex(x => x.id == children[j].id);
                    mapping[index].ref_id = 0;
                    //var child= mapping.filter(x => x.id == child_id).ref_id == 0;
                }
            }
            return mapping;
        },
        deleteEquipment: function (id) {
            _workSpaceActions.remove(id);
            _EquipmentBuilder.action.selectModel({});
            _EquipmentBuilder.render.allModels();
        },
        resetEquipment: function () {
            confirm('Are you sure do you want to reset this equipment!', function () {
                _selectedModel = _workSpaceActions.select(1);
                API.call(APIURLS.resetEquipment, { modelId: _selectedModel.ref_id }, render.resetEquipment);
            });
        },
        resetOnDelete: function (id) {
            _selectedModel = _workSpaceActions.select(1);
            if (id == _selectedModel.ref_id) {
                $("#closeEquipmentView").trigger("click");
            }
        },
        validate: function () {
            var _isValid = true;
            _selectedModel = _workSpaceActions.select(1);
            if (parseFloat($(DE.rackWidth).val()) < _selectedModel.width) { _isValid = false; alert('Equipment width can not be greater than rack!'); } else
                if (parseFloat($(DE.rackHeight).val()) < _selectedModel.height) { _isValid = false; alert('Equipment hight can not be greater than rack!'); }
            return _isValid;
        }
    }
    var load = {
        equipmentView: function () {
            render.equipmentView(true);
            render.roomView(false);
            render.hideRackViewMenuContext(false);
            setTimeout(function () { equipmentBuilder.render.enableDimension(true); $(DE.modelUnit).attr('disabled', true); }, 400);
        }
    };
    var bind = {
        tabEvents: function () {
            let $linkEquipmentView = $(DE.linkEquipmentView);
            $linkEquipmentView.on('click', function () {
                render.equipmentView(true);
                render.roomView(false);
            });

            let $tabRoomView = $(DE.tabRoomView);
            $tabRoomView.on('click', function () {
                render.equipmentView(false);
                render.roomView(true);
            });
            let $closeEquipmentView = $(DE.closeEquipmentView);
            $closeEquipmentView.on('click', function () {
                render.equipmentView(false);
                render.roomView(true);
                $(DE.tabEquipmentView).hide();
            });

            let $deleteSubMenuContext = $(DE.deleteSubMenuContextEquipment);
            $deleteSubMenuContext.on('click', action.onDeleteSubMenuContext);

            let $resetEquipmentWorkspace = $(DE.resetEquipmentWorkspace);
            $resetEquipmentWorkspace.on('click', action.resetEquipment);
            

        },
        RightPanelEvents: function () {
            let $rackSave = $(DE.updateEquipment);
            $rackSave.on('click', action.onUpdateEquipment);
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
        BuilderAPIURLS.getWorkspaceData = 'ISP/GetWorkspaceData';
        BuilderAPIURLS.getModelAllRules = 'ISP/GetModelAllRules';
        BuilderAPIURLS.getLibraryData = 'ISP/GetLibraryData';
        BuilderAPIURLS.getModelMasterData = 'ISP/getModelMasterData';
        BuilderAPIURLS.saveModel = 'ISP/SaveModel',
            _EquipmentBuilder.init(configs);
        $.extend(true, DE, configs);
        setup();
    };
    return {
        init: init,
        action: action
    };
})();