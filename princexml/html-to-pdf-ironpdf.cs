```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderHtmlFileAsPdf("input.html");
        pdf.SaveAs("output.pdf");
        Console.WriteLine("PDF created successfully"); // PDFが正常に作成されました
    }
}
```