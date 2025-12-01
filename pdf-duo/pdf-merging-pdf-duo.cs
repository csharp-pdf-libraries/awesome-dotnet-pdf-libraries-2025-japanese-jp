```csharp
// NuGet: Install-Package PDFDuo.NET
using PDFDuo;
using System;

class Program
{
    static void Main()
    {
        var merger = new PdfMerger();
        merger.AddFile("document1.pdf"); // "document1.pdf"を追加
        merger.AddFile("document2.pdf"); // "document2.pdf"を追加
        merger.Merge("merged.pdf"); // マージ
        Console.WriteLine("PDFs merged successfully!"); // PDFが正常にマージされました！
    }
}
```