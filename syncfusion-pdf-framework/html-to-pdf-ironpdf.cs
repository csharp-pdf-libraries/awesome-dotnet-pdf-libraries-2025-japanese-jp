```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;

class Program
{
    static void Main()
    {
        // URLからPDFを作成
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderUrlAsPdf("https://www.example.com");
        
        // PDFを保存
        pdf.SaveAs("Output.pdf");
    }
}
```