```csharp
// NuGet: Install-Package Gnostice.PDFOne.DLL
using Gnostice.PDFOne;
using Gnostice.PDFOne.Graphics;
using System;

class Program
{
    static void Main()
    {
        PDFDocument doc = new PDFDocument();
        doc.Open();
        
        PDFPage page = doc.Pages.Add();
        
        // PDFOneは直接HTMLからPDFへの変換をサポートしていません
        // HTML変換にはDocument Studioを使用する必要があります
        // または、HTML要素を手動で解析してレンダリングします
        
        PDFTextElement textElement = new PDFTextElement();
        textElement.Text = "Simple text conversion instead of HTML";
        textElement.Draw(page, 10, 10);
        
        doc.Save("output.pdf");
        doc.Close();
    }
}
```