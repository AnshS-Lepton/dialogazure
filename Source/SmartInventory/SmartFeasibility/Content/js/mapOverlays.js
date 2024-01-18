function EditMenu() {
    var menu = this;
    this.div_ = document.createElement('div');
    this.div_.className = 'edit-menu';
    this.div_.innerHTML = 'Edit';

    this.div_.addEventListener('click', function () {
        menu.editPath();
    });
}

EditMenu.prototype = new google.maps.OverlayView();

EditMenu.prototype.onAdd = function () {
    var editMenu = this;
    var map = this.getMap();
    this.getPanes().floatPane.appendChild(this.div_);

    this.divListener_ = map.getDiv().addEventListener('mousedown', function (e) {
        if (e.target != editMenu.div_) {
            editMenu.close();
        }
    }, true);
};

EditMenu.prototype.onRemove = function () {
    google.maps.event.removeListener(this.divListener_);
    this.div_.parentNode.removeChild(this.div_);

    this.set('position');
};

EditMenu.prototype.close = function () {
    this.setMap(null);
};

EditMenu.prototype.draw = function () {
    var position = this.get('position');
    var projection = this.getProjection();

    if (!position || !projection) {
        return;
    }

    var point = projection.fromLatLngToDivPixel(position);
    this.div_.style.top = point.y + 'px';
    this.div_.style.left = point.x + 'px';
};

EditMenu.prototype.open = function (map, polyline, pos) {
    if (polyline.editable)
        this.div_.innerHTML = 'Done Editing';
    else
        this.div_.innerHTML = 'Edit';

    this.set('polyline', polyline);
    this.set('position', pos);
    this.setMap(map);
    this.draw();
};

EditMenu.prototype.editPath = function () {
    var polyline = this.get('polyline');

    if (!polyline) {
        return;
    }
    if (!polyline.editable) {
        sf.resetPathEdits();
        polyline.setOptions({ editable: true });
    }
    else {
        polyline.setOptions({ editable: false });
    }
    sf.selectPath(sf.feasiblePaths[polyline.pathIndex]);

    this.close();
};

function DetailsMenu() {
    var menu = this;
    this.currentPath = undefined;
    this.div_ = document.createElement('div');
    this.div_.className = 'details-menu';
}

DetailsMenu.prototype = new google.maps.OverlayView();

DetailsMenu.prototype.onAdd = function () {
    var detailsMenu = this;
    var map = this.getMap();
    var pos = this.get('position');

    this.getPanes().floatPane.appendChild(this.div_);

    this.divListener_ = map.getDiv().addEventListener('mousedown', function (e) {

        if (e.target.parentNode.className == 'downloadFeasStatus') {
            sf.dowloadFeasibilityReport($(e.target.parentElement).data('type'));
        }
        else if (e.target.parentNode.className == 'SaveFeas') {
            sf.getFeasibilityDetails();
        }
        else if (e.target != detailsMenu.div_ && e.target.className != 'downloadArea' && e.target.className != 'feasiStatus') {
            detailsMenu.close();
        }

    }, true);
};

DetailsMenu.prototype.onRemove = function () {
    google.maps.event.removeListener(this.divListener_);
    this.div_.parentNode.removeChild(this.div_);

    this.set('position');
};

DetailsMenu.prototype.close = function () {
    this.isOpen = false;
    this.setMap(null);
};

DetailsMenu.prototype.draw = function () {
    if (!this.isOpen) {
        this.isOpen = true;
        var position = this.get('position');
        var projection = this.getProjection();

        if (!position || !projection) {
            return;
        }

        var point = projection.fromLatLngToDivPixel(position);
        this.div_.style.top = point.y + 'px';
        this.div_.style.left = point.x + 'px';
    }
};

DetailsMenu.prototype.open = function (map, polyline, pos) {
    let total_length = polyline.totalDistance ? polyline.totalDistance : 0;
    let outside_A_length = sf.FeasibilityDistances.outside_A_Distance ? sf.FeasibilityDistances.outside_A_Distance : 0;
    let outside_B_length = sf.FeasibilityDistances.outside_B_Distance ? sf.FeasibilityDistances.outside_B_Distance : 0;
    let inside_length = sf.FeasibilityDistances.insideDistance ? sf.FeasibilityDistances.insideDistance : 0;
    let inside_A_length = sf.FeasibilityDistances.inside_A_Distance ? sf.FeasibilityDistances.inside_A_Distance : 0;
    let inside_P_length = sf.FeasibilityDistances.inside_P_Distance ? sf.FeasibilityDistances.inside_P_Distance : 0;
    let lmc_start_length = sf.FeasibilityDistances.lmc_A_Distance ? sf.FeasibilityDistances.lmc_A_Distance : 0;
    let lmc_end_length = sf.FeasibilityDistances.lmc_B_Distance ? sf.FeasibilityDistances.lmc_B_Distance : 0;

    // find feasibility
    let totalInsideLength = inside_length + inside_P_length + inside_A_length;
    let percentInside = (totalInsideLength * 100 / total_length).toFixed(2);
    var feasibilityStatus = 'Not Feasible';
    if (percentInside > 80) {
        feasibilityStatus = 'Feasible';
    }
    else if (percentInside >= 50) {
        feasibilityStatus = 'Partially Feasible';
    }

    var coreFeasibilityStatus = 'Core level: ';

    if (sf.is_core_feasibile) {
        coreFeasibilityStatus += 'Feasible';
    }
    else {
        coreFeasibilityStatus += 'Not Feasible';
    }

    this.set('polyline', polyline);
    this.set('position', pos);

    var html = '<strong class="feasihead">Feasibility Details</strong>' +
        '<div class="downloadArea">' +
        '<span class="SaveFeas" data-type="SaveCable" title="Save Feasibility" style="cursor:pointer;"><i class="fa fa-save fa-lg" aria-hidden="true"></i></span>&nbsp;&nbsp;' +
        '<span class="downloadFeasStatus" data-type="BOM" title="Download BOM Report"><i class="fa fa-file-excel-o fa-lg" aria-hidden="true"></i></span>&nbsp;&nbsp;' +
        '<span class="downloadFeasStatus" data-type="XLS" title="Download Feasibility Report"><i class="fa fa-file-excel-o fa-lg" aria-hidden="true"></i></span>&nbsp;&nbsp;' +
        '<span class="downloadFeasStatus" data-type="PDF" title="Download PDF"><i class="fa fa-file-pdf-o fa-lg" aria-hidden="true"></i></span>&nbsp;&nbsp;' +
        '<span class="downloadFeasStatus" data-type="KML" title="Download KML"><i class="fa fa-map fa-lg" aria-hidden="true"></i></span>&nbsp;&nbsp;</div>' +
        '<div class="feasiStatus">' + feasibilityStatus + '</div>' +
        (totalInsideLength > 0 ? ('<div class="feasiStatus">' + coreFeasibilityStatus + '<br/></div><br/>') : '<br/>') +
        'Total Length: ' + sf.getDistanceString(total_length) + '<br/><br/>' +
        (inside_length ? ('Inside Length: ' + sf.getDistanceString(inside_length) + '<br/><br/>') : '') +
        (inside_P_length ? ('Inside Planned Length: ' + sf.getDistanceString(inside_P_length) + '<br/><br/>') : '') +
        (inside_A_length ? ('Inside As-Built Length: ' + sf.getDistanceString(inside_A_length) + '<br/><br/>') : '') +
        (outside_A_length ? ('Outside Length (A): ' + sf.getDistanceString(outside_A_length) + '<br/><br/>') : '') +
        (outside_B_length ? ('Outside Length (B): ' + sf.getDistanceString(outside_B_length) + '<br/><br/>') : '') +
        (lmc_start_length ? ('LMC Length (A): ' + sf.getDistanceString(lmc_start_length) + '<br/><br/>') : '') +
        (lmc_end_length ? ('LMC Length (B): ' + sf.getDistanceString(lmc_end_length) + '<br/><br/>') : '');

    $(this.div_).html(html);

    this.setMap(map);
    this.draw();
};

//FTTH DETAILS MENU

function DetailsMenuFTTH() {
    var menu = this;
    this.currentPath = undefined;
    this.div_ = document.createElement('div');
    this.div_.className = 'details-menu';
}

DetailsMenuFTTH.prototype = new google.maps.OverlayView();

DetailsMenuFTTH.prototype.onAdd = function () {
    var detailsMenuFtth = this;
    var map = this.getMap();
    var pos = this.get('position');

    this.getPanes().floatPane.appendChild(this.div_);

    this.divListener_ = map.getDiv().addEventListener('mousedown', function (e) {

        if (e.target.parentNode.className == 'SaveFeasFtth') {
            sf.getFeasibilityDetailsFtth();
        }
        else if (e.target != detailsMenuFtth.div_ && e.target.className != 'downloadArea' && e.target.className != 'feasiStatus') {
            detailsMenuFtth.close();
        }

    }, true);
};

DetailsMenuFTTH.prototype.onRemove = function () {
    google.maps.event.removeListener(this.divListener_);
    this.div_.parentNode.removeChild(this.div_);

    this.set('position');
};

DetailsMenuFTTH.prototype.close = function () {
    this.isOpen = false;
    this.setMap(null);
};

DetailsMenuFTTH.prototype.draw = function () {
    if (!this.isOpen) {
        this.isOpen = true;
        var position = this.get('position');
        var projection = this.getProjection();

        if (!position || !projection) {
            return;
        }

        var point = projection.fromLatLngToDivPixel(position);
        this.div_.style.top = point.y + 'px';
        this.div_.style.left = point.x + 'px';
    }
};

DetailsMenuFTTH.prototype.open = function (map, polyline, pos, totalDistance) {
    //let total_length = polyline.totalDistance ? polyline.totalDistance : 0;
    

    // find feasibility
    

    this.set('polyline', polyline);
    this.set('position', pos);

    var html = '<strong class="feasihead">Feasibility Details</strong>' +
        '<div class="downloadAreaFtth">' +
        '<div class="SaveFeasFtth" data-type="SaveCable" title="Save Feasibility" style="cursor:pointer;padding:5px 0 5px 0;border-bottom:0.5px solid grey"><i class="fa fa-save fa-lg" aria-hidden="true"></i></div>&nbsp;&nbsp;' +
        '<div id="dist-ftth" style="padding-bottom:10px;">Total Length: ' + totalDistance + '</div><br/><br/>';
    $(this.div_).html(html);

    this.setMap(map);
    this.draw();
};