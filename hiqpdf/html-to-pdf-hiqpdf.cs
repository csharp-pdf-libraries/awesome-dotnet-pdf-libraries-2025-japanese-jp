```csharp
// NuGet: Install-Package HiQPdf をインストール
using HiQPdf;
using System;

class Program
{
    static void Main()
    {
        HtmlToPdf htmlToPdfConverter = new HtmlToPdf();
        byte[] pdfBuffer = htmlToPdfConverter.ConvertUrlToMemory("https://example.com");
        System.IO.File.WriteAllBytes("output.pdf", pdfBuffer);
        
        // HTML文字列を変換
        string html = "<h1>Hello World</h1><p>This is a PDF document.</p>";
        byte[] pdfFromHtml = htmlToPdfConverter.ConvertHtmlToMemory(html, "");
        System.IO.File.WriteAllBytes("fromhtml.pdf", pdfFromHtml);
    }
}
```