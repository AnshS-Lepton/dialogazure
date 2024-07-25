using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Web;
using System.Web.Http;
using BusinessLogics;
using DataAccess;
using Models;
using Models.API;
using Newtonsoft.Json;
using Utility;

namespace IntegrationServices.Controllers
{
	[Authorize]
	[RoutePrefix("api/v1")]
	public class LocationDeltaController : ApiController
    {
		[HttpGet]
		[Route("locationDelta")]
		public HttpResponseMessage getLocationDelta(string entity_type, string delta_date)
		{
			var response = new ApiResponse<dynamic>();
			List<GetLocationDelta> getLocationDeltaList = new List<GetLocationDelta>();
			try
			{
				if (string.IsNullOrEmpty(entity_type))
				{
					return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "entity_type can not be blank!" });
				}
				var layerDetails = new BLLayer().GetLayerDetails().Where(x => x.layer_title.ToUpper() == entity_type.ToUpper()).FirstOrDefault();
				if (layerDetails == null)
				{
					return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "entity_type not found!" });
				}

				getLocationDeltaList = new BLLocationDelta().getLocationDelta(entity_type, delta_date);
					if (getLocationDeltaList.Count > 0)
					{
						response.results = getLocationDeltaList;
						response.status = StatusCodes.OK.ToString();
						return this.Request.CreateResponse(HttpStatusCode.OK, response);

					}
					else
					{
						return this.Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "No Delta Found!" });
					}
				

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("locationDelta()", "LocationDeltaController", "", ex);
				return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = "UNKNOWN_ERROR", message = "Error while processing the request." });

			}

		}

		[HttpGet]
		[Route("locations")]
		public HttpResponseMessage getSiteLocations(string entity_type, int? page, int? page_size)
		{
			var response = new ApiResponse<dynamic>();
			List<GetSiteLocationList> getSiteLocationList = new List<GetSiteLocationList>();
			GetSiteLocation getSiteLocation = new GetSiteLocation();			
			try
			{
				Uri uri = HttpContext.Current.Request.Url;
				string requestURL = uri.GetLeftPart(UriPartial.Path);

				if (page_size > 100 || page_size < 0)
				{
				 return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "page_size cannot be greater than 100 and negative value!" });
					
				}
				if (string.IsNullOrEmpty(entity_type))
				{
					return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "entity_type can not be blank!" });
				}

				var layerDetails = new BLLayer().GetLayerDetails().Where(x => x.layer_title.ToUpper() == entity_type.ToUpper()).FirstOrDefault();
				if (layerDetails == null)
				{
					return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "entity_type not found!" });
				}

				getSiteLocationList = new BLLocationDelta().getSiteLocations(entity_type, page, page_size);
					if (getSiteLocationList.Count > 0)
					{
						List<GetSiteLocationDetails> getLocationList = new List<GetSiteLocationDetails>();
						foreach (var item in getSiteLocationList)
						{
							GetSiteLocationDetails getSiteLocationDetails = new GetSiteLocationDetails();
							getSiteLocationDetails.entity_type = item.entity_type;
							getSiteLocationDetails.network_id = item.network_id;
							getSiteLocationDetails.system_id = item.system_id;
							getSiteLocationDetails.site_code = item.site_code;
							getSiteLocationDetails.network_status = item.network_status;
							getSiteLocationDetails.site_location = item.site_location;
							getLocationList.Add(getSiteLocationDetails);
						}
						getSiteLocation.data = getLocationList;
						PaginationMetaData paginationMetaData = getSiteLocationList[0].pagination_metadata;
						int? nextPage = paginationMetaData.total_pages == paginationMetaData.page ? 0 : paginationMetaData.page + 1;
						int? perviousPage = paginationMetaData.page == 1 ? 0 : paginationMetaData.page - 1;
						paginationMetaData.next_page = nextPage == 0 ? "" :string.Format("{0}?entity_type={1}&page={2}&page_size={3}", requestURL, entity_type, nextPage, paginationMetaData.page_size);
						paginationMetaData.previous_page = perviousPage == 0 ? "" :string.Format("{0}?entity_type={1}&page={2}&page_size={3}", requestURL, entity_type, perviousPage, paginationMetaData.page_size);
						getSiteLocation.pagination_metadata = paginationMetaData;
						response.results = getSiteLocation;
						response.status = StatusCodes.OK.ToString();
						//return this.Request.CreateResponse(HttpStatusCode.OK, response);
						var responseWithHeader = Request.CreateResponse(HttpStatusCode.OK, response);
						responseWithHeader.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetaData)); // Adding the X-Pagination in headers
						return responseWithHeader;

					}
					else
					{
						return this.Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = "No Delta Found!" });
					}
				

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("getSiteLocations()", "LocationDeltaController", "", ex);
				return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = "UNKNOWN_ERROR", message = "Error while processing the request." });

			}

		}

		[HttpGet]
		[Route("faultLocation")]
		public HttpResponseMessage getFaultLocations(string fiber_link_id, string equipment_id, string site_code, string port_id, int? optical_distance)
		{
			var response = new ApiResponse<dynamic>();
			List<GETFAULTLOCATIONLIST> getFaultLocationList = new List<GETFAULTLOCATIONLIST>();
			GETFAULTLOCATION getSiteLocation = new GETFAULTLOCATION();
			try
			{
				if (string.IsNullOrEmpty(fiber_link_id))
				{
					return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "fiber_link_id can not be blank!" });
				}
				if (string.IsNullOrEmpty(equipment_id))
				{
					return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "equipment_id can not be blank!" });
				}
				if (string.IsNullOrEmpty(port_id))
				{
					return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "port_id can not be blank!" });
				}
				if (optical_distance == null || optical_distance <= 0)
				{
					return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "optical_distance can not be blank,negative and zero!" });
				}

					getFaultLocationList = new BLLocationDelta().getFaultLocations(fiber_link_id, equipment_id, site_code, port_id, optical_distance);
					if (getFaultLocationList.Count > 0 && getFaultLocationList[0].status != "Failed")
					{
						List<ASSOCIATED_FIBER_LINKS> getFiberList = new List<ASSOCIATED_FIBER_LINKS>();
						foreach (var item in getFaultLocationList)
						{
							ASSOCIATED_FIBER_LINKS getAssociatedFiber = new ASSOCIATED_FIBER_LINKS();
							getAssociatedFiber.fiber_link_network_id = item.fiber_link_network_id;
							getAssociatedFiber.fiber_link_name = item.fiber_link_name;
							getFiberList.Add(getAssociatedFiber);
						}
						getSiteLocation.associated_fiber_link = getFiberList;
						FAULT_LOCATION fault_location = getFaultLocationList[0].fault_Location;
						AFFECTED_CABLE_SEGMENT affected_cable_segment = getFaultLocationList[0].affected_Cable_Segment;
						getSiteLocation.fault_location = fault_location;
						getSiteLocation.affected_cable_segment = affected_cable_segment;
						response.results = getSiteLocation;
				    	 response.status = StatusCodes.OK.ToString();
				    	var responseWithHeader = Request.CreateResponse(HttpStatusCode.OK, response);
					return responseWithHeader;

					}
					else
					{
						return this.Request.CreateResponse(HttpStatusCode.NotFound, new { status = 404, message = getFaultLocationList[0].error_message});
					}


			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("getFaultLocations()", "LocationDeltaController", "", ex);
				return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = "UNKNOWN_ERROR", message = "Error while processing the request." });

			}

		}

	}
}
