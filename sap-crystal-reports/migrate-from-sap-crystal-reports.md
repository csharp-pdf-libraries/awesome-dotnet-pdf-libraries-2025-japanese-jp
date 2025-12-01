---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [sap-crystal-reports/migrate-from-sap-crystal-reports.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/sap-crystal-reports/migrate-from-sap-crystal-reports.md)
🇯🇵 **日本語:** [sap-crystal-reports/migrate-from-sap-crystal-reports.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/sap-crystal-reports/migrate-from-sap-crystal-reports.md)

---

# SAP Crystal ReportsからIronPDFへの移行方法は？

## SAP Crystal Reportsから移行する理由は？

SAP Crystal Reportsは、従来のエンタープライズレポーティングプラットフォームであり、現代の.NET開発にとってますます負担となっています。移行する主な理由：

1. **大規模なインストール**：Crystal Reports Runtimeは500MB以上で、複雑なインストールが必要
2. **SAPエコシステムへのロックイン**：SAPの価格設定、サポートサイクル、製品ロードマップに縛られる
3. **複雑なライセンス**：SAPのエンタープライズ販売プロセスにおけるプロセッサ/ユーザーごとのライセンス
4. **レガシーアーキテクチャ**：現代の64ビットデプロイメントを複雑にする32ビットCOM依存関係
5. **.NET Coreのサポート終了**：現代の.NETプラットフォームへの限定サポート
6. **レポートデザイナーの依存**：Visual Studioの拡張機能またはスタンドアロンのデザイナーが必要
7. **遅いパフォーマンス**：重いランタイム初期化とメモリフットプリント

### Crystal Reportsの隠れたコスト

| コスト要因 | Crystal Reports | IronPDF |
|-------------|-----------------|---------|
| ランタイムサイズ | 500MB+ | 約20MB |
| インストール | 複雑 | NuGetパッケージ |
| デプロイメント | 特別なインストーラー | xcopy |
| 64ビットサポート | 問題あり | ネイティブ |
| .NET Core/5/6/7/8 | 限定 | 完全サポート |
| クラウドデプロイメント | 困難 | 簡単 |

---

## クイックスタート：Crystal ReportsからIronPDFへ

### ステップ1：Crystal Reportsを削除

```bash
# Crystal Reportsパッケージを削除
dotnet remove package CrystalDecisions.CrystalReports.Engine
dotnet remove package CrystalDecisions.Shared
dotnet remove package CrystalDecisions.ReportAppServer
dotnet remove package CrystalDecisions.Web

# プロジェクト参照からレガシーアセンブリを削除
# プロジェクト参照でCrystalDecisions.*.dllをチェック

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ2：名前空間を更新

```csharp
// 以前
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.ReportAppServer;

// 以後
using IronPdf;
```

### ステップ3：ライセンスを初期化

```csharp
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

---

## APIマッピングリファレンス

| Crystal Reports | IronPDF | 備考 |
|-----------------|---------|-------|
| `ReportDocument` | `ChromePdfRenderer` | コアレンダリング |
| `ReportDocument.Load()` | `RenderHtmlAsPdf()` | コンテンツを読み込む |
| `.rpt` ファイル | HTML/CSS テンプレート | テンプレート形式 |
| `SetDataSource()` | HTMLでデータをバインド | データバインディング |
| `SetParameterValue()` | 文字列補間 | パラメータ |
| `ExportToDisk()` | `pdf.SaveAs()` | ファイルを保存 |
| `ExportToStream()` | `pdf.BinaryData` | バイトを取得 |
| `PrintToPrinter()` | `pdf.Print()` | 印刷 |
| `Database.Tables` | C# データアクセス | データソース |
| `FormulaFieldDefinitions` | C# ロジック | 計算 |
| `SummaryInfo` | `pdf.MetaData` | PDFメタデータ |
| `ExportFormatType.PortableDocFormat` | デフォルト出力 | PDFネイティブ |

---

## コード例

### 例1：基本的なレポートからPDFへ

**Crystal Reports:**
```csharp
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

// レポートを読み込む
ReportDocument report = new ReportDocument();
report.Load(Server.MapPath("~/Reports/Invoice.rpt"));

// データソースを設定
report.SetDataSource(GetInvoiceDataSet());

// パラメータを設定
report.SetParameterValue("InvoiceNumber", "INV-001");
report.SetParameterValue("CustomerName", "Acme Corp");

// PDFにエクスポート
report.ExportToDisk(ExportFormatType.PortableDocFormat, @"C:\Output\Invoice.pdf");

// クリーンアップ（必須！）
report.Close();
report.Dispose();
```

**IronPDF:**
```csharp
using IronPdf;

// データを取得
var invoice = GetInvoice("INV-001");

var html = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; padding: 40px; }}
        h1 {{ color: #2c3e50; border-bottom: 3px solid #3498db; padding-bottom: 15px; }}
        .header-info {{ display: flex; justify-content: space-between; margin: 30px 0; }}
        .info-block p {{ margin: 5px 0; }}
        .label {{ font-weight: bold; color: #666; }}
        table {{ width: 100%; border-collapse: collapse; margin-top: 30px; }}
        th {{ background: #3498db; color: white; padding: 12px; text-align: left; }}
        td {{ padding: 10px; border-bottom: 1px solid #eee; }}
        tr:nth-child(even) {{ background: #f9f9f9; }}
        .amount {{ text-align: right; }}
        .total-row {{ background: #2c3e50 !important; color: white; font-size: 16px; }}
    </style>
</head>
<body>
    <h1>INVOICE</h1>

    <div class='header-info'>
        <div class='info-block'>
            <p><span class='label'>Invoice #:</span> {invoice.Number}</p>
            <p><span class='label'>Date:</span> {invoice.Date:yyyy-MM-dd}</p>
            <p><span class='label'>Due Date:</span> {invoice.DueDate:yyyy-MM-dd}</p>
        </div>
        <div class='info-block'>
            <p><span class='label'>Bill To:</span></p>
            <p>{invoice.CustomerName}</p>
            <p>{invoice.CustomerAddress}</p>
        </div>
    </div>

    <table>
        <thead>
            <tr>
                <th>Description</th>
                <th>Qty</th>
                <th>Unit Price</th>
                <th>Total</th>
            </tr>
        </thead>
        <tbody>
            {string.Join("", invoice.Items.Select(i => $@"
            <tr>
                <td>{i.Description}</td>
                <td class='amount'>{i.Quantity}</td>
                <td class='amount'>{i.UnitPrice:C}</td>
                <td class='amount'>{i.LineTotal:C}</td>
            </tr>"))}
        </tbody>
        <tfoot>
            <tr>
                <td colspan='3' class='amount'><strong>Subtotal:</strong></td>
                <td class='amount'>{invoice.Subtotal:C}</td>
            </tr>
            <tr>
                <td colspan='3' class='amount'><strong>Tax ({invoice.TaxRate:P0}):</strong></td>
                <td class='amount'>{invoice.TaxAmount:C}</td>
            </tr>
            <tr class='total-row'>
                <td colspan='3' class='amount'><strong>TOTAL:</strong></td>
                <td class='amount'><strong>{invoice.Total:C}</strong></td>
            </tr>
        </tfoot>
    </table>
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs(@"C:\Output\Invoice.pdf");
// IronPDFはクリーンアップを処理するため、Dispose()は不要
```

### 例2：データベース接続レポート

**Crystal Reports:**
```csharp
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Data;

ReportDocument report = new ReportDocument();
report.Load(@"Reports\SalesReport.rpt");

// データベースログオン認証情報を設定
foreach (Table table in report.Database.Tables)
{
    TableLogOnInfo logOnInfo = table.LogOnInfo;
    logOnInfo.ConnectionInfo.ServerName = "SQLServer";
    logOnInfo.ConnectionInfo.DatabaseName = "SalesDB";
    logOnInfo.ConnectionInfo.UserID = "user";
    logOnInfo.ConnectionInfo.Password = "password";
    table.ApplyLogOnInfo(logOnInfo);
}

// パラメータを設定
report.SetParameterValue("StartDate", startDate);
report.SetParameterValue("EndDate", endDate);
report.SetParameterValue("Region", "North");

// エクスポート
Stream stream = report.ExportToStream(ExportFormatType.PortableDocFormat);
Response.ContentType = "application/pdf";
Response.BinaryWrite(((MemoryStream)stream).ToArray());

report.Close();
```

**IronPDF:**
```csharp
using IronPdf;
using Microsoft.Data.SqlClient;

// 既存のデータアクセスを使用してデータを取得
var sales = await GetSalesDataAsync(startDate, endDate, "North");

var html = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; padding: 40px; }}
        h1 {{ color: #2c3e50; }}
        .report-header {{ background: #ecf0f1; padding: 20px; margin-bottom: 30px; }}
        table {{ width: 100%; border-collapse: collapse; }}
        th {{ background: #34495e; color: white; padding: 12px; text-align: left; }}
        td {{ padding: 10px; border-bottom: 1px solid #ddd; }}
        tr:hover {{ background: #f5f5f5; }}
        .number {{ text-align: right; font-family: 'Courier New', monospace; }}
        .summary {{ margin-top: 30px; padding: 20px; background: #3498db; color: white; }}
    </style>
</head>
<body>
    <h1>Sales Report</h1>

    <div class='report-header'>
        <p><strong>Period:</strong> {startDate:MMM d, yyyy} - {endDate:MMM d, yyyy}</p>
        <p><strong>Region:</strong> North</p>
        <p><strong>Generated:</strong> {DateTime.Now:f}</p>
    </div>

    <table>
        <thead>
            <tr>
                <th>Date</th>
                <th>Product</th>
                <th>Customer</th>
                <th>Qty</th>
                <th>Amount</th>
            </tr>
        </thead>
        <tbody>
            {string.Join("", sales.Select(s => $@"
            <tr>
                <td>{s.Date:yyyy-MM-dd}</td>
                <td>{s.Product}</td>
                <td>{s.Customer}</td>
                <td class='number'>{s.Quantity}</td>
                <td class='number'>{s.Amount:C}</td>
            </tr>"))}
        </tbody>
    </table>

    <div class='summary'>
        <h2>Summary</h2>
        <p>Total Transactions: {sales.Count}</p>
        <p>Total Units: {sales.Sum(s => s.Quantity):N0}</p>
        <p>Total Revenue: {sales.Sum(s => s.Amount):C}</p>
    </div>
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);

return File(pdf.BinaryData, "application/pdf", "SalesReport.pdf");
```

### 例3：計算式フィールドと計算

**Crystal Reports:** .rptファイル内のCrystal計算式を使用

```
// Crystal 計算式フィールド
{@TotalWithDiscount} = {Orders.Amount} * (1 - {Orders.DiscountRate})
{@ProfitMargin} = ({Orders.Amount} - {Orders.Cost}) / {Orders.Amount} * 100
```

**IronPDF:** すべての計算にC#を使用

```csharp
using IronPdf;

var orders = GetOrders();

// C#で計算を実行
var orderData = orders.Select(o => new
{
    o.OrderId,
    o.Product,
    o.Amount,
    o.DiscountRate,
    o.Cost,
    TotalWithDiscount = o.Amount * (1 - o.DiscountRate),
    ProfitMargin = ((o.Amount - o.Cost) / o.Amount) * 100
}).ToList();

var html = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial; padding: 30px; }}
        table {{ width: 100%; border-collapse: collapse; }}
        th {{ background: #2c3e50; color: white; padding: 10px; }}
        td {{ padding: 8px; border: 1px solid #ddd; }}
        .number {{ text-align: right; }}
        .positive {{ color: green; }}
        .negative {{ color: red; }}
    </style>
</head>
<body>
    <h1>Orders with Calculations</h1>
    <table>
        <tr>
            <th>Order ID</th>
            <th>Product</th>
            <th>Amount</th>
            <th>Discount</th>
            <th>Net Amount</th>
            <th>Profit Margin</th>
        </tr>
        {string.Join("", orderData.Select(o => $@"
        <tr>
            <td>{o.OrderId}</td>
            <td>{o.Product}</td>
            <td class='number'>{o.Amount:C}</td>
            <td class='number'>{o.DiscountRate:P0}</td>
            <td class='number'>{o.TotalWithDiscount:C}</td>
            <td class='number {(o.ProfitMargin >= 0 ? "positive" : "negative")}'>{o.ProfitMargin:F1}%</td>
        </tr>"))}
    </table>

    <h3>Totals</h3>
    <p>Total Revenue: {orderData.Sum(o => o.TotalWithDiscount):C}</p>
    <p>Average Profit Margin: {orderData.Average(o => o.ProfitMargin):F1}%</p>
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("orders_with_calculations.pdf");
```

### 例4：サブレポート

**Crystal Reports:**
```csharp
using CrystalDecisions.CrystalReports.Engine;

ReportDocument mainReport = new ReportDocument();
mainReport.Load(@"Reports\MainReport.rpt");

// サブレポートは.rptファイルに埋め込まれ、パラメータ経由でリンクされる
mainReport.SetDataSource(GetMainData());

// サブレポートのデータソースを設定
mainReport.Subreports["DetailSubreport.rpt"].SetDataSource(GetDetailData());

mainReport.ExportToDisk(ExportFormatType.PortableDocFormat, "report.pdf");
```

**IronPDF:**
```csharp
using IronPdf;

// オプション1：すべてのデータを含む単一のHTML
var customers = GetCustomersWithDetails();

var html = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial; padding: 30px; }}
        .customer {{ margin-bottom: 50px; page-break-inside: avoid; }}
        .customer-header {{ background: #2c3e50; color: white; padding: 15px; }}
        .details {{ margin-left: 20px; }}
        table {{ width: 100%; border-collapse: collapse; margin: 10px 0; }}
        th, td {{ padding: 8px; border: 1px solid #ddd; text-align: left; }}
        th {{ background: #ecf0f1; }}
    </style>
</head>
<body>
    <h1>Customer Report with Details</h1>

    {string.Join("", customers.Select(c => $@"
    <div class='customer'>
        <div class='customer-header'>
            <h2>{c.Name