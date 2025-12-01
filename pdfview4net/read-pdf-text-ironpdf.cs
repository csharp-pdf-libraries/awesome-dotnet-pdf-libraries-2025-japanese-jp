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
    }
}
```