```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using IronPdf.Rendering;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        renderer.RenderingOptions.PaperSize = PdfPaperSize.Letter;
        renderer.RenderingOptions.MarginTop = 40;
        renderer.RenderingOptions.MarginBottom = 40;
        renderer.RenderingOptions.MarginLeft = 40;
        renderer.RenderingOptions.MarginRight = 40;
        renderer.RenderingOptions.PrintHtmlBackgrounds = true;
        
        var pdf = renderer.RenderHtmlFileAsPdf("document.html"); // "document.html" をPDFに変換
        pdf.SaveAs("output.pdf"); // "output.pdf" として保存
    }
}
```