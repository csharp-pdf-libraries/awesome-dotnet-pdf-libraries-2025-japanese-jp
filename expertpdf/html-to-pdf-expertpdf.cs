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
        
        // HTML文字列をPDFに変換
        byte[] pdfBytes = pdfConverter.GetPdfBytesFromHtmlString("<h1>Hello World</h1><p>This is a PDF document.</p>");
        
        // ファイルに保存
        System.IO.File.WriteAllBytes("output.pdf", pdfBytes);
        
        Console.WriteLine("PDF created successfully!");
    }
}
```