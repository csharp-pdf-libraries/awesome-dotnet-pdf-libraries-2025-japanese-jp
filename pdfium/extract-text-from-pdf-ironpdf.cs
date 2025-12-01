```csharp
// NuGet: IronPdfをインストール
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        string pdfPath = "document.pdf";
        
        var pdf = PdfDocument.FromFile(pdfPath);
        string text = pdf.ExtractAllText();
        
        Console.WriteLine(text);
    }
}
```