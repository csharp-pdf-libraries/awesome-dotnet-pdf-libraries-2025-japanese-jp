---
**  (Japanese Translation)**

 **English:** [telerik-reporting/migrate-from-telerik-reporting.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/telerik-reporting/migrate-from-telerik-reporting.md)
 **:** [telerik-reporting/migrate-from-telerik-reporting.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/telerik-reporting/migrate-from-telerik-reporting.md)

---
# Telerik ReportingからIronPDFへの移行方法は？

## なぜTelerik Reportingから移行するのか？

Telerik Reportingはビジュアルデザイナーを備えたフル機能のエンタープライズレポーティングプラットフォームですが、PDF生成タスクにはかなりのオーバーヘッドが伴います。移行する主な理由は以下の通りです：

1. **高価なバンドルライセンス**：DevCraftバンドル（開発者1人あたり$1,000以上）またはスタンドアロンライセンスが必要
2. **レポートデザイナーの依存性**：Visual Studioの拡張機能とランタイムコンポーネントのインストールが必要
3. **複雑なインフラストラクチャ**：レポートサービスのホスティング、接続文字列、およびデータソースの設定が必要
4. **独自のフォーマット**：`.trdp`/`.trdx`ファイルを使用し、Telerikエコシステムにロックインされます
5. **重いランタイム**：単純なPDF生成のためには大きなデプロイメントフットプリントが必要
6. **年間サブスクリプション**：アップデートとサポートのための継続的なコスト

### Telerik Reportingが過剰な場合

データからPDFを生成するためだけにTelerikを使用している場合、必要のない機能に対して支払いをしています：

| 必要なもの | Telerikが提供する（未使用） |
|------------|-----------------------------|
| HTMLからのPDF | ビジュアルデザイナー、ドリルダウン |
| シンプルなレポート | インタラクティブビューアー、エクスポート |
| サーバーサイドPDF | デスクトップコントロール、チャートエンジン |

IronPDFは、エンタープライズレポーティングのオーバーヘッドなしに、集中的なPDF生成を提供します。

---

## クイックスタート：Telerik ReportingからIronPDFへ

### ステップ1：NuGetパッケージの置き換え

```bash
# Telerik Reportingパッケージを削除
dotnet remove package Telerik.Reporting
dotnet remove package Telerik.Reporting.Services.AspNetCore
dotnet remove package Telerik.ReportViewer.Mvc

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ2：名前空間の更新

```csharp
// 以前
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Telerik.Reporting.Drawing;

// 以後
using IronPdf;
```

### ステップ3：ライセンスの初期化

```csharp
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

---

## APIマッピングリファレンス

| Telerik Reporting | IronPDF | メモ |
|-------------------|---------|------|
| `Report` クラス | `ChromePdfRenderer` | コアレンダリング |
| `ReportProcessor` | `renderer.RenderHtmlAsPdf()` | PDF生成 |
| `ReportSource` | HTML文字列またはファイル | コンテンツソース |
| `.trdp` / `.trdx` ファイル | HTML/CSSテンプレート | テンプレートフォーマット |
| `ReportParameter` | 文字列補間 / Razor | パラメーター |
| `ReportDataSource` | C#データバインディング | データソース |
| `RenderReport("PDF")` | `RenderHtmlAsPdf()` | PDF出力 |
| `Export()` | `pdf.SaveAs()` | ファイル保存 |
| `TextBox` レポートアイテム | HTML `<span>`, `<p>`, `<div>` | テキスト要素 |
| `Table` レポートアイテム | HTML `<table>` | テーブル |
| `PictureBox` | HTML `<img>` | 画像 |
| `PageSettings` | `RenderingOptions` | ページ設定 |

---

## コード例

### 例1：基本的なレポート生成

**Telerik Reporting:**
```csharp
using Telerik.Reporting;
using Telerik.Reporting.Processing;

// レポート定義をロード
var reportSource = new TypeReportSource();
reportSource.TypeName = typeof(InvoiceReport).AssemblyQualifiedName;

// パラメータを設定
reportSource.Parameters.Add("InvoiceNumber", "INV-001");
reportSource.Parameters.Add("CustomerName", "Acme Corp");

// PDFにレンダリング
var reportProcessor = new ReportProcessor();
var result = reportProcessor.RenderReport("PDF", reportSource, null);

// 保存
using (FileStream fs = new FileStream("invoice.pdf", FileMode.Create))
{
    fs.Write(result.DocumentBytes, 0, result.DocumentBytes.Length);
}
```

**IronPDF:**
```csharp
using IronPdf;

var invoiceNumber = "INV-001";
var customerName = "Acme Corp";

var html = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; padding: 40px; }}
        h1 {{ color: navy; }}
        .invoice-header {{ margin-bottom: 30px; }}
        .field {{ margin: 10px 0; }}
        .label {{ font-weight: bold; display: inline-block; width: 120px; }}
    </style>
</head>
<body>
    <div class='invoice-header'>
        <h1>Invoice</h1>
        <div class='field'><span class='label'>Invoice #:</span> {invoiceNumber}</div>
        <div class='field'><span class='label'>Customer:</span> {customerName}</div>
        <div class='field'><span class='label'>Date:</span> {DateTime.Now:yyyy-MM-dd}</div>
    </div>
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("invoice.pdf");
```

### 例2：テーブルを使用したデータ駆動型レポート

**Telerik Reporting:**
```csharp
using Telerik.Reporting;
using Telerik.Reporting.Processing;

// プログラムでレポートを作成
var report = new Report();
report.Name = "SalesReport";

// データソースを設定
var sqlDataSource = new SqlDataSource();
sqlDataSource.ConnectionString = connectionString;
sqlDataSource.SelectCommand = "SELECT * FROM Orders WHERE Year = @Year";
sqlDataSource.Parameters.Add("@Year", DbType.Int32, 2024);
report.DataSource = sqlDataSource;

// テーブルを作成
var table = new Table();
table.DataSource = report.DataSource;

// 列とバインディングを設定（冗長な設定）
// ...多くの列設定...

report.Items.Add(table);

var reportProcessor = new ReportProcessor();
var result = reportProcessor.RenderReport("PDF", report, null);
```

**IronPDF:**
```csharp
using IronPdf;
using System.Data.SqlClient;

// データを取得
var orders = new List<Order>();
using (var conn = new SqlConnection(connectionString))
{
    var cmd = new SqlCommand("SELECT * FROM Orders WHERE Year = @Year", conn);
    cmd.Parameters.AddWithValue("@Year", 2024);
    conn.Open();

    using (var reader = cmd.ExecuteReader())
    {
        while (reader.Read())
        {
            orders.Add(new Order
            {
                Id = reader.GetInt32(0),
                Customer = reader.GetString(1),
                Amount = reader.GetDecimal(2),
                Date = reader.GetDateTime(3)
            });
        }
    }
}

var html = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; padding: 40px; }}
        h1 {{ color: navy; margin-bottom: 30px; }}
        table {{ width: 100%; border-collapse: collapse; }}
        th {{ background: #f0f0f0; padding: 12px; text-align: left; border: 1px solid #ddd; }}
        td {{ padding: 10px; border: 1px solid #eee; }}
        tr:nth-child(even) {{ background: #f9f9f9; }}
        .amount {{ text-align: right; }}
        .total {{ font-weight: bold; background: #e8e8e8; }}
    </style>
</head>
<body>
    <h1>Sales Report - 2024</h1>
    <table>
        <thead>
            <tr>
                <th>Order ID</th>
                <th>Customer</th>
                <th>Date</th>
                <th>Amount</th>
            </tr>
        </thead>
        <tbody>
            {string.Join("", orders.Select(o => $@"
            <tr>
                <td>{o.Id}</td>
                <td>{o.Customer}</td>
                <td>{o.Date:yyyy-MM-dd}</td>
                <td class='amount'>{o.Amount:C}</td>
            </tr>"))}
            <tr class='total'>
                <td colspan='3'>Total</td>
                <td class='amount'>{orders.Sum(o => o.Amount):C}</td>
            </tr>
        </tbody>
    </table>
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("sales_report.pdf");
```

### 例3：ページ番号を含むヘッダーとフッター

**Telerik Reporting:**
```csharp
using Telerik.Reporting;
using Telerik.Reporting.Drawing;

var report = new Report();

// ページヘッダー
var pageHeader = new PageHeaderSection();
pageHeader.Height = new Unit(0.5, UnitType.Inch);

var headerText = new TextBox();
headerText.Value = "Company Report - Confidential";
headerText.Style.Font.Size = new Unit(10, UnitType.Point);
headerText.Style.Color = Color.Gray;
pageHeader.Items.Add(headerText);
report.Items.Add(pageHeader);

// ページ番号を含むページフッター
var pageFooter = new PageFooterSection();
pageFooter.Height = new Unit(0.5, UnitType.Inch);

var pageNumber = new TextBox();
pageNumber.Value = "= 'Page ' + PageNumber + ' of ' + PageCount";
pageNumber.Style.TextAlign = HorizontalAlign.Right;
pageFooter.Items.Add(pageNumber);
report.Items.Add(pageFooter);

// 処理してレンダリング
var processor = new ReportProcessor();
var result = processor.RenderReport("PDF", report, null);
```

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

// HTMLヘッダー
renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
{
    HtmlFragment = @"
        <div style='width: 100%; font-size: 10pt; color: gray; border-bottom: 1px solid #ccc; padding-bottom: 5px;'>
            Company Report - Confidential
        </div>",
    MaxHeight = 30
};

// ページ番号を含むHTMLフッター
renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
{
    HtmlFragment = @"
        <div style='width: 100%; font-size: 10pt; color: gray; text-align: right; border-top: 1px solid #ccc; padding-top: 5px;'>
            Page {page} of {total-pages}
        </div>",
    MaxHeight = 30
};

// ヘッダー/フッター用の余白
renderer.RenderingOptions.MarginTop = 40;
renderer.RenderingOptions.MarginBottom = 40;

var pdf = renderer.RenderHtmlAsPdf(reportContent);
pdf.SaveAs("report.pdf");
```

### 例4：サブレポート/ネストされたレポート

**Telerik Reporting:**
```csharp
using Telerik.Reporting;

var mainReport = new Report();

// サブレポートを作成
var subReport = new SubReport();
subReport.ReportSource = new TypeReportSource
{
    TypeName = typeof(DetailReport).AssemblyQualifiedName
};
subReport.Parameters.Add("ParentId", "= Fields.Id");

mainReport.Items.Add(subReport);

var processor = new ReportProcessor();
var result = processor.RenderReport("PDF", mainReport, null);
```

**IronPDF:**
```csharp
using IronPdf;

// オプション1：メインHTMLに詳細セクションを埋め込む
var mainHtml = $@"
<h1>Master Report</h1>
{string.Join("", masterRecords.Select(m => $@"
    <div class='section'>
        <h2>{m.Name}</h2>
        <table>
            {string.Join("", m.Details.Select(d => $"<tr><td>{d.Item}</td><td>{d.Value}</td></tr>"))}
        </table>
    </div>
    <div style='page-break-after: always;'></div>
"))}";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(mainHtml);
pdf.SaveAs("report.pdf");

// オプション2：別々のPDFをマージ
var coverPdf = renderer.RenderHtmlAsPdf(coverHtml);
var contentPdf = renderer.RenderHtmlAsPdf(contentHtml);
var appendixPdf = renderer.RenderHtmlAsPdf(appendixHtml);

var merged = PdfDocument.Merge(coverPdf, contentPdf, appendixPdf);
merged.SaveAs("complete_report.pdf");
```

### 例5：チャートとビジュアライゼーション

**Telerik Reporting:**
```csharp
using Telerik.Reporting;

var report = new Report();

// チャートを作成
var chart = new Chart();
chart.ChartType = ChartTypes.Bar;
chart.DataSource = GetChartData();
chart.Series.Add(new ChartSeries
{
    DataFieldY = "Value",
    DataFieldX = "Category"
});

report.Items.Add(chart);

var processor = new ReportProcessor();
var result = processor.RenderReport("PDF", report, null);
```

**IronPDF (Chart.jsを使用):**
```csharp
using IronPdf;

var chartData = GetChartData();

var html = $@"
<!DOCTYPE html>
<html>
<head>
    <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>
    <style>
        body {{ font-family: Arial, sans-serif; padding: 40px; }}
        .chart-container {{ width: 600px; height: 400px; margin: 0 auto; }}
    </style>
</head>
<body>
    <h1>Sales by Category</h1>
    <div class='chart-container'>
        <canvas id='chart'></canvas>
    </div>
    <script>
        new Chart(document.getElementById('chart'), {{
            type: 'bar',
            data: {{
                labels: [{string.Join(",", chartData.Select(d => $"'{d.Category}'"))}],
                datasets: [{{
                    label: 'Value',
                    data: [{string.Join(",", chartData.Select(d => d.Value))}],
                    backgroundColor: 'rgba(54, 162, 235, 0.8)'
                }}]
            }},
            options: {{
                responsive: true,
                maintainAspectRatio: true
            }}
        }});
    </script>
</body>
</html>";

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.EnableJavaScript = true;
renderer.RenderingOptions.WaitFor.JavaScript(2000);

var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("chart_report.pdf");
```

### 例6：ASP.NET Core Web APIエンドポイント

**Telerik Reporting:**
```csharp
using Microsoft.AspNetCore.Mvc;
using Telerik.Reporting;
using Telerik.Reporting.Processing;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    [HttpGet("invoice/{id}")]
    public IActionResult GetInvoice(int id)
    {
        var reportSource = new TypeReportSource();
        reportSource.TypeName = typeof(InvoiceReport).AssemblyQualifiedName;
        reportSource.Parameters.Add("InvoiceId", id);

        var reportProcessor = new ReportProcessor();
        var result = reportProcessor.RenderReport("PDF", reportSource, null);

        return File(result.DocumentBytes, "application/pdf", $"invoice_{id}.pdf");
    }
}
```

**IronPDF:**
```csharp
using Microsoft.AspNetCore.Mvc;
using IronPdf;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public ReportsController(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpGet("invoice/{id}")]
    public IActionResult GetInvoice(int id)
    {
        var invoice = _invoiceService.GetInvoice(id);

        var html = GenerateInvoiceHtml(invoice);

        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderHtmlAsPdf(html);

        return File(pdf.BinaryData, "application/pdf", $"invoice_{id}.pdf");
    }

    private string GenerateInvoiceHtml(Invoice invoice)
    {
        return $@"
        <!DOCTYPE html>
        <html>
        <head>
            <style>
                body {{ font-family: Arial; padding: 40px; }}