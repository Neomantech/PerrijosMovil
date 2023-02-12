using System;
using System.Collections.Generic;
using PerrijosGatijos.ViewModels;

using Xamarin.Forms;

namespace PerrijosGatijos.Views
{
    public partial class CheckCodePage : ContentPage
    {
        public CheckCodePage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<CheckCodePageViewModel>();
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}

