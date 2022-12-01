using System;
using Xamarin.Forms;

namespace PerrijosGatijos.Views.Controls
{
	public class PickerCustomControl: Picker
	{
        public static readonly BindableProperty ImageProperty =
            BindableProperty.Create(nameof(Image), typeof(string), typeof(PickerCustomControl), string.Empty);

        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }
    }
}

