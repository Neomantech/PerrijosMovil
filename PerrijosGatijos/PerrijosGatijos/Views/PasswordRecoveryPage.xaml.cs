using System;
using System.Collections.Generic;
using PerrijosGatijos.ViewModels;
using Xamarin.Forms;

namespace PerrijosGatijos.Views
{	
	public partial class PasswordRecoveryPage : ContentPage
	{
		private PasswordRecoveryPageViewModel viewModel;

		public PasswordRecoveryPage ()
		{
			InitializeComponent ();
			//BindingContext = viewModel = new PasswordRecoveryPageViewModel();
			//viewModel.Navigation = this.Navigation;
			BindingContext = App.GetViewModel<PasswordRecoveryPageViewModel>();
		}
	}
}

