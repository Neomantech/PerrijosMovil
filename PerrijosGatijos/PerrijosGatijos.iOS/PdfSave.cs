using System;
using PdfSharpCore.Pdf;
using PerrijosGatijos.Models.Interfaces;

namespace PerrijosGatijos.iOS
{
	public class PdfSave: IPdfSave
	{
	
        public void Save(PdfDocument doc, string fileName)
        {
			string path = System.IO.Path.GetTempPath() + fileName;

			doc.Save(System.IO.Path.GetTempPath() + fileName);

			global::Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
				title: "Success",
				message: $"Your PDF generated and saved @ {path}",
				cancel: "OK");
		}
    }
}

//var basepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
//var pdfpath = Path.Combine(basepath, "Carnet.pdf");
////DependencyService.Get<IPdfSave>().Save(pdf, "Carnet.pdf");
//pdf.Save(pdfpath);

//         try
//         {
//             await Share.RequestAsync(new ShareFileRequest(new ShareFile(pdfpath)));

//}
//catch (Exception ex)
//         {
//             Debug.Write(ex.Message);
//         }