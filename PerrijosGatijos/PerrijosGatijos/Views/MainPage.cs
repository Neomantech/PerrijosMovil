using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PerrijosGatijos.ViewModels;
using Xamarin.Forms;

namespace PerrijosGatijos
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<MainPageViewModel>();
        }
    }
}

