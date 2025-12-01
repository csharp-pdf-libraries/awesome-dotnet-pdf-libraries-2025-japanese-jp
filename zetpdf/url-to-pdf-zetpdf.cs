```csharp
// NuGet: ZetPDFをインストール
using ZetPDF;
using System;

class Program
{
    static void Main()
    {
        var converter = new HtmlToPdfConverter();
        var url = "https://www.example.com";
        converter.ConvertUrlToPdf(url, "webpage.pdf");
        Console.WriteLine("PDF from URL created successfully"); // URLからPDFが正常に作成されました
    }
}
```