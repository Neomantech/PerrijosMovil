using System;
using PdfSharpCore.Pdf;

namespace PerrijosGatijos.Models.Interfaces
{
	public interface IPdfSave
	{
		void Save(PdfDocument doc, string fileName);

	}
}

