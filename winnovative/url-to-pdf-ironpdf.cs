```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        // PDF レンダラーを作成
        var renderer = new ChromePdfRenderer();
        
        // URL を PDF に変換
        string url = "https://www.example.com";
        var pdf = renderer.RenderUrlAsPdf(url);
        
        // ファイルに保存
        pdf.SaveAs("webpage.pdf");
        
        Console.WriteLine("PDF from URL created successfully");
    }
}
```