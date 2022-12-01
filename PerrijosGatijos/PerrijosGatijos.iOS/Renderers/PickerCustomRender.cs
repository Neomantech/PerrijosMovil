using System;
using PerrijosGatijos.iOS.Renderers;
using PerrijosGatijos.Views.Controls;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PickerCustomControl), typeof(PickerCustomRender))]
namespace PerrijosGatijos.iOS.Renderers
{
	public class PickerCustomRender: PickerRenderer
	{
        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            var element = (PickerCustomControl)Element;

            if (Control != null && Element != null && !string.IsNullOrEmpty(element.Image))
            {
                var downarrow = UIImage.FromBundle(element.Image);
                Control.RightViewMode = UITextFieldViewMode.Always;

                var view = new UIImageView(downarrow);
                view.LayoutMargins = new UIEdgeInsets(0, 0, 0, 10);
                Control.RightView = view;
            }
        }
    }
}

