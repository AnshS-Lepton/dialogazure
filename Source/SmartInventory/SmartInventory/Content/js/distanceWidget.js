function Distance_Widget(opt_options) {
    var options = opt_options || {};

    this.setValues(options);

    if (!this.get('position')) {
        this.set('position', opt_options.center);
    }

    // Add a marker to the page at the map center or specified position
    var marker = opt_options.marker;
    ;
    marker.bindTo('map', this);
    marker.bindTo('zIndex', this);
    marker.bindTo('position', this);
    

    // Create a new radius widget
    var radiusWidget = new Radius_Widget(options['distance'] || 0 , marker);
    ;
    if (options['distance'] == 0) {
        marker.setLabel("");
    }
    // Bind the radius widget properties.
    radiusWidget.bindTo('center', this, 'position');
    radiusWidget.bindTo('map', this);
    radiusWidget.bindTo('zIndex', marker);

    this.bindTo('distance', radiusWidget);
    this.bindTo('bounds', radiusWidget);
    var me = this;

    //google.maps.event.addListener(marker, 'dragend', function (e) {
    //    me.fitCenter();
    //});

   // si.map.fitBounds(me.get('bounds'));
    this.gCircle = radiusWidget.gCircle;
}

// prototype
Distance_Widget.prototype = new google.maps.MVCObject();

//To set the widget at center 
//DistanceWidget.prototype.fitCenter = function () {
//    var new_center_point = this.get('position')
//    si.map.setCenter(new_center_point);
//};

function Radius_Widget(opt_distance, marker) {
    var circle = new google.maps.Circle({
        strokeWeight: 0.5,
        strokeColor: '#EE9A4D',
        fillColor: '#EE9A4D',
        fillOpacity: 0.4,
        clickable: true,
        draggable: true
    });
    if (opt_distance > 0) {
        marker.setLabel(SetRadiusCircleLabel(opt_distance));
    }

    this.set('distance', opt_distance);
    this.set('active', false);
    this.bindTo('bounds', circle);
    circle.bindTo('center', this);
    circle.bindTo('zIndex', this);
    circle.bindTo('map', this);
    circle.bindTo('radius', this);
    this.gCircle = circle;
}

Radius_Widget.prototype = new google.maps.MVCObject();

Radius_Widget.prototype.distance_changed = function () {
    this.set('radius', this.get('distance') * 1);
};


//To Set Label
function SetRadiusCircleLabel(rds) {
    return {
        color: '#050404',
        fontWeight: 'bold',
        fontSize: '10px',
        text: String("Radius : " + rds +" (meter)"),
        className: 'marker-label-margin'
    }
}
