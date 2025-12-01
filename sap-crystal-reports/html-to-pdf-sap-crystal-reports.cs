```csharp
// NuGet: Install-Package CrystalReports.Engine
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;

class Program
{
    static void Main()
    {
        // Crystal Reports には .rpt ファイルテンプレートが必要です
        ReportDocument reportDocument = new ReportDocument();
        reportDocument.Load("Report.rpt");
        
        // Crystal Reports は直接 HTML をサポートしていません
        // レポートテンプレートにデータをバインドする必要があります
        // reportDocument.SetDataSource(dataSet);
        
        ExportOptions exportOptions = reportDocument.ExportOptions;
        exportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
        exportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
        
        DiskFileDestinationOptions diskOptions = new DiskFileDestinationOptions();
        diskOptions.DiskFileName = "output.pdf";
        exportOptions.DestinationOptions = diskOptions;
        
        reportDocument.Export();
        reportDocument.Close();
        reportDocument.Dispose();
    }
}
```