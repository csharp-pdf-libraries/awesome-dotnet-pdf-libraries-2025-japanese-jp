---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [xfiniumpdf/migrate-from-xfiniumpdf.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/xfiniumpdf/migrate-from-xfiniumpdf.md)
🇯🇵 **日本語:** [xfiniumpdf/migrate-from-xfiniumpdf.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/xfiniumpdf/migrate-from-xfiniumpdf.md)

---

# XFINIUM.PDFからIronPDFへの移行方法は？

## XFINIUM.PDFから移行する理由は？

XFINIUM.PDFは、座標ベースのグラフィックスプログラミングに依存する低レベルのPDFライブラリであり、開発者はページ上の各要素を手動で配置する必要があります。このアプローチは、要件が変更されるとメンテナンスの悪夢になります。移行する主な理由：

1. **HTMLサポートなし**：HTML/CSSをPDFに変換できず、低レベルの描画プリミティブのみ
2. **座標ベースのAPI**：`DrawString("text", font, brush, 50, 100)`のようなピクセル座標での手動位置指定
3. **手動フォント管理**：フォントオブジェクトを明示的に作成および管理する必要がある
4. **CSSスタイリングなし**：現代のWebスタイリングをサポートしておらず、色、フォント、レイアウトを手動で処理する必要がある
5. **JavaScriptレンダリングなし**：静的コンテンツのみ - 動的なWebコンテンツをレンダリングできない
6. **複雑なテキストレイアウト**：手動のテキスト測定および折り返し計算が必要
7. **限定的なドキュメント**：主流ソリューションと比較してコミュニティが小さい

### 核心問題：グラフィックスAPI対HTML

XFINIUM.PDFは、ドキュメントデザイナーではなく、グラフィックスプログラマーのように考えることを強制します：

```csharp
// XFINIUM.PDF: すべての要素を手動で配置
page.Graphics.DrawString("Invoice", titleFont, titleBrush, new XPoint(50, 50));
page.Graphics.DrawString("Customer:", labelFont, brush, new XPoint(50, 80));
page.Graphics.DrawString(customer.Name, valueFont, brush, new XPoint(120, 80));
page.Graphics.DrawLine(pen, 50, 100, 550, 100);
// ... シンプルなドキュメントのための数百行
```

IronPDFは馴染みのあるHTML/CSSを使用します：

```csharp
// IronPDF: 宣言的HTML
var html = @"<h1>Invoice</h1><p><b>Customer:</b> " + customer.Name + "</p><hr>";
var pdf = renderer.RenderHtmlAsPdf(html);
```

---

## クイックスタート：XFINIUM.PDFからIronPDFへ

### ステップ1：NuGetパッケージの置き換え

```bash
# XFINIUM.PDFを削除
dotnet remove package Xfinium.Pdf

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ2：名前空間の更新

```csharp
// 以前
using Xfinium.Pdf;
using Xfinium.Pdf.Graphics;
using Xfinium.Pdf.Content;
using Xfinium.Pdf.Forms;

// 以後
using IronPdf;
```

### ステップ3：ライセンスの初期化

```csharp
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

---

## APIマッピングリファレンス

| XFINIUM.PDF | IronPDF | メモ |
|-------------|---------|-------|
| `PdfFixedDocument` | `ChromePdfRenderer` | ドキュメントではなくレンダラーを作成 |
| `PdfPage` | 自動 | HTMLコンテンツからページを作成 |
| `page.Graphics.DrawString()` | HTMLテキスト要素 | `<p>`, `<h1>`, `<span>`など |
| `page.Graphics.DrawImage()` | `<img>`タグ | HTMLイメージ |
| `page.Graphics.DrawLine()` | CSS `border`または`<hr>` | HTML/CSSライン |
| `page.Graphics.DrawRectangle()` | `<div>`上のCSS `border` | HTMLボックス |
| `PdfUnicodeTrueTypeFont` | CSS `font-family` | フォントオブジェクトは不要 |
| `PdfRgbColor` | CSS `color` | 標準のCSSカラー |
| `PdfBrush` | CSSプロパティ | 背景、色など |
| `PdfPen` | CSS `border` | 線のスタイリング |
| `PdfHtmlTextElement` | `RenderHtmlAsPdf()` | 完全なHTMLサポート |
| `document.Save(stream)` | `pdf.SaveAs()`または`pdf.BinaryData` | 複数の出力オプション |
| `PdfStringAppearanceOptions` | CSSスタイリング | 外観にCSSを使用 |
| `PdfStringLayoutOptions` | CSSレイアウト | Flexbox、Gridなど |

---

## コード例

### 例1：基本的なドキュメント作成

**XFINIUM.PDF:**
```csharp
using Xfinium.Pdf;
using Xfinium.Pdf.Graphics;

PdfFixedDocument document = new PdfFixedDocument();
PdfPage page = document.Pages.Add();

// フォントを作成
PdfUnicodeTrueTypeFont font = new PdfUnicodeTrueTypeFont("Arial", 12, true);
PdfBrush brush = new PdfBrush(new PdfRgbColor(0, 0, 0));

// 正確な座標でテキストを描画
page.Graphics.DrawString("Hello World", font, brush, 50, 50);

using (FileStream fs = new FileStream("output.pdf", FileMode.Create))
{
    document.Save(fs);
}
```

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1>Hello World</h1>");
pdf.SaveAs("output.pdf");
```

### 例2：スタイル付き請求書ドキュメント

**XFINIUM.PDF:**
```csharp
using Xfinium.Pdf;
using Xfinium.Pdf.Graphics;

PdfFixedDocument document = new PdfFixedDocument();
PdfPage page = document.Pages.Add();
PdfGraphics graphics = page.Graphics;

// フォント - それぞれを作成する必要がある
PdfUnicodeTrueTypeFont titleFont = new PdfUnicodeTrueTypeFont("Arial", 24, true);
PdfUnicodeTrueTypeFont headerFont = new PdfUnicodeTrueTypeFont("Arial", 14, true);
PdfUnicodeTrueTypeFont bodyFont = new PdfUnicodeTrueTypeFont("Arial", 12, false);

// 色とブラシ
PdfBrush titleBrush = new PdfBrush(new PdfRgbColor(0, 0, 128));
PdfBrush blackBrush = new PdfBrush(new PdfRgbColor(0, 0, 0));
PdfBrush grayBrush = new PdfBrush(new PdfRgbColor(128, 128, 128));
PdfPen linePen = new PdfPen(new PdfRgbColor(200, 200, 200), 1);

// すべてのものに対して手動で位置を指定
double y = 50;

graphics.DrawString("INVOICE", titleFont, titleBrush, 50, y);
y += 40;

graphics.DrawLine(linePen, 50, y, 550, y);
y += 20;

graphics.DrawString("Invoice #:", headerFont, blackBrush, 50, y);
graphics.DrawString("INV-2024-001", bodyFont, blackBrush, 150, y);
y += 20;

graphics.DrawString("Date:", headerFont, blackBrush, 50, y);
graphics.DrawString(DateTime.Now.ToString("yyyy-MM-dd"), bodyFont, blackBrush, 150, y);
y += 20;

graphics.DrawString("Customer:", headerFont, blackBrush, 50, y);
graphics.DrawString("Acme Corporation", bodyFont, blackBrush, 150, y);
y += 40;

// テーブルヘッダー
graphics.DrawString("Item", headerFont, blackBrush, 50, y);
graphics.DrawString("Qty", headerFont, blackBrush, 300, y);
graphics.DrawString("Price", headerFont, blackBrush, 400, y);
graphics.DrawString("Total", headerFont, blackBrush, 500, y);
y += 25;

// テーブルデータ
graphics.DrawString("Widget A", bodyFont, blackBrush, 50, y);
graphics.DrawString("10", bodyFont, blackBrush, 300, y);
graphics.DrawString("$25.00", bodyFont, blackBrush, 400, y);
graphics.DrawString("$250.00", bodyFont, blackBrush, 500, y);
y += 20;

graphics.DrawString("Widget B", bodyFont, blackBrush, 50, y);
graphics.DrawString("5", bodyFont, blackBrush, 300, y);
graphics.DrawString("$40.00", bodyFont, blackBrush, 400, y);
graphics.DrawString("$200.00", bodyFont, blackBrush, 500, y);
y += 30;

// 合計
graphics.DrawLine(linePen, 400, y, 550, y);
y += 10;
graphics.DrawString("Grand Total:", headerFont, blackBrush, 400, y);
graphics.DrawString("$450.00", headerFont, titleBrush, 500, y);

using (FileStream fs = new FileStream("invoice.pdf", FileMode.Create))
{
    document.Save(fs);
}
```

**IronPDF:**
```csharp
using IronPdf;

var invoice = new
{
    Number = "INV-2024-001",
    Date = DateTime.Now,
    Customer = "Acme Corporation",
    Items = new[]
    {
        new { Name = "Widget A", Qty = 10, Price = 25.00m },
        new { Name = "Widget B", Qty = 5, Price = 40.00m }
    }
};

var html = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; padding: 50px; }}
        h1 {{ color: navy; margin-bottom: 20px; }}
        .info {{ margin-bottom: 30px; }}
        .info p {{ margin: 5px 0; }}
        .info strong {{ display: inline-block; width: 100px; }}
        table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
        th {{ background: #f5f5f5; text-align: left; padding: 10px; border-bottom: 2px solid #ddd; }}
        td {{ padding: 10px; border-bottom: 1px solid #eee; }}
        .total-row {{ font-weight: bold; }}
        .total-row td {{ border-top: 2px solid #333; padding-top: 15px; }}
        .amount {{ text-align: right; }}
        .grand-total {{ color: navy; font-size: 18px; }}
    </style>
</head>
<body>
    <h1>INVOICE</h1>

    <div class='info'>
        <p><strong>Invoice #:</strong> {invoice.Number}</p>
        <p><strong>Date:</strong> {invoice.Date:yyyy-MM-dd}</p>
        <p><strong>Customer:</strong> {invoice.Customer}</p>
    </div>

    <table>
        <tr>
            <th>Item</th>
            <th>Qty</th>
            <th class='amount'>Price</th>
            <th class='amount'>Total</th>
        </tr>
        {string.Join("", invoice.Items.Select(i => $@"
        <tr>
            <td>{i.Name}</td>
            <td>{i.Qty}</td>
            <td class='amount'>{i.Price:C}</td>
            <td class='amount'>{i.Qty * i.Price:C}</td>
        </tr>"))}
        <tr class='total-row'>
            <td colspan='3'>Grand Total:</td>
            <td class='amount grand-total'>{invoice.Items.Sum(i => i.Qty * i.Price):C}</td>
        </tr>
    </table>
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("invoice.pdf");
```

### 例3：データを含むテーブル

**XFINIUM.PDF:**
```csharp
using Xfinium.Pdf;
using Xfinium.Pdf.Graphics;

PdfFixedDocument document = new PdfFixedDocument();
PdfPage page = document.Pages.Add();
PdfGraphics graphics = page.Graphics;

PdfUnicodeTrueTypeFont headerFont = new PdfUnicodeTrueTypeFont("Arial", 12, true);
PdfUnicodeTrueTypeFont cellFont = new PdfUnicodeTrueTypeFont("Arial", 10, false);
PdfBrush textBrush = new PdfBrush(PdfRgbColor.Black);
PdfBrush headerBg = new PdfBrush(new PdfRgbColor(240, 240, 240));
PdfPen borderPen = new PdfPen(PdfRgbColor.Black, 0.5);

double x = 50, y = 50;
double[] colWidths = { 150, 100, 100, 100 };
double rowHeight = 25;

// ヘッダー行を描画
graphics.DrawRectangle(borderPen, headerBg, x, y, colWidths.Sum(), rowHeight);

double cellX = x;
string[] headers = { "Product", "Quantity", "Unit Price", "Total" };
for (int i = 0; i < headers.Length; i++)
{
    graphics.DrawString(headers[i], headerFont, textBrush, cellX + 5, y + 8);
    graphics.DrawLine(borderPen, cellX, y, cellX, y + rowHeight);
    cellX += colWidths[i];
}
graphics.DrawLine(borderPen, cellX, y, cellX, y + rowHeight);

y += rowHeight;

// データ行を描画
var data = GetProducts();
foreach (var item in data)
{
    graphics.DrawRectangle(borderPen, x, y, colWidths.Sum(), rowHeight);

    cellX = x;
    graphics.DrawString(item.Name, cellFont, textBrush, cellX + 5, y + 8);
    cellX += colWidths[0];

    graphics.DrawString(item.Quantity.ToString(), cellFont, textBrush, cellX + 5, y + 8);
    cellX += colWidths[1];

    graphics.DrawString(item.UnitPrice.ToString("C"), cellFont, textBrush, cellX + 5, y + 8);
    cellX += colWidths[2];

    graphics.DrawString(item.Total.ToString("C"), cellFont, textBrush, cellX + 5, y + 8);

    y += rowHeight;

    // ページブレークをチェック
    if (y > 750)
    {
        page = document.Pages.Add();
        graphics = page.Graphics;
        y = 50;
    }
}

document.Save("table.pdf");
```

**IronPDF:**
```csharp
using IronPdf;

var data = GetProducts();

var html = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; padding: 40px; }}
        table {{ width: 100%; border-collapse: collapse; }}
        th {{
            background: #f0f0f0;
            padding: 10px;
            text-align: left;
            border: 1px solid #333;
            font-weight: bold;
        }}
        td {{
            padding: 10px;
            border: 1px solid #ccc;
        }}
        tr:nth-child(even) {{ background: #f9f9f9; }}
        .number {{ text-align: right; }}
    </style>
</head>
<body>
    <table>
        <thead>
            <tr>
                <th>Product</th>
                <th>Quantity</th>
                <th>Unit Price</th>
                <th>Total</th>
            </tr>
        </thead>
        <tbody>
            {string.Join("", data.Select(item => $@"
            <tr>
                <td>{item.Name}</td>
                <td class='number'>{item.Quantity}</td>
                <td class='number'>{item.UnitPrice:C}</td>
                <td class='number'>{item.Total:C}</td>
            </tr>"))}
        </tbody>
    </table>
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("table.pdf");
```

### 例4：ヘッダーとフッター

**XFINIUM.PDF:**
```csharp
using Xfinium.Pdf;
using Xfinium.Pdf.Graphics;

PdfFixedDocument document = new PdfFixedDocument();
PdfUnicodeTrueTypeFont headerFont = new PdfUnicodeTrueTypeFont("Arial", 10, false);
PdfBrush brush = new PdfBrush(new PdfRgbColor(128, 128, 128));
PdfPen linePen = new PdfPen(new PdfRgbColor(200, 200, 200), 1);

int totalPages = 5;
for