using System;
namespace PerrijosGatijos.Models.Api
{
	public class RequestOptions
	{
		public HttpContentType ContentType { get; set; } = HttpContentType.JsonContent;
		public bool Authenticate { get; set; } = true;
		public bool BypassException { get; set; } = false;
	}
}

