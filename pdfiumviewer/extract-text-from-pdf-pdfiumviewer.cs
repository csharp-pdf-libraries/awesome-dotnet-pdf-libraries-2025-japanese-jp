```csharp
// NuGet: Install-Package PdfiumViewer
using PdfiumViewer;
using System;
using System.Text;

string pdfPath = "document.pdf";

// PDFiumViewerは限定的なテキスト抽出機能しか持っていません
// 主にレンダリング用に設計されており、テキスト抽出用ではありません
using (var document = PdfDocument.Load(pdfPath))
{
    int pageCount = document.PageCount;
    Console.WriteLine($"Total pages: {pageCount}");
    
    // PDFiumViewerには組み込みのテキスト抽出機能がありません
    // OCRや他のライブラリを使用する必要があります
    // ページを画像としてのみレンダリングできます
    for (int i = 0; i < pageCount; i++)
    {
        var pageImage = document.Render(i, 96, 96, false);
        Console.WriteLine($"Rendered page {i + 1}");
    }
}
```