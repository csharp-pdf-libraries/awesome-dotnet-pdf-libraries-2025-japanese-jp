```csharp
// NuGet: Install-Package ExpertPdf.HtmlToPdf
using ExpertPdf.HtmlToPdf;
using System;

class Program
{
    static void Main()
    {
        // PDF コンバーターを作成する
        PdfConverter pdfConverter = new PdfConverter();
        
        // ヘッダーを有効にする
        pdfConverter.PdfHeaderOptions.ShowHeader = true;
        pdfConverter.PdfHeaderOptions.HeaderText = "Document Header";
        pdfConverter.PdfHeaderOptions.HeaderTextAlignment = HorizontalTextAlign.Center;
        
        // ページ番号付きのフッターを有効にする
        pdfConverter.PdfFooterOptions.ShowFooter = true;
        pdfConverter.PdfFooterOptions.FooterText = "Page &p; of &P;";
        pdfConverter.PdfFooterOptions.FooterTextAlignment = HorizontalTextAlign.Right;
        
        // HTML ファイルを PDF に変換する
        byte[] pdfBytes = pdfConverter.GetPdfBytesFromHtmlFile("input.html");
        
        // ファイルに保存する
        System.IO.File.WriteAllBytes("output-with-header-footer.pdf", pdfBytes);
        
        Console.WriteLine("PDF with headers and footers created successfully!");
    }
}
```