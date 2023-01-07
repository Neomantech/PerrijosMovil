using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PerrijosGatijos.Helpers;
using PerrijosGatijos.Services;
using PerrijosGatijos.Services.Interfaces;
using PerrijosGatijos.ViewModels;
using PerrijosGatijos.ViewModels.Controls;
using PerrijosGatijos.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PerrijosGatijos
{
    public partial class App : Application
    {

        //private string secretAppiOS=""
        private ServiceCollection services;

        /// <summary>
        /// Service Provider
        /// </summary>Se ha iniciado Syste
        protected static IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets a ViewModel
        /// </summary>
        public static TViewModel GetViewModel<TViewModel>() where TViewModel : BaseViewModel => ServiceProvider.GetService<TViewModel>();


        /// <summary>
        /// Current navigation
        /// </summary>
        public static INavigation Navigation
        {
            get
            {
                if (Current.MainPage is NavigationPage page)
                {
                    if (page.CurrentPage is TabbedPage tabs)
                    {
                        return tabs.CurrentPage.Navigation;
                    }
                }

                return Current.MainPage.Navigation;
            }
        }

        public App (Action<IServiceCollection> addPlatformServices = null)
        {
            InitializeComponent();
            SetupServices(addPlatformServices);
            if (Settings.IsLoggedIn)
            {
                MainPage = new AppShell();
                //MainPage = new MainPage();
            }
            else
            {
                //MainPage = new NavigationPage(new LoginPage());
                MainPage = new NavigationPage(new CheckCodePage());
            }
        }

        /// <summary>
        /// Configure services
        /// </summary>
        void SetupServices(Action<IServiceCollection> addPlatformServices= null)
        {
            services = new ServiceCollection();

            // Configure JSON Mapper
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
            };

            // Register Platform Services
            addPlatformServices.Invoke(services);

            // Register ViewModels
            services.AddTransient<BaseViewModel>();
            services.AddTransient<LoginPageViewModel>();
            services.AddTransient<PasswordRecoveryPageViewModel>();
            services.AddTransient<RegisterUserPageViewModel>();
            services.AddTransient<MainPageViewModel>();
            services.AddTransient<MapViewModel>();
            services.AddTransient<FlyoutFooterViewModel>();
            services.AddTransient<PetPageViewModel>();
            services.AddTransient<ProfilePageViewModel>();

            // Register Services
            services.AddSingleton<IPerrijosGatijosApi>(provider =>
            {
                var client = provider.GetRequiredService<HttpClient>();
                //client.BaseAddress = new Uri(Constants.UriBase);

                return new PerrijosGatijosApi(client, GetToken);
            });

            ServiceProvider = services.BuildServiceProvider();
        }

        private async Task<string> GetToken()
        {
            //long expires = Preferences.Get(Constants.TokenExpires, (long)0);
            //if (DateTime.Now.Ticks > expires)
            //{
            //    var api = services.BuildServiceProvider().GetService<IPerrijosGatijosApi>();
            //    var authentication = await api.RefreshToken(new RefreshTokenRequestModel() { RefreshToken = Preferences.Get(Constants.RefreshToken, string.Empty) });
            //    var datetime = new DateTime(DateTime.Now.Ticks + authentication.ExpiresIn * TimeSpan.TicksPerSecond);
            //    Preferences.Set(Constants.TokenKey, authentication.AccessToken);
            //    Preferences.Set(Constants.TokenExpires, datetime.Ticks);
            //    Preferences.Set(Constants.RefreshToken, authentication.RefreshToken);
            //}
            return Preferences.Get(Constants.TokenKey, string.Empty);
        }


        private async void Connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            switch (e.NetworkAccess)
            {
                case NetworkAccess.Internet:
                    await App.Current.MainPage.DisplayAlert("Network Access", "Internet", "Aceptar");
                    break;
                case NetworkAccess.Local:
                    await App.Current.MainPage.DisplayAlert("Network Access", "Conectado pero sin internet", "Aceptar");
                    break;
                case NetworkAccess.None:
                    await App.Current.MainPage.DisplayAlert("Network Access", "Sin internet", "Aceptar");
                    break;
                case NetworkAccess.ConstrainedInternet:
                    await App.Current.MainPage.DisplayAlert("Network Access", "Constrained Internet", "Aceptar");
                    break;
                case NetworkAccess.Unknown:
                    await App.Current.MainPage.DisplayAlert("Network Access", "Unknown", "Aceptar");
                    break;
                default:
                    break;
            }
        }

        protected override void OnStart ()
        {
        }

        protected override void OnSleep ()
        {
        }

        protected override void OnResume ()
        {
        }
    }
}

