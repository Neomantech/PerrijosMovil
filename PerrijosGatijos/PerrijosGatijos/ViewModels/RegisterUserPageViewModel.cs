using System;
using PerrijosGatijos.Services.Interfaces;
using Xamarin.Forms;

namespace PerrijosGatijos.ViewModels
{
	public class RegisterUserPageViewModel: BaseViewModel
	{
		private readonly IPerrijosGatijosApi _api;


		public Command RegisterUserCommand => new Command(RegisterUser);

		public RegisterUserPageViewModel(IPerrijosGatijosApi api)
		{
			Title = "Registrar Usuario";
            _api = api;
        }

		public async void RegisterUser()
        {
			await App.Current.MainPage.DisplayAlert("Test", "Test de crear usuario", "Ok");
        }
	}
}

