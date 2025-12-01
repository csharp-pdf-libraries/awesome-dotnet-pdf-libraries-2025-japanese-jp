```csharp
// NuGet: IronPdfをインストール
using IronPdf;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderHtmlAsPdf("<h1>ドキュメントのメインコンテンツ</h1>");
        
        pdf.AddTextHeader("ドキュメントのヘッダー");
        pdf.AddTextFooter("ページ {page}");
        
        pdf.SaveAs("header-footer.pdf");
    }
}
```