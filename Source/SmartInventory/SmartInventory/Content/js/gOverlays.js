google.maps.Polygon.prototype.getBound = function () {
    var bounds = new google.maps.LatLngBounds()
    this.getPath().forEach(function (element, index) { bounds.extend(element) })
    return bounds;
}

// Define the overlay, derived from google.maps.OverlayView
google.maps.Blinker = function (opt_options) {
    // Initialization
    this.visible = !0;
    this.setValues(opt_options);

    var div = this.div_ = document.createElement('div');
    div.innerHTML = '<div class="crclAnim"></div><div class="fly"></div>';
    div.className = 'imhere';
}

google.maps.Blinker.prototype = new google.maps.OverlayView;

// Implement onAdd
google.maps.Blinker.prototype.onAdd = function () {
    var pane = this.getPanes().overlayImage;
    pane.appendChild(this.div_);
    // Ensures the Blinker is redrawn if the text or position is changed.
    var me = this;
    this.listeners_ = [
      google.maps.event.addListener(this, 'position_changed', function () { me.draw(); }),
      google.maps.event.addListener(this, 'visible_changed', function () { me.draw(); }),
      google.maps.event.addListener(this, 'clickable_changed', function () { me.draw(); }),
      google.maps.event.addListener(this, 'text_changed', function () { me.draw(); }),
      google.maps.event.addListener(this, 'zindex_changed', function () { me.draw(); }),
        this.div_.addEventListener('click', function () {
          if (me.get('clickable')) {
              google.maps.event.trigger(me, 'click');
          }
      })
    ];
};

// Implement onRemove
google.maps.Blinker.prototype.onRemove = function () {
    this.div_.parentNode.removeChild(this.div_);

    // Blinker is removed from the map, stop updating its position/text.
    for (var i = 0, I = this.listeners_.length; i < I; ++i) {
        google.maps.event.removeListener(this.listeners_[i]);
    }
};

// Implement draw
google.maps.Blinker.prototype.draw = function () {
    var projection = this.getProjection();
    var position = projection.fromLatLngToDivPixel(this.get('position'));

    var div = this.div_;
    div.style.left = position.x + 'px';
    div.style.top = position.y + 'px';
    div.style.display = this.visible ? 'block' : 'none';
};


// Define the overlay, derived from google.maps.OverlayView
google.maps.Label = function (opt_options) {
    // Initialization
    this.visible = !0;
    this.setValues(opt_options);
    this.Marker = opt_options.Marker;
    // Label specific
    var span = this.span_ = document.createElement('span');
    span.className = opt_options.cssName || "rLabel";

    //span.style.cssText = 'position: relative; left: -50%; top: -8px; ' +
    //'white-space: nowrap; border: 1px solid blue; ' +
    //'padding: 2px; background-color: white';
    var div = this.div_ = document.createElement('div');
    div.appendChild(span);
    div.style.cssText = 'position: absolute; display: none';
}

google.maps.Label.prototype = new google.maps.OverlayView;

// Implement onAdd
google.maps.Label.prototype.onAdd = function () {
    var pane = this.getPanes().overlayImage;
    pane.appendChild(this.div_);
    var _me = this;
    if (this.Marker)
        google.maps.event.addListener(this.Marker, 'dragend', function () {
            me.set('position', me.Marker.get('position'));
        });

    // Ensures the label is redrawn if the text or position is changed.
    var me = this;
    this.listeners_ = [
      google.maps.event.addListener(this, 'position_changed', function () { me.draw(); }),
      google.maps.event.addListener(this, 'visible_changed', function () { me.draw(); }),
      google.maps.event.addListener(this, 'clickable_changed', function () { me.draw(); }),
      google.maps.event.addListener(this, 'text_changed', function () { me.draw(); }),
      google.maps.event.addListener(this, 'zindex_changed', function () { me.draw(); }),
        this.div_.addEventListener('click', function () {
          if (me.get('clickable')) {
              google.maps.event.trigger(me, 'click');
          }
      })
    ];
};


// Implement onRemove
google.maps.Label.prototype.onRemove = function () {
    this.div_.parentNode.removeChild(this.div_);

    // Label is removed from the map, stop updating its position/text.
    for (var i = 0, I = this.listeners_.length; i < I; ++i) {
        google.maps.event.removeListener(this.listeners_[i]);
    }
};

// Implement draw
google.maps.Label.prototype.draw = function () {
    var projection = this.getProjection();
    var position = projection.fromLatLngToDivPixel(this.get('position'));

    var div = this.div_;
    div.style.left = position.x + 'px';
    div.style.top = position.y + 'px';
    div.style.display = this.visible ? 'block' : 'none';

    this.span_.innerHTML = this.get('text');
};


// Define the overlay, derived from google.maps.OverlayView
google.maps.RMarker = function (opt_options) {
    // Initialization    
    this.ready_ = false;
    this.dragging_ = false;
    this.Label = opt_options.Label;
    this.visible = !0;
    this.text = '';
    this.title = '';
    this.clickable = true,
    this.setValues(opt_options);

    // RMarker specific
    var span = this.span_ = document.createElement('span');
    span.className = "RM " + (opt_options.cssName || "flaticon-pins53");
    span.style.cssText = 'position: absolute; display: none;color:' + (opt_options.color || '#000;');
}

google.maps.RMarker.prototype = new google.maps.OverlayView;

// Implement onAdd
google.maps.RMarker.prototype.onAdd = function () {
    var pane = this.getPanes().overlayImage;
    pane.appendChild(this.span_);


    // Ensures the RMarker is redrawn if the text or position is changed.
    var me = this;
    this.listeners_ = [   
      google.maps.event.addListener(this, 'position_changed', function () { me.draw(); }),
      google.maps.event.addListener(this, 'visible_changed', function () { me.draw(); }),
      google.maps.event.addListener(this, 'clickable_changed', function () { me.draw(); }),
      google.maps.event.addListener(this, 'text_changed', function () { me.draw(); }),
      google.maps.event.addListener(this, 'zindex_changed', function () { me.draw(); }),
        this.span_.addEventListener('click', function () {
          if (me.clickable) {
              google.maps.event.trigger(me, 'click');
          }
      })
    ];
    this.ready_ = true;
    this.draggable_changed();
};

// Implement onRemove
google.maps.RMarker.prototype.onRemove = function () {
    this.span_.parentNode.removeChild(this.span_);
    // RMarker is removed from the map, stop updating its position/text.

    if (this.Label)
        this.Label.setMap(null);

    for (var i = 0, I = this.listeners_.length; i < I; ++i) {
        google.maps.event.removeListener(this.listeners_[i]);
    }
};

// Implement draw
google.maps.RMarker.prototype.draw = function () {
    if (!this.ready_ || this.dragging_) {
        return;
    }

    var projection = this.getProjection();
    var position = projection.fromLatLngToDivPixel(this.get('position'));

    if (this.Label) {
        this.Label.set('position', this.get('position'));     
    }

    var spn = this.span_;
    spn.innerHTML = this.text;
    spn.title = this.title;
    spn.style.left = position.x + 'px';
    spn.style.top = position.y + 'px';
    spn.style.display = this.visible ? 'block' : 'none';

};

google.maps.RMarker.prototype.updateColor = function (_clr) {
    this.span_.style.color = _clr;
};

google.maps.RMarker.prototype.getDraggable = function () {
    return  (this.get('draggable'));
};

google.maps.RMarker.prototype['getDraggable'] = google.maps.RMarker.prototype.getDraggable;


google.maps.RMarker.prototype.setDraggable = function (draggable) {
    this.set('draggable', !!draggable);
};

google.maps.RMarker.prototype['setDraggable'] = google.maps.RMarker.prototype.setDraggable;

google.maps.RMarker.prototype.draggable_changed = function () {
    if (this.ready_) {
        if (this.getDraggable()) {
            this.addDragging_(this.span_);
        } else {
            this.removeDragListeners_();
        }
    }
};
google.maps.RMarker.prototype['draggable_changed'] = google.maps.RMarker.prototype.draggable_changed;


google.maps.RMarker.prototype.setCursor_ = function (whichCursor) {
    if (!this.ready_) {
        return;
    }

    var cursor = '';
    if (navigator.userAgent.indexOf('Gecko/') !== -1) {
        // Moz has some nice cursors :)
        if (whichCursor == 'dragging') {
            cursor = '-moz-grabbing';
        }

        if (whichCursor == 'dragready') {
            cursor = '-moz-grab';
        }

        if (whichCursor == 'draggable') {
            cursor = 'pointer';
        }
    } else {
        if (whichCursor == 'dragging' || whichCursor == 'dragready') {
            cursor = 'move';
        }

        if (whichCursor == 'draggable') {
            cursor = 'pointer';
        }
    }

    if (this.span_.style.cursor != cursor) {
        this.span_.style.cursor = cursor;
    }
};


google.maps.RMarker.prototype.startDrag = function (e) {
    if (!this.getDraggable()) {
        return;
    }

    if (!this.dragging_) {
        this.dragging_ = true;
        var map = this.getMap();
        this.mapDraggable_ = map.get('draggable');
        map.set('draggable', false);

        // Store the current mouse position
        this.mouseX_ = e.clientX;
        this.mouseY_ = e.clientY;

        this.setCursor_('dragready');

        // Stop the text from being selectable while being dragged
        this.span_.style['MozUserSelect'] = 'none';
        this.span_.style['KhtmlUserSelect'] = 'none';
        this.span_.style['WebkitUserSelect'] = 'none';

        this.span_['unselectable'] = 'on';
        this.span_['onselectstart'] = function () {
            return false;
        };

        this.addDraggingListeners_();

        google.maps.event.trigger(this, 'dragstart');
    }
};


/**
 * Stop dragging.
 */
google.maps.RMarker.prototype.stopDrag = function () {
    if (!this.getDraggable()) {
        return;
    }

    if (this.dragging_) {
        this.dragging_ = false;
        this.getMap().set('draggable', this.mapDraggable_);
        this.mouseX_ = this.mouseY_ = this.mapDraggable_ = null;

        // Allow the text to be selectable again
        this.span_.style['MozUserSelect'] = '';
        this.span_.style['KhtmlUserSelect'] = '';
        this.span_.style['WebkitUserSelect'] = '';
        this.span_['unselectable'] = 'off';
        this.span_['onselectstart'] = function () { };

        this.removeDraggingListeners_();

        this.setCursor_('draggable');
        google.maps.event.trigger(this, 'dragend');

        this.draw();
    }
};


google.maps.RMarker.prototype.drag = function (e) {
    if (!this.getDraggable() || !this.dragging_) {
        // This object isn't draggable or we have stopped dragging
        this.stopDrag();
        return;
    }

    var dx = this.mouseX_ - e.clientX;
    var dy = this.mouseY_ - e.clientY;

    this.mouseX_ = e.clientX;
    this.mouseY_ = e.clientY;

    var left = parseInt(this.span_.style['left'], 10) - dx;
    var top = parseInt(this.span_.style['top'], 10) - dy;

    this.span_.style['left'] = left + 'px';
    this.span_.style['top'] = top + 'px';

    // Set the position property and adjust for the anchor offset
    var point = new google.maps.Point(left + this.span_.offsetWidth - 24, top + this.span_.offsetHeight - 24);
    var projection = this.getProjection();
    this.set('position', projection.fromDivPixelToLatLng(point));

    this.setCursor_('dragging');
    google.maps.event.trigger(this, 'drag');
};

/**
 * Removes the drag listeners associated with the marker.
 *
 * @private
 */
google.maps.RMarker.prototype.removeDragListeners_ = function () {
    if (this.draggableListener_) {
        google.maps.event.removeListener(this.draggableListener_);
        delete this.draggableListener_;
    }
    this.setCursor_('');
};


/**
 * Add dragability events to the marker.
 *
 * @param {Node} node The node to apply dragging to.
 * @private
 */
google.maps.RMarker.prototype.addDragging_ = function (node) {
    if (!node) {
        return;
    }

    var that = this;
    this.draggableListener_ =
        node.addEventListener('mousedown', function (e) {
          that.startDrag(e);
      });

    this.setCursor_('draggable');
};


/**
 * Add dragging listeners.
 *
 * @private
 */
google.maps.RMarker.prototype.addDraggingListeners_ = function () {
    var that = this;
    if (this.span_.setCapture) {
        this.span_.setCapture(true);
        this.draggingListeners_ = [
            this.span_.addEventListener('mousemove', function (e) {
              that.drag(e);
          }, true),
            this.span_.addEventListener('mouseup', function () {
              that.stopDrag();
              that.span_.releaseCapture();
          }, true)
        ];
    } else {
        this.draggingListeners_ = [
          window.addEventListener('mousemove', function (e) {
              that.drag(e);
          }, true),
            window.addEventListener('mouseup', function () {
              that.stopDrag();
          }, true)
        ];
    }
};


/**
 * Remove dragging listeners.
 *
 * @private
 */
google.maps.RMarker.prototype.removeDraggingListeners_ = function () {
    if (this.draggingListeners_) {
        for (var i = 0, listener; listener = this.draggingListeners_[i]; i++) {
            google.maps.event.removeListener(listener);
        }
        this.draggingListeners_.length = 0;
    }
};



