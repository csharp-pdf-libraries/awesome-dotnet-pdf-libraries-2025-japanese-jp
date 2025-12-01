```csharp
// NuGet: Install-Package IronPdf
using IronPdf;
using System;

string pdfPath = "document.pdf";

// PDFを開いてテキストを抽出する
PdfDocument pdf = PdfDocument.FromFile(pdfPath);

// すべてのページからテキストを抽出する
string allText = pdf.ExtractAllText();
Console.WriteLine("Extracted Text:");
Console.WriteLine(allText);

// 特定のページからテキストを抽出する
string pageText = pdf.ExtractTextFromPage(0);
Console.WriteLine($"\nFirst page text: {pageText}");

Console.WriteLine($"\nTotal pages: {pdf.PageCount}");
```