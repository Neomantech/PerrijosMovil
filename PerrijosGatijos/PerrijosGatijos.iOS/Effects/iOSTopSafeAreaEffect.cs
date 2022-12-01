using System;
using PerrijosGatijos.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(PerrijosGatijos.iOS.Effects.iOSTopSafeAreaEffect), nameof(TopSafeAreaEffect))]
namespace PerrijosGatijos.iOS.Effects
{
    public class iOSTopSafeAreaEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            if (Element is Layout element)
            {
                var insets = UIApplication.SharedApplication.Windows[0].SafeAreaInsets;
                element.Padding = new Thickness(element.Padding.Left, element.Padding.Top + insets.Top, element.Padding.Right, element.Padding.Bottom);
            }
        }

        protected override void OnDetached()
        {
            if (Element is Layout element)
            {
                var insets = UIApplication.SharedApplication.Windows[0].SafeAreaInsets;
                element.Padding = new Thickness(element.Padding.Left, element.Padding.Top - insets.Top, element.Padding.Right, element.Padding.Bottom);
            }
        }
    }
}
