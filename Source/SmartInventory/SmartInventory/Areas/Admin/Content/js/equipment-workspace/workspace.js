var workspace = (function () {
    'use strict';

    var _DE = {};
    var _workArea = [];
    var _selectedModel = {};
    var _multiSelected = [];
    var action = {
        reset: function () {
            _selectedModel = {};
            _workArea = [];
            _multiSelected = [];
        },
        clear: function () {
            _selectedModel = {};
        },
        add: function () {
            _selectedModel = action.newModel();
             
            _workArea.push(_selectedModel);
            return _selectedModel;
        },
        remove: function (id) {
            let index = action.getIndex(id);
            if (index > -1) {
                //_workArea = _workArea.filter(x => x.parent != _workArea[index].id);
                action.removeChildren(id);
                _workArea.splice(index, 1);
            }
            _selectedModel = {};
        },
        resize: function (height, width, depth) {
            _selectedModel.height = height;
            _selectedModel.width = width;
            _selectedModel.depth = depth;
        },
        move: function (dx, dy) {
            _selectedModel.position.x = dx;
            _selectedModel.position.y = dy;
        },
        newModel: function () {

            let id = 0;
            let parent = -1;
            return {
                id: id,
                name: '',
                position: { x: 0, y: 0 },
                parent: parent,
                color: '',
                img_id: '',
                stroke: '',
                height: 0,
                width: 0,
                depth: 0,
                offset_x: 0,
                offset_y: 0,
                image_data: '',
                is_static: false,
                db_id: 0,
                rotation_angle: 0,
                model_view_id: 1,
                model_id: 0,
                model_type_id: 0,
                model_type: '',
                border_width: 0,
                is_editable: true,
                db_height: 0,
                db_depth: 0,
                db_width: 0,
                unit: 0,
                db_parent: 0,
                ref_id: 0,
                ref_parent: 0,
                font_size: 14,
                font_color: '#000000',
                text_orientation: 'LR',
                bg_image: '',
                font_weight: '400',
                border_color: '#6E6E6E',
                isNewEquipment: false
            };
        },
        getWorkArea: function () {
            action.linearId(_workArea);
            return _workArea;
        },
        select: function (id) {
            _selectedModel = {};
            let result = _workArea.filter(function (m) {
                if (m.id == id)
                    return m;
            });
            if (result.length > 0)
                _selectedModel = result[0];
            return _selectedModel;
        },
        getCurrent: function () {
            return _selectedModel;
        },
        getMaxId: function () {
            var maxid = 0;
            _workArea.map(function (obj) {
                if (obj.id > maxid) maxid = obj.id;
            });
            return maxid;
        },
        getIndex: function (id) {
            var res = _workArea.findIndex(x => x.id == id);
            return res;
        },
        setWorkArea: function (workarea) {
             
            _selectedModel = {};
            _workArea = workarea;
        },
        linearId: function (list) {
            if (!list || !list.length)
                return;
            let index = 1;
            list[0].parent = -1;
            for (let i = 0; i < list.length; i++) {
                let children = [];
                children = list.filter(x=>x.parent == list[i].id);

                for (let j = 0; j < children.length; j++) {
                    children[j].parent = index;
                }
                list[i].id = index;
                index++;
            }
        },
        getAbsoluteRect: function (id, isRotation) {
            let index = action.getIndex(id);
            if (index == -1)
                return null;
            let rect = {
                x: action.modelPositionX(_workArea[index]),
                y: action.modelPositionY(_workArea[index]),
                height: _workArea[index].height,
                width: _workArea[index].width
            };
            if (isRotation) {
                switch (_workArea[index].rotation_angle) {
                    case 90:
                    case 270:
                        rect = {
                            x: _workArea[index].position.x - _workArea[index].height / 2,
                            y: _workArea[index].position.y - _workArea[index].width / 2,
                            height: _workArea[index].width,
                            width: _workArea[index].height
                        };
                        break;
                }
            }
            while (index != -1) {
                index = action.getIndex(_workArea[index].parent);
                if (index == -1)
                    break;
                rect.x += action.modelPositionX(_workArea[index]);
                rect.y += action.modelPositionY(_workArea[index]);
            }
            return rect;
        },
        contains: function (id, point) {
            let rect = action.getAbsoluteRect(id);
            return rect.x <= point.x && point.x <= rect.x + rect.width &&
             rect.y <= point.y && point.y <= rect.y + rect.height;

        },
        removeChildren: function (parentId) {
            let children = _workArea.filter(x => x.parent == parentId);
            if (children && children.length) {
                for (let i = 0; i < children.length; i++) {
                    action.removeChildren(children[i].id);
                    let index = action.getIndex(children[i].id);
                    if (index > -1)
                        _workArea.splice(index, 1);
                }

            }

        },
        copy: function (id) {
            let index = action.getIndex(id);
            if (index > -1) {
                let newModel = {};
                $.extend(true, newModel, _workArea[index]);
                newModel.id = action.getMaxId() + 1;
                 
                _workArea.push(newModel);
                action.copyChildren(id, newModel.id);
                _selectedModel = newModel;
            }

        },
        copyChildren: function (parentId, newParentId) {
            let children = _workArea.filter(x => x.parent == parentId);
            if (children && children.length) {
                for (let i = 0; i < children.length; i++) {

                    let index = action.getIndex(children[i].id);
                    if (index > -1) {
                        let newModel = {};
                        $.extend(true, newModel, children[i]);
                        newModel.id = action.getMaxId() + 1;
                        newModel.parent = newParentId;
                         
                        _workArea.push(newModel);
                        action.copyChildren(children[i].id, newModel.id);
                    }
                }

            }
        },
        getDraggable: function (id) {
            let index = action.getIndex(id);
            if (index == -1) return undefined;
            if (_workArea[index].parent == -1)
                return _workArea[index];
            if (!_workArea[index].is_static)
                return _workArea[index];
            return action.getDraggable(_workArea[index].parent);
        },
        getSizeLimitByChildren: function (id, isRotation) {
            let limit = {
                x: { min: 0, max: 0 },
                y: { min: 0, max: 0 }
            };
            let children = _workArea.filter(x => x.parent == id);
            if (children && children.length) {
                for (let i = 0; i < children.length; i++) {
                    let x = action.modelPositionX(children[i]);
                    let y = action.modelPositionY(children[i]);
                    let height = children[i].height;
                    let width = children[i].width;
                    if (isRotation) {
                        switch (children[i].rotation_angle) {
                            case 90:
                            case 270:

                                x = children[i].position.x - children[i].height / 2;
                                y = children[i].position.y - children[i].width / 2;
                                height = children[i].width;
                                width = children[i].height;

                                break;
                        }
                    }
                    if (limit.x.min > x) {
                        limit.x.min = x;
                    }
                    if (limit.y.min > y) {
                        limit.y.min = y;
                    }
                    if (limit.x.max < x + width) {
                        limit.x.max = x + width;
                    }
                    if (limit.y.max < y + height) {
                        limit.y.max = y + height;
                    }
                }

            }
            return limit;
        },
        selectMany: function (id) {
            if (id > 0)
                _multiSelected.push(id);
            return _multiSelected;
        },
        clearManySelect: function () {
            _multiSelected = [];
        },
        isRectOverlapped: function (rectA, rectB) {
            //RectA.Left < RectB.Right && RectA.Right > RectB.Left &&  RectA.Top > RectB.Bottom && RectA.Bottom < RectB.Top
            return (rectA.x < (rectB.x + rectB.width) && (rectA.x + rectA.width) > rectB.x && rectA.y < (rectB.y + rectB.height) && (rectA.y + rectA.height) > rectB.y);
        },
        modelPositionX: function (d) {
            //return d.position.x ;
            return d.position.x - (d.width / 2);
        },
        modelPositionY: function (d) {
            //return d.position.y;
            return d.position.y - (d.height / 2);
        },
        getRealRotation: function (id) {
            let index = action.getIndex(id);
            if (index == -1)
                return 0;
            let angle = _workArea[index].rotation_angle;
            while (index != -1) {
                index = action.getIndex(_workArea[index].parent);
                if (index == -1)
                    break;
                angle += _workArea[index].rotation_angle;
            }
            return angle;
        }
    };


    var setup = function () {
        //Bind events
        //Load Data
        //D3 init
    };
    var init = function (configs) {
        $.extend(true, _DE, configs);
        setup();
    };
    return {
        init: init,
        action: action
    };
})();


