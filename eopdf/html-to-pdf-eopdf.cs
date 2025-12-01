```csharp
// NuGet: Install-Package EO.Pdfをインストール
using EO.Pdf;
using System;

class Program
{
    static void Main()
    {
        string html = "<html><body><h1>Hello World</h1><p>This is a PDF generated from HTML.</p></body></html>";
        
        HtmlToPdf.ConvertHtml(html, "output.pdf");
        
        Console.WriteLine("PDFが正常に作成されました！");
    }
}
```