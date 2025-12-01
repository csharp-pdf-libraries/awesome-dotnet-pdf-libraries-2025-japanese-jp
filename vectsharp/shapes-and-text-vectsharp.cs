```csharp
// NuGet: Install-Package VectSharp.PDF をインストール
using VectSharp;
using VectSharp.PDF;
using System;

class Program
{
    static void Main()
    {
        Document doc = new Document();
        Page page = new Page(595, 842);
        Graphics graphics = page.Graphics;
        
        // 四角形を描画
        graphics.FillRectangle(50, 50, 200, 100, Colour.FromRgb(0, 0, 255));
        
        // 円を描画
        GraphicsPath circle = new GraphicsPath();
        circle.Arc(400, 100, 50, 0, 2 * Math.PI);
        graphics.FillPath(circle, Colour.FromRgb(255, 0, 0));
        
        // テキストを追加
        graphics.FillText(50, 200, "VectSharp Graphics", 
            new Font(FontFamily.ResolveFontFamily(FontFamily.StandardFontFamilies.Helvetica), 20));
        
        doc.Pages.Add(page);
        doc.SaveAsPDF("shapes.pdf");
    }
}
```