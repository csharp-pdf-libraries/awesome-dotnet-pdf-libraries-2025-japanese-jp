---
**  (Japanese Translation)**

 **English:** [pdfsharp/migrate-from-pdfsharp.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/pdfsharp/migrate-from-pdfsharp.md)
 **:** [pdfsharp/migrate-from-pdfsharp.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/pdfsharp/migrate-from-pdfsharp.md)

---
# PDFSharpからIronPDFへの移行方法は？

## PDFSharpからIronPDFへ移行する理由

PDFSharpは、GDI+スタイルの座標を使用して各要素を手動で配置する必要があり、文書生成が面倒でエラーが発生しやすくなります。IronPDFは、最新のCSS3（flexboxやgridを含む）を使用したネイティブのHTMLからPDFへの変換をサポートしており、X、Y位置を計算する代わりにWeb技術を活用できます。これにより開発時間が大幅に削減され、標準的なHTML/CSSスキルを通じてPDF生成を維持できます。

### 座標計算の問題

PDFSharpのGDI+アプローチでは、以下が必要です：
- 各要素の正確なX、Y位置を計算する
- ページオーバーフローのためにコンテンツの高さを手動で追跡する
- 行の折り返しとテキストの計測を自分で処理する
- 境界線の計算でセルごとにテーブルを描画する
- 手動のページ区切りで複数ページの文書を管理する

### 移行の主な利点

| アスペクト | PDFSharp | IronPDF |
|--------|----------|---------|
| 文書作成 | 座標ベースの描画 | HTML/CSSテンプレート |
| レイアウトシステム | 手動のX、Y配置 | CSS Flow/Flexbox/Grid |
| ページ区切り | 手動計算 | 自動 + CSS制御 |
| テーブル | セルを個別に描画 | HTML `<table>` |
| スタイリング | コードベースのフォント/色 | CSSスタイルシート |
| メンテナンス | 修正が難しい | HTML/CSSを編集 |
| 学習曲線 | GDI+の知識が必要 | Webスキルの移行 |
| ライセンス | MIT（無料） | 商用 |

---

## NuGetパッケージの変更

```bash
# PDFSharpを削除
dotnet remove package PdfSharp
dotnet remove package PdfSharp-wpf
dotnet remove package PdfSharp.Charting

# IronPDFを追加
dotnet add package IronPdf
```

---

## 名前空間のマッピング

| PDFSharp | IronPDF |
|----------|---------|
| `PdfSharp.Pdf` | `IronPdf` |
| `PdfSharp.Drawing` | 不要（HTML/CSSを使用） |
| `PdfSharp.Pdf.IO` | `IronPdf` |
| `PdfSharp.Charting` | HTML/CSSまたはチャートライブラリ |

---

## APIマッピング

| PDFSharp API | IronPDF API | メモ |
|--------------|-------------|-------|
| `new PdfDocument()` | `ChromePdfRenderer.RenderHtmlAsPdf()` | HTMLから作成 |
| `document.AddPage()` | 自動 | HTMLコンテンツからページを作成 |
| `XGraphics.FromPdfPage()` | 不要 | HTML要素を使用 |
| `XGraphics.DrawString()` | HTML `<p>`、`<h1>`など | CSSで位置を指定 |
| `XGraphics.DrawImage()` | HTML `<img>`タグ | CSSで位置を指定 |
| `XGraphics.DrawLine()` | CSSの境界線またはSVG | `<hr>`またはCSS |
| `XGraphics.DrawRectangle()` | CSSまたはSVG | 境界線付きの`<div>` |
| `XGraphics.DrawEllipse()` | SVG `<ellipse>` | またはCSSのborder-radius |
| `XFont` | CSS `font-family`、`font-size` | 標準のCSS |
| `XBrush`、`XPen` | CSSの色/境界線 | `color`、`background-color` |
| `XRect` | CSSでの位置指定 | `margin`、`padding`、`width`、`height` |
| `document.Save()` | `pdf.SaveAs()` | 類似の機能 |
| `PdfReader.Open()` | `PdfDocument.FromFile()` | 既存のPDFを開く |
| `document.Pages.Count` | `pdf.PageCount` | ページ数 |
| `document.Version` | `pdf.MetaData` | 文書のプロパティ |

---

## コード例

### 例1：テキストのみのシンプルな文書

**PDFSharp前：**
```csharp
using PdfSharp.Pdf;
using PdfSharp.Drawing;

PdfDocument document = new PdfDocument();
document.Info.Title = "My Document";

PdfPage page = document.AddPage();
XGraphics gfx = XGraphics.FromPdfPage(page);
XFont font = new XFont("Arial", 20);

gfx.DrawString("Hello, World!", font, XBrushes.Black,
    new XRect(50, 50, page.Width, page.Height),
    XStringFormats.TopLeft);

document.Save("output.pdf");
```

**IronPDF後：**
```csharp
using IronPdf;

IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(@"
    <html>
    <head><title>My Document</title></head>
    <body>
        <h1>Hello, World!</h1>
    </body>
    </html>");

pdf.SaveAs("output.pdf");
```

### 例2：複数の要素を含むスタイル付き文書

**PDFSharp前：**
```csharp
using PdfSharp.Pdf;
using PdfSharp.Drawing;

PdfDocument document = new PdfDocument();
PdfPage page = document.AddPage();
XGraphics gfx = XGraphics.FromPdfPage(page);

// タイトル
XFont titleFont = new XFont("Arial", 24, XFontStyle.Bold);
gfx.DrawString("Invoice", titleFont, XBrushes.Black,
    new XRect(50, 50, page.Width, page.Height),
    XStringFormats.TopLeft);

// 本文テキスト
XFont bodyFont = new XFont("Arial", 12);
gfx.DrawString("Customer: John Doe", bodyFont, XBrushes.Black,
    new XRect(50, 100, page.Width, page.Height),
    XStringFormats.TopLeft);

gfx.DrawString("Total: $150.00", bodyFont, XBrushes.Black,
    new XRect(50, 130, page.Width, page.Height),
    XStringFormats.TopLeft);

// 線を描画
XPen pen = new XPen(XColors.Black, 1);
gfx.DrawLine(pen, 50, 90, 300, 90);

document.Save("invoice.pdf");
```

**IronPDF後：**
```csharp
using IronPdf;

IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

string html = @"
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; padding: 50px; }
        h1 { font-size: 24px; font-weight: bold; border-bottom: 1px solid black; padding-bottom: 10px; }
        p { font-size: 12px; margin: 10px 0; }
    </style>
</head>
<body>
    <h1>Invoice</h1>
    <p>Customer: John Doe</p>
    <p>Total: $150.00</p>
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("invoice.pdf");
```

### 例3：画像を含む文書

**PDFSharp前：**
```csharp
using PdfSharp.Pdf;
using PdfSharp.Drawing;

PdfDocument document = new PdfDocument();
PdfPage page = document.AddPage();
XGraphics gfx = XGraphics.FromPdfPage(page);

XFont font = new XFont("Arial", 16);
gfx.DrawString("Company Report", font, XBrushes.Black,
    new XRect(50, 50, page.Width, page.Height),
    XStringFormats.TopLeft);

XImage image = XImage.FromFile("logo.png");
gfx.DrawImage(image, 50, 100, 150, 75);

document.Save("report.pdf");
```

**IronPDF後：**
```csharp
using IronPdf;

IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

string html = @"
<html>
<head>
    <style>
        h1 { font-size: 16px; }
        img { width: 150px; height: 75px; }
    </style>
</head>
<body>
    <h1>Company Report</h1>
    <img src='logo.png' alt='Logo' />
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("report.pdf");
```

### 例4：テーブル

**PDFSharp前：**
```csharp
using PdfSharp.Pdf;
using PdfSharp.Drawing;

PdfDocument document = new PdfDocument();
PdfPage page = document.AddPage();
XGraphics gfx = XGraphics.FromPdfPage(page);

XFont headerFont = new XFont("Arial", 12, XFontStyle.Bold);
XFont cellFont = new XFont("Arial", 10);
XPen pen = new XPen(XColors.Black, 1);

// 手動でテーブルを描画 - 面倒！
double startX = 50, startY = 50;
double colWidth = 100, rowHeight = 25;

// ヘッダー行
gfx.DrawRectangle(pen, XBrushes.LightGray, startX, startY, colWidth * 3, rowHeight);
gfx.DrawString("Product", headerFont, XBrushes.Black, startX + 5, startY + 17);
gfx.DrawString("Qty", headerFont, XBrushes.Black, startX + colWidth + 5, startY + 17);
gfx.DrawString("Price", headerFont, XBrushes.Black, startX + colWidth * 2 + 5, startY + 17);

// データ行 - 各行に繰り返し
double y = startY + rowHeight;
gfx.DrawRectangle(pen, startX, y, colWidth * 3, rowHeight);
gfx.DrawString("Widget", cellFont, XBrushes.Black, startX + 5, y + 15);
gfx.DrawString("10", cellFont, XBrushes.Black, startX + colWidth + 5, y + 15);
gfx.DrawString("$9.99", cellFont, XBrushes.Black, startX + colWidth * 2 + 5, y + 15);

// 列の区切り線を描画
gfx.DrawLine(pen, startX + colWidth, startY, startX + colWidth, y + rowHeight);
gfx.DrawLine(pen, startX + colWidth * 2, startY, startX + colWidth * 2, y + rowHeight);

document.Save("table.pdf");
```

**IronPDF後：**
```csharp
using IronPdf;

IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

string html = @"
<html>
<head>
    <style>
        table { border-collapse: collapse; width: 300px; }
        th { background-color: lightgray; font-weight: bold; padding: 5px; border: 1px solid black; }
        td { padding: 5px; border: 1px solid black; }
    </style>
</head>
<body>
    <table>
        <tr>
            <th>Product</th>
            <th>Qty</th>
            <th>Price</th>
        </tr>
        <tr>
            <td>Widget</td>
            <td>10</td>
            <td>$9.99</td>
        </tr>
    </table>
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("table.pdf");
```

### 例5：ページ区切りを含む複数ページの文書

**PDFSharp前：**
```csharp
using PdfSharp.Pdf;
using PdfSharp.Drawing;

PdfDocument document = new PdfDocument();

// 手動でページを作成し、コンテンツの位置を追跡する必要がある
for (int i = 1; i <= 3; i++)
{
    PdfPage page = document.AddPage();
    XGraphics gfx = XGraphics.FromPdfPage(page);
    XFont font = new XFont("Arial", 16);

    gfx.DrawString($"Page {i} of 3", font, XBrushes.Black,
        new XRect(50, 50, page.Width, page.Height),
        XStringFormats.TopLeft);
}

document.Save("multipage.pdf");
```

**IronPDF後：**
```csharp
using IronPdf;

IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

string html = @"
<html>
<head>
    <style>
        .page { page-break-after: always; padding: 50px; }
        .page:last-child { page-break-after: auto; }
    </style>
</head>
<body>
    <div class='page'><h1>Page 1</h1><p>Content for page 1...</p></div>
    <div class='page'><h1>Page 2</h1><p>Content for page 2...</p></div>
    <div class='page'><h1>Page 3</h1><p>Content for page 3...</p></div>
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("multipage.pdf");
```

### 例6：ヘッダーとフッター

**PDFSharp前：**
```csharp
using PdfSharp.Pdf;
using PdfSharp.Drawing;

PdfDocument document = new PdfDocument();
PdfPage page = document.AddPage();
XGraphics gfx = XGraphics.FromPdfPage(page);

XFont headerFont = new XFont("Arial", 10);
XFont bodyFont = new XFont("Arial", 12);

// ヘッダー - 上部に描画
gfx.DrawString("Company Name", headerFont, XBrushes.Gray,
    new XRect(50, 20, page.Width - 100, 20),
    XStringFormats.TopLeft);

// 線を手動で描画する必要がある
XPen pen = new XPen(XColors.Gray, 0.5);
gfx.DrawLine(pen, 50, 35, page.Width - 50, 35);

// 本文コンテンツ
gfx.DrawString("Document content here...", bodyFont, XBrushes.Black,
    new XRect(50, 60, page.Width - 100, page.Height),
    XStringFormats.TopLeft);

// フッター - 下からの位置を計算
gfx.DrawLine(pen, 50, page.Height - 40, page.Width - 50, page.Height - 40);
gfx.DrawString("Page 1", headerFont, XBrushes.Gray,
    new XRect(50, page.Height - 35, page.Width - 100, 20),
    XStringFormats.TopLeft);

document.Save("header-footer.pdf");
```

**IronPDF後：**
```csharp
using IronPdf;

IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

var renderer = new ChromePdfRenderer();

renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
{
    HtmlFragment = "<div style='text-align: center; font-size: 10px; color: gray; border-bottom: 1px solid gray;'>Company Name</div>"
};

renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
{
    HtmlFragment = "<div style='text-align: center; font-size: 10px; color: gray; border-top: 1px solid gray;'>Page {page} of {total-pages}</div>"
};

var pdf = renderer.RenderHtmlAsPdf("<h1>Document Content</h1><p>Your content here...</p>");
pdf.SaveAs("header-footer.pdf");
```

### 例7：既存のPDFを開いて修正する

**PDFSharp前：**
```csharp
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;

// 既存のPDFを開く
PdfDocument document = PdfReader.Open("existing.pdf", PdfDocumentOpenMode.Modify);
PdfPage page = document.Pages[0];

// ページのグラフィックスオブジェクトを取得
XGraphics gfx = XGraphics.FromPdfPage(page);
XFont font = new XFont("Arial", 20, XFontStyle.Bold);

// 計算された位置に透かしを追加
gfx.DrawString("CONFIDENTIAL", font, XBrushes.Red,
    new XPoint(200, 400));

document.Save("modified.pdf");
```

**IronPDF後：**
```csharp
using IronPdf;
using IronPdf.Editing;

IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

var pdf = PdfDocument.FromFile("existing.pdf");

var textStamper = new TextStamper
{
    Text = "CONFIDENTIAL",
    FontSize = 20,
    FontFamily = "Arial",