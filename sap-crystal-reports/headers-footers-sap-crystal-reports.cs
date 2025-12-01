```csharp
// NuGet: Install-Package CrystalReports.Engine
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System;

class Program
{
    static void Main()
    {
        // Crystal Reportsは、設計時の構成が必要です
        ReportDocument reportDocument = new ReportDocument();
        reportDocument.Load("Report.rpt");
        
        // ヘッダーとフッターは、.rptファイル内で設計する必要があります
        // Crystal Reportsデザイナーを使用して
        // パラメータ値はプログラムで設定できます
        reportDocument.SetParameterValue("HeaderText", "Company Name");
        reportDocument.SetParameterValue("FooterText", "Page ");
        
        // Crystal Reportsは、デザイナーで設定された数式フィールドを通じて
        // ページ番号を処理します
        
        reportDocument.ExportToDisk(ExportFormatType.PortableDocFormat, "output.pdf");
        reportDocument.Close();
        reportDocument.Dispose();
    }
}
```