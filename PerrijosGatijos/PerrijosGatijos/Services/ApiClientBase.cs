using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PerrijosGatijos.Models;
using PerrijosGatijos.Models.Api;

namespace PerrijosGatijos.Services
{
	public class ApiClientBase
	{
		private readonly HttpClient _client;
		private readonly Func<Task<string>> _tokenGetter;
		private readonly JsonSerializer _serializer = JsonSerializer.CreateDefault();


		/// <summary>
		/// Initializes a new instance of the <see cref="ApiClientBase"/> class.
		/// </summary>
		/// <param name="client">The Http Client.</param>
		/// <param name="tokenGetter">The token getter.</param>
		public ApiClientBase(HttpClient client, Func<Task<string>> tokenGetter)
		{
			_client = client;
			_tokenGetter = tokenGetter;
		}

        /// <summary>
        /// Send POST request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="body">The body.</param>
        /// <param name="options">The request options.</param>
        protected async Task<T> PostAsync<T>(string endpoint, object body = null, RequestOptions options = null)
        {
            options = options ?? new RequestOptions();

            using (var message = new HttpRequestMessage(HttpMethod.Post, CombineUrl(_client.BaseAddress, endpoint)))
            {
                message.Content = GetContent(body, options.ContentType);
                return await SendAsync<T>(message, options.Authenticate, options.BypassException);
            }
        }

        /// <summary>
        /// Send POST request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="body">The body.</param>
        /// <param name="options">The request options.</param>
        protected async Task<PerrijosGatijosOperation<T>> PostOperationAsync<T>(string endpoint, object body = null, RequestOptions options = null)
        {
            options = options ?? new RequestOptions();

            using (var message = new HttpRequestMessage(HttpMethod.Post, CombineUrl(_client.BaseAddress, endpoint)))
            {
                message.Content = GetContent(body);
                return await SendOperationAsync<T>(message, options.Authenticate);
            }
        }

        /// <summary>
        /// Send GET request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="query">The query.</param>
        protected async Task<T> GetAsync<T>(string endpoint, object query = null) where T : class
        {
            var builder = new UriBuilder(CombineUrl(_client.BaseAddress, endpoint));

            if (query != null)
            {
                var baseQuery = string.IsNullOrEmpty(builder.Query) ? string.Empty : builder.Query.Substring(1) + "&";
                builder.Query = baseQuery + query.ToQueryString();
            }

            using (var message = new HttpRequestMessage(HttpMethod.Get, builder.Uri))
            {
                return await SendAsync<T>(message);
            }
        }

        /// <summary>
        /// Send POST request 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="stream">The body.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="options">The request options.</param>

        protected async Task<T> PostStreamAsync<T>(string endpoint, Stream stream, string filename, RequestOptions options = null)
        {
            options = options ?? new RequestOptions();

            using (var message = new HttpRequestMessage(HttpMethod.Post, CombineUrl(_client.BaseAddress, endpoint)))
            {
                using (var content = new MultipartFormDataContent())
                {
                    content.Add(new StreamContent(stream), "image", filename);
                    message.Content = content;

                    return await SendAsync<T>(message, options.Authenticate, options.BypassException);
                }
            }
        }


        protected async Task<Stream> GetStreamAsync(string endpoint, object query = null)
        {
            var builder = new UriBuilder(CombineUrl(_client.BaseAddress, endpoint));

            if (query != null)
            {
                var baseQuery = string.IsNullOrEmpty(builder.Query) ? string.Empty : builder.Query.Substring(1) + "&";
                builder.Query = baseQuery + query.ToQueryString();
            }

            using (var message = new HttpRequestMessage(HttpMethod.Get, builder.Uri))
            {
                return await SendStreamAsync(message);
            }
        }

        /// <summary>
        /// Send HTTP request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <param name="authenticate"></param>
        /// <param name="BypassException"></param>
        private async Task<T> SendAsync<T>(HttpRequestMessage request, bool authenticate = true, bool BypassException = false)
        {
            try
            {
                var response = await SendMessageAsync(request, authenticate);

                if (!BypassException)
                {
                    response.EnsureSuccessStatusCode();
                }
                else
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        return default(T);
                    }
                }

                // Decode result
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                using (var json = new JsonTextReader(reader))
                {
                    return _serializer.Deserialize<T>(json);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                System.Diagnostics.Debug.WriteLine(ex.InnerException);
                return default(T);
            }
        }

        private async Task<PerrijosGatijosOperation<T>> SendOperationAsync<T>(HttpRequestMessage request, bool authenticate = true)
        {
            var response = await SendMessageAsync(request, authenticate);
            var content = await response.Content.ReadAsStringAsync();

            var result = new PerrijosGatijosOperation<T>()
            {
                IsSucessful = response.IsSuccessStatusCode,
                Message = response.IsSuccessStatusCode ? null : content,
                Result = response.IsSuccessStatusCode ? JsonConvert.DeserializeObject<T>(content) : default(T)
            };

            return result;
        }

        private async Task<Stream> SendStreamAsync(HttpRequestMessage request, bool authenticate = true)
        {
            var response = await SendMessageAsync(request, authenticate);
            var result = await response.Content.ReadAsStreamAsync();

            return result;
        }

        private async Task<HttpResponseMessage> SendMessageAsync(HttpRequestMessage request, bool authenticate = true)
        {
            if (authenticate)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _tokenGetter());
            }

            // Send request
            return await _client.SendAsync(request);
        }

        private HttpContent GetContent(object body, HttpContentType type = HttpContentType.JsonContent)
        {
            switch (type)
            {
                case HttpContentType.JsonContent:
                    return body.ToJsonContent();
                case HttpContentType.UrlEncodedContent:
                    return body.ToUrlEncodedContent();
                default:
                    throw new NotImplementedException();
            }
        }

        private Uri CombineUrl(Uri baseUrl, string relativeUrl)
        {
            if (Uri.TryCreate(baseUrl, relativeUrl, out var relativeUri))
            {
                return relativeUri;
            }

            throw new ArgumentException("Unable to combine specified url values");
        }
    }
}

