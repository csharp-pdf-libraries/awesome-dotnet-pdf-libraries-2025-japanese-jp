# SQL Server Reporting Services (SSRS) から IronPDF への移行方法は？

## SSRS から移行する理由は？

SQL Server Reporting Services (SSRS) は、Microsoft のエンタープライズレポーティングプラットフォームであり、大きなインフラ投資が必要です。移行する主な理由は以下の通りです：

1. **重いインフラストラクチャ**：SQL Server、レポートサーバー、および IIS の設定が必要
2. **Microsoft エコシステムへのロックイン**：SQL Server のライセンスと Windows Server に縛られる
3. **複雑なデプロイメント**：レポートのデプロイメント、セキュリティ設定、およびサブスクリプション管理
4. **高価なライセンス**：特にエンタープライズ機能のための SQL Server ライセンス
5. **限定的な Web サポート**：最新の SPA フレームワークとの統合が難しい
6. **メンテナンスのオーバーヘッド**：サーバーパッチ、データベースメンテナンス、レポート管理
7. **クラウドネイティブオプションなし**：オンプレミス用に設計されており、クラウドサポートは不格好

### SSRS が過剰な場合

多くの PDF 生成シナリオでは、SSRS インフラストラクチャは過剰です：

| あなたのニーズ | SSRS のオーバーヘッド |
|-----------|---------------|
| 請求書の生成 | 完全なレポートサーバー |
| データテーブルのエクスポート | SQL Server ライセンス |
| データからの PDF 作成 | Windows Server |
| シンプルなドキュメント生成 | レポートサブスクリプション |

IronPDF は、サーバーインフラストラクチャなしでプロセス内 PDF 生成を提供します。

---

## クイックスタート：SSRS から IronPDF へ

### ステップ 1: 依存関係の置き換え

```bash
# SSRS は直接の NuGet がなく、サーバーベースです
# ReportViewer コントロールを削除する

dotnet remove package Microsoft.ReportingServices.ReportViewerControl.WebForms
dotnet remove package Microsoft.ReportingServices.ReportViewerControl.WinForms

# IronPDF をインストール
dotnet add package IronPdf
```

### ステップ 2: 名前空間の更新

```csharp
// 以前
using Microsoft.Reporting.WebForms;
using Microsoft.Reporting.WinForms;

// 以降
using IronPdf;
```

### ステップ 3: ライセンスの初期化

```csharp
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

---

## API マッピングリファレンス

| SSRS の概念 | IronPDF の相当 | メモ |
|--------------|-------------------|-------|
| `LocalReport` | `ChromePdfRenderer` | コアレンダリング |
| `ServerReport` | `RenderUrlAsPdf()` | URL ベースのレンダリング |
| `.rdlc` ファイル | HTML/CSS テンプレート | テンプレート形式 |
| `ReportParameter` | 文字列補間 | パラメータ |
| `ReportDataSource` | C# データ + HTML | データバインディング |
| `LocalReport.Render("PDF")` | `RenderHtmlAsPdf()` | PDF 出力 |
| `SubReport` | マージされた PDF | ネストされたレポート |
| `Report Server URL` | 不要 | サーバー不要 |
| `ReportViewer` コントロール | 不要 | 直接 PDF 生成 |
| エクスポート形式 | PDF がネイティブ | 焦点を当てた出力 |

---

## コード例

### 例 1: 基本的なレポート生成 (C#)

**SSRS と LocalReport：**
```csharp
using Microsoft.Reporting.WebForms;
using System.Data;

// RDLC レポートをロード
LocalReport report = new LocalReport();
report.ReportPath = Server.MapPath("~/Reports/Invoice.rdlc");

// データソースを設定
DataTable invoiceData = GetInvoiceData(invoiceId);
ReportDataSource rds = new ReportDataSource("InvoiceDataSet", invoiceData);
report.DataSources.Add(rds);

// パラメータを設定
ReportParameter[] parameters = new ReportParameter[]
{
    new ReportParameter("InvoiceNumber", "INV-001"),
    new ReportParameter("CustomerName", "Acme Corp"),
    new ReportParameter("InvoiceDate", DateTime.Now.ToString("yyyy-MM-dd"))
};
report.SetParameters(parameters);

// PDF にレンダリング
string mimeType, encoding, fileNameExtension;
string[] streams;
Warning[] warnings;

byte[] pdfBytes = report.Render(
    "PDF",
    null,
    out mimeType,
    out encoding,
    out fileNameExtension,
    out streams,
    out warnings);

// 保存または返却
File.WriteAllBytes("invoice.pdf", pdfBytes);
```

**IronPDF：**
```csharp
using IronPdf;

// データを取得
var invoice = GetInvoice(invoiceId);

var html = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; padding: 40px; }}
        h1 {{ color: navy; border-bottom: 2px solid navy; padding-bottom: 10px; }}
        .info {{ margin: 20px 0; }}
        .info p {{ margin: 5px 0; }}
        .label {{ font-weight: bold; display: inline-block; width: 120px; }}
        table {{ width: 100%; border-collapse: collapse; margin-top: 30px; }}
        th {{ background: #f0f0f0; padding: 12px; text-align: left; border: 1px solid #ddd; }}
        td {{ padding: 10px; border: 1px solid #eee; }}
        .amount {{ text-align: right; }}
        .total {{ font-weight: bold; font-size: 18px; }}
    </style>
</head>
<body>
    <h1>INVOICE</h1>

    <div class='info'>
        <p><span class='label'>Invoice #:</span> {invoice.Number}</p>
        <p><span class='label'>Customer:</span> {invoice.CustomerName}</p>
        <p><span class='label'>Date:</span> {invoice.Date:yyyy-MM-dd}</p>
    </div>

    <table>
        <thead>
            <tr><th>Item</th><th>Qty</th><th>Price</th><th>Total</th></tr>
        </thead>
        <tbody>
            {string.Join("", invoice.Items.Select(i => $@"
            <tr>
                <td>{i.Description}</td>
                <td class='amount'>{i.Quantity}</td>
                <td class='amount'>{i.UnitPrice:C}</td>
                <td class='amount'>{i.Total:C}</td>
            </tr>"))}
        </tbody>
        <tfoot>
            <tr class='total'>
                <td colspan='3'>Grand Total</td>
                <td class='amount'>{invoice.GrandTotal:C}</td>
            </tr>
        </tfoot>
    </table>
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("invoice.pdf");
```

### 例 2: データベース駆動型レポート

**SSRS：**
```csharp
using Microsoft.Reporting.WebForms;
using System.Data.SqlClient;

LocalReport report = new LocalReport();
report.ReportPath = "Reports/SalesReport.rdlc";

// クエリを実行してデータセットを埋める
using (SqlConnection conn = new SqlConnection(connectionString))
{
    SqlCommand cmd = new SqlCommand(@"
        SELECT ProductName, Category, SUM(Quantity) as TotalQty, SUM(Amount) as TotalAmount
        FROM Sales
        WHERE YEAR(SaleDate) = @Year
        GROUP BY ProductName, Category
        ORDER BY TotalAmount DESC", conn);

    cmd.Parameters.AddWithValue("@Year", 2024);

    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
    DataTable dt = new DataTable();
    adapter.Fill(dt);

    report.DataSources.Add(new ReportDataSource("SalesDataSet", dt));
}

report.SetParameters(new ReportParameter("ReportYear", "2024"));

byte[] pdfBytes = report.Render("PDF");
Response.BinaryWrite(pdfBytes);
```

**IronPDF：**
```csharp
using IronPdf;
using Microsoft.Data.SqlClient;

// データを取得
var salesData = new List<SalesItem>();
using (var conn = new SqlConnection(connectionString))
{
    var cmd = new SqlCommand(@"
        SELECT ProductName, Category, SUM(Quantity) as TotalQty, SUM(Amount) as TotalAmount
        FROM Sales
        WHERE YEAR(SaleDate) = @Year
        GROUP BY ProductName, Category
        ORDER BY TotalAmount DESC", conn);

    cmd.Parameters.AddWithValue("@Year", 2024);
    conn.Open();

    using (var reader = cmd.ExecuteReader())
    {
        while (reader.Read())
        {
            salesData.Add(new SalesItem
            {
                ProductName = reader.GetString(0),
                Category = reader.GetString(1),
                TotalQty = reader.GetInt32(2),
                TotalAmount = reader.GetDecimal(3)
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
        h1 {{ color: #333; }}
        .subtitle {{ color: #666; margin-bottom: 30px; }}
        table {{ width: 100%; border-collapse: collapse; }}
        th {{ background: #2c3e50; color: white; padding: 12px; text-align: left; }}
        td {{ padding: 10px; border-bottom: 1px solid #eee; }}
        tr:nth-child(even) {{ background: #f9f9f9; }}
        .number {{ text-align: right; }}
        .category {{ color: #666; font-size: 12px; }}
        .summary {{ margin-top: 20px; padding: 15px; background: #f0f0f0; }}
    </style>
</head>
<body>
    <h1>Sales Report</h1>
    <p class='subtitle'>Year: 2024</p>

    <table>
        <thead>
            <tr>
                <th>Product</th>
                <th>Category</th>
                <th>Quantity</th>
                <th>Total Amount</th>
            </tr>
        </thead>
        <tbody>
            {string.Join("", salesData.Select(s => $@"
            <tr>
                <td>{s.ProductName}</td>
                <td class='category'>{s.Category}</td>
                <td class='number'>{s.TotalQty:N0}</td>
                <td class='number'>{s.TotalAmount:C}</td>
            </tr>"))}
        </tbody>
    </table>

    <div class='summary'>
        <strong>Total Revenue:</strong> {salesData.Sum(s => s.TotalAmount):C}
    </div>
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);

// Web レスポンスで返却
return File(pdf.BinaryData, "application/pdf", "SalesReport_2024.pdf");
```

### 例 3: ヘッダーとフッターにページ番号を含む

**SSRS：** `.rdlc` デザイナーで `=Globals!PageNumber` のような式を設定

**IronPDF：**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

// ロゴを含む HTML ヘッダー
renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
{
    HtmlFragment = @"
        <div style='width: 100%; padding: 10px 0; border-bottom: 2px solid #2c3e50;'>
            <table style='width: 100%;'>
                <tr>
                    <td style='width: 50%;'>
                        <img src='logo.png' style='height: 40px;'>
                    </td>
                    <td style='width: 50%; text-align: right; color: #666; font-size: 10pt;'>
                        Sales Report - {date}
                    </td>
                </tr>
            </table>
        </div>",
    MaxHeight = 50,
    BaseUrl = new Uri("file:///C:/images/")
};

// ページ番号を含む HTML フッター
renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
{
    HtmlFragment = @"
        <div style='width: 100%; padding: 10px 0; border-top: 1px solid #ccc; font-size: 9pt; color: #666;'>
            <table style='width: 100%;'>
                <tr>
                    <td style='width: 50%;'>Confidential - Internal Use Only</td>
                    <td style='width: 50%; text-align: right;'>Page {page} of {total-pages}</td>
                </tr>
            </table>
        </div>",
    MaxHeight = 40
};

renderer.RenderingOptions.MarginTop = 60;
renderer.RenderingOptions.MarginBottom = 50;

var pdf = renderer.RenderHtmlAsPdf(reportHtml);
pdf.SaveAs("report.pdf");
```

### 例 4: サブレポート / マスター-ディテールレポート

**SSRS：** パラメータを持つ SubReport コントロールを使用

**IronPDF：**
```csharp
using IronPdf;

// オプション 1: ネストされたデータを含む 1 つの HTML をすべて生成
var customers = GetCustomersWithOrders();

var html = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial; padding: 30px; }}
        .customer {{ margin-bottom: 40px; }}
        .customer-header {{ background: #2c3e50; color: white; padding: 15px; }}
        .orders-table {{ width: 100%; border-collapse: collapse; margin-top: 10px; }}
        .orders-table th, .orders-table td {{ padding: 8px; border: 1px solid #ddd; }}
        .page-break {{ page-break-after: always; }}
    </style>
</head>
<body>
    {string.Join("", customers.Select((c, idx) => $@"
    <div class='customer {(idx < customers.Count - 1 ? "page-break" : "")}'>
        <div class='customer-header'>
            <h2>{c.Name}</h2>
            <p>Customer ID: {c.Id} | Total Orders: {c.Orders.Count}</p>
        </div>

        <table class='orders-table'>
            <tr><th>Order #</th><th>Date</th><th>Items</th><th>Amount</th></tr>
            {string.Join("", c.Orders.Select(o => $@"
            <tr>
                <td>{o.OrderNumber}</td>
                <td>{o.Date:yyyy-MM-dd}</td>
                <td>{o.ItemCount}</td>
                <td style='text-align:right;'>{o.Amount:C}</td>
            </tr>"))}
        </table>

        <p style='text-align:right; font-weight:bold;'>
            Customer Total: {c.Orders.Sum(o => o.Amount):C}
        </p>
    </div>"))}
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("customer_orders.pdf");

// オプション 2: 別々の PDF をマージ
var coverPdf = renderer.RenderHtmlAsPdf(coverHtml);
var summaryPdf = renderer.RenderHtmlAsPdf(summaryHtml);
var detailPdf = renderer.RenderHtmlAsPdf(detailHtml);

var merged = PdfDocument.Merge(coverPdf, summaryPdf, detailPdf);
merged.SaveAs("complete_report.pdf");
```

### 例 5: グルーピングと集計

**SSRS：** テーブル/マトリックスコントロールでのグループ式の使用

**IronPDF：**
```csharp
using IronPdf;

var salesByRegion = GetSales()
    .GroupBy(s => s.Region)
    .Select(g => new
    {
        Region = g.Key,
        Products = g.GroupBy(p => p.Product)
                    .Select(pg => new { Product = pg.Key, Total = pg.Sum(x => x.Amount) }),
        RegionTotal = g.Sum(x => x.Amount)
    });

var html = $@"
<!DOCTYPE html>
<html>
<head>
    <style>