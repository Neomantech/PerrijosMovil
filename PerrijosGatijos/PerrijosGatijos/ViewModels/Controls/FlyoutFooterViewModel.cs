using System;
using System.Diagnostics;
using System.Linq;
using PerrijosGatijos.Helpers;
using PerrijosGatijos.Views;
using Xamarin.Forms;

namespace PerrijosGatijos.ViewModels.Controls
{
    public class FlyoutFooterViewModel : BaseViewModel
    {
        public Command CloseSessionCommand => new Command(CloseSession);

        public FlyoutFooterViewModel()
        {
        }

        public async void CloseSession()
        {
            try
            {
                Settings.IsLoggedIn = false;
                await Shell.Current.Navigation.PushAsync(new LoginPage());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            
        }
    }
}

