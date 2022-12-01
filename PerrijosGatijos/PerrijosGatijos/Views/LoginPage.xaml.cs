using System;
using System.Collections.Generic;
using PerrijosGatijos.ViewModels;
using Xamarin.Forms;

namespace PerrijosGatijos.Views
{	
	public partial class LoginPage : ContentPage
	{
		private LoginPageViewModel viewModel;
		public LoginPage()
		{
			InitializeComponent();
			//BindingContext = viewModel = new LoginPageViewModel();
			//viewModel.Navigation = this.Navigation;
			BindingContext = App.GetViewModel<LoginPageViewModel>();
			NavigationPage.SetHasNavigationBar(this, false);
			//BindingContext = App.GetViewModel<PasswordRecoveryPageViewModel>();
		}
	}
}

