```csharp
// NuGet: Install-Package IronPdf を使用
using IronPdf;
using System;
using System.Linq;

string pdfPath = "document.pdf";
string outputImage = "page1.png";

// PDFを開いて画像に変換
PdfDocument pdf = PdfDocument.FromFile(pdfPath);

// 最初のページを画像に変換
var firstPageImage = pdf.ToBitmap(0);
firstPageImage[0].Save(outputImage);
Console.WriteLine($"Page rendered to {outputImage}");

// すべてのページを画像に変換
var allPageImages = pdf.ToBitmap();
for (int i = 0; i < allPageImages.Length; i++)
{
    allPageImages[i].Save($"page_{i + 1}.png");
    Console.WriteLine($"Saved page {i + 1}");
}

Console.WriteLine($"Total pages converted: {pdf.PageCount}");
```