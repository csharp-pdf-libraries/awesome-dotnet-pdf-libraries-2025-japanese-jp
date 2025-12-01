```csharp
// NuGet: Install-Package PSPDFKit.Dotnet
using PSPDFKit.Pdf;
using PSPDFKit.Pdf.Annotation;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        using var processor = await PdfProcessor.CreateAsync();
        var document = await processor.OpenAsync("document.pdf");
        
        for (int i = 0; i < document.PageCount; i++)
        {
            var watermark = new TextAnnotation("CONFIDENTIAL")
            {
                Opacity = 0.5, // 不透明度を0.5に設定
                FontSize = 48 // フォントサイズを48に設定
            };
            await document.AddAnnotationAsync(i, watermark); // ウォーターマークを追加
        }
        
        await document.SaveAsync("watermarked.pdf"); // "watermarked.pdf"として保存
    }
}
```