```csharp
// NuGet: Install-Package HiQPdfをインストール
using HiQPdf;
using System;

class Program
{
    static void Main()
    {
        HtmlToPdf htmlToPdfConverter = new HtmlToPdf();
        
        // ヘッダーを追加
        htmlToPdfConverter.Document.Header.Height = 50;
        HtmlToPdfVariableElement headerHtml = new HtmlToPdfVariableElement("<div style='text-align:center'>Page Header</div>", "");
        htmlToPdfConverter.Document.Header.Add(headerHtml);
        
        // ページ番号付きのフッターを追加
        htmlToPdfConverter.Document.Footer.Height = 50;
        HtmlToPdfVariableElement footerHtml = new HtmlToPdfVariableElement("<div style='text-align:center'>Page {CrtPage} of {PageCount}</div>", "");
        htmlToPdfConverter.Document.Footer.Add(footerHtml);
        
        byte[] pdfBuffer = htmlToPdfConverter.ConvertHtmlToMemory("<h1>Document with Headers and Footers</h1>", "");
        System.IO.File.WriteAllBytes("header-footer.pdf", pdfBuffer);
    }
}
```