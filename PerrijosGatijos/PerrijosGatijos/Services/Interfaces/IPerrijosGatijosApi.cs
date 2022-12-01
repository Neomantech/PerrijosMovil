using System;
using System.Threading.Tasks;
using PerrijosGatijos.Models.Authentication;

namespace PerrijosGatijos.Services.Interfaces
{
	public interface IPerrijosGatijosApi
	{
		Task<AuthenticationResponseModel> Authenticate(AuthenticationRequestModel model);

	}
}

