using System;
using System.Windows.Input;
using PerrijosGatijos.Helpers;
using PerrijosGatijos.Models;
using PerrijosGatijos.Models.Authentication;
using PerrijosGatijos.Models.Class;
using PerrijosGatijos.Services.Interfaces;
using PerrijosGatijos.Views;
using Xamarin.Forms;

namespace PerrijosGatijos.ViewModels
{
	public class LoginPageViewModel:BaseViewModel
	{
        private readonly IPerrijosGatijosApi _api;

        #region Commands
        public Command LoginCommand => new Command(login);
        public Command FogotCommand => new Command(FogotPassword);
        public Command RegisterUserCommand => new Command(RegisterUser);
        #endregion

        #region Properties

        private LoginModel _login = new LoginModel();

        public LoginModel Login
        {
            get { return _login; }
            set { SetProperty(ref _login, value); }
        }

        #endregion


        public LoginPageViewModel(IPerrijosGatijosApi api)
		{
            _api = api;
#if DEBUG
            Login.User = "User";
            Login.Password = "12345";
#endif
        }

        public async void login()
        {
            IsBusy = true;
            Title = string.Empty;
            try
            {
                if (Login.User != null  )
                {
                    if (Login.Password != null)
                    {

                        if (Login.User == "User" && Login.Password == "12345")
                        {


                            //var aunthentication = await _api.Authenticate(new AuthenticationRequestModel() { Username = Login.User, Password = Login.Password });
                            Settings.IsLoggedIn = true;
                            Application.Current.MainPage = new AppShell();
                            await App.Navigation.PopToRootAsync(true);
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Alerta", "Usuario o contraseña incorrectos", "OK");
                        }

                        IsBusy = false;
                    }

                    else
                    {
                        IsBusy = false;
                        await App.Current.MainPage.DisplayAlert("Error", "La contraseña es requerida", "Ok");

                    }
                }

                else
                {
                    IsBusy = false;
                    await App.Current.MainPage.DisplayAlert("Error", "El usuario es requerido", "Ok");
                }
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await App.Current.MainPage.DisplayAlert("Error de conexion", ex.Message, "Ok");
            }
        }


        public async void FogotPassword()
        {
            IsBusy = false;
            Title = string.Empty;
            try
            {
                await App.Navigation.PushAsync(new PasswordRecoveryPage());
        }
            catch (Exception ex)
            {
                IsBusy = false;
                await App.Current.MainPage.DisplayAlert("Error de conexion", ex.Message, "Ok");
    }
}

        public async void RegisterUser()
        {
            IsBusy = false;
            Title = String.Empty;
            try
            {
                await App.Navigation.PushAsync(new RegisterUserPage());
            }
            catch (Exception ex)
            {
                IsBusy = false;
                await App.Current.MainPage.DisplayAlert("Error de conexion", ex.Message, "Ok");
            }
        }
    }
}

