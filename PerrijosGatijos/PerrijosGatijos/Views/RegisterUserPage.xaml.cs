using System;
using System.Collections.Generic;
using PerrijosGatijos.ViewModels;
using Xamarin.Forms;

namespace PerrijosGatijos.Views
{	
	public partial class RegisterUserPage : ContentPage
	{
		private RegisterUserPageViewModel viewModel;

		public RegisterUserPage ()
		{
			InitializeComponent ();
			//BindingContext = viewModel = new RegisterUserPageViewModel();
			//viewModel.Navigation = this.Navigation;
			BindingContext = App.GetViewModel<RegisterUserPageViewModel>();


		}
	}
}

