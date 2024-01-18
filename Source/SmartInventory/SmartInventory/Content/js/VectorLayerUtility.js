class VectorLayerUtil {
    constructor() {
        let GeoJsonLayer = deck.GeoJsonLayer;
        let GoogleMapsOverlay = deck.GoogleMapsOverlay;
        let DataFilterExtension = deck.DataFilterExtension;
        let ICON_MAPPING = null;
        //Variables to Store Point Entity Json
        this.poleGeoJson = {};
        this.manholeGeoJson = {};
        this.wallmountGeoJson = {};
        this.fdbGeoJson = {};
        this.splitterGeoJson = {};
        this.bdbGeoJson = {};
        this.adbGeoJson = {};
        this.spliceclosureGeoJson = {};
        //Variables to Store Line Entity Json
        this.cableGeoJson = {};
        //Variables to Store PolyGon Entity Json
        this.areaGeoJson = {};
        this.subareaGeoJson = {};
        this.dsaGeoJson = {};
        this.csaGeoJson = {};
        //Variables to Store PolyGon Entity Json
        this.poleVectorLayer = null;
        this.manholeVectorLayer = null;
        this.wallmountVectorLayer = null;
        this.splitterVectorLayer = null;
        this.fdbVectorLayer = null;
        this.bdbVectorLayer = null;
        this.adbVectorLayer = null;
        this.spliceclosureVectorLayer = null;
        this.dsaVectorLayer = null;
        this.csaVectorLayer = null;
        this.subareaVectorLayer = null;
        this.areaVectorLayer = null;
        this.cableVectorLayer = null;
        //Variables to Store Selected Layers to Load
        this.ActivePlannedVectorlayers = [];
        this.ActiveAsBuiltVectorlayers = [];
        this.PolygonVectorlayers = [];
        this.ActivePlannedVectorlayersWithLabels = [];
        this.ActiveAsBuiltVectorlayersWithLabels = [];
        this.PolygonVectorlayersWithLabel = [];

        this.vectorFetchTime = null;
        this.VectorDeltaRequestInProcess = false;
        this.vectorFSAID = 66;
        this.minLayerIndex = 99;
        this.provinceLimitToStoreDatainBrowser = 2;
        this.VectorInfoWindow = new google.maps.InfoWindow({ pixelOffset: new google.maps.Size(40, -40) });
        this.IsActionEnabled = false;
    }

    fetchVectorLayerData(vectorPrvinceSelected) {
        console.log("Request Fetch Time:" + app.vectorFetchTime);
        let requestResult = new Promise(function (resolve, reject) {
            ajaxReq('VectorLayer/GetVectorGeojson', { vectorPrvinceIds: vectorPrvinceSelected }, true, function (resp) {
                if (resp.status == 'OK') {
                    console.log(resp);
                    let layersData = resp.results.LayersData;
                    app.vectorFetchTime = resp.results.FetchDateTime;
                    const allLayerVector = layersData.reduce((acc, obj) => {
                        const key = obj.layer;
                        if (!acc[key]) {
                            acc[key] = [];
                        }
                        acc[key].push(obj.feature);
                        return acc;
                    }, {});
                    if (app.poleGeoJson.features) {
                        app.poleGeoJson = { "type": "FeatureCollection", "features": app.poleGeoJson.features.concat(allLayerVector.Pole ? allLayerVector.Pole : []) };
                    } else {
                        app.poleGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Pole };
                    }

                    if (app.manholeGeoJson.features) {
                        app.manholeGeoJson = { "type": "FeatureCollection", "features": app.manholeGeoJson.features.concat(allLayerVector.Manhole ? allLayerVector.Manhole : []) };
                    } else {
                        app.manholeGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Manhole };
                    }

                    if (app.wallmountGeoJson.features) {
                        app.wallmountGeoJson = { "type": "FeatureCollection", "features": app.wallmountGeoJson.features.concat(allLayerVector.WallMount ? allLayerVector.WallMount : []) };
                    } else {
                        app.wallmountGeoJson = { "type": "FeatureCollection", "features": allLayerVector.WallMount };
                    }

                    if (app.fdbGeoJson.features) {
                        app.fdbGeoJson = { "type": "FeatureCollection", "features": app.fdbGeoJson.features.concat(allLayerVector.FDB ? allLayerVector.FDB : []) };
                    } else {
                        app.fdbGeoJson = { "type": "FeatureCollection", "features": allLayerVector.FDB };
                    }

                    if (app.bdbGeoJson.features) {
                        app.bdbGeoJson = { "type": "FeatureCollection", "features": app.bdbGeoJson.features.concat(allLayerVector.BDB ? allLayerVector.BDB : []) };
                    } else {
                        app.bdbGeoJson = { "type": "FeatureCollection", "features": allLayerVector.BDB };
                    }

                    if (app.adbGeoJson.features) {
                        app.adbGeoJson = { "type": "FeatureCollection", "features": app.adbGeoJson.features.concat(allLayerVector.ADB ? allLayerVector.ADB : []) };
                    } else {
                        app.adbGeoJson = { "type": "FeatureCollection", "features": allLayerVector.ADB };
                    }

                    if (app.spliceclosureGeoJson.features) {
                        app.spliceclosureGeoJson = { "type": "FeatureCollection", "features": app.spliceclosureGeoJson.features.concat(allLayerVector.SpliceClosure ? allLayerVector.SpliceClosure : []) };
                    } else {
                        app.spliceclosureGeoJson = { "type": "FeatureCollection", "features": allLayerVector.SpliceClosure };
                    }

                    if (app.splitterGeoJson.features) {
                        app.splitterGeoJson = { "type": "FeatureCollection", "features": app.splitterGeoJson.features.concat(allLayerVector.Splitter ? allLayerVector.Splitter : []) };
                    } else {
                        app.splitterGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Splitter };
                    }

                    if (app.areaGeoJson.features) {
                        app.areaGeoJson = { "type": "FeatureCollection", "features": app.areaGeoJson.features.concat(allLayerVector.Area ? allLayerVector.Area : []) };
                    } else {
                        app.areaGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Area };
                    }


                    if (app.subareaGeoJson.features) {
                        app.subareaGeoJson = { "type": "FeatureCollection", "features": app.subareaGeoJson.features.concat(allLayerVector.SubArea ? allLayerVector.SubArea : []) };
                    } else {
                        app.subareaGeoJson = { "type": "FeatureCollection", "features": allLayerVector.SubArea };
                    }

                    if (app.dsaGeoJson.features) {
                        app.dsaGeoJson = { "type": "FeatureCollection", "features": app.dsaGeoJson.features.concat(allLayerVector.DSA ? allLayerVector.DSA : []) };
                    } else {
                        app.dsaGeoJson = { "type": "FeatureCollection", "features": allLayerVector.DSA };
                    }

                    if (app.csaGeoJson.features) {
                        app.csaGeoJson = { "type": "FeatureCollection", "features": app.csaGeoJson.features.concat(allLayerVector.CSA ? allLayerVector.CSA : []) };
                    } else {
                        app.csaGeoJson = { "type": "FeatureCollection", "features": allLayerVector.CSA };
                    }

                    if (app.cableGeoJson.features) {
                        app.cableGeoJson = { "type": "FeatureCollection", "features": app.cableGeoJson.features.concat(allLayerVector.Cable ? allLayerVector.Cable : []) };
                    } else {
                        app.cableGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Cable };
                    }


                    //app.manholeGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Manhole };
                    //app.wallmountGeoJson = { "type": "FeatureCollection", "features": allLayerVector.WallMount };
                    //app.fdbGeoJson = { "type": "FeatureCollection", "features": allLayerVector.FDB };
                    //app.bdbGeoJson = { "type": "FeatureCollection", "features": allLayerVector.BDB };
                    //app.adbGeoJson = { "type": "FeatureCollection", "features": allLayerVector.ADB };
                    //app.spliceclosureGeoJson = { "type": "FeatureCollection", "features": allLayerVector.SpliceClosure };
                    //app.splitterGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Splitter };

                    //app.cableGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Cable };

                    //app.areaGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Area };
                    //app.subareaGeoJson = { "type": "FeatureCollection", "features": allLayerVector.SubArea };
                    //app.dsaGeoJson = { "type": "FeatureCollection", "features": allLayerVector.DSA };
                    //app.csaGeoJson = { "type": "FeatureCollection", "features": allLayerVector.CSA };
                    resolve("Data Loading work completed");
                }
                else {
                    console.log('error');
                    reject("There is some error");
                }

            }, true, true);
        });
        requestResult.then(function successValue(result) {
            console.log(result);
            Array.prototype.push.apply(app.provinceListData, app.ActiveProvincelayers);
            console.log("Data loaded for below province:");
            console.log(app.provinceListData);
            app.LoadVectorLayers();
        });
    }

    fetchVectorLayerData = function (vectorPrvinceSelected) {
        console.log("Request Fetch Time:" + app.vectorFetchTime);
        let requestResult = new Promise(function (resolve, reject) {
            ajaxReq('VectorLayer/GetVectorGeojson', { vectorPrvinceIds: vectorPrvinceSelected }, true, function (resp) {
                if (resp.status == 'OK') {
                    console.log(resp);
                    let layersData = resp.results.LayersData;
                    app.vectorFetchTime = resp.results.FetchDateTime;
                    const allLayerVector = layersData.reduce((acc, obj) => {
                        const key = obj.layer;
                        if (!acc[key]) {
                            acc[key] = [];
                        }
                        acc[key].push(obj.feature);
                        return acc;
                    }, {});
                    if (app.poleGeoJson.features) {
                        app.poleGeoJson = { "type": "FeatureCollection", "features": app.poleGeoJson.features.concat(allLayerVector.Pole ? allLayerVector.Pole : []) };
                    } else {
                        app.poleGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Pole };
                    }

                    if (app.manholeGeoJson.features) {
                        app.manholeGeoJson = { "type": "FeatureCollection", "features": app.manholeGeoJson.features.concat(allLayerVector.Manhole ? allLayerVector.Manhole : []) };
                    } else {
                        app.manholeGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Manhole };
                    }

                    if (app.wallmountGeoJson.features) {
                        app.wallmountGeoJson = { "type": "FeatureCollection", "features": app.wallmountGeoJson.features.concat(allLayerVector.WallMount ? allLayerVector.WallMount : []) };
                    } else {
                        app.wallmountGeoJson = { "type": "FeatureCollection", "features": allLayerVector.WallMount };
                    }

                    if (app.fdbGeoJson.features) {
                        app.fdbGeoJson = { "type": "FeatureCollection", "features": app.fdbGeoJson.features.concat(allLayerVector.FDB ? allLayerVector.FDB : []) };
                    } else {
                        app.fdbGeoJson = { "type": "FeatureCollection", "features": allLayerVector.FDB };
                    }

                    if (app.bdbGeoJson.features) {
                        app.bdbGeoJson = { "type": "FeatureCollection", "features": app.bdbGeoJson.features.concat(allLayerVector.BDB ? allLayerVector.BDB : []) };
                    } else {
                        app.bdbGeoJson = { "type": "FeatureCollection", "features": allLayerVector.BDB };
                    }

                    if (app.adbGeoJson.features) {
                        app.adbGeoJson = { "type": "FeatureCollection", "features": app.adbGeoJson.features.concat(allLayerVector.ADB ? allLayerVector.ADB : []) };
                    } else {
                        app.adbGeoJson = { "type": "FeatureCollection", "features": allLayerVector.ADB };
                    }

                    if (app.spliceclosureGeoJson.features) {
                        app.spliceclosureGeoJson = { "type": "FeatureCollection", "features": app.spliceclosureGeoJson.features.concat(allLayerVector.SpliceClosure ? allLayerVector.SpliceClosure : []) };
                    } else {
                        app.spliceclosureGeoJson = { "type": "FeatureCollection", "features": allLayerVector.SpliceClosure };
                    }

                    if (app.splitterGeoJson.features) {
                        app.splitterGeoJson = { "type": "FeatureCollection", "features": app.splitterGeoJson.features.concat(allLayerVector.Splitter ? allLayerVector.Splitter : []) };
                    } else {
                        app.splitterGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Splitter };
                    }

                    if (app.areaGeoJson.features) {
                        app.areaGeoJson = { "type": "FeatureCollection", "features": app.areaGeoJson.features.concat(allLayerVector.Area ? allLayerVector.Area : []) };
                    } else {
                        app.areaGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Area };
                    }


                    if (app.subareaGeoJson.features) {
                        app.subareaGeoJson = { "type": "FeatureCollection", "features": app.subareaGeoJson.features.concat(allLayerVector.SubArea ? allLayerVector.SubArea : []) };
                    } else {
                        app.subareaGeoJson = { "type": "FeatureCollection", "features": allLayerVector.SubArea };
                    }

                    if (app.dsaGeoJson.features) {
                        app.dsaGeoJson = { "type": "FeatureCollection", "features": app.dsaGeoJson.features.concat(allLayerVector.DSA ? allLayerVector.DSA : []) };
                    } else {
                        app.dsaGeoJson = { "type": "FeatureCollection", "features": allLayerVector.DSA };
                    }

                    if (app.csaGeoJson.features) {
                        app.csaGeoJson = { "type": "FeatureCollection", "features": app.csaGeoJson.features.concat(allLayerVector.CSA ? allLayerVector.CSA : []) };
                    } else {
                        app.csaGeoJson = { "type": "FeatureCollection", "features": allLayerVector.CSA };
                    }

                    if (app.cableGeoJson.features) {
                        app.cableGeoJson = { "type": "FeatureCollection", "features": app.cableGeoJson.features.concat(allLayerVector.Cable ? allLayerVector.Cable : []) };
                    } else {
                        app.cableGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Cable };
                    }
                    resolve("Data Loading work completed");
                }
                else {
                    console.log('error');
                    reject("There is some error");
                }

            }, true, true);
        });
        requestResult.then(function successValue(result) {
            console.log(result);
            Array.prototype.push.apply(app.provinceListData, app.ActiveProvincelayers);
            console.log("Data loaded for below province:");
            console.log(app.provinceListData);
            app.LoadVectorLayers();
        });
    }

    fetchVectorDelta = function () {
        if (app.VectorDeltaRequestInProcess) {
            return true;
        }
        app.ActiveProvincelayers = app.getActiveProvinceLayers();
        let vectorPrvinceSelected = app.ActiveProvincelayers.join(",");
        console.log(vectorPrvinceSelected);
        console.log("Request Fetch Time:" + app.vectorFetchTime);
        app.VectorDeltaRequestInProcess = true;
        ajaxReq('VectorLayer/GetVectorDelta', { lastFetchTime: (app.vectorFetchTime ? app.vectorFetchTime : ''), vectorPrvinceIds: vectorPrvinceSelected, vectorFSAID: app.vectorFSAID }, true, function (resp) {
            let isDelta = false;
            //google.maps.event.clearListeners(app.map, evt);
            if (resp.status == 'OK') {
                app.VectorDeltaRequestInProcess = false;
                console.log(resp);
                //if (resp.results && resp.results.length){
                //    app.poleVector = resp.results[0];
                //}

                let layersData = resp.results.LayersData;
                app.vectorFetchTime = resp.results.FetchDateTime;
                console.log("Response Fetch Time:" + resp.results.FetchDateTime);
                const allLayerVector = layersData.reduce((acc, obj) => {
                    const key = obj.layer;
                    if (!acc[key]) {
                        acc[key] = [];
                    }
                    acc[key].push(obj.feature);
                    return acc;
                }, {});
                if (allLayerVector.Area) {
                    app.areaGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Area };
                    isDelta = true;
                    app.RenderVectorLayer(app.layestList.indexOf("Area"));
                }
                if (allLayerVector.SubArea) {
                    app.subareaGeoJson = { "type": "FeatureCollection", "features": allLayerVector.SubArea };
                    isDelta = true;
                    app.RenderVectorLayer(app.layestList.indexOf("SubArea"));
                }
                if (allLayerVector.DSA) {
                    app.dsaGeoJson = { "type": "FeatureCollection", "features": allLayerVector.DSA };
                    isDelta = true;
                    app.RenderVectorLayer(app.layestList.indexOf("DSA"));
                }
                if (allLayerVector.CSA) {
                    app.csaGeoJson = { "type": "FeatureCollection", "features": allLayerVector.CSA };
                    isDelta = true;
                    app.RenderVectorLayer(app.layestList.indexOf("CSA"));
                }
                //console.log(allLayerVector);
                console.log("Before the Pole");
                if (allLayerVector.Pole) {
                    console.log("within the Pole");
                    app.poleGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Pole };
                    //app.areaGeoJson.features = app.areaGeoJson.features.concat(allLayerVector.Pole);
                    isDelta = true;
                    app.RenderVectorLayer(app.layestList.indexOf("Pole"));
                }
                if (allLayerVector.Manhole) {
                    app.manholeGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Manhole };
                    isDelta = true;
                    app.RenderVectorLayer(app.layestList.indexOf("Manhole"));
                }
                if (allLayerVector.WallMount) {
                    app.wallmountGeoJson = { "type": "FeatureCollection", "features": allLayerVector.WallMount };
                    isDelta = true;
                    app.RenderVectorLayer(app.layestList.indexOf("WallMount"));
                }
                if (allLayerVector.FDB) {
                    app.fdbGeoJson = { "type": "FeatureCollection", "features": allLayerVector.FDB };
                    isDelta = true;
                    app.RenderVectorLayer(app.layestList.indexOf("FDB"));
                }
                if (allLayerVector.BDB) {
                    app.bdbGeoJson = { "type": "FeatureCollection", "features": allLayerVector.BDB };
                    isDelta = true;
                    app.RenderVectorLayer(app.layestList.indexOf("BDB"));
                }
                if (allLayerVector.Splitter) {
                    app.splitterGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Splitter };
                    isDelta = true;
                    app.RenderVectorLayer(app.layestList.indexOf("Splitter"));
                }
                if (allLayerVector.ADB) {
                    app.adbGeoJson = { "type": "FeatureCollection", "features": allLayerVector.ADB };
                    isDelta = true;
                    app.RenderVectorLayer(app.layestList.indexOf("ADB"));
                }
                if (allLayerVector.SpliceClosure) {
                    app.spliceclosureGeoJson = { "type": "FeatureCollection", "features": allLayerVector.SpliceClosure };
                    isDelta = true;
                    app.RenderVectorLayer(app.layestList.indexOf("SpliceClosure"));
                }

                if (allLayerVector.Cable) {
                    app.cableGeoJson = { "type": "FeatureCollection", "features": allLayerVector.Cable };
                    isDelta = true;
                    app.RenderVectorLayer(app.layestList.indexOf("Cable"));
                }


                console.log("app.minLayerIndex-" + app.minLayerIndex);
                //alert(app.minLayerIndex);
                if (isDelta && app.minLayerIndex != 99) {
                    //app.RenderVectorLayer(app.minLayerIndex);
                }
            }
            else {
                app.VectorDeltaRequestInProcess = false;
                console.log('error');
            }

        }, true, false);
    }
    RefreshVectorDataAndLayer = function () {
        //Get All selected Province Layers
        app.ActiveProvincelayers = app.getActiveProvinceLayers();
        //Check if new Province Selected
        app.ActiveProvincelayers = app.ActiveProvincelayers.filter(item => !app.provinceListData.includes(item));
        //Check if new Province Selected then  load data for that layer
        if (app.ActiveProvincelayers.length > 0) {
            app.ClearOldVectorData();
            let vectorPrvinceSelected = app.ActiveProvincelayers.join(",");
            console.log(vectorPrvinceSelected);
            app.fetchVectorLayerData(vectorPrvinceSelected);
        } else {
            app.LoadVectorLayers();
        }
    }
    ClearOldVectorData = function () {
        if (app.provinceListData.length == this.provinceLimitToStoreDatainBrowser) {
            let iOldestProvinceId = app.provinceListData[0];
            app.provinceListData.shift();
            if (app.poleGeoJson.features) {
                app.poleGeoJson = { "type": "FeatureCollection", "features": app.poleGeoJson.features.filter(feature => feature.properties.province_id != iOldestProvinceId) };
            }
            if (app.manholeGeoJson.features) {
                app.manholeGeoJson = { "type": "FeatureCollection", "features": app.manholeGeoJson.features.filter(feature => feature.properties.province_id != iOldestProvinceId) };
            }
            if (app.wallmountGeoJson.features) {
                app.wallmountGeoJson = { "type": "FeatureCollection", "features": app.wallmountGeoJson.features.filter(feature => feature.properties.province_id != iOldestProvinceId) };
            }
            if (app.fdbGeoJson.features) {
                app.fdbGeoJson = { "type": "FeatureCollection", "features": app.fdbGeoJson.features.filter(feature => feature.properties.province_id != iOldestProvinceId) };
            }
            if (app.bdbGeoJson.features) {
                app.bdbGeoJson = { "type": "FeatureCollection", "features": app.bdbGeoJson.features.filter(feature => feature.properties.province_id != iOldestProvinceId) };
            }
            if (app.adbGeoJson.features) {
                app.adbGeoJson = { "type": "FeatureCollection", "features": app.adbGeoJson.features.filter(feature => feature.properties.province_id != iOldestProvinceId) };
            }
            if (app.splitterGeoJson.features) {
                app.splitterGeoJson = { "type": "FeatureCollection", "features": app.splitterGeoJson.features.filter(feature => feature.properties.province_id != iOldestProvinceId) };
            }
            if (app.spliceclosureGeoJson.features) {
                app.spliceclosureGeoJson = { "type": "FeatureCollection", "features": app.spliceclosureGeoJson.features.filter(feature => feature.properties.province_id != iOldestProvinceId) };
            }

            if (app.cableGeoJson.features) {
                app.cableGeoJson = { "type": "FeatureCollection", "features": app.cableGeoJson.features.filter(feature => feature.properties.province_id != iOldestProvinceId) };
            }

            if (app.areaGeoJson.features) {
                app.areaGeoJson = { "type": "FeatureCollection", "features": app.areaGeoJson.features.filter(feature => feature.properties.province_id != iOldestProvinceId) };
            }
            if (app.subareaGeoJson.features) {
                app.subareaGeoJson = { "type": "FeatureCollection", "features": app.subareaGeoJson.features.filter(feature => feature.properties.province_id != iOldestProvinceId) };
            }
            if (app.dsaGeoJson.features) {
                app.dsaGeoJson = { "type": "FeatureCollection", "features": app.dsaGeoJson.features.filter(feature => feature.properties.province_id != iOldestProvinceId) };
            }
            if (app.csaGeoJson.features) {
                app.csaGeoJson = { "type": "FeatureCollection", "features": app.csaGeoJson.features.filter(feature => feature.properties.province_id != iOldestProvinceId) };
            }

        }
    }

    ShowHideVectorLayerByZoomSetting = function () {
        $.each(app.geomTypes, function (i, itm) {
            let _Zoom = app.map.getZoom();
            $.each($(si.DE.ulNetworkLayers + " li[data-geomtype='" + itm + "'] .mainLyr:checked").not(":disabled").filter(function () {
                return ((app.IsZoomInTriggered == true && _Zoom == parseInt($(this).attr('data-minzoomlvl'))) || (app.IsZoomInTriggered == false && _Zoom == parseInt($(this).attr('data-minzoomlvl')) - 1))
            }).sort(function (a, b) { return parseInt($(a).attr('data-maplayerseq')) - parseInt($(b).attr('data-maplayerseq')) }), function () {

                // app.ZoomChangeLayerList.push($(this).attr('data-mapabbr'));
                let layrAbbr = $(this).attr('data-mapabbr');
                app.RenderVectorLayer(app.layerListAbbr.indexOf(layrAbbr));
            });
        });
    }
    ShowWhatIsHere = function (info) {
        let sContent = "<table  border='0' cellspacing='0' class='grid'>";
        let latLng;
        let pickedObjects = [];
        let radius = 10;
        if (info?.object) {
            const { x, y } = info;
            pickedObjects = app.poleVectorLayer.pickMultipleObjects({
                x,
                y,
                radius,
            });
            if (app.manholeVectorLayer) {
                pickedObjects = pickedObjects.concat(app.manholeVectorLayer.pickMultipleObjects({
                    x,
                    y,
                    radius,
                }));
            }
            if (app.wallmountVectorLayer) {
                pickedObjects = pickedObjects.concat(app.wallmountVectorLayer.pickMultipleObjects({
                    x,
                    y,
                    radius,
                }));
            }
            if (app.splitterVectorLayer) {
                pickedObjects = pickedObjects.concat(app.splitterVectorLayer.pickMultipleObjects({
                    x,
                    y,
                    radius,
                }));
            }
            if (app.cableVectorLayer) {
                pickedObjects = pickedObjects.concat(app.cableVectorLayer.pickMultipleObjects({
                    x,
                    y,
                    radius,
                }));
            }
            if (app.fdbVectorLayer) {

                pickedObjects = pickedObjects.concat(app.fdbVectorLayer.pickMultipleObjects({
                    x,
                    y,
                    radius,
                }));
            }
            if (app.bdbVectorLayer) {
                pickedObjects = pickedObjects.concat(app.bdbVectorLayer.pickMultipleObjects({
                    x,
                    y,
                    radius,
                }));
            }
            if (app.adbVectorLayer) {
                pickedObjects = pickedObjects.concat(app.adbVectorLayer.pickMultipleObjects({
                    x,
                    y,
                    radius,
                }));
            }
            if (app.spliceclosureVectorLayer) {
                pickedObjects = pickedObjects.concat(app.spliceclosureVectorLayer.pickMultipleObjects({
                    x,
                    y,
                    radius,
                }));
            }
            if (app.csaVectorLayer) {
                pickedObjects = pickedObjects.concat(app.csaVectorLayer.pickMultipleObjects({
                    x,
                    y,
                    radius,
                }));
            }
            if (app.dsaVectorLayer) {
                pickedObjects = pickedObjects.concat(app.dsaVectorLayer.pickMultipleObjects({
                    x,
                    y,
                    radius,
                }));
            }
            if (app.subareaVectorLayer) {
                pickedObjects = pickedObjects.concat(app.subareaVectorLayer.pickMultipleObjects({
                    x,
                    y,
                    radius,
                }));
            }
            if (app.areaVectorLayer) {
                pickedObjects = pickedObjects.concat(app.areaVectorLayer.pickMultipleObjects({
                    x,
                    y,
                    radius,
                }));
            }
            if (!pickedObjects || pickedObjects.length == 0) {
                return;
            }
            console.log(pickedObjects);
            debugger;
            jQuery.each(pickedObjects, function (i, val) {
                var feature = val.object;
                var properties = feature.properties;
                var entity_type = feature.entity_type;
                var system_id = properties.system_id;
                var network_id = properties.network_id;
                var network_stage = properties.network_status;
                var display_name = properties.display_name;
                var geom_type = feature.geometry.type;
                var geomStr = '';
                var geomStrFirstPoint = '';
                var geom = feature.geometry;
                var entityCategory = feature.geometry.type == "LineString" ? "Line" : feature.geometry.type == "Point" ? "Point" : "Polygon";
                if (feature.geometry.type == 'LineString') {
                    geomStr = feature.geometry.type.toString().toUpperCase() + '(' + feature.geometry.coordinates.map(a => a.join(" ")).join(",") + ')';
                    geomStrFirstPoint = feature.geometry.type.toString().toUpperCase() + '(' + feature.geometry.coordinates.map(a => a.join(" ")).join(",") + ')';
                } else if (feature.geometry.type == 'Point') {
                    geomStr = feature.geometry.type.toString().toUpperCase() + '(' + feature.geometry.coordinates.join(" ") + ')';
                }
                else {
                    geomStr = feature.geometry.type.toString().toUpperCase() + '(' + feature.geometry.coordinates[0].map(a => a.join(" ")).join(",") + ')';
                    geomStrFirstPoint = feature.geometry.type.toString().toUpperCase() + '(' + feature.geometry.coordinates[0][0].join(" ") + ')';
                }
                var sFocusFunction = `si._focusMe('${entityCategory}',si.getLatLongArr('${geomStr}'),'${entity_type}',si.getLatLongArr('${geomStrFirstPoint}'));`;
                var sOpenElementInfoWindowFunction = `si.OpenElementInfoWindow('${system_id}','${entity_type}','${entityCategory}','${network_id}','${network_stage}','${display_name}')`;
                let linkContent = '<span class="linkStyle" onclick ="' + sOpenElementInfoWindowFunction + '">' + display_name + "</span>";
                sContent = sContent + '<tr onmouseout="si.removeInfoHoverItem()" onmouseover="' + sFocusFunction + '" ><td><b>' + entity_type + '</b></td><td>' + linkContent + '</td></tr>';
            });
            sContent = sContent + "</table>";
        }
        latLng = new google.maps.LatLng(
            info.coordinate[1],
            info.coordinate[0]
        );
        app.VectorInfoWindow.setContent(sContent);
        // Open the info window at the clicked location
        app.VectorInfoWindow.setPosition(latLng);
        app.VectorInfoWindow.open(app.map);
    }

    OpenElementInfoWindow = function (_system_id, _entity_type, _geom_type, _network_id, _network_stage, _display_name) {
        ajaxReq('Main/GetInfo', {}, true, function (resp) {
            $(app.DE.infoMain).html(resp);
            $(app.DE.InfoDiv).show();
            $('.infoBack').hide();
            $("#rdbDisplayedLayer").trigger("click");

            app.showElementInfo(_system_id, _entity_type, _geom_type, _network_id, _network_stage, _display_name);

        }, false, true);
    }
}