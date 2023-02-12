using System;
using Xamarin.Forms;
using PerrijosGatijos.Views;
using PerrijosGatijos.Models.ValidateCode;

namespace PerrijosGatijos.ViewModels
{
    public class CheckCodePageViewModel: BaseViewModel
    {
        public Command GotoLoginCommand => new Command(GoToLogin);
        public bool Enabled = false;

        private ValidateCodeModel _validateCode = new ValidateCodeModel();

        public ValidateCodeModel ValidateCode
        {
            get { return _validateCode; }
            set { SetProperty(ref _validateCode, value); }
        }

        public CheckCodePageViewModel()
        {
        }

        public async void GoToLogin()
        {
            try
            {

            
            if (ValidateCode.Code!=null)
            {
                if (ValidateCode.Code=="12345")
                {
                    await App.Navigation.PushAsync(new LoginPage());

                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Atención", "Codigo no valido", "Ok");
                }
            }

            else
            {
                await App.Current.MainPage.DisplayAlert("Atención", "Por favor ingresa el codigo de verificación", "Ok");

            }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error de conexion", ex.Message, "Ok");
            }
        }
    }
}

