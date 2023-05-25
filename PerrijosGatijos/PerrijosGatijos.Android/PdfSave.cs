using System;
using System.IO;
using PdfSharpCore.Pdf;
using PerrijosGatijos.Droid;
using PerrijosGatijos.Models.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: Dependency(typeof(PdfSave))]
namespace PerrijosGatijos.Droid
{
	public class PdfSave: IPdfSave
	{
		public PdfSave()
		{
		}

        public async void Save(PdfDocument doc, string fileName)
        {
            string basepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var pdfpath = Path.Combine(basepath, fileName);
            doc.Save(pdfpath);
			doc.Close();

		await global::Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
				title: "Success",
				message: $"Your PDF generated and saved @ {fileName}",
				cancel: "OK");
			await Launcher.OpenAsync(new OpenFileRequest(Path.GetFileName(pdfpath), new ReadOnlyFile(pdfpath)));
            //await global::Xamarin.Essentials.Share.RequestAsync(new ShareFileRequest(new ShareFile(pdfpath)));

        }
    }
}

