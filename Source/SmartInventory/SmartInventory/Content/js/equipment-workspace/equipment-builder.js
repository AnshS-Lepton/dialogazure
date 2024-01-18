var equipmentBuilder = (function () {
    'use strict';
    var DE = {};
    var _$workArea, _$libArea;
    var _workSpaceActions = workspace.action;
    var _workAreaData;
    var _libraryData = [
        {
            id: 1,
            position: { x: 0, y: 0 },
            height: 50,
            width: 50,

            stroke: '#000',
            image_data: '<linearGradient id="1" gradientUnits="userSpaceOnUse" x1="25.0098" y1="50.1631" x2="25.0098" y2="-0.1641"><stop offset="0" style="stop-color:#ffd800" /> <stop offset="1" style="stop-color:#00A935;stop-opacity:0.4" /></linearGradient><ellipse fill="url(#1)" cx="25.01" cy="25" rx="25" ry="25.164" />'
        }
    ];

    var urls = {
        getPortImages: 'Equipment/GetPortImages',
        saveModel: 'Equipment/SaveModel'
    };

    var action = {
        addModel: function (e) {

            let current = _workSpaceActions.add();

            current.id = _workSpaceActions.getMaxId() + 1;
            current.position = { x: 0, y: 0 };
            current.height = 50;
            current.width = 50;
            current.color = 'Transparent';
            current.stroke = 'Transparent';
            if (!isNaN(e.id)) {
                current.color = 'Transparent';
                current.imgUrl = '#img_' + e.id;
                current.stroke = 'Transparent';
            }
            current.image_data = e.content;
            current.isStatic = false;
            render.setInCenter(current);
            _workAreaData = _workSpaceActions.getWorkArea();
            render.model();

        },
        onStartModelDrag: function (d) {
           
            if (!d.isStatic) {
                d.offsetX = d.position.x - d3.event.x;
                d.offsetY = d.position.y - d3.event.y;
            }
        },
        onModelDrag: function (d) {
            let modelFound = d3.select(this);
            //let id = parseInt(modelFound.attr(DE.modelId));
            //let index = _workSpaceActions.getIndex(id);
            if (!d.isStatic) {
                d.position.x = d3.event.x + d.offsetX;
                d.position.y = d3.event.y + d.offsetY;
                modelFound.attr("x", d.position.x).attr("y", d.position.y);
            }
        },
        onEndModelDrag: function (d) {

            if (!d.isStatic) {
                d.offsetX = 0;
                d.offsetY = 0;
            }

        },
        onModelContextMenu: function () {
        },
        onLibAddButton: function () {
            let $current = d3.select(this);
            let id = parseInt($current.attr(DE.libraryId));
            let content = $current.html().trim();
            action.addModel({ id: id, content: content });
        },
        onChangeGridColor: function (e) {
            let radioValue = $(DE.gridColorOptions + ":checked").val();
            render.gridColor(radioValue);
        },
        reset: function () {
            render.lockCreateModel(false);
            _workSpaceActions.reset();
            _workAreaData = _workSpaceActions.getWorkArea();
            render.model();
            render.resetResizeControl();
        },
        initCreate: function (e) {
            //Validate model selection here

            //Lock model selection
            render.lockCreateModel(true);
        },
        onResetWorkspace: function (e) {

            showConfirm('Are you sure you want to reset your workspace? You will loose your progress.', action.reset);
        },
        onModelClick: function (d) {
            //Draw outer boundery add circle in corner
            render.resizeControls(d);
        },
        onResizeDragStart: function (d, isTop) {

        },
        onResizeDrag: function (d, isTop) {
            if (isTop) {

                var newWidth = Math.max(d.width + d.position.x - d3.event.x, 50);

                d.position.x += d.width - newWidth;
                d.width = newWidth;

                var newHeight = Math.max(d.height + d.position.y - d3.event.y, 50);

                d.position.y += d.height - newHeight;
                d.height = newHeight;

            } else {

                d.width = Math.max(d3.event.x - d.position.x, 50);
                d.height = Math.max(d3.event.y - d.position.y, 50);

            }
            render.model();
            render.resizeControls(d);
        },
        onSaveModelClick: function (e) {

        },
        onChangeSelectModel: function (e) {

        },
        onChangeSelectModelType: function (e) {

        }
    };

    var API = {
        getPortImages: function () {
            ajaxReq(urls.getPortImages, {}, true, function (res) {
                generate.libraryList(res);
                render.library();
            }, false, false, true);
        },
        saveModel: function () {
            ajaxReq(urls.saveModel, {}, true, function (res) {
                
            }, false, false, true);
        }
    };

    var generate = {
        libraryList: function (req) {
            _libraryData = [];
            let len = req.length;
            let xOffset = 100, yOffset = 100;
            let pos_x = 0, pos_y = -yOffset, h = 50, w = 50;
            for (let i = 0; i < len; i++) {
                if (i % 2 === 0) {
                    pos_x = 0;
                    pos_y += yOffset;
                }
                _libraryData.push({
                    id: req[i].id,
                    position: { x: pos_x, y: pos_y },
                    height: h,
                    width: w,
                    image_data: req[i].image_data
                });
                pos_x += xOffset;

            }
        }
    };

    var render = {
        model: function () {
            _$workArea.selectAll(DE.model).remove();
            let workData = _$workArea.selectAll(DE.model)
                .data(_workAreaData);
            workData.exit().remove();
            let svgMain = workData.enter()
                        .append("svg")
                        .attr("pointer-events", "visible")
                        .attr(DE.modelId, function (d) { return d.id; })
                        .attr("class", function (d) { return DE.modelClass })
                        .attr("x", function (d) { return d.position.x; })
                        .attr("y", function (d) { return d.position.y; })
                        .attr("preserveAspectRatio", "xMinYMin meet")
                        .attr("viewBox", function (d) { return "0 0 50 50"; })
                        .attr("height", function (d) { return d.height; })
                        .attr("width", function (d) { return d.width; })
                        .style("fill", function (d) {
                            let res = d.color;
                            //if (d.imgUrl && d.imgUrl != '')
                            //    res ="url('"+ d.imgUrl +"')";
                            return res;
                        })
                        .style("stroke", function (d) { return d.stroke; })

                        .call(d3.drag()
                        .on("start", action.onStartModelDrag)
                        .on("drag", action.onModelDrag)
                        .on("end", action.onEndModelDrag))
                        .on("contextmenu", action.onModelContextMenu)
                        .on("dblclick", action.onModelClick);
            svgMain.append("g")
                    .html(function (d) { return d.image_data; });


        },
        library: function () {
            let libCount = _libraryData.length;
            _$libArea.attr('height', Math.ceil(libCount / 2) * 100);
            _$libArea.selectAll(DE.btnLibAdd).remove();
            let libData = _$libArea.selectAll(DE.btnLibAdd)
                .data(_libraryData);
            libData.exit().remove();
            libData.enter()
                .append("svg")
                .attr("pointer-events", "visible")
                .attr(DE.libraryId, function (d) {
                    return d.id;
                })
                .attr("class", function (d) { return DE.libAddButtonClass; })
                .attr("x", function (d) { return d.position.x; })
                .attr("y", function (d) { return d.position.y; })
                .attr("height", function (d) { return d.height; })
                .attr("width", function (d) { return d.width; })
                .html(function (d) { return d.image_data; })
                .on('click', action.onLibAddButton);

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
            $selectModel.attr('disabled', isLocked);
            $selectModelType.attr('disabled', isLocked);
            if (isLocked)
            { $lockModel.css('visibility', 'visible'); }
            else { $lockModel.css('visibility', 'hidden'); }
        },
        setInCenter: function (model) {
            let rect = _$workArea.node().getBoundingClientRect()
            let pos_x = rect.width / 2 - model.width / 2, pos_y = rect.height / 2 - model.height / 2;
            model.position.x = pos_x;
            model.position.y = pos_y;
        },
        resizeControls: function (e) {
            d3.select(resizeTopLeft)
                   .attr("r", 3)
                   .style("stroke", "#000")
                   .style("fill", "#fff")
                   .attr("cx", e.position.x)
                   .attr("cy", e.position.y)
                   .call(d3.drag().on("start", function () {
                       action.onResizeDragStart(e, true);
                   }).on("drag", function () {
                       action.onResizeDrag(e, true);
                   }));
            d3.select(resizeBottomRight)
                    .attr("r", 3)
                    .style("stroke", "#000").style("fill", "#fff")
                    .attr("cx", e.position.x + e.width)
                    .attr("cy", e.position.y + e.height)
                .call(d3.drag().on("start", function () {
                    action.onResizeDragStart(e, false);
                }).on("drag", function () {
                    action.onResizeDrag(e, false);
                }));
            d3.select(resizeBoundry)
                    .attr("x", e.position.x)
                   .attr("y", e.position.y)
                    .attr("height", e.height)
                    .attr("width", e.width);
        },
        resetResizeControl: function () {
            d3.select(resizeTopLeft).attr("r", 0)
                                    .attr("cx", 0)
                                    .attr("cy", 0);

            d3.select(resizeBottomRight).attr("r", 0)
                                        .attr("cx", 0)
                                        .attr("cy", 0);
            d3.select(resizeBoundry).attr("x", 0)
                                    .attr("y", 0)
                                    .attr("height", 0)
                                    .attr("width", 0);
        },
        setModelDetail: function () {

        }
    };

    var bind = {
        libraryEvents: function () {
            _$libArea.selectAll(DE.btnLibAdd).on('click', action.onLibAddButton);
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
        }

    };

    var load = {
        workArea: function () {
            _$workArea = d3.select(DE.svgWorkArea);
            _$libArea = d3.select(DE.svgLibArea);
            _workAreaData = _workSpaceActions.getWorkArea();

        },
        library: function () {
            API.getPortImages();
            //render.library();
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