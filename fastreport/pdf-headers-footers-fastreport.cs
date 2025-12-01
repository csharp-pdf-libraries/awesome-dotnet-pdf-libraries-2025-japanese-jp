```csharp
// NuGet: Install-Package FastReport.OpenSource
using FastReport;
using FastReport.Export.PdfSimple;
using System.IO;

class Program
{
    static void Main()
    {
        using (Report report = new Report())
        {
            report.Load("template.frx");
            
            // レポートページのプロパティを設定
            FastReport.ReportPage page = report.Pages[0] as FastReport.ReportPage;
            
            // ページヘッダーを追加
            FastReport.PageHeaderBand header = new FastReport.PageHeaderBand();
            header.Height = 50;
            FastReport.TextObject headerText = new FastReport.TextObject();
            headerText.Text = "Document Header";
            header.Objects.Add(headerText);
            page.Bands.Add(header);
            
            // ページフッターを追加
            FastReport.PageFooterBand footer = new FastReport.PageFooterBand();
            footer.Height = 50;
            FastReport.TextObject footerText = new FastReport.TextObject();
            footerText.Text = "Page [Page]";
            footer.Objects.Add(footerText);
            page.Bands.Add(footer);
            
            report.Prepare();
            
            PDFSimpleExport pdfExport = new PDFSimpleExport();
            using (FileStream fs = new FileStream("report.pdf", FileMode.Create))
            {
                report.Export(pdfExport, fs);
            }
        }
    }
}
```