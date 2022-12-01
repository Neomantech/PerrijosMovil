using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PerrijosGatijos.Helpers;
using PerrijosGatijos.Services.Interfaces;
using PerrijosGatijos.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PerrijosGatijos.ViewModels
{
	public class PasswordRecoveryPageViewModel: BaseViewModel
	{
        private readonly IPerrijosGatijosApi _api;


        #region Commands
        public Command emailCommand => new Command(SendEmail);

        #endregion

        #region Properties

        public string Email { get; set; }

        #endregion

        public PasswordRecoveryPageViewModel(IPerrijosGatijosApi api)
		{
            Title = "Recuperar contrseña";
            _api = api;
        }

		private async void SendEmail()
		{
            List<string> result = Email.Split(',').ToList();

            if (string.IsNullOrEmpty(Email))
            {
                await App.Current.MainPage.DisplayAlert("Atencion", "Ingresa un email", "Ok");
                return;
            }

            if (!RegexUtilities.IsValidEmail(Email))
            {
                await App.Current.MainPage.DisplayAlert("Atencion", "Email no valido", "Ok");
                return;
            }

            //Aqui va para el envio de correo ya sea dommy o por webservice

            try
            {
                var message = new EmailMessage
                {
                    Subject = "Restablecer contraseña",
                    Body = "Mesnaje de prueba",
                    To = result
                };

                await Xamarin.Essentials.Email.ComposeAsync(message);
                await App.Current.MainPage.DisplayAlert("Exito", "Tu correo se a mandado de manera exitosa", "Ok");
                await App.Navigation.PushAsync(new LoginPage());
            }

            catch (FeatureNotSupportedException fbsEx)
            {
                // Email is not supported on this device

                await App.Current.MainPage.DisplayAlert("Error", fbsEx.ToString(), "Ok");
                Debug.WriteLine(fbsEx);
            }

            catch (Exception ex)
            {
                // Some other exception occurred

                await App.Current.MainPage.DisplayAlert("Error", ex.ToString(), "Ok");
                Debug.WriteLine(ex);
            }
        }
	}
}

