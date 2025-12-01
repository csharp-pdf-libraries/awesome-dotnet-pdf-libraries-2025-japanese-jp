```csharp
// NuGet: IronPdfをインストール
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        var pdf = PdfDocument.FromFile("document.pdf");
        string text = pdf.ExtractAllText();
        Console.WriteLine(text);
        
        // 特定のページからテキストを抽出する
        string pageText = pdf.ExtractTextFromPage(0);
        Console.WriteLine(pageText);
    }
}
```