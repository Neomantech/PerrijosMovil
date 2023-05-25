using System;
using System.IO;
using PdfSharpCore.Pdf;
using PerrijosGatijos.iOS;
using PerrijosGatijos.Models.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(PdfSave))]
namespace PerrijosGatijos.iOS
{
	public class PdfSave: IPdfSave
	{
	
        public async void Save(PdfDocument doc, string fileName)
        {
            var basepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var pdfpath = Path.Combine(basepath, fileName);

            doc.Save(fileName);
			doc.Close();

			await global::Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
				title: "Success",
				message: $"Your PDF generated and saved @ {fileName}",
				cancel: "OK");

            await Launcher.OpenAsync(new OpenFileRequest(Path.GetFileName(fileName), new ReadOnlyFile(fileName)));

            //await global::Xamarin.Essentials.Share.RequestAsync(new ShareFileRequest(new ShareFile(pdfpath)));
        }
    }
}