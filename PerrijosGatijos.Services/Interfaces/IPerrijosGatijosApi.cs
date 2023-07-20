using System;
using PerrijosGatijos.Models.Authentication;
using System.Threading.Tasks;

namespace PerrijosGatijos.Services.Interfaces
{
    public interface IPerrijosGatijosApi
    {
        Task<AuthenticationResponseModel> Authenticate(AuthenticationRequestModel model);

    }
}

