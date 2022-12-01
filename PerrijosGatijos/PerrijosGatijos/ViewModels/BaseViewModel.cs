using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FFImageLoading.Svg.Forms;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace PerrijosGatijos.ViewModels
{
	public class BaseViewModel: ObservableObject
    {

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected bool CanExecute()
        {
            return !IsBusy;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        protected async Task RunBusyTask(Func<Task> task)
        {
            IsBusy = true;

            await task();

            IsBusy = false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        protected async Task RunBusyTask(Action task)
        {
            IsBusy = true;

            await Task.Run(task);

            IsBusy = false;
        }

        protected async Task DisplayAlertXF(string title, string message)
        {
            await App.Current.MainPage.DisplayAlert(title, message, "Aceptar");
        }

    }
}

