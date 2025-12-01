```csharp
using Kaizen.IO;
using System;
using System.IO;

class Program
{
    static void Main()
    {
        var converter = new HtmlToPdfConverter(); // HtmlからPdfへのコンバーターを作成します。
        var htmlContent = File.ReadAllText("input.html"); // HTMLコンテンツをファイルから読み込みます。
        var options = new ConversionOptions
        {
            PageSize = PageSize.A4, // ページサイズをA4に設定します。
            Orientation = Orientation.Portrait // オリエンテーションを縦向きに設定します。
        };
        var pdfBytes = converter.Convert(htmlContent, options); // HTMLコンテンツをPDFに変換します。
        File.WriteAllBytes("document.pdf", pdfBytes); // PDFバイトをファイルに書き込みます。
    }
}
```