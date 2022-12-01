using System;
using System.Collections.Generic;
using PerrijosGatijos.ViewModels;
using Xamarin.Forms;

namespace PerrijosGatijos.Views
{
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<MapViewModel>();
        }
    }
}

