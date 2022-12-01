using System;
using System.Collections.Generic;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using PerrijosGatijos.Droid.Renderers;
using PerrijosGatijos.Views.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRender))]
namespace PerrijosGatijos.Droid.Renderers
{
    public class CustomMapRender: MapRenderer
    {

        List<Position> routeCoordinates;

        public CustomMapRender(Context context):base(context)
        {
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (sender != null)
            {
                var formsMaps = (CustomMap)sender;
                routeCoordinates = formsMaps.RouteCoordinates;
                Control.GetMapAsync(this);
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {

            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                routeCoordinates = formsMap.RouteCoordinates;
                Control.GetMapAsync(this);
            }
        }

        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);

            var polylineOptions = new PolylineOptions();
            polylineOptions.InvokeColor(0x66FF0000);

            foreach (var position in routeCoordinates)
            {
                polylineOptions.Add(new LatLng(position.Latitude, position.Longitude));
            }

            NativeMap.AddPolyline(polylineOptions);
        }
    }
}

