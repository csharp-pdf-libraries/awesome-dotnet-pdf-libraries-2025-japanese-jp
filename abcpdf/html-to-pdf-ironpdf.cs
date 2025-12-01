```csharp
// NuGet: IronPdfをインストール
using System;
using IronPdf;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderUrlAsPdf("https://www.example.com");
        pdf.SaveAs("output.pdf");
    }
}
```