```csharp
using Kaizen.IO;
using System;
using System.IO;

class Program
{
    static void Main()
    {
        var converter = new HtmlToPdfConverter();
        var options = new ConversionOptions
        {
            // ヘッダー設定
            Header = new HeaderOptions { HtmlContent = "<div style='text-align:center'>Company Header</div>" },
            // フッター設定
            Footer = new FooterOptions { HtmlContent = "<div style='text-align:center'>Page {page} of {total}</div>" },
            // 上マージン設定
            MarginTop = 20,
            // 下マージン設定
            MarginBottom = 20
        };
        // URLからPDFに変換
        var pdfBytes = converter.ConvertUrl("https://example.com", options);
        // PDFファイルを保存
        File.WriteAllBytes("webpage.pdf", pdfBytes);
    }
}
```