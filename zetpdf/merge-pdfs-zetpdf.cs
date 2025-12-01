```csharp
// NuGet: Install-Package ZetPDF をインストール
using ZetPDF;
using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var merger = new PdfMerger();
        var files = new List<string> { "document1.pdf", "document2.pdf" };
        merger.MergeFiles(files, "merged.pdf");
        Console.WriteLine("PDFs merged successfully"); // PDFが正常に結合されました
    }
}
```