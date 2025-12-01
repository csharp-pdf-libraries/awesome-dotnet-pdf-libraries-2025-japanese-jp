```csharp
// Apache PDFBox .NET ポートは実験的で不完全です
using PdfBoxDotNet.Pdmodel;
using PdfBoxDotNet.Text;
using System;
using System.IO;

class Program
{
    static void Main()
    {
        // 注意: PDFBox-dotnet は機能が限定されています
        using (var document = PDDocument.Load("document.pdf"))
        {
            var stripper = new PDFTextStripper();
            string text = stripper.GetText(document);
            Console.WriteLine(text);
        }
    }
}
```