```csharp
// SSRS - SQL Server Reporting Services
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Reporting.WebForms;
using System.IO;

class SSRSHtmlToPdf
{
    static void Main()
    {
        // ReportViewer インスタンスを作成
        var reportViewer = new ReportViewer();
        reportViewer.ProcessingMode = ProcessingMode.Local;
        
        // RDLC レポート定義をロード
        reportViewer.LocalReport.ReportPath = "Report.rdlc";
        
        // HTML コンテンツをパラメータまたはデータセットとして追加
        var htmlContent = "<h1>Hello World</h1><p>This is HTML content.</p>";
        var param = new ReportParameter("HtmlContent", htmlContent);
        reportViewer.LocalReport.SetParameters(param);
        
        // レポートを PDF にレンダリング
        string mimeType, encoding, fileNameExtension;
        string[] streams;
        Warning[] warnings;
        
        byte[] bytes = reportViewer.LocalReport.Render(
            "PDF",
            null,
            out mimeType,
            out encoding,
            out fileNameExtension,
            out streams,
            out warnings);
        
        File.WriteAllBytes("output.pdf", bytes);
    }
}
```