var workspace = (function () {
    'use strict';

    var _DE = {};
    var _workArea = [];
    var _selectedModel = {};

    var action = {
        reset: function () {
            _selectedModel = {};
            _workArea = [];
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
            let index = _workArea.indexOf(_selectedModel);
            if (index > -1)
            {
                _workArea.splice(index, 1);
            }
            _selectedModel = {};
        },
        resize: function (height, width, depth) {
            _selectedModel.height = height;
            _selectedModel.width = width;
            _selectedModel.depth = depth;
        },
        move: function (dx,dy) {
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
                imgUrl: '',
                stroke: '',
                height: 0,
                width: 0,
                depth: 0,
                offsetX: 0,
                offsetY: 0,
                image_data: '',
                isStatic:false
            };
        },
        getWorkArea: function () {
            return _workArea;
        },
        select: function (id) {
            _selectedModel = {};
            let result = _workArea.filter(function(m){
                if(m.id == id)
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
        action:action
    };
})();