```csharp
// SSRS - SQL Server Reporting Services
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Reporting.WebForms;
using System.IO;

class SSRSDatabaseReport
{
    static void Main()
    {
        // ReportViewer インスタンスを作成
        var reportViewer = new ReportViewer();
        reportViewer.ProcessingMode = ProcessingMode.Local;
        reportViewer.LocalReport.ReportPath = "SalesReport.rdlc";
        
        // データベース接続を作成し、データを取得
        string connString = "Server=localhost;Database=SalesDB;Integrated Security=true;";
        using (var connection = new SqlConnection(connString))
        {
            var adapter = new SqlDataAdapter("SELECT * FROM Sales", connection);
            var dataSet = new DataSet();
            adapter.Fill(dataSet, "Sales");
            
            // レポートにデータをバインド
            var dataSource = new ReportDataSource("SalesDataSet", dataSet.Tables[0]);
            reportViewer.LocalReport.DataSources.Clear();
            reportViewer.LocalReport.DataSources.Add(dataSource);
        }
        
        // PDF にレンダリング
        string mimeType, encoding, fileNameExtension;
        string[] streams;
        Warning[] warnings;
        
        byte[] bytes = reportViewer.LocalReport.Render(
            "PDF", null, out mimeType, out encoding,
            out fileNameExtension, out streams, out warnings);
        
        File.WriteAllBytes("sales-report.pdf", bytes);
    }
}
```