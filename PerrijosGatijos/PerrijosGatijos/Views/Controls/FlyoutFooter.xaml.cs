using System;
using System.Collections.Generic;
using PerrijosGatijos.ViewModels.Controls;
using Xamarin.Forms;

namespace PerrijosGatijos.Views.Controls
{
    public partial class FlyoutFooter : Grid
    {
        public FlyoutFooter()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<FlyoutFooterViewModel>();
        }
    }
}

