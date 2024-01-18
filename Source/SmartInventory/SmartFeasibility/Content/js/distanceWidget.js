function DistanceWidget(opt_options) {
    var options = opt_options || {};

    this.setValues(options);

    if (!this.get('position')) {
        this.set('position', opt_options.center);
    }

    // Add a marker to the page at the map center or specified position
    var marker = opt_options.marker;

    marker.bindTo('map', this);
    marker.bindTo('zIndex', this);
    marker.bindTo('position', this);

    // Create a new radius widget
    var radiusWidget = new RadiusWidget(options['distance'] || 50);

    // Bind the radius widget properties.
    radiusWidget.bindTo('center', this, 'position');
    radiusWidget.bindTo('map', this);
    radiusWidget.bindTo('zIndex', marker);

    this.bindTo('distance', radiusWidget);
    this.bindTo('bounds', radiusWidget);
    var me = this;
    google.maps.event.addListener(marker, 'dragend', function (e) {
        me.fitCenter();
    });

    sf.map.fitBounds(me.get('bounds'));
    this.gCircle = radiusWidget.gCircle;
}

// prototype
DistanceWidget.prototype = new google.maps.MVCObject();

//To set the widget at center 
DistanceWidget.prototype.fitCenter = function () {
    var new_center_point = this.get('position')
    sf.map.setCenter(new_center_point);
};

function RadiusWidget(opt_distance) {
    var circle = new google.maps.Circle({
        strokeWeight: 1,
        strokeColor: '#e73571',//#50b3ea',
        fillColor: '#e73571',//'#50b3ea',
        fillOpacity: 0.10,
        clickable: false
    });

    this.set('distance', opt_distance);
    this.set('active', false);
    this.bindTo('bounds', circle);

    circle.bindTo('center', this);
    circle.bindTo('zIndex', this);
    circle.bindTo('map', this);
    circle.bindTo('radius', this);
    this.gCircle = circle;
}

RadiusWidget.prototype = new google.maps.MVCObject();

RadiusWidget.prototype.distance_changed = function () {
    this.set('radius', this.get('distance') * 1000);
};
