using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PerrijosGatijos.Views.Controls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ViewButton : ContentView
	{
        public event EventHandler Clicked;

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ViewButton), null);
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(ViewButton), null);
        public object CommandParameter
        {
            get => (object)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public ViewButton ()
		{
			InitializeComponent ();
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(() =>
                {
                    Clicked?.Invoke(this, EventArgs.Empty);
                    if (Command != null)
                    {
                        if (Command.CanExecute(CommandParameter))
                        {
                            Command.Execute(CommandParameter);
                        }
                    }
                })
            });
        }
	}
}

