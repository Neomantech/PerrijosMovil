using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Biometry.Core.Services;
using PerrijosGatijos.Helpers;
using PerrijosGatijos.Models;
using PerrijosGatijos.Models.Authentication;
using PerrijosGatijos.Models.Class;
using PerrijosGatijos.Services.Interfaces;
using PerrijosGatijos.Views;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PerrijosGatijos.ViewModels
{
	public class LoginPageViewModel:BaseViewModel
	{
        private readonly IPerrijosGatijosApi _api;
        private readonly IBiometryService _biometryService;

        #region Commands
        public IAsyncCommand StartCommand => CommandFactory.Create(OnStart, _ => !IsBusy);
        public IAsyncCommand LoginCommand => CommandFactory.Create(DoLogin, _ => !IsBusy, allowsMultipleExecutions: false);
        public IAsyncCommand BiometricCommand => CommandFactory.Create(DoBiometricLogin, () => !IsBusy);
        //public Command LoginCommand => new Command(login);
        public Command FogotCommand => new Command(FogotPassword);
        public Command RegisterUserCommand => new Command(RegisterUser);
        #endregion

        #region Properties

        private LoginModel _login = new LoginModel();
        private bool _isFingerprintAvailable;
        //private LoginModel _model = new();


        public LoginModel Login
        {
            get =>  _login; 
            set => SetProperty(ref _login, value); 
        }

        public bool IsFingerprintAvailable
        {
            get => _isFingerprintAvailable;
            set => SetProperty(ref _isFingerprintAvailable, value);
        }

        #endregion


        public LoginPageViewModel(IPerrijosGatijosApi api, IBiometryService biometryService)
		{
            _api = api;
            _biometryService = biometryService;
        }

        private async Task OnStart()
        {
#if DEBUG
            Login.Username = "User";
            Login.Password = "12345";
#endif

            var isFingerPrintAvilable = _biometryService.IsAvailableAsync();
            Login.BiometricAvailable = isFingerPrintAvilable;

            if (Preferences.ContainsKey(Constants.UserKey))
            {
                Login.Username = Preferences.Get(Constants.UserKey, string.Empty);
                var credentials = Preferences.Get(Constants.CredentialsSaved, false);
                Login.SaveCredentials = credentials;
                IsFingerprintAvailable = Login.BiometricAvailable && Login.SaveCredentials;
                if (Login.SaveCredentials)
                {
                    //Check AutoLogin
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        var page = Application.Current.MainPage.Navigation.NavigationStack.Last();
                        foreach (var item in Application.Current.MainPage.Navigation.NavigationStack)
                        {
                            if (item != page)
                            {
                                Application.Current.MainPage.Navigation.RemovePage(item);
                            }
                            await Application.Current.MainPage.Navigation.PushAsync(new AppShell());
                            Application.Current.MainPage.Navigation.RemovePage(page);
                            IsBusy = false;
                        }
                    });

                    await BiometricLogin();
                }
                else
                {

                }
            }

            else
            {
                if (!Preferences.Get(Constants.SkipOnboardingKey,false))
                {
                    Preferences.Set(Constants.SkipOnboardingKey, true);
                    await Application.Current.MainPage.Navigation.PushModalAsync(new LoginPage());
                }
            }

        }

        private async Task DoLogin()
        {
            await login();
        }

        public async Task login(bool fromFingerprint = false)
        {
            IsBusy = true;
            Title = string.Empty;
            if (Login.Username != null && Login.Password != null)
            {
                try
                {
                    if (Login.Username != null)
                    {
                        if (Login.Password != null)
                        {

                            if (Login.Username == "User" && Login.Password == "12345")
                            {


                                //var aunthentication = await _api.Authenticate(new AuthenticationRequestModel() { Username = Login.User, Password = Login.Password });

                                if (!fromFingerprint)
                                {


                                    Preferences.Set(Constants.UserKey, Login.Username); //Cambiar "Login.User" por la propiedad del modelo de respuesta cuando Nery mem pase el servicio
                                    Preferences.Set(Constants.CredentialsSaved, Login.SaveCredentials);
                                    if (Login.SaveCredentials)
                                    {
                                        var response = await _biometryService.AddOrUpdateItemAsync(Constants.PasswordKey, Login.Password);
                                        if (response == BiometryStorageResult.Success)
                                        {
                                            if (Device.RuntimePlatform == Device.iOS)
                                            {
                                                var authFaceID = await _biometryService.AuthenticateAsync();
                                                switch (authFaceID)
                                                {
                                                    case BiometryAuthResult.BIOMETRY_SUCCESS:
                                                        Preferences.Set(Constants.PasswordKey, Login.Password);
                                                        break;
                                                    case BiometryAuthResult.BIOMETRY_FAILED:
                                                        Preferences.Set(Constants.CredentialsSaved, false);
                                                        _biometryService.DeleteItem(Constants.PasswordKey);
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                Preferences.Set(Constants.PasswordKey, Login.Password);
                                            }
                                        }
                                        else
                                        {
                                            Preferences.Set(Constants.CredentialsSaved, false);
                                            _biometryService.DeleteItem(Constants.PasswordKey);
                                        }
                                        
                                    }
                                    else
                                    {
                                        _biometryService.DeleteItem(Constants.PasswordKey);
                                    }

                                }
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
                    LoginCommand.RaiseCanExecuteChanged();
                }
            }
            else
            {
                IsBusy = false;
                await App.Current.MainPage.DisplayAlert("Error", "Usuario y contraseña requeridos", "Ok");
            }
        }

        private async Task DoBiometricLogin()
        {
            await OnBiometricLogin();
        }


        private async Task OnBiometricLogin()
        {
            await RunBusyTask(BiometricLogin);
        }

        private async Task BiometricLogin()
        {
            if (Login.BiometricAvailable)
            {
                var (result, message) = await _biometryService.GetItemAsync<string>(Constants.PasswordKey);

                if (result == BiometryStorageResult.Success)
                {
                    Login.Password = message;
                    await login(true);
                }
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

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName is nameof(IsBusy))
            {
                StartCommand.RaiseCanExecuteChanged();
                LoginCommand.RaiseCanExecuteChanged();
                BiometricCommand.RaiseCanExecuteChanged();
            }
        }
    }
}

