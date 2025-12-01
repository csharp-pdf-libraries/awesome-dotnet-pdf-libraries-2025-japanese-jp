```csharp
// NuGet: IronPdfをインストール
using IronPdf;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        
        string url = "https://www.example.com";
        var pdf = renderer.RenderUrlAsPdf(url);
        
        pdf.SaveAs("webpage.pdf");
    }
}
```