using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IntegrationServices.Controllers
{
	[RoutePrefix("api")]
	public class HealthController : ApiController
    {
		[HttpGet]
		[Route("health")]
		public string healthCheck()
		{
			string response = "LIVE";
			return response;

		}
	}
}
