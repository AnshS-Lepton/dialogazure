using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BusinessLogics;
using DataAccess;
using Models;
using Models.API;
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
						//return Request.CreateResponse(HttpStatusCode.NoContent);
						//var noContentResponse = Request.CreateResponse(HttpStatusCode.NoContent, response);
						//noContentResponse.Headers.Add("X-Message", "No Delta Found!"); // Adding the message in headers
						//return noContentResponse;
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
				logHelper.ApiLogWriter("locationDelta()", "DistanceController", "", ex);
				return this.Request.CreateResponse(HttpStatusCode.InternalServerError, new { status = "UNKNOWN_ERROR", message = "Error while processing the request." });

			}

		}
	}
}
