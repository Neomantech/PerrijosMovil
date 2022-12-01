using System;
using Newtonsoft.Json;

namespace PerrijosGatijos.Models.Authentication
{
    public class AuthenticationResponseModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}

