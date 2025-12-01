---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [zetpdf/migrate-from-zetpdf.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/zetpdf/migrate-from-zetpdf.md)
🇯🇵 **日本語:** [zetpdf/migrate-from-zetpdf.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/zetpdf/migrate-from-zetpdf.md)

---

# ZetPDFからIronPDFへの移行方法は？

## ZetPDFから移行する理由は？

ZetPDFはPDFSharpのフォークで、同様の制限があります。移行する主な理由：

1. **HTMLサポートなし**：HTML/URLをPDFに変換できない - 低レベルのグラフィック描画のみ
2. **座標ベースのAPI**：各要素の複雑な手動位置決めが必要
3. **CSSサポートなし**：スタイリングシステムがない - 手動でのフォントと色の管理
4. **JavaScriptなし**：動的なWebコンテンツをレンダリングできない
5. **機能が限定的**：透かし、ヘッダー/フッター、またはマージ操作がない
6. **手動ページ区切り**：ページオーバーフローを手動で計算して管理する必要がある
7. **テキスト測定が必要**：テキストラッピングのための手動計算が必要

### 根本的な問題

ZetPDF/PDFSharpでは、正確な座標で各要素を配置する必要があります：

```csharp
// ZetPDF: 手動位置決めの悪夢
graphics.DrawString("Name:", font, brush, new XPoint(50, 100));
graphics.DrawString("John Doe", font, brush, new XPoint(100, 100));
graphics.DrawString("Address:", font, brush, new XPoint(50, 120));
graphics.DrawString("123 Main St", font, brush, new XPoint(100, 120));
// ... シンプルなフォームのための数百行
```

IronPDFはHTML/CSSを使用します - レイアウトエンジンがすべてを処理します：

```csharp
// IronPDF: シンプルなHTML
var html = @"
<p><strong>Name:</strong> John Doe</p>
<p><strong>Address:</strong> 123 Main St</p>";
var pdf = renderer.RenderHtmlAsPdf(html);
```

---

## クイックスタート：ZetPDFからIronPDFへ

### ステップ1：NuGetパッケージを置き換える

```bash
# ZetPDFを削除
dotnet remove package ZetPDF

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ2：名前空間を更新する

```csharp
// 以前
using ZetPdf;
using ZetPdf.Drawing;
using ZetPdf.Fonts;

// 以降
using IronPdf;
```

### ステップ3：HTMLベースのレンダリングに移行する

```csharp
// 古い：複雑な座標ベースの描画
var document = new PdfDocument();
var page = document.AddPage();
var graphics = XGraphics.FromPdfPage(page);
graphics.DrawString("Hello", font, brush, 50, 50);

// 新しい：シンプルなHTML
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1>Hello</h1>");
pdf.SaveAs("output.pdf");
```

---

## APIマッピングリファレンス

| ZetPDF | IronPDF | メモ |
|--------|---------|-------|
| `new PdfDocument()` | `new ChromePdfRenderer()` | レンダラーを作成 |
| `document.AddPage()` | 自動 | HTMLからページを作成 |
| `XGraphics.FromPdfPage(page)` | 該当なし | HTML/CSSを代わりに使用 |
| `graphics.DrawString()` | HTMLテキスト要素 | `<p>`, `<h1>`など |
| `graphics.DrawImage()` | `<img>`タグ | HTML画像 |
| `graphics.DrawLine()` | CSSボーダー | または`<hr>` |
| `graphics.DrawRectangle()` | CSS `border` + `div` | HTMLボックス |
| `new XFont()` | CSS `font-family` | Webフォントがサポート |
| `XBrushes.Black` | CSS `color` | フルカラーサポート |
| `document.Save()` | `pdf.SaveAs()` | ファイルに保存 |
| `PdfReader.Open()` | `PdfDocument.FromFile()` | 既存のPDFを読み込む |

---

## コード例

### 例1：基本的なテキストドキュメント

**ZetPDF：**
```csharp
using ZetPdf;
using ZetPdf.Drawing;

var document = new PdfDocument();
var page = document.AddPage();
page.Width = XUnit.FromMillimeter(210);
page.Height = XUnit.FromMillimeter(297);

var graphics = XGraphics.FromPdfPage(page);
var titleFont = new XFont("Arial", 24, XFontStyle.Bold);
var bodyFont = new XFont("Arial", 12);

graphics.DrawString("Company Report", titleFont, XBrushes.Navy,
    new XPoint(50, 50));
graphics.DrawString("This is the introduction paragraph.", bodyFont, XBrushes.Black,
    new XPoint(50, 80));
graphics.DrawString("Generated: " + DateTime.Now.ToString(), bodyFont, XBrushes.Gray,
    new XPoint(50, 100));

document.Save("report.pdf");
```

**IronPDF：**
```csharp
using IronPdf;

var html = $@"
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; padding: 50px; }}
        h1 {{ color: navy; }}
        .date {{ color: gray; }}
    </style>
</head>
<body>
    <h1>Company Report</h1>
    <p>This is the introduction paragraph.</p>
    <p class='date'>Generated: {DateTime.Now}</p>
</body>
</html>";

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("report.pdf");
```

### 例2：テーブルデータ

**ZetPDF（非常に複雑）：**
```csharp
using ZetPdf;
using ZetPdf.Drawing;

var document = new PdfDocument();
var page = document.AddPage();
var graphics = XGraphics.FromPdfPage(page);
var font = new XFont("Arial", 10);

// 手動でテーブルを描画 - 悪夢！
double x = 50, y = 50;
double colWidth = 100;
double rowHeight = 20;

// ヘッダー
graphics.DrawRectangle(XBrushes.LightGray, x, y, colWidth * 3, rowHeight);
graphics.DrawString("Name", font, XBrushes.Black, x + 5, y + 14);
graphics.DrawString("Quantity", font, XBrushes.Black, x + colWidth + 5, y + 14);
graphics.DrawString("Price", font, XBrushes.Black, x + colWidth * 2 + 5, y + 14);

// データ行
y += rowHeight;
foreach (var item in items)
{
    graphics.DrawRectangle(XPens.Black, x, y, colWidth * 3, rowHeight);
    graphics.DrawString(item.Name, font, XBrushes.Black, x + 5, y + 14);
    graphics.DrawString(item.Quantity.ToString(), font, XBrushes.Black, x + colWidth + 5, y + 14);
    graphics.DrawString(item.Price.ToString("C"), font, XBrushes.Black, x + colWidth * 2 + 5, y + 14);
    y += rowHeight;
}

document.Save("table.pdf");
```

**IronPDF：**
```csharp
using IronPdf;

var rows = items.Select(i => $@"
    <tr>
        <td>{i.Name}</td>
        <td>{i.Quantity}</td>
        <td>{i.Price:C}</td>
    </tr>");

var html = $@"
<html>
<head>
    <style>
        table {{ border-collapse: collapse; width: 100%; }}
        th, td {{ border: 1px solid black; padding: 8px; text-align: left; }}
        th {{ background-color: #f2f2f2; }}
    </style>
</head>
<body>
    <table>
        <tr>
            <th>Name</th>
            <th>Quantity</th>
            <th>Price</th>
        </tr>
        {string.Join("", rows)}
    </table>
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("table.pdf");
```

### 例3：複数ページのドキュメント

**ZetPDF：**
```csharp
using ZetPdf;
using ZetPdf.Drawing;

var document = new PdfDocument();
var font = new XFont("Arial", 12);

// 各ページを手動で作成し、位置を追跡する必要がある
double y = 50;
double pageHeight = 800;
PdfPage currentPage = document.AddPage();
XGraphics graphics = XGraphics.FromPdfPage(currentPage);

foreach (var paragraph in paragraphs)
{
    // 手動でページ区切りを検出
    if (y + 50 > pageHeight)
    {
        currentPage = document.AddPage();
        graphics = XGraphics.FromPdfPage(currentPage);
        y = 50;
    }

    graphics.DrawString(paragraph, font, XBrushes.Black, new XPoint(50, y));
    y += 20;
}

document.Save("multipage.pdf");
```

**IronPDF：**
```csharp
using IronPdf;

// 自動ページ区切り！
var html = @"
<html>
<body>
" + string.Join("", paragraphs.Select(p => $"<p>{p}</p>")) + @"
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("multipage.pdf");

// または、明示的にページ区切りを制御
var htmlWithBreaks = @"
<div>Page 1 content</div>
<div style='page-break-after: always;'></div>
<div>Page 2 content</div>";
```

### 例4：画像の追加

**ZetPDF：**
```csharp
using ZetPdf;
using ZetPdf.Drawing;

var document = new PdfDocument();
var page = document.AddPage();
var graphics = XGraphics.FromPdfPage(page);

// 画像を読み込む
var image = XImage.FromFile("logo.png");

// 特定の位置に描画
graphics.DrawImage(image, 50, 50, 200, 100);

document.Save("with_image.pdf");
```

**IronPDF：**
```csharp
using IronPdf;

var html = @"
<html>
<body>
    <img src='logo.png' style='width: 200px; height: 100px;'>
    <p>Company document with logo</p>
</body>
</html>";

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.BaseUrl = new Uri("file:///C:/path/to/images/");
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("with_image.pdf");
```

### 例5：既存のPDFの変更

**ZetPDF：**
```csharp
using ZetPdf;
using ZetPdf.Drawing;
using ZetPdf.IO;

var document = PdfReader.Open("existing.pdf", PdfDocumentOpenMode.Modify);
var page = document.Pages[0];
var graphics = XGraphics.FromPdfPage(page);

// 透かしを追加
var font = new XFont("Arial", 72, XFontStyle.Bold);
graphics.RotateTransform(-45);
graphics.DrawString("DRAFT", font, new XSolidBrush(XColor.FromArgb(50, 255, 0, 0)),
    new XPoint(-100, 500));

document.Save("watermarked.pdf");
```

**IronPDF：**
```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("existing.pdf");

// HTML透かしと完全なCSS！
pdf.ApplyWatermark(@"
    <div style='
        font-size: 72px;
        font-weight: bold;
        color: rgba(255, 0, 0, 0.2);
        transform: rotate(-45deg);
    '>
        DRAFT
    </div>");

pdf.SaveAs("watermarked.pdf");
```

### 例6：PDFのマージ（ZetPDFでは利用不可）

**ZetPDF：** 限定的/ネイティブサポートなし

**IronPDF：**
```csharp
using IronPdf;

var pdf1 = PdfDocument.FromFile("doc1.pdf");
var pdf2 = PdfDocument.FromFile("doc2.pdf");

var merged = PdfDocument.Merge(pdf1, pdf2);
merged.SaveAs("merged.pdf");
```

---

## 機能比較

| 機能 | ZetPDF | IronPDF |
|---------|--------|---------|
| HTMLからPDFへ | いいえ | はい |
| URLからPDFへ | いいえ | はい |
| CSSサポート | いいえ | 完全なCSS3 |
| JavaScript | いいえ | 完全なES2024 |
| 自動レイアウト | いいえ | はい |
| 自動ページ区切り | いいえ | はい |
| テーブル | 手動描画 | HTML `<table>` |
| 画像 | 手動配置 | `<img>`タグ |
| ヘッダー/フッター | 手動 | HTML/CSS |
| 透かし | 手動コード | 組み込み |
| PDFのマージ | 限定的 | はい |
| PDFの分割 | 限定的 | はい |
| デジタル署名 | いいえ | はい |
| PDF/A | いいえ | はい |
| クロスプラットフォーム | はい | はい |

---

## 移行チェックリスト

### 移行前

- [ ] **座標ベースの描画コードをすべて特定する**
  ```csharp
  // 以前（ZetPDF）
  graphics.DrawString("Name:", font, brush, new XPoint(50, 100));
  graphics.DrawString("John Doe", font, brush, new XPoint(100, 100));

  // 以降（IronPDF）
  var html = "<p><strong>Name:</strong> John Doe</p>";
  var pdf = renderer.RenderHtmlAsPdf(html);
  ```
  **理由：** IronPDFはレイアウトにHTML/CSSを使用し、手動座標位置決めを排除します。

- [ ] **フォントとスタイルの使用を文書化する**
  ```csharp
  // 以前（ZetPDF）
  var font = new XFont("Arial", 12, XFontStyle.Bold);

  // 以降（IronPDF）
  var html = "<p style='font-family: Arial; font-size: 12px; font-weight: bold;'>Text</p>";
  ```
  **理由：** CSSスタイリングへの移行は、PDF内の一貫性と柔軟性のあるデザインを保証します。

- [ ] **レイアウトをHTML構造にマッピングする**
  ```csharp
  // 以前（ZetPDF）
  graphics.DrawRectangle(pen, x, y, width, height);

  // 以降（IronPDF）
  var html = "<div style='border: 1px solid black; width: 100px; height: 50px;'></div>";
  ```
  **理由：** HTML構造はレイアウト管理を簡素化し、レスポンシブデザインをサポートします。

- [ ] **IronPDFライセンスキーを取得する**
  **理由：** IronPDFは本番使用にライセンスキーを要求します。無料試用版はhttps://ironpdf.com/で入手可能です。

### コードの更新

- [ ] **NuGetパッケージを置き換える**
  ```bash
  dotnet remove package ZetPDF
  dotnet add package IronPdf
  ```
  **理由：** PDF機能にIronPDFを使用するようにプロジェクトを確実にします。

- [ ] **名前空間を更新する**
  ```csharp
  // 以前（ZetPDF）
  using ZetPdf;

  // 以降（IronPDF）
  using IronPdf;
  ```
  **理由：** IronPDFクラスとメソッドにアクセスするために正しい名前空間が必要です。

- [ ] **グラフィックス描画