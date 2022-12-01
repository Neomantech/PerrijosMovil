using System;
using System.Collections.Generic;
using PerrijosGatijos.ViewModels;
using PerrijosGatijos.ViewModels.Controls;
using PerrijosGatijos.Views;
using PerrijosGatijos.Views.Controls;
using Xamarin.Forms;

namespace PerrijosGatijos
{
    public partial class AppShell : ExtendedShell
    {
        public AppShell()
        {
            InitializeComponent();
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}

