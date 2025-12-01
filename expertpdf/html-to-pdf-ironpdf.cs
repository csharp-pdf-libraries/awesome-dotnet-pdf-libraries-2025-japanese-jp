```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        // HTML文字列からPDFを作成
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderHtmlAsPdf("<h1>Hello World</h1><p>This is a PDF document.</p>");
        
        // ファイルに保存
        pdf.SaveAs("output.pdf");
        
        Console.WriteLine("PDF created successfully!");
    }
}
```