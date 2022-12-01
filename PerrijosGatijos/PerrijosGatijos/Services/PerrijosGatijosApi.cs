using System;
using System.Net.Http;
using System.Threading.Tasks;
using PerrijosGatijos.Models;
using PerrijosGatijos.Models.Api;
using PerrijosGatijos.Models.Authentication;
using PerrijosGatijos.Services.Interfaces;

namespace PerrijosGatijos.Services
{
	public class PerrijosGatijosApi: ApiClientBase, IPerrijosGatijosApi
	{
		public PerrijosGatijosApi(HttpClient client, Func<Task<string>> tokenGetter) : base(client, tokenGetter )
		{
		}

        public Task<AuthenticationResponseModel> Authenticate(AuthenticationRequestModel model)
        {
            return PostAsync<AuthenticationResponseModel>("Token", model, new RequestOptions()
            {
                Authenticate = false,
                ContentType = HttpContentType.UrlEncodedContent
            });
        }
    }
}

