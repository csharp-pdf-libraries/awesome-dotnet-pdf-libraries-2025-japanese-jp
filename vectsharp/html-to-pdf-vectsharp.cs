```csharp
// NuGet: Install-Package VectSharp.PDF
using VectSharp;
using VectSharp.PDF;
using VectSharp.SVG;
using System.IO;

class Program
{
    static void Main()
    {
        // VectSharpはHTMLからPDFへの直接サポートを提供していません
        // グラフィックオブジェクトの手動作成が必要です
        Document doc = new Document();
        Page page = new Page(595, 842); // A4 サイズ
        Graphics graphics = page.Graphics;
        
        graphics.FillText(100, 100, "Hello from VectSharp", 
            new Font(FontFamily.ResolveFontFamily(FontFamily.StandardFontFamilies.Helvetica), 24));
        
        doc.Pages.Add(page);
        doc.SaveAsPDF("output.pdf");
    }
}
```