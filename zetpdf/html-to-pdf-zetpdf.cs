```csharp
// NuGet: Install-Package ZetPDF をインストール
using ZetPDF;
using System;

class Program
{
    static void Main()
    {
        var converter = new HtmlToPdfConverter();
        var htmlContent = "<html><body><h1>Hello World</h1></body></html>";
        converter.ConvertHtmlToPdf(htmlContent, "output.pdf");
        Console.WriteLine("PDF created successfully"); // PDFが正常に作成されました
    }
}
```