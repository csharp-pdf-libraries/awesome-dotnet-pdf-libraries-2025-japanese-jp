```csharp
// NuGet: IronPdfをインストール
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        // PDFレンダラーを作成
        var renderer = new ChromePdfRenderer();
        
        // HTML文字列をPDFに変換
        string htmlString = "<html><body><h1>Hello World</h1></body></html>";
        var pdf = renderer.RenderHtmlAsPdf(htmlString);
        
        // ファイルに保存
        pdf.SaveAs("output.pdf");
        
        Console.WriteLine("PDF created successfully");
    }
}
```