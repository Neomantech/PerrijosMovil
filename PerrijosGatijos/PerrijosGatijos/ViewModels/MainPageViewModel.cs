using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using PdfSharp.Xamarin.Forms;
using PdfSharpCore.Pdf;
using PerrijosGatijos.Models.Interfaces;
using PerrijosGatijos.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PerrijosGatijos.ViewModels
{
	public class MainPageViewModel:BaseViewModel
	{
		//public Command createPdfCommand => new Command(CreatePdf);
		public Command createPdfCommand => new Command<MainPage>(async data =>
		{
			await CreatePdf(data);
		});

		private bool _isEnabled = true;
		public bool IsEnabled
        {
            get { return _isEnabled; }
			set { SetProperty(ref _isEnabled, value); }
		}

		

		public MainPageViewModel()
		{
			//ComponentInfo.SetLicense("FREE-LIMITED-KEY");
		}

		private async Task CreatePdf(MainPage Page)
        {

			IsEnabled = false;
			IsBusy = true;
            var pdf = PDFManager.GeneratePDFFromView(Page.Content);

            var basepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var pdfpath = Path.Combine(basepath, "Test.pdf");
            pdf.Save(pdfpath);

            try
            {
                await Share.RequestAsync(new ShareFileRequest(new ShareFile(pdfpath)));

			}
			catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }


            IsBusy = false;
			IsEnabled = true;

		}

		private string CreateDocument(MainPage Page)
		{
			var document = new PdfDocument();

			//for (int i = 0; i < pages.Value; ++i)
				document.Pages.Add();

			var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Example.pdf");

			document.Save(filePath);

			return filePath;
		}
	}
}

