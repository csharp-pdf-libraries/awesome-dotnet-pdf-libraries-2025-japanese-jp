```csharp
// NuGet: Install-Package PdfSharp-MigraDoc-GDI
using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        // MigraDocはHTMLを直接サポートしていません
        // ドキュメント構造を手動で作成する必要があります
        Document document = new Document();
        Section section = document.AddSection();
        
        Paragraph paragraph = section.AddParagraph();
        paragraph.AddFormattedText("Hello World", TextFormat.Bold);
        paragraph.Format.Font.Size = 16;
        
        PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer();
        pdfRenderer.Document = document;
        pdfRenderer.RenderDocument();
        pdfRenderer.PdfDocument.Save("output.pdf");
    }
}
```