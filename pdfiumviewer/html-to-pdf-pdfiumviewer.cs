```csharp
// NuGet: Install-Package PdfiumViewer
using PdfiumViewer;
using System.IO;
using System.Drawing.Printing;

// PDFiumViewerは主にPDFビューアー/レンダラーであり、ジェネレーターではありません
// HTMLをPDFに直接変換することはできません
// 最初に別のライブラリを使用してPDFを作成する必要があります
// その後、PDFiumViewerを使用して表示します：

string htmlContent = "<h1>Hello World</h1><p>This is a test document.</p>";

// この機能はPDFiumViewerでは利用できません
// wkhtmltopdfや類似の別のライブラリが必要になります
// PDFiumViewerは既存のPDFを開いて表示することのみ可能です：

string existingPdfPath = "output.pdf";
using (var document = PdfDocument.Load(existingPdfPath))
{
    // 既存のPDFのみをレンダリング/表示できます
    var image = document.Render(0, 300, 300, true);
}
```