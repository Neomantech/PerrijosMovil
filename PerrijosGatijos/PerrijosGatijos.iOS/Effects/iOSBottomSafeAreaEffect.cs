using System;
using PerrijosGatijos.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(PerrijosGatijos.iOS.Effects.iOSBottomSafeAreaEffect), nameof(BottomSafeAreaEffect))]
namespace PerrijosGatijos.iOS.Effects
{
    public class iOSBottomSafeAreaEffect: PlatformEffect
    {

        protected override void OnAttached()
        {
            if (Element is Layout element)
            {
                var insets = UIApplication.SharedApplication.Windows[0].SafeAreaInsets;
                element.Padding = new Thickness(element.Padding.Left, element.Padding.Top, element.Padding.Right, element.Padding.Bottom + insets.Bottom);
            }
        }

        protected override void OnDetached()
        {
            if (Element is Layout element)
            {
                element.Margin = new Thickness();
            }
        }
    }
}
