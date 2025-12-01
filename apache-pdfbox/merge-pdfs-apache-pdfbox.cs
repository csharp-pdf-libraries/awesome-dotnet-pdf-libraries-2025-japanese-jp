```csharp
// Apache PDFBox .NET ポート試み (不完全なサポート)
using PdfBoxDotNet.Pdmodel;
using PdfBoxDotNet.Multipdf;
using System;
using System.IO;

class Program
{
    static void Main()
    {
        // PDFBox-dotnet ポートは API カバレッジが不完全です
        var merger = new PDFMergerUtility();
        merger.AddSource("document1.pdf");
        merger.AddSource("document2.pdf");
        merger.SetDestinationFileName("merged.pdf");
        merger.MergeDocuments();
        Console.WriteLine("PDFs merged");
    }
}
```