using System;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace PerrijosGatijos.Models.Authentication
{
    public class AuthenticationRequestModel
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }
        [DataMember(Name = "password")]
        public string Password { get; set; }
    }
}

