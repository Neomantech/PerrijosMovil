﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Biometry.Core.Services;
using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.DependencyInjection;
using PerrijosGatijos.iOS.Services;
using UIKit;

namespace PerrijosGatijos.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();
            Xamarin.FormsMaps.Init();
            PdfSharp.Xamarin.Forms.iOS.Platform.Init();

            AppCenter.Start(Constants.AppCenteriOS,
                typeof(Analytics), typeof(Crashes));

            LoadApplication(new App(AddServices));

            return base.FinishedLaunching(app, options);
        }

        static void AddServices(IServiceCollection services)
        {
            //services.AddSingleton<ILocationService, iOS.Services.LocationService>();
            //services.AddSingleton<IDBPath, DBPath>();
            //services.AddSingleton<IFirebaseService, FirbaseService>();
            services.AddSingleton<IBiometryService, BiometryService>();
            //services.AddSingleton<IAnalyticsEvents, AnalyticsEvents>();
            services.AddTransient<HttpClient>(provider => new HttpClient(new NSUrlSessionHandler()));
        }
    }
}

