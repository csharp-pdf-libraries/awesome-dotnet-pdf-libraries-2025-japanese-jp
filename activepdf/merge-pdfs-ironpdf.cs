```csharp
// NuGet: Install-Package IronPdf をインストール
using IronPdf;
using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var pdf1 = PdfDocument.FromFile("document1.pdf");
        var pdf2 = PdfDocument.FromFile("document2.pdf");
        
        var merged = PdfDocument.Merge(pdf1, pdf2);
        merged.SaveAs("merged.pdf");
        
        Console.WriteLine("PDFs merged successfully"); // PDFが正常にマージされました
    }
}
```