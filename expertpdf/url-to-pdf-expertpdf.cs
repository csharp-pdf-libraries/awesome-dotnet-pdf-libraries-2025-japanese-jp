```csharp
// NuGet: Install-Package ExpertPdf.HtmlToPdf
using ExpertPdf.HtmlToPdf;
using System;

class Program
{
    static void Main()
    {
        // PDFコンバータを作成する
        PdfConverter pdfConverter = new PdfConverter();
        
        // ページサイズと向きを設定する
        pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
        pdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
        
        // URLをPDFに変換する
        byte[] pdfBytes = pdfConverter.GetPdfBytesFromUrl("https://www.example.com");
        
        // ファイルに保存する
        System.IO.File.WriteAllBytes("webpage.pdf", pdfBytes);
        
        Console.WriteLine("PDF from URL created successfully!");
    }
}
```