using System;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PerrijosGatijos.Services
{
    public class PhotoSevice
    {
        private static PhotoSevice _instanacia;

        public static PhotoSevice Instancia
        {
            get
            {
                if (_instanacia == null)
                {
                    _instanacia = new PhotoSevice();
                }

                return _instanacia;
            }
        }

        public async Task<MediaFile> Capture()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

                if (status == PermissionStatus.Granted)
                {
                    var options = new MediaPickerOptions()
                    {
                        Title = "Select a photho",
                    };

                    if (MediaPicker.IsCaptureSupported && CrossMedia.Current.IsCameraAvailable)
                    {

                        //return await MediaPicker.CapturePhotoAsync(options);
                        return await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions());
                    }
                    //else
                    //{
                    //    return await MediaPicker.PickPhotoAsync(options);
                    //}
                }
                else
                {
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        if (status == PermissionStatus.Unknown)
                        {
                            var request = await Permissions.RequestAsync<Permissions.Camera>();

                            if (request == PermissionStatus.Granted)
                            {
                                return await Capture();
                            }
                        }
                    }
                    else if (Device.RuntimePlatform == Device.Android)
                    {
                        if (status == PermissionStatus.Denied)
                        {
                            var request = await Permissions.RequestAsync<Permissions.Camera>();

                            if (request == PermissionStatus.Granted)
                            {
                                return await Capture();
                            }
                        }
                    }
                    else
                    {
                        throw new OperationCanceledException("Permission are disabled for this operation");
                    }
                }

                throw new OperationCanceledException("Operation is not supported");
            }
            catch (OperationCanceledException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            }

            return null;
        }

        public async Task<MediaFile> SelectFile()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

                if (status == PermissionStatus.Granted)
                {
                    var options = new MediaPickerOptions()
                    {
                        Title = "Select a photho",
                    };
                    //return await MediaPicker.PickPhotoAsync(options);
                    return await CrossMedia.Current.PickPhotoAsync();
                }
                else
                {
                    var flag = await PermissionCamera(status);
                    if (flag)
                        return await SelectFile();
                    else
                    {
                        throw new OperationCanceledException("Permission are disabled for this operation");
                    }
                }

                throw new OperationCanceledException("Operation is not supported");
            }
            catch (OperationCanceledException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            }

            return null;
        }

        private async Task<bool> PermissionCamera(PermissionStatus status)
        {
            if (Device.RuntimePlatform == Device.iOS)
            {
                if (status == PermissionStatus.Unknown)
                {
                    var request = await Permissions.RequestAsync<Permissions.Camera>();

                    if (request == PermissionStatus.Granted)
                    {
                        return true;
                        //return await Capture();
                    }
                }
            }
            else if (Device.RuntimePlatform == Device.Android)
            {
                if (status == PermissionStatus.Denied)
                {
                    var request = await Permissions.RequestAsync<Permissions.Camera>();

                    if (request == PermissionStatus.Granted)
                    {
                        return true;
                        //return await Capture();
                    }
                }
            }
            else
            {
                return false;
                //throw new OperationCanceledException("Permission are disabled for this operation");
            }
            return false;
        }
    }
}

