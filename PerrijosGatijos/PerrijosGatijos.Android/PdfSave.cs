using System;
using System.IO;
using PdfSharpCore.Pdf;
using PerrijosGatijos.Models.Interfaces;
using Xamarin.Essentials;

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

            //string path = System.IO.Path.Combine(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + fileName);


            /*
			 * var basepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var pdfpath = Path.Combine(basepath, $"/Carnet.pdf");
			 */

            doc.Save(pdfpath);
			doc.Close();

		await global::Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
				title: "Success",
				message: $"Your PDF generated and saved @ {pdfpath}",
				cancel: "OK");
			await Launcher.OpenAsync(new OpenFileRequest(Path.GetFileName(fileName), new ReadOnlyFile(fileName)));

		}
	}
}

