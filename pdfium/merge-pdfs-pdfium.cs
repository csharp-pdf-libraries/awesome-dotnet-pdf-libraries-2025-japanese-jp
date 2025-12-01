```csharp
// NuGet: Install-Package PdfiumViewer
using PdfiumViewer;
using System;
using System.IO;
using System.Collections.Generic;

// 注意: PdfiumViewerはネイティブにPDFのマージ機能を持っていません
// 追加のライブラリを使用するか、カスタムロジックを実装する必要があります
class Program
{
    static void Main()
    {
        List<string> pdfFiles = new List<string> 
        { 
            "document1.pdf", 
            "document2.pdf", 
            "document3.pdf" 
        };
        
        // PdfiumViewerは主にレンダリング/表示用です
        // PDFのマージはネイティブにサポートされていません
        // iTextSharpやPdfSharpのような別のライブラリを使用する必要があります
        
        Console.WriteLine("PDF merging not natively supported in PdfiumViewer");
    }
}
```