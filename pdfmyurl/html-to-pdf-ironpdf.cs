```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using System;

class Example
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderUrlAsPdf("https://example.com");
        pdf.SaveAs("output.pdf");
    }
}
```