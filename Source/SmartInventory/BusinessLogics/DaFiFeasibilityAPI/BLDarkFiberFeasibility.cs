using DataAccess;
using DataAccess.DaFiFeasibilityAPI;
using Microsoft.AspNet.SignalR.Json;
using Models;
using Models.DaFiFeasibilityAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BusinessLogics.DaFiFeasibilityAPI
{
    public class BLDarkFiberFeasibility
    {
        BLDarkFiberFeasibility()
        {

        }
        private static BLDarkFiberFeasibility objBLDarkFiberFeasibility = null;
        private static readonly object lockObject = new object();
        public static BLDarkFiberFeasibility Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (objBLDarkFiberFeasibility == null)
                    {
                        objBLDarkFiberFeasibility = new BLDarkFiberFeasibility();
                    }
                }
                return objBLDarkFiberFeasibility;
            }
        }

        public List<Route> GetNewFiberRoutes(string request_id, string a_lat_lng, string z_lat_lng, string apiKey, double a_buffer)
        {
            var dir = GoogleDirectionsServiceHelper.GetRouteGeoJsonAndLength(a_lat_lng, z_lat_lng, apiKey).Result;

            var newbuilt = JsonConvert.DeserializeObject<GeoJsonLineString>(dir.GeoJson);

            var route = new Route
            {
                route_id = Guid.NewGuid().ToString(),
                route_type = "New",
                geojson_new_built = dir.GeoJson,
                total_new_length = dir.LengthInMeters
            };

            return new List<Route> { route };
        }

        public List<Route> GetExistingFiberRoutes(string request_id, string a_lat_lng, string z_lat_lng, int fiber_cores, string apiKey, double a_buffer, double z_buffer)
        {
            var a_coords = a_lat_lng.Split(',');
            var z_coords = z_lat_lng.Split(',');

            var source = $"{a_coords[1].Trim()} {a_coords[0].Trim()}";
            var destination = $"{z_coords[1].Trim()} {z_coords[0].Trim()}";
            var routes = DADarkFiberFeasibility.Instance.GetExistingFiberRoutes(request_id, source, destination, fiber_cores, a_buffer, z_buffer);

            foreach (var route in routes)
            {
                if (route.cable_segments != null && route.cable_segments.Count > 0)
                {
                    route.start_entity = route.cable_segments.First().start_entity;
                    route.end_entity = route.cable_segments.Last().end_entity;

                    var existing_start_point = route.existing_start_point;
                    var existing_end_point = route.existing_end_point;

                    var dir_start = GoogleDirectionsServiceHelper.GetRouteGeoJsonAndLength(a_lat_lng, existing_start_point, apiKey).Result;
                    var dir_end = GoogleDirectionsServiceHelper.GetRouteGeoJsonAndLength(existing_end_point, z_lat_lng, apiKey).Result;


                    var newbuilt_start = JsonConvert.DeserializeObject<GeoJsonLineString>(dir_start.GeoJson);

                    newbuilt_start.coordinates.Insert(0, new List<double> { double.Parse(a_lat_lng.Split(',')[1]), double.Parse(a_lat_lng.Split(',')[0]) });
                    newbuilt_start.coordinates.Add(new List<double> { double.Parse(existing_start_point.Split(',')[1]), double.Parse(existing_start_point.Split(',')[0]) });

                    var newbuilt_end = JsonConvert.DeserializeObject<GeoJsonLineString>(dir_end.GeoJson);

                    newbuilt_end.coordinates.Insert(0, new List<double> { double.Parse(existing_end_point.Split(',')[1]), double.Parse(existing_end_point.Split(',')[0]) });
                    newbuilt_end.coordinates.Add(new List<double> { double.Parse(z_lat_lng.Split(',')[1]), double.Parse(z_lat_lng.Split(',')[0]) });

                    var multiLineString = new GeoJsonMultiLineString
                    {
                        type = "MultiLineString",
                        coordinates = new List<List<List<double>>>
                        {
                            newbuilt_start.coordinates,
                            newbuilt_end.coordinates
                        }
                    };

                    string geojson_new_built = JsonConvert.SerializeObject(multiLineString);
                    var total_existing_length = dir_start.LengthInMeters + dir_start.LengthInMeters;


                    route.geojson_new_built = geojson_new_built;
                    route.total_new_length = total_existing_length;

                    DADarkFiberFeasibility.Instance.SaveDarkFiber(request_id, "Existing", route.route_id, geojson_new_built, total_existing_length);
                }
                else
                {
                    throw new Exception("No route found.");
                }
            }

            return routes;
        }

        public DbMessage SaveDarkFiber(string request_id, string route_type, string route_id, string geojson_new_built, double total_new_length)
        {
            try
            {
                var response = DADarkFiberFeasibility.Instance.SaveDarkFiber(request_id, route_type, route_id, geojson_new_built, total_new_length);
                return response;
            }
            catch
            {
                throw;
            }
        }
        public DbMessageForDaFiFeasibility ReserveFeasibilityRoute(ReserveFeasibility obj)
        {
            try
            {
                var response = DADarkFiberFeasibility.Instance.ReserveFeasibilityRoute(obj);
                return response;
            }
            catch
            {
                throw;
            }
        }
        public DbMessageForDaFiFeasibility ReleaseFeasibilityRoute(ReleaseFeasibility obj)
        {
            try
            {
                var response = DADarkFiberFeasibility.Instance.ReleaseFeasibilityRoute(obj);
                return response;
            }
            catch
            {
                throw;
            }
        }

        public string GetDarkFiberKML(string request_id, string route_id)
        {
            var feasibleRoutesKML = DADarkFiberFeasibility.Instance.GetDarkFiberKML(request_id, route_id);
            string kmlContent = GenerateKmlContent(feasibleRoutesKML);

            return kmlContent;
        }

        public string GenerateKmlContent(List<FeasibileRouteKML> feasibleRoutes)
        {
            XNamespace ns = "http://www.opengis.net/kml/2.2";

            XDocument kmlDoc = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement(ns + "kml",
                    new XElement(ns + "Document")
                )
            );

            foreach (var route in feasibleRoutes)
            {
                XElement routeFolder = new XElement(ns + "Folder", new XElement(ns + "name", route.route_id));

                if (!string.IsNullOrEmpty(route.kml_existing_built))
                {
                    XElement existingBuiltGeometry = XElement.Parse(route.kml_existing_built);

                    XElement existingBuiltPlacemark = new XElement(ns + "Placemark",
                        new XElement(ns + "name", "Existing Built"),
                        new XElement(ns + "description", "Existing built route"),
                        new XElement(ns + "Style",
                            new XElement(ns + "LineStyle",
                                new XElement(ns + "color", "ff00ff00"),  // Green color (AGBR Format)
                                    new XElement(ns + "width", 3)
                            )
                        ),
                        existingBuiltGeometry
                    );

                    routeFolder.Add(existingBuiltPlacemark);
                }

                if (!string.IsNullOrEmpty(route.kml_new_built))
                {
                    XElement newBuiltGeometry = XElement.Parse(route.kml_new_built);

                    XElement newBuiltPlacemark = new XElement(ns + "Placemark",
                        new XElement(ns + "name", "New Built"),
                        new XElement(ns + "description", "New built route"),
                        new XElement(ns + "Style",
                            new XElement(ns + "LineStyle",
                                new XElement(ns + "color", "ff0000ff"),  // Red color (AGBR Format)
                                    new XElement(ns + "width", 3)
                            )
                        ),
                        newBuiltGeometry
                    );

                    routeFolder.Add(newBuiltPlacemark);
                }

                kmlDoc.Root.Element(ns + "Document").Add(routeFolder);
            }

            return kmlDoc.Declaration + Environment.NewLine + kmlDoc.ToString();
        }

        public DbMessage CheckReservedRoute(string request_id)
        {
            return DADarkFiberFeasibility.Instance.CheckReservedRoute(request_id);
        }
    }
}
