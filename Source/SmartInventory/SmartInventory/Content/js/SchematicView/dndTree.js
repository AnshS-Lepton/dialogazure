function getStringLengthInPixel(str) {

    var canvas = document.createElement('canvas');
    var ctx = canvas.getContext("2d");
    ctx.font = "11px Arial";
    return ctx.measureText(str).width;
}
function SchematicView(jsonData, divId, width, height, customScale, searchText) {
    treeJSON = d3.json("", function (error) {
        //treeJSON = d3.json("lepton.json", function(error, treeData) {
        treeData = JSON.parse(jsonData);
        // Calculate total nodes, max label length
        var totalNodes = 0;
        var maxLabelLength = 0;
        // variables for drag/drop
        var selectedNode = null;
        var draggingNode = null;
        // panning variables
        var panSpeed = 200;
        var panBoundary = 20; // Within 20px from edges will pan when dragging.
        // Misc. variables
        var i = 0;
        var duration = 750;
        var root;

        // size of the diagram
        var viewerWidth = $(document).width();
        var viewerHeight = $(document).height();
        viewerWidth = width;// viewerWidth - 27;
        viewerHeight = height;//viewerHeight - 73;

        var tree = d3.layout.tree()
            .size([viewerHeight, viewerWidth]);

        view_width = viewerWidth;
        view_height = viewerHeight;

        // define a d3 diagonal projection for use by the node paths later on.
        var diagonal = d3.svg.diagonal()
            .projection(function (d) {
                return [d.y, d.x];
            });


        // A recursive helper function for performing some setup by walking through all nodes

        function visit(parent, visitFn, childrenFn) {

            if (!parent) return;

            visitFn(parent);

            var children = childrenFn(parent);
            if (children) {
                var count = children.length;
                for (var i = 0; i < count; i++) {
                    visit(children[i], visitFn, childrenFn);
                }
            }
        }

        // Call visit function to establish maxLabelLength
        visit(treeData, function (d) {
            totalNodes++;
            maxLabelLength = Math.max(d.entity_name.length, maxLabelLength); //dj width betw label

        }, function (d) {
            return d.children && d.children.length > 0 ? d.children : null;
        });


        // sort the tree according to the node names

        function sortTree() {
            tree.sort(function (a, b) {
                return b.entity_name.toLowerCase() < a.entity_name.toLowerCase() ? 1 : -1;
            });
        }
        // Sort the tree initially incase the JSON isn't in a sorted order.
        sortTree();

        // TODO: Pan function, can be better implemented.

        function pan(domNode, direction) {
            var speed = panSpeed;
            if (panTimer) {
                clearTimeout(panTimer);
                translateCoords = d3.transform(svgGroup.attr("transform"));
                if (direction == 'left' || direction == 'right') {
                    translateX = direction == 'left' ? translateCoords.translate[0] + speed : translateCoords.translate[0] - speed;
                    translateY = translateCoords.translate[1];
                } else if (direction == 'up' || direction == 'down') {
                    translateX = translateCoords.translate[0];
                    translateY = direction == 'up' ? translateCoords.translate[1] + speed : translateCoords.translate[1] - speed;
                }
                scaleX = translateCoords.scale[0];
                scaleY = translateCoords.scale[1];
                scale = zoomListener.scale();
                svgGroup.transition().attr("transform", "translate(" + translateX + "," + translateY + ")scale(" + scale + ")");
                d3.select(domNode).select('g.node').attr("transform", "translate(" + translateX + "," + translateY + ")");
                zoomListener.scale(zoomListener.scale());
                zoomListener.translate([translateX, translateY]);
                panTimer = setTimeout(function () {
                    pan(domNode, speed, direction);
                }, 50);
            }
        }

        // Define the zoom function for the zoomable tree

        function zoom() {
            zoom_scale = d3.event.scale;
            view_left = d3.event.translate[0];
            view_top = d3.event.translate[1];
            svgGroup.attr("transform", "translate(" + d3.event.translate + ")scale(" + d3.event.scale + ")");
        }


        // define the zoomListener which calls the zoom function on the "zoom" event constrained within the scaleExtents
        var zoomListener = d3.behavior.zoom().scaleExtent([0.1, 10]).on("zoom", zoom);

        function initiateDrag(d, domNode) {
            draggingNode = d;
            d3.select(domNode).select('.ghostCircle').attr('pointer-events', 'none');
            d3.selectAll('.ghostCircle').attr('class', 'ghostCircle show');
            d3.select(domNode).attr('class', 'node activeDrag');

            svgGroup.selectAll("g.node").sort(function (a, b) { // select the parent and sort the path's
                if (a.id != draggingNode.id) return 1; // a is not the hovered element, send "a" to the back
                else return -1; // a is the hovered element, bring "a" to the front
            });
            // if nodes has children, remove the links and nodes
            if (nodes.length > 1) {
                // remove link paths
                links = tree.links(nodes);
                nodePaths = svgGroup.selectAll("path.link")
                    .data(links, function (d) {
                        return d.target.id;
                    }).remove();
                // remove child nodes
                nodesExit = svgGroup.selectAll("g.node")
                    .data(nodes, function (d) {
                        return d.id;
                    }).filter(function (d, i) {
                        if (d.id == draggingNode.id) {
                            return false;
                        }
                        return true;
                    }).remove();
            }

            // remove parent link
            parentLink = tree.links(tree.nodes(draggingNode.parent));
            svgGroup.selectAll('path.link').filter(function (d, i) {
                if (d.target.id == draggingNode.id) {
                    return true;
                }
                return false;
            }).remove();

            dragStarted = null;
        }

        // define the baseSvg, attaching a class for styling and the zoomListener
        //d3.select(divId).append("text")
        //        .attr("x", 0)
        //        .attr("dy", ".25em")
        //        .attr('class', 'heading')
        //        .text("Heading");

        var baseSvg = d3.select(divId).append("svg")
            .attr("width", viewerWidth)
            .attr("height", viewerHeight)
            .attr("class", "overlay")
            .call(zoomListener);



        // Define the drag listeners for drag/drop behaviour of nodes.
        dragListener = d3.behavior.drag()
            .on("dragstart", function (d) {
                if (d == root) {
                    return;
                }
                dragStarted = true;
                nodes = tree.nodes(d);
                d3.event.sourceEvent.stopPropagation();
                // it's important that we suppress the mouseover event on the node being dragged. Otherwise it will absorb the mouseover event and the underlying node will not detect it d3.select(this).attr('pointer-events', 'none');
            })
            .on("drag", function (d) {
                if (d == root) {
                    return;
                }
                if (dragStarted) {
                    domNode = this;
                    initiateDrag(d, domNode);
                }

                // get coords of mouseEvent relative to svg container to allow for panning
                relCoords = d3.mouse($('svg').get(0));
                if (relCoords[0] < panBoundary) {
                    panTimer = true;
                    pan(this, 'left');
                } else if (relCoords[0] > ($('svg').width() - panBoundary)) {

                    panTimer = true;
                    pan(this, 'right');
                } else if (relCoords[1] < panBoundary) {
                    panTimer = true;
                    pan(this, 'up');
                } else if (relCoords[1] > ($('svg').height() - panBoundary)) {
                    panTimer = true;
                    pan(this, 'down');
                } else {
                    try {
                        clearTimeout(panTimer);
                    } catch (e) {

                    }
                }

                d.x0 += d3.event.dy;
                d.y0 += d3.event.dx;
                var node = d3.select(this);
                node.attr("transform", "translate(" + d.y0 + "," + d.x0 + ")");
                updateTempConnector();
            }).on("dragend", function (d) {
                if (d == root) {
                    return;
                }
                domNode = this;
                if (selectedNode) {
                    // now remove the element from the parent, and insert it into the new elements children
                    var index = draggingNode.parent.children.indexOf(draggingNode);
                    if (index > -1) {
                        draggingNode.parent.children.splice(index, 1);
                    }
                    if (typeof selectedNode.children !== 'undefined' || typeof selectedNode._children !== 'undefined') {
                        if (typeof selectedNode.children !== 'undefined') {
                            selectedNode.children.push(draggingNode);
                        } else {
                            selectedNode._children.push(draggingNode);
                        }
                    } else {
                        selectedNode.children = [];
                        selectedNode.children.push(draggingNode);
                    }
                    // Make sure that the node being added to is expanded so user can see added node is correctly moved
                    expand(selectedNode);
                    sortTree();
                    endDrag();
                } else {
                    endDrag();
                }
            });

        function endDrag() {
            selectedNode = null;
            d3.selectAll('.ghostCircle').attr('class', 'ghostCircle');
            d3.select(domNode).attr('class', 'node');
            // now restore the mouseover event or we won't be able to drag a 2nd time
            d3.select(domNode).select('.ghostCircle').attr('pointer-events', '');
            updateTempConnector();
            if (draggingNode !== null) {
                update(root);
                //centerNode(draggingNode); //check again
                draggingNode = null;
            }
        }

        // Helper functions for collapsing and expanding nodes.

        function collapse(d) {
            if (d.children) {
                d._children = d.children;
                d._children.forEach(collapse);
                d.children = null;
            }
        }

        function expand(d) {
            if (d._children) {
                d.children = d._children;
                d.children.forEach(expand);
                d._children = null;
            }
        }

        var overCircle = function (d) {
            selectedNode = d;
            updateTempConnector();
        };
        var outCircle = function (d) {
            selectedNode = null;
            updateTempConnector();
        };

        // Function to update the temporary connector indicating dragging affiliation
        var updateTempConnector = function () {
            var data = [];
            if (draggingNode !== null && selectedNode !== null) {
                // have to flip the source coordinates since we did this for the existing connectors on the original tree
                data = [{
                    source: {
                        x: selectedNode.y0,
                        y: selectedNode.x0
                    },
                    target: {
                        x: draggingNode.y0,
                        y: draggingNode.x0
                    }
                }];
            }
            var link = svgGroup.selectAll(".templink").data(data);

            link.enter().append("path")
                .attr("class", "templink")
                .attr("d", d3.svg.diagonal())
                .attr('pointer-events', 'none');

            link.attr("d", d3.svg.diagonal());

            link.exit().remove();
        };

        // Function to center node when clicked/dropped so node doesn't get lost when collapsing/moving with large amount of children.

        function centerNode(source) {
            scale = zoomListener.scale();
            x = -source.y0;
            y = -source.x0;
            x = 180;//x * scale + viewerWidth / 2; //check again            
            y = y * scale + viewerHeight / 2;
            d3.select('g').transition()
                .duration(duration)
                .attr("transform", "translate(" + x + "," + y + ")scale(" + scale + ")");
            zoomListener.scale(scale);
            zoomListener.translate([x, y]);
        }

        // Toggle children function

        function toggleChildren(d) {
            if (d.children) {
                d._children = d.children;
                d.children = null;
            } else if (d._children) {
                d.children = d._children;
                d._children = null;
            }
            return d;
        }

        // Toggle children on click.

        function click(d) {
            if (d3.event.defaultPrevented) return; // click suppressed
            d = toggleChildren(d);
            update(d);
            //centerNode(d); // check again
        }

        function SplitLabelAndAppend(d) {
            var labelText = '<tspan >' + (d.entity_name.split(':').length > 1 ? d.entity_name.split(':')[0] + ":" : d.entity_name) + '</tspan>';
            if (d.entity_name.split(':').length > 1) {
                labelText = labelText + '<tspan y="-10" x="27">' + d.entity_name.split(':')[1] + '</tspan>';
            }
            return labelText;
        }

        function update(source) {
            // Compute the new height, function counts total children of root node and sets tree height accordingly.
            // This prevents the layout looking squashed when new nodes are made visible or looking sparse when nodes are removed
            // This makes the layout more consistent.
            var levelWidth = [1];
            var childCount = function (level, n) {
                if (n.children && n.children.length > 0) {
                    if (levelWidth.length <= level + 1) levelWidth.push(0);
                    levelWidth[level + 1] += n.children.length;
                    n.children.forEach(function (d) {
                        childCount(level + 1, d);
                    });

                }
            };
            childCount(0, root);
            var newHeight = d3.max(levelWidth) * 60; //dj node height
            tree = tree.size([newHeight, viewerWidth]);

            // Compute the new tree layout.
            var nodes = tree.nodes(root);//.reverse(),
            links = tree.links(nodes);

            // Set widths between levels based on maxLabelLength.
            jsonNodeDetail = [];
            nodes.forEach(function (d) {
                nodeitem = {};
                //d.y = (d.depth * 280);// (maxLabelLength * 8)); //maxLabelLength * 10px //dj width betw label
                // alternatively to keep a fixed scale one can set a fixed depth per level
                // Normalize for fixed-depth by commenting out below line
                //console.log('depth:' + d.depth);
                //console.log('Previous Postion' + d.y);
                var typeWidth = d.entity_type.length;
                var marginWidth = d.entity_name.length > 20 ? 40 : 20;
                marginWidth = typeWidth > d.entity_name.length ? typeWidth + 40 : marginWidth;
                var labelWidth = getStringLengthInPixel(d.entity_name); // ggn-cbl0001(1 OUt)
                if (d.depth != 0) {
                    if (jsonNodeDetail[d.depth] != undefined) {
                        if (d.entity_name.length > (jsonNodeDetail[d.depth]["entity_name"] == undefined ? d.entity_name.length : jsonNodeDetail[d.depth]["entity_name"].length)) {
                            d.y = jsonNodeDetail[d.depth - 1]["depth"] + labelWidth + marginWidth;
                        }
                        else {
                            d.y = jsonNodeDetail[d.depth]["depth"];
                        }
                        jsonNodeDetail[d.depth]["depth"] = d.y;
                        jsonNodeDetail[d.depth]["entity_name"] = d.entity_name;
                    }
                    else {
                        d.y = (jsonNodeDetail[d.depth - 1]["depth"] + labelWidth + marginWidth);
                        nodeitem["depth"] = d.y;
                        nodeitem["entity_name"] = d.entity_name;
                        jsonNodeDetail.push(nodeitem);
                    }
                }
                else {
                    marginWidth = d.entity_name.length > 25 ? 50 : 45;
                    marginWidth = typeWidth > d.entity_name.length ? typeWidth + 40 : marginWidth;
                    d.y = d.depth + marginWidth; //500px per level.
                    nodeitem["depth"] = d.y;
                    nodeitem["entity_name"] = d.entity_name;
                    jsonNodeDetail.push(nodeitem);
                }
                //console.log(d.entity_name);
                //arrNodeLength.push(d.y);
                //console.log('New Postion' + d.y);
                //console.log('label Width:' + labelWidth);
            });

            // Update the nodes…
            node = svgGroup.selectAll("g.node")
                .data(nodes, function (d) {
                    return d.id || (d.id = ++i);
                });

            // Enter any new nodes at the parent's previous position.
            var nodeEnter = node.enter().append("g")
                .call(dragListener)
                .attr("class", "node")
                .attr("transform", function (d) {
                    return "translate(" + source.y0 + "," + source.x0 + ")";
                })
                .on('click', click);

            nodeEnter.append("circle")
                .attr('class', 'nodeCircle')
                .attr("r", 10)
                .style("fill", function (d) {
                    return d._children ? "green" : "#fff";
                });


            nodeEnter.append("text")
                .attr("x", function (d) {
                    return 76;//d.children || d._children ? -10 : 10;
                })
                .attr("dy", function (d) {
                    return -8;
                })  //firt line valign :: .attr("dy", ".25em")
                .attr('class', 'nodeText')
                .attr("text-anchor", function (d) {
                    return d.children || d._children ? "end" : "start";
                })
                .text(function (d) {
                    return d.entity_name;
                })
                .style("fill-opacity", 0);


            // phantom node to give us mouseover in a radius around it
            nodeEnter.append("circle")
                .attr('class', 'ghostCircle')
                .attr("r", 13)
                .attr("opacity", 0.2) // change this to zero to hide the target area
                .style("fill", "red")
                .attr('pointer-events', 'mouseover')
                .on("mouseover", function (node) {
                    overCircle(node);
                })
                .on("mouseout", function (node) {
                    outCircle(node);
                });

            //dj label
            // Update the text to reflect whether node has children or not.
            node.select('text')
                .attr("x", function (d) {
                    return 10;//d.children || d._children ? -10 : 10;
                })
                //.attr("y", function (d) {
                //    return d.entity_name.length > 25 ? -13 : 0;
                //})
                 //.attr("transform", function (d) {
                 //    return d.entity_name.length > 35 && !d.entity_type.includes("Equipment") ? 'rotate(3 0,' + d.entity_name.length * 2 + ')' : 'rotate(0 0,0)';
                 //})

                .attr("text-anchor", function (d) {
                    return "end";// d.children || d._children ? "end" : "start";
                })
                .attr("font-size", "11px")
                //first line
                .html(function (d) {
                    return SplitLabelAndAppend(d);
                    //return d.entity_name.split(':').length > 1 ? d.entity_name.split(':')[0] + " : ↓" : d.entity_name + " ↓";
                    //return d.entity_name.split(':').length > 1 ? d.entity_name.split(':')[0] + ":" : d.entity_name;
                    //return  d.entity_name;
                })
                 .style("font-weight", function (d) {
                     return d.entity_type.includes("Source") ? "Bold" : "Normal";
                 })
                 .style("fill", function (d) {
                     return d.entity_type.includes("Source") ? "green" : "";
                 })
                //second line
                .append('tspan')
                .text(function (d) {
                    return "[ " + d.entity_type + " ]";
                })
                .attr("x", 10)
                .attr("y", 20)
                //.attr("y", function (d) {
                //    return d.entity_type.includes("Source") || d.entity_type.includes("Splitter") || d.entity_type.includes("Splice") ? 18 : 15;
                //})              
                .attr('class', 'node2Text')
                //.style("fill",function (d) {
                //    // return d.entity_name.split('~')[1].includes("Splitter") ? "Red" : 'gray';
                //    return "";//d.entity_type.includes("Splitter") || d.entity_type.includes("Splice") ? "#f7981b" : d.entity_type.includes("Source") ? "#089603" : "gray";
                //}) 
                //.style("font-size", function (d) {
                //    return d.entity_type.includes("Source") || d.entity_type.includes("Splitter") || d.entity_type.includes("Splice") ? "12px" : "10px";
                //})
                //.style("font-weight", function (d) {
                //    return d.entity_type.includes("Source")  ? "Bold" : "Normal";
                //})

                 .style("font-weight", function (d) {
                     return d.entity_type.includes("Source") ? "Bold" : "Normal";
                 })
                 .style("fill", function (d) {
                     return d.entity_type.includes("Source") ? "green" : "";
                 })

                 //third line
                .append('tspan')
                .text(function (d) {
                    return d.comment != null ? d.comment.split(',')[0] : "";
                })
                .attr("x", function (d) {
                    return d.comment != null ? -60 : 0; //-60
                })
                .attr("y", 10)
                .attr('class', 'node2Text')
                .style("fill", "#f7981b")
                .style("font-size", "12px")

                // .append('tspan')
                //.text(function (d) {
                //    return d.comment != null ? d.comment.split(':')[1] : (d.entity_name.length > 30 ? d.entity_name.split(':')[1] : "");
                //})
                // //fourth line
                //.append('tspan')
                //.text(function (d) {
                //    return d.comment != null ? d.comment.split(':')[1] : (d.entity_name.length > 30 ? d.entity_name.split(':')[1] : "");
                //})





                .attr("x", function (d) {
                    return d.comment != null ? -50 : (d.entity_name.length > 35 ? 120 : 0);
                })
                .attr("y", function (d) {
                    return d.entity_name.length > 30 ? -12 : 22;
                })
                .attr('class', 'node2Text')
                .style("fill", function (d) {
                    return d.entity_name.length > 30 ? "#000" : "#f7981b";
                })
                .style("font-size", "12px")
            ;



            // Change the circle fill depending on whether it has children and is collapsed
            node.select("circle.nodeCircle")
                .attr("r", 5.5)
                .style("stroke-width", 2)
                .style("stroke", function (d) {
                    return d._children ? "#000" : "#333";
                })
                .style("fill", function (d) {
                    return d._children ? "green" : ((d.entity_type.includes("Splitter") || d.entity_type.includes("ONT")) ? "#f7611b" : "#fff");
                });

            // Transition nodes to their new position.
            var nodeUpdate = node.transition()
                .duration(duration)
                .attr("transform", function (d) {
                    return "translate(" + d.y + "," + d.x + ")";
                });

            // Fade the text in
            nodeUpdate.select("text")
                .style("fill-opacity", 1);
            // Transition exiting nodes to the parent's new position.
            var nodeExit = node.exit().transition()
                .duration(duration)
                .attr("transform", function (d) {
                    return "translate(" + source.y + "," + source.x + ")";
                })
                .remove();

            nodeExit.select("circle")
                .attr("r", 0);

            nodeExit.select("text")
                .style("fill-opacity", 0);

            // Update the links…
            var link = svgGroup.selectAll("path.link")
                .data(links, function (d) {
                    return d.target.id;
                });

            // Enter any new links at the parent's previous position.
            link.enter().insert("path", "g")
                .attr("class", "link")
                .attr("d", function (d) {
                    var o = {
                        x: source.x0,
                        y: source.y0
                    };
                    return diagonal({
                        source: o,
                        target: o
                    });
                });

            // Transition links to their new position.
            link.transition()
                .duration(duration)
                .attr("d", diagonal);

            // Transition exiting nodes to the parent's new position.
            link.exit().transition()
                .duration(duration)
                .attr("d", function (d) {
                    var o = {
                        x: source.x,
                        y: source.y
                    };
                    return diagonal({
                        source: o,
                        target: o
                    });
                })
                .remove();

            // Stash the old positions for transition.
            nodes.forEach(function (d) {
                d.x0 = d.x;
                d.y0 = d.y;
            });
        }
        // Append a group which holds all nodes and which the zoom Listener can act upon.
        var svgGroup = baseSvg.append("g");

        // Define the root
        root = treeData;
        root.x0 = viewerHeight / 2;
        root.y0 = 0;

        // Layout the tree initially and center on the root node.
        update(root);
        centerNode(root);
    });
}