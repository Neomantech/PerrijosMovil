using System;
using System.Collections.Generic;
using PerrijosGatijos.ViewModels;
using Xamarin.Forms;

namespace PerrijosGatijos.Views
{
    public partial class PetPage : ContentPage
    {
        public PetPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<PetPageViewModel>();
        }
    }
}

