```csharp
// NuGet: IronPdfをインストール
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        // URLからPDFを作成
        var renderer = new ChromePdfRenderer();
        
        var pdf = renderer.RenderUrlAsPdf("https://example.com");
        pdf.SaveAs("output.pdf");
        
        Console.WriteLine("PDF created from URL successfully!");
    }
}
```