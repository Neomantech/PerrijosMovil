using System;
using Plugin.Media.Abstractions;
using PerrijosGatijos.Services;
using Xamarin.Forms;

namespace PerrijosGatijos.ViewModels
{
    public class ProfilePageViewModel : BaseViewModel
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

        #endregion

        public ProfilePageViewModel()
        {
        }

        //public async void Load()
        //{
        //    var bitmap = await  FirmaPaciente.GetImageStreamAsync(SignatureImageFormat.Png, Color.Black, Color.White, 1f);
        //    var dest = System.IO.File.OpenWrite(path);
        //    bitmap.CopyToAsync(dest);
        //}

        public async void TakePhoto()
        {
            try
            {
                var actionSheet = await App.Current.MainPage.DisplayActionSheet("¿Que deseas realizar?", "Cancelar", null, "Tomar Foto", "Abrir Galeria");
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
    }
}

