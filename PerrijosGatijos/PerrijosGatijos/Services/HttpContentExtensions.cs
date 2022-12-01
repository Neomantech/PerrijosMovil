using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PerrijosGatijos.Services
{

	/// <summary>
	/// HttpContent extensions
	/// </summary>
	public static class HttpContentExtensions
	{

		/// <summary>
		/// Converts an object to URL Encoded Content
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="body"></param>
		public static HttpContent ToUrlEncodedContent(this object body)
		{
			var content = new FormUrlEncodedContent(body.ToDictionary().AsEnumerable());
			content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
			return content;
		}

		/// <summary>
		/// Converts an object to JSON Content
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="body"></param>
		public static HttpContent ToJsonContent(this object body)
		{
			var json = body != null ? JsonConvert.SerializeObject(body, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss" }) : "{}";
			return new StringContent(json, Encoding.UTF8, "application/json");
		}
	}
}

