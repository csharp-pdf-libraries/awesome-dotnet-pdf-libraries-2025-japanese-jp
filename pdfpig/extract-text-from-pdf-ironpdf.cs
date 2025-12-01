```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        var pdf = PdfDocument.FromFile("input.pdf");
        string text = pdf.ExtractAllText();
        Console.WriteLine(text);
    }
}
```