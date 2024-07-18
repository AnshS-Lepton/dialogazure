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
				if (!string.IsNullOrEmpty(entity_type))
				{
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
				else
				{
					return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "entity_type can not be blank!" });
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
				if (!string.IsNullOrEmpty(entity_type))
				{
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
				else
				{
					return this.Request.CreateResponse(HttpStatusCode.BadRequest, new { status = 400, message = "entity_type can not be blank!" });
				}

			}
			catch (Exception ex)
			{
				ErrorLogHelper logHelper = new ErrorLogHelper();
				logHelper.ApiLogWriter("getSiteLocations()", "LocationDeltaController", "", ex);
				return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = "UNKNOWN_ERROR", message = "Error while processing the request." });

			}

		}
	}
}
