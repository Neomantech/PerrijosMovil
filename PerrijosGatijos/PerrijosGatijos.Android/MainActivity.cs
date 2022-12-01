using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Xamarin.Android.Net;
using System.Net.Http;
using Android;
using Plugin.CurrentActivity;

namespace PerrijosGatijos.Droid
{
    [Activity(Label = "PerrijosGatijos", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        const int RequestLocationId = 0;
        readonly string[] PermissionLocation =
        {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init(enableFastRenderer: true);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.FormsMaps.Init(this, savedInstanceState);
            PdfSharp.Xamarin.Forms.Droid.Platform.Init();
            LoadApplication(new App(AddServices));

            CrossCurrentActivity.Current.Init(this, savedInstanceState);

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();
        }

        static void AddServices(IServiceCollection services)
        {
            //services.AddSingleton<IDBPath, DBPath>();
            //services.AddSingleton<IFirebaseService, FirbaseService>();
            //services.AddSingleton<IBiometryService, BiometryService>();
            //services.AddSingleton<ILocationService, Droid.Services.LocationService>();
            //services.AddSingleton<IAnalyticsEvents, AnalyticsEvents>();
            services.AddTransient(provider => new HttpClient(new AndroidClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            }));
        }

        protected override void OnStart()
        {
            base.OnStart();
            if ((int)Build.VERSION.SdkInt >=23)
            {
                if (CheckSelfPermission(Manifest.Permission.AccessFineLocation)!=Permission.Granted)
                {
                    RequestPermissions(PermissionLocation, RequestLocationId);
                }
                else
                {
                    //Todo inventar logica de que hara esta madre cuando no se autorice el permiso, ahorita solo manda un mensaje por consola
                    System.Diagnostics.Debug.WriteLine("El usuio no autorizo el permiso");
                }
            }
        }
    }
}
