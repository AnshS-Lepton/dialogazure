using BusinessLogics;
using BusinessLogics.DaFiFeasibilityAPI;
using IntegrationServices.Settings;
using Models;
using Models.DaFiFeasibilityAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using System.Web.Http;
using Utility;

namespace IntegrationServices.Controllers
{
    public class DaFiFeasibilityController : ApiController
    {

        [HttpGet]
        [Route("api/v1/enterpriseFeasibility")]
        public HttpResponseMessage enterpriseFeasibility(string request_id, double latitude, double longitude, double radius = 15.00,int route_buffer = 0)
        {

            try
            {
                int bufferInMtrs;
                var apiKey = ApplicationSettings.Map_Key_Backend;
                if (!isLatLngValid(latitude.ToString(), longitude.ToString()))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Invalid latitude or longitude!" });
                }

                if (!isbufferValid(radius, out bufferInMtrs))
                {
                    var buffervalue = ApplicationSettings.feasibility_buffer_limit;
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = $"Invalid buffer Value. The value must be non-negative and less than {buffervalue}." });
                }


                if (string.IsNullOrEmpty(request_id))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "request_id cannot be blank!" });
                }

                var lstentities= new BLMisc().getNearByFeasibility(latitude,longitude, bufferInMtrs);

                if (lstentities.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "No structure found within the buffer range!" });
                }

                var proposedStructure= new ProStructure();
                var nearestStructure = new Structure();

                #region nearest_structure
                #region get latitude and longitude from db

                double strLlongitude = 0, strLatitude = 0, endLongitude = 0, endLatitude = 0;
               // Extract coordinates using regex
                string pattern = @"POINT\s*\((.*?)\)";
                Match match = Regex.Match(lstentities[0].geom, pattern);
                if (match.Success)
                {
                    string coordinates = match.Groups[1].Value;
                    string[] points = coordinates.Split(' ');
                    int pointsCount = points.Length;
                    strLatitude = double.Parse(points[0]);
                    strLlongitude = double.Parse(points[1]);
                    endLongitude = double.Parse(points[pointsCount - 1]);
                    endLatitude = double.Parse(points[pointsCount - 2]);
                }
                #endregion
                if(lstentities[0].entity_type == Models.EntityType.Pole.ToString() || lstentities[0].entity_type == Models.EntityType.Manhole.ToString())
                {
                    var roadPathRouteApiData = BLDarkFiberFeasibility.Instance.GetNewFiberRoutes(request_id, (latitude + "," + longitude).ToString(), (strLlongitude + "," + strLatitude).ToString(), apiKey, radius);
                    GeoData data = JsonConvert.DeserializeObject<GeoData>(roadPathRouteApiData[0].geojson_new_built);
                    double StrRoadpathLatitude = 0, StrRoadpathLongitude = 0;
                    string nearestlatlngdata = string.Empty;
                    double lon = data.Coordinates[0][0];
                    double lat = data.Coordinates[0][1];
                     StrRoadpathLongitude = data.Coordinates[0][0];
                     StrRoadpathLatitude = data.Coordinates[0][1];
                    string geom = $"{StrRoadpathLongitude} {StrRoadpathLatitude}, {longitude} {latitude}";
                    foreach (var coord in data.Coordinates)
                    {
                        StrRoadpathLongitude = coord[0]; StrRoadpathLatitude = coord[1];
                        nearestlatlngdata += StrRoadpathLongitude + " " + StrRoadpathLatitude + ",";
                    }
                    nearestlatlngdata = nearestlatlngdata.TrimEnd(',');
                    if (!nearestlatlngdata.Contains(","))
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Road path not found!" });
                    string inputcoordinates ="LINESTRING(" + nearestlatlngdata+")";
                    List<Points> points = new BLMisc().getStartEndPointsFeasibility(inputcoordinates);
                    string geo_end_point = getLatLongDetails(string.IsNullOrEmpty(points[0].end_point) ? " " : points[0].end_point);
                    string[] end_point = geo_end_point.Split(' ');
                    var totaldistance = new BLPlan().GetLineLength(geom);
                    var customer_to_road_routeData = GetRouteList(latitude, longitude, StrRoadpathLatitude, StrRoadpathLongitude);
                    var structureToRoadRouteData = GetRouteList(Convert.ToDouble(end_point[1]), Convert.ToDouble(end_point[0]), endLongitude, endLatitude);
                    var nearestlocation = CreateLocation(latitude, longitude); 
                    var customerToRoadRouteSegment = bindRouteData(customer_to_road_routeData[0].geojson_new_built, totaldistance);
                    var roadPathRouteSegment = bindRouteData(roadPathRouteApiData[0].geojson_new_built, roadPathRouteApiData[0].total_new_length);
                    string geomDistS2R = $"{geo_end_point}, {StrRoadpathLongitude} {latitude}";

                    var totalDistS2R = new BLPlan().GetLineLength(geomDistS2R);

                    var structureToRoadRouteSegment = bindRouteData(structureToRoadRouteData[0].geojson_new_built, totalDistS2R);
                    var route = getRoutesDetails(customerToRoadRouteSegment, roadPathRouteSegment, structureToRoadRouteSegment);
                    nearestStructure = bindStructureData(lstentities[0].entity_title/*structure_type*/, lstentities[0].display_name/*structure_id*/, nearestlocation, route, inputcoordinates, route_buffer);

                }

                #endregion
                #region proposed_structure

                #region get latitude and longitude from db
                double strProLongitude = 0, strProLatitude = 0, endProLongitude = 0, endProLatitude = 0;
               int i=0;
                if(lstentities.Count>1)
                    i = 1;
                string proPattern = @"POINT\s*\((.*?)\)";
                Match matchPro = Regex.Match(lstentities[i].geom, proPattern);
                if (matchPro.Success)
                {
                    string proCoordinates = matchPro.Groups[1].Value;
                    string[] proLines = proCoordinates.Split(' ');
                    int proLineCount = proLines.Length;
                    strProLatitude = double.Parse(proLines[0]);
                    strProLongitude = double.Parse(proLines[1]);
                }


                #endregion
                if (lstentities[i].entity_type == Models.EntityType.Cable.ToString())
                {
                    var proposedRoadpathRouteApiData = BLDarkFiberFeasibility.Instance.GetNewFiberRoutes(request_id, (latitude + "," + longitude).ToString(), (strProLongitude + "," + strProLatitude).ToString(), apiKey, radius);
                    GeoData proposedData = JsonConvert.DeserializeObject<GeoData>(proposedRoadpathRouteApiData[0].geojson_new_built);
                    double proposedstrRoadpathLatitude = 0, proposedStrRoadpathLongitude = 0;

                    List<List<double>> proposedcoordinates = proposedData.Coordinates;
                    string latlngdata= string.Empty;
                    
                    foreach (var proposedcoord in proposedData.Coordinates)
                    {
                        proposedStrRoadpathLongitude = proposedcoord[0]; proposedstrRoadpathLatitude = proposedcoord[1];
                        latlngdata += proposedStrRoadpathLongitude + " " + proposedstrRoadpathLatitude + ",";
                    }
                    latlngdata = latlngdata.TrimEnd(',');

                    if (!latlngdata.Contains(","))
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Proposed structure road path not found!" });

                    string inputprocoordinates = "LINESTRING(" + latlngdata + ")";
                    List<Points> propoints = new BLMisc().getStartEndPointsFeasibility(inputprocoordinates);
                    string geo_proend_point = getLatLongDetails(propoints[0].end_point);
                    string[] proend_point = geo_proend_point.Split(' ');
                    endProLatitude = double.Parse(proend_point[1]);
                    endProLongitude = double.Parse(proend_point[0]);

                    var proposedCustomerToRoadRouteData = GetRouteList(latitude, longitude, proposedstrRoadpathLatitude, proposedStrRoadpathLongitude);
                    var proposedStructureToRoad = GetRouteList(endProLatitude, endProLongitude, proposedstrRoadpathLatitude, proposedStrRoadpathLongitude);
                    var proposedNearestlocation = CreateLocation(latitude, longitude);
                    var proposedCustomerToRoadRouteSegment = bindRouteData(proposedCustomerToRoadRouteData[0].geojson_new_built, proposedCustomerToRoadRouteData[0].total_new_length);
                    var proposedRoadPathRouteSegment = bindRouteData(proposedRoadpathRouteApiData[0].geojson_new_built, proposedRoadpathRouteApiData[0].total_new_length);
                    var proposedStructureToRoadRouteSegment = bindRouteData(proposedStructureToRoad[0].geojson_new_built, proposedStructureToRoad[0].total_new_length);
                    var proposedroute = getRoutesDetails(proposedCustomerToRoadRouteSegment, proposedRoadPathRouteSegment, proposedStructureToRoadRouteSegment);

                     proposedStructure = bindProposedStructureData(lstentities[i].entity_title/*structure_type*/, lstentities[i].display_name/*structure_id*/, proposedNearestlocation, proposedroute, inputprocoordinates, route_buffer);
                }
                   
                #endregion

                var responseData = new ResponseData
                {
                    request_id = request_id,
                    customer_latitude = latitude,
                    customer_longitude = longitude,
                    nearest_structure = nearestStructure,
                    proposed_structure = proposedStructure,
                    
                };
                return this.Request.CreateResponse(HttpStatusCode.OK, responseData);
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getDarkFiberFeasibility()", "DaFiFeasibilityController", "", ex);
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = "UNKNOWN_ERROR", message = "Error while processing the request." });
            }
        }

        [HttpGet]
        [Route("feasibilityKML")]
        public HttpResponseMessage getfeasibilityKML(string feasibility_request_id = "", string route_id = "")
        {

            try
            {
                if (string.IsNullOrEmpty(feasibility_request_id) && string.IsNullOrEmpty(route_id))
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "Invalid Request!" });
                }
                var kmlContent = BLDarkFiberFeasibility.Instance.GetDarkFiberKML(feasibility_request_id, route_id);
                byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(kmlContent);

                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(fileBytes)
                };

                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.google-earth.kml+xml");
                response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = $"{feasibility_request_id}_routes.kml"
                };

                return response;
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("getfeasibilityKML()", "DaFiFeasibilityController", "", ex);
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = "UNKNOWN_ERROR", message = "Error while processing the request." });
            }
        }

        [HttpPost]
        [Route("reserveFeasibilityRoute")]
        public HttpResponseMessage reserveFeasibilityRoute(ReserveFeasibility obj)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Select(x => x.Key)
                              .ToList();

                    DbMessageForDaFiFeasibility ret = new DbMessageForDaFiFeasibility();
                    ret.status = "BadRequest";
                    ret.message = errors.ToString();
                    foreach (var err in errors)
                    {
                        ret.message += err + " ";
                    }

                    return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { status = ret.status, message = ret.message });


                }
                else
                {
                    var objRet = BLDarkFiberFeasibility.Instance.ReserveFeasibilityRoute(obj);
                    //return this.Request.CreateResponse(HttpStatusCode.OK, objRet);
                    return this.Request.CreateResponse(commonUtil.getHttpsStatus(objRet.status), new { status = objRet.status, message = objRet.message });
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("reserveFeasibilityRoute()", "DaFiFeasibilityController", "", ex);
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = "UNKNOWN_ERROR", message = "Error while processing the request." });
            }
        }
        [HttpPost]
        [Route("releaseFeasibilityRoute")]
        public HttpResponseMessage releaseFeasibilityRoute(ReleaseFeasibility obj)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    var errors = ModelState.Select(x => x.Key)
                //             .ToList();

                //    DbMessageForDaFiFeasibility ret = new DbMessageForDaFiFeasibility();
                //    ret.status = "BadRequest";
                //    ret.message = errors.ToString();
                //    foreach (var err in errors)
                //    {
                //        ret.message += err + "  ";
                //    }

                //    return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { status = ret.status, message = ret.message });

                //}
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Select(x => x.Key.Replace("obj.", "")).ToList();

                    var ret = new DbMessageForDaFiFeasibility
                    {
                        status = "BadRequest",
                        message = errors.Count == 1 ? $"Please provide a {errors[0]} .it cannot be null." : $"Please provide a {string.Join(" and ", errors)} .it cannot be null."
                    };
                    return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { status = ret.status, message = ret.message });
                }

                else
                {
                    var objRet = BLDarkFiberFeasibility.Instance.ReleaseFeasibilityRoute(obj);
                    //return this.Request.CreateResponse(HttpStatusCode.OK, objRet);
                    return this.Request.CreateResponse(commonUtil.getHttpsStatus(objRet.status), new { status = objRet.status, message = objRet.message });
                }
            }
            catch (Exception ex)
            {
                ErrorLogHelper logHelper = new ErrorLogHelper();
                logHelper.ApiLogWriter("releaseFeasibilityRoute()", "DaFiFeasibilityController", "", ex);
                return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = "UNKNOWN_ERROR", message = "Error while processing the request." });
            }
        }

        private bool isLatLngValid(string lat, string lng)
        {
            double latitude, longitude;

            try
            {
                    latitude = double.Parse(lat);
                    longitude = double.Parse(lng);

                    return latitude >= -90 && latitude <= 90 && longitude >= -180 && longitude <= 180;

            }

            catch { return false; }
        }


        private bool isbufferValid(double strBuffer, out int dblBuffer)
        {
            var buffervalue = ApplicationSettings.feasibility_buffer_limit;

            try
            {
                dblBuffer = Convert.ToInt32(strBuffer) ;
                return dblBuffer > 0 && dblBuffer <= Convert.ToInt32(buffervalue);
            }
            catch
            {
                dblBuffer = 0;
                return false;
            }

        }
        private bool isfiberValid(string fibernuber, out int val_fiber_cores)
        {

            var fibervalue = ApplicationSettings.fiber_core_limit;

            try
            {
                val_fiber_cores = int.Parse(fibernuber);
                return val_fiber_cores > 0 && val_fiber_cores <= Convert.ToInt32(fibervalue);
            }
            catch
            {
                val_fiber_cores = 0;
                return false;
            }
        }

        public static string getLatLongDetails(string geom)
        {
            
            string pointPattern = @"POINT\s*\((.*?)\)";

            // Check if it is a POINT
            Match pointMatch = Regex.Match(geom, pointPattern);
            if (pointMatch.Success)
            {
                string proCoordinates = pointMatch.Groups[1].Value;
                string[] proLines = proCoordinates.Split(' ');
                int proLineCount = proLines.Length;
                double longitude = double.Parse(proLines[1]);
                double latitude = double.Parse(proLines[0]);
                return $"{latitude} {longitude}"; // Return in "lat, lon" format
            }

            throw new ArgumentException("Invalid geometry format");
        }
        public Location CreateLocation(double latitude, double longitude)
        {
            return new Location
            {
                latitude = latitude,
                longitude = longitude
            };
        }
        private RouteSegment bindRouteData(string geometry,double length)
        {
            RouteSegment segment=new RouteSegment();
            segment.geometry = geometry;
            segment.length = length;
            return segment;

        }

        private ProStructure bindProposedStructureData(string structure_type, string structure_id, Location objlocation, Routes objroute, string coordinates, int route_buffer)
        {
            ProStructure structure = new ProStructure();
            structure.location = objlocation;
            structure.route = objroute;
            structure.cables_in_route_buffer = getRouteBufferDetail(coordinates, route_buffer);
            return structure;

        }
        private List<RouteBuffer> getRouteBufferDetail(string inputcoordinates,int route_buffer)
        {
            List<RouteBuffer> routeBuffer = new List<RouteBuffer>();
            routeBuffer= new BLMisc().getRouteBufferFeasibility(inputcoordinates, route_buffer);
            foreach (var item in routeBuffer)
            {
                List<List<double>> coordinates = ParseLineString(item.geometry);
                item.geometry = JsonConvert.SerializeObject(new { type = "LineString", coordinates });
            }
            return routeBuffer;

        }
        static List<List<double>> ParseLineString(string wkt)
        {
            // Remove "LINESTRING(" and ")" from the string
            string coordsText = wkt.Replace("LINESTRING(", "").Replace(")", "");

            // Split by commas to get each coordinate pair
            string[] pairs = coordsText.Split(',');

            // Convert to List<List<double>>
            return pairs.Select(pair => {
                string[] values = pair.Trim().Split(' '); // Split by space
                return new List<double> { double.Parse(values[0]), double.Parse(values[1]) }; // Longitude, Latitude
            }).ToList();
        }
    
private Structure bindStructureData(string structure_type, string structure_id, Location objlocation, Routes objroute,string coordinates, int route_buffer)
        {
            Structure structure = new Structure();
            structure.structure_type = structure_type;
            structure.structure_id = structure_id;
            structure.location = objlocation;
            structure.route = objroute;
            structure.cables_in_route_buffer= getRouteBufferDetail(coordinates, route_buffer);
            return structure;

        }

        static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371.0; // Radius of Earth in kilometers
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Distance in km
        }
        // Convert degrees to radians
        static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        // Calculate total distance from a list of coordinates
        static double CalculateTotalDistance(List<List<double>> coordinates)
        {
            double totalDistance = 0.0;

            for (int i = 0; i < coordinates.Count - 1; i++)
            {
                double lon1 = coordinates[i][0], lat1 = coordinates[i][1];
                double lon2 = coordinates[i + 1][0], lat2 = coordinates[i + 1][1];

                totalDistance += HaversineDistance(lat1, lon1, lat2, lon2);
                totalDistance = Math.Round(totalDistance, 2);
            }

            return totalDistance;
        }

        public List<Route> GetRouteList(double latitude, double longitude, double strpathLatitude, double strpathLongitude)
        {
            var lstRoute = new List<Route>();

            //if (latitude != strpathLatitude && longitude != strpathLongitude)
            //{
                var coordinates = new List<List<double>>
                {
                     new List<double> { longitude, latitude },
                     new List<double> { strpathLongitude, strpathLatitude }
                };

                lstRoute = GetNewRoutes(coordinates);
           // }

            return lstRoute;
        }
        public List<Route> GetNewRoutes(List<List<double>> coordinates)
        {
            var route = new Route
            {
                route_id = Guid.NewGuid().ToString(),
                route_type = "New",
                geojson_new_built = JsonConvert.SerializeObject(new { type = "LineString", coordinates }),
                total_new_length = CalculateTotalDistance(coordinates)
            };

            return new List<Route> { route };
        }

        private Routes getRoutesDetails(RouteSegment customer_to_road_routeSegment, RouteSegment road_path_routeSegment, RouteSegment structure_to_road_routeSegment)
        {
            var routes = new Routes
            {
                customer_to_road = customer_to_road_routeSegment,
                road_path = road_path_routeSegment,
                structure_to_road = structure_to_road_routeSegment
            };

            return routes;
        }
    }
}
