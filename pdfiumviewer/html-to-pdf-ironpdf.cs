```csharp
// NuGet: IronPdfをインストール
using IronPdf;
using System;

string htmlContent = "<h1>Hello World</h1><p>This is a test document.</p>";

// HTML文字列からPDFを作成
var renderer = new ChromePdfRenderer();
PdfDocument pdf = renderer.RenderHtmlAsPdf(htmlContent);

// PDFを保存
pdf.SaveAs("output.pdf");

Console.WriteLine("PDF created successfully!");
```