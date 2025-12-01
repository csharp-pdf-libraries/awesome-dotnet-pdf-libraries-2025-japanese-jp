```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using IronPdf.Rendering;

class Program
{
    static void Main()
    {
        // HTML文字列からPDFを作成
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderHtmlAsPdf("<h1>Hello, World!</h1>");
        
        // ドキュメントを保存
        pdf.SaveAs("Output.pdf");
    }
}
```