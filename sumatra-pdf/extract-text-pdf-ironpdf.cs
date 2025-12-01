```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        var pdf = PdfDocument.FromFile("document.pdf");
        
        // すべてのページからテキストを抽出
        string allText = pdf.ExtractAllText();
        Console.WriteLine("Extracted Text:");
        Console.WriteLine(allText);
        
        // 特定のページからテキストを抽出
        string pageText = pdf.ExtractTextFromPage(0);
        Console.WriteLine($"\nFirst Page Text:\n{pageText}");
    }
}
```