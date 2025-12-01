```csharp
// NuGet: IronPdfをインストール
using IronPdf;

class Program
{
    static void Main(string[] args)
    {
        var renderer = new ChromePdfRenderer();
        
        var pdf = renderer.RenderUrlAsPdf("https://www.example.com");
        pdf.SaveAs("webpage.pdf");
    }
}
```