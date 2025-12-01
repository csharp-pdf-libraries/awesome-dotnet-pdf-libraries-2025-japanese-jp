```csharp
// NuGet: IronPdfをインストール
using IronPdf;

class IronPdfExample
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderHtmlFileAsPdf("input.html");
        pdf.SaveAs("output.pdf");
    }
}
```