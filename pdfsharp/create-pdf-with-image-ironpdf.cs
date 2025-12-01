```csharp
// NuGet: Install-Package IronPdf を使用
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        // HTMLと画像からPDFを作成
        var renderer = new ChromePdfRenderer();
        
        string html = @"
            <h1>Image in PDF</h1>
            <img src='image.jpg' style='width:200px; height:200px;' />
            <p>HTMLを使った簡単な画像埋め込み</p>";
        
        var pdf = renderer.RenderHtmlAsPdf(html);
        pdf.SaveAs("output.pdf");
        
        // 代替案: 既存のPDFに画像を追加
        var existingPdf = new ChromePdfRenderer().RenderHtmlAsPdf("<h1>Document</h1>");
        var imageStamper = new IronPdf.Editing.ImageStamper(new Uri("image.jpg"))
        {
            VerticalAlignment = IronPdf.Editing.VerticalAlignment.Top
        };
        existingPdf.ApplyStamp(imageStamper);
    }
}
```