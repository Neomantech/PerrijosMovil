using System;
using PdfSharp.Xamarin.Forms;
using System.Diagnostics;
using PerrijosGatijos.Services;
using Plugin.Media.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;
using PerrijosGatijos.Views;
using System.IO;
using System.Threading.Tasks;
using PdfSharpCore.Drawing;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;
using PerrijosGatijos.Models.Interfaces;

namespace PerrijosGatijos.ViewModels
{
    public class PetPageViewModel : BaseViewModel
    {


        #region Propertys
        private MediaFile _foto;

        public MediaFile Foto
        {
            get { return _foto; }
            set { SetProperty(ref _foto, value); }
        }

        private ImageSource _imageSource;

        public ImageSource ImageSource
        {
            get { return _imageSource; }
            set { SetProperty(ref _imageSource, value); }
        }

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        #endregion

        #region Commands
        public Command PhotoCommand => new Command(TakePhoto);



        public Command GeneratePdfCommand => new Command<PetPage>( async data =>
        {
            await GeneratePdf(data);
        });

        #endregion

        public PetPageViewModel()
        {
        }

        public  async void TakePhoto()
        {
            try
            {
               var actionSheet= await App.Current.MainPage.DisplayActionSheet("¿Que deseas realizar?", "Cancelar",null, "Tomar Foto", "Abrir Galeria");
                switch (actionSheet)
                {
                    case "Cancel":
                        await App.Navigation.PopModalAsync();
                        break;
                    case "Tomar Foto":
                        Foto = await PhotoSevice.Instancia.Capture();
                        if (Foto != null)
                        {
                            ImageSource = ImageSource.FromStream(() => Foto.GetStream());
                        }
                        break;
                    case "Abrir Galeria":
                        Foto = await PhotoSevice.Instancia.SelectFile();
                        if (Foto != null)
                        {
                            ImageSource = ImageSource.FromStream(() => Foto.GetStream());
                        }
                        break;
                    default:
                        break;
                }
                
            }
            catch (Exception ex)
            {
                string error = ex.Message;
            }
        }

        private async Task GeneratePdf(PetPage petPage)
        {
            IsEnabled = false;
            IsBusy = true;
            var fileName = Constants.PdfName;
            var pdf = PDFManager.GeneratePDFFromView(petPage.Content);
            try
            {
                DependencyService.Get<IPdfSave>().Save(pdf, fileName);
                Analytics.TrackEvent("Pdf creado de manera exitosa");

            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                Debug.Write(ex.Message);
            }
            IsBusy = false;
            IsEnabled = true;
        }
    }
}

