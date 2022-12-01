using System;
using System.Collections.Generic;
using PerrijosGatijos.ViewModels;

using Xamarin.Forms;

namespace PerrijosGatijos.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<ProfilePageViewModel>();
        }
    }
}

