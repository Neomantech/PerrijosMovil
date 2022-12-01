
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PerrijosGatijos.Droid
{
	[Activity (Label = "PerrijosGatijos", Theme = "@style/SplashTheme", MainLauncher = true, NoHistory = true, ConfigurationChanges = ConfigChanges.ScreenSize)]
	public class SplashActivity : Activity
	{
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			StartActivity(new Intent(Application.Context, typeof(MainActivity)));

			// Create your application here
		}
	}
}

