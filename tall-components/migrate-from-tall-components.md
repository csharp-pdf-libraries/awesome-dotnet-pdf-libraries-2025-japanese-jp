# Tall Components（TallPDF、PDFKit）からC#でIronPDFへの移行方法は？

## ⚠️ 重要：Tall Componentsは廃止されました

**Tall ComponentsはApryseに買収され、もはや利用可能ではありません。** 新しいライセンスは購入できず、既存のユーザーはiText SDKにリダイレクトされています。まだTall Componentsを使用している場合、移行はオプションではなく必須です。

### なぜ今すぐ移行する必要があるのか

1. **製品が廃止されました**：Apryseの買収以来、新しいライセンスは利用できません
2. **サポートなし**：バグ修正、セキュリティパッチ、または更新がありません
3. **既知のレンダリングバグ**：空白ページ、グラフィックの欠落、フォントの問題に関する文書化された問題
4. **HTMLサポートなし**：XMLベースのドキュメント作成のみ - 現代のWebワークフローには全く適していません
5. **レガシーアーキテクチャ**：.NET開発の異なる時代のために構築されました
6. **ベンダーロックインのリスク**：高価なiText SDKに向けて押し進められています

### Tall Componentsの問題

廃止前に、Tall Componentsはすでに問題がありました：

```csharp
// Tall Components: 冗長なXMLベースのアプローチ
Document document = new Document();
Section section = document.Sections.Add();
TextParagraph paragraph = new TextParagraph();
paragraph.Text = "Hello World";
paragraph.Font = new Font("Arial", 24);
section.Paragraphs.Add(paragraph);
document.Write("output.pdf");
```

IronPDFは現代的でHTMLベースのアプローチを提供します：

```csharp
// IronPDF: シンプルで現代的
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1 style='font-size:24px'>Hello World</h1>");
pdf.SaveAs("output.pdf");
```

---

## クイックスタート：Tall ComponentsからIronPDFへ

### ステップ1：NuGetパッケージの置き換え

```bash
# Tall Componentsパッケージを削除
dotnet remove package TallComponents.PDF.Kit
dotnet remove package TallComponents.PDF.Layout
dotnet remove package TallComponents.PDF.Layout.Drawing

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ2：名前空間の更新

```csharp
// 以前
using TallComponents.PDF.Kit;
using TallComponents.PDF.Layout;
using TallComponents.PDF.Layout.Drawing;
using TallComponents.PDF.Layout.Paragraphs;

// 以後
using IronPdf;
```

### ステップ3：ライセンスの初期化

```csharp
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

---

## APIマッピングリファレンス

| Tall Components | IronPDF | メモ |
|-----------------|---------|-------|
| `Document` | `ChromePdfRenderer` | レンダラーの作成 |
| `Section` | 自動 | HTML構造からのセクション |
| `TextParagraph` | HTMLテキスト要素 | `<p>`, `<h1>`, `<div>`など |
| `ImageParagraph` | `<img>`タグ | HTML画像 |
| `TableParagraph` | HTML `<table>` | 標準のHTMLテーブル |
| `Font` | CSS `font-family` | Webフォントがサポートされています |
| `document.Write()` | `pdf.SaveAs()` | ファイルに保存 |
| `document.Write(stream)` | `pdf.BinaryData`または`pdf.Stream` | ストリーム出力 |
| `Page.Canvas` | HTML/CSSレンダリング | 手動キャンバスは不要 |
| `XmlDocument.Generate()` | `RenderHtmlAsPdf()` | HTMLがXMLを置き換えます |
| `PdfKit.Merger.Merge()` | `PdfDocument.Merge()` | PDFのマージ |
| `Document.Security` | `pdf.SecuritySettings` | PDFセキュリティ |
| `PageLayout` | `RenderingOptions` | ページ設定 |

---

## コード例

### 例1：基本的なドキュメント作成

**Tall Components:**
```csharp
using TallComponents.PDF.Layout;
using TallComponents.PDF.Layout.Paragraphs;

Document document = new Document();
Section section = document.Sections.Add();

TextParagraph paragraph = new TextParagraph();
paragraph.Text = "Hello World";
section.Paragraphs.Add(paragraph);

using (FileStream fs = new FileStream("output.pdf", FileMode.Create))
{
    document.Write(fs);
}
```

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1>Hello World</h1>");
pdf.SaveAs("output.pdf");
```

### 例2：画像付きのフォーマットされたドキュメント

**Tall Components:**
```csharp
using TallComponents.PDF.Layout;
using TallComponents.PDF.Layout.Paragraphs;
using TallComponents.PDF.Layout.Shapes;

Document document = new Document();
Section section = document.Sections.Add();

// タイトル
TextParagraph title = new TextParagraph();
title.Text = "Report Title";
title.Font = new Font("Arial", 24);
title.FontColor = new RgbColor(0, 0, 128);
section.Paragraphs.Add(title);

// 画像
ImageParagraph imagePara = new ImageParagraph();
imagePara.Image = new FileImage("logo.png");
imagePara.Width = 200;
imagePara.Height = 100;
section.Paragraphs.Add(imagePara);

// 本文
TextParagraph body = new TextParagraph();
body.Text = "This is the report introduction paragraph with important information.";
body.Font = new Font("Arial", 12);
section.Paragraphs.Add(body);

using (FileStream fs = new FileStream("report.pdf", FileMode.Create))
{
    document.Write(fs);
}
```

**IronPDF:**
```csharp
using IronPdf;

var html = @"
<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; padding: 40px; }
        h1 { color: navy; font-size: 24px; }
        .logo { width: 200px; height: 100px; margin: 20px 0; }
        p { font-size: 12px; line-height: 1.6; }
    </style>
</head>
<body>
    <h1>Report Title</h1>
    <img src='logo.png' class='logo' alt='Logo'>
    <p>This is the report introduction paragraph with important information.</p>
</body>
</html>";

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.BaseUrl = new Uri("file:///C:/path/to/images/");
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("report.pdf");
```

### 例3：テーブル

**Tall Components:**
```csharp
using TallComponents.PDF.Layout;
using TallComponents.PDF.Layout.Paragraphs;
using TallComponents.PDF.Layout.Tables;

Document document = new Document();
Section section = document.Sections.Add();

// テーブルの作成
TableParagraph table = new TableParagraph();
table.Columns.Add(new Column(150));
table.Columns.Add(new Column(100));
table.Columns.Add(new Column(100));
table.Columns.Add(new Column(100));

// ヘッダー行
Row headerRow = table.Rows.Add();
headerRow.BackgroundColor = new RgbColor(240, 240, 240);
headerRow[0].Add(new TextParagraph { Text = "Product", Font = new Font("Arial", 12, FontStyle.Bold) });
headerRow[1].Add(new TextParagraph { Text = "Qty", Font = new Font("Arial", 12, FontStyle.Bold) });
headerRow[2].Add(new TextParagraph { Text = "Price", Font = new Font("Arial", 12, FontStyle.Bold) });
headerRow[3].Add(new TextParagraph { Text = "Total", Font = new Font("Arial", 12, FontStyle.Bold) });

// データ行
var products = GetProducts();
foreach (var product in products)
{
    Row row = table.Rows.Add();
    row[0].Add(new TextParagraph { Text = product.Name });
    row[1].Add(new TextParagraph { Text = product.Qty.ToString() });
    row[2].Add(new TextParagraph { Text = product.Price.ToString("C") });
    row[3].Add(new TextParagraph { Text = product.Total.ToString("C") });
}

section.Paragraphs.Add(table);
document.Write("table.pdf");
```

**IronPDF:**
```csharp
using IronPdf;

var products = GetProducts();

var html = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; padding: 40px; }}
        table {{ width: 100%; border-collapse: collapse; }}
        th {{
            background: #f0f0f0;
            padding: 12px;
            text-align: left;
            border: 1px solid #ccc;
            font-weight: bold;
        }}
        td {{
            padding: 10px 12px;
            border: 1px solid #ddd;
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
                <th>Qty</th>
                <th>Price</th>
                <th>Total</th>
            </tr>
        </thead>
        <tbody>
            {string.Join("", products.Select(p => $@"
            <tr>
                <td>{p.Name}</td>
                <td class='number'>{p.Qty}</td>
                <td class='number'>{p.Price:C}</td>
                <td class='number'>{p.Total:C}</td>
            </tr>"))}
        </tbody>
    </table>
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("table.pdf");
```

### 例4：ページレイアウトとマージン

**Tall Components:**
```csharp
using TallComponents.PDF.Layout;

Document document = new Document();
Section section = document.Sections.Add();

// ページ設定
section.PageSize = new PageSize(PageFormat.A4);
section.PageOrientation = PageOrientation.Portrait;
section.Margin.Top = 72;     // 1インチをポイントで
section.Margin.Bottom = 72;
section.Margin.Left = 54;    // 0.75インチ
section.Margin.Right = 54;

// コンテンツの追加...
section.Paragraphs.Add(new TextParagraph { Text = "Content here" });

document.Write("layout.pdf");
```

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

// ページ設定
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Portrait;
renderer.RenderingOptions.MarginTop = 25;      // mm
renderer.RenderingOptions.MarginBottom = 25;
renderer.RenderingOptions.MarginLeft = 19;
renderer.RenderingOptions.MarginRight = 19;

var pdf = renderer.RenderHtmlAsPdf("<p>Content here</p>");
pdf.SaveAs("layout.pdf");
```

### 例5：ヘッダーとフッター

**Tall Components:**
```csharp
using TallComponents.PDF.Layout;
using TallComponents.PDF.Layout.Paragraphs;

Document document = new Document();
Section section = document.Sections.Add();

// ヘッダー
Header header = section.Header;
header.Paragraphs.Add(new TextParagraph
{
    Text = "Company Report - Confidential",
    Font = new Font("Arial", 10),
    FontColor = new RgbColor(128, 128, 128)
});

// ページ番号付きフッター
Footer footer = section.Footer;
footer.Paragraphs.Add(new TextParagraph
{
    Text = "Page #p# of #P#",  // 特別なプレースホルダー
    Font = new Font("Arial", 10),
    FontColor = new RgbColor(128, 128, 128),
    HorizontalAlignment = HorizontalAlignment.Right
});

// コンテンツ
section.Paragraphs.Add(new TextParagraph { Text = "Report content..." });

document.Write("report.pdf");
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

// ページ番号付きHTMLフッター
renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
{
    HtmlFragment = @"
        <div style='width: 100%; font-size: 10pt; color: gray; text-align: right; border-top: 1px solid #ccc; padding-top: 5px;'>
            Page {page} of {total-pages}
        </div>",
    MaxHeight = 30
};

renderer.RenderingOptions.MarginTop = 40;
renderer.RenderingOptions.MarginBottom = 40;

var pdf = renderer.RenderHtmlAsPdf("<p>Report content...</p>");
pdf.SaveAs("report.pdf");
```

### 例6：PDFのマージ

**Tall Components:**
```csharp
using TallComponents.PDF.Kit;

Document doc1 = new Document("file1.pdf");
Document doc2 = new Document("file2.pdf");

PdfKit.Merger merger = new PdfKit.Merger();
merger.Append(doc1);
merger.Append(doc2);

Document merged = merger.Merge();
using (FileStream fs = new FileStream("merged.pdf", FileMode.Create))
{
    merged.Write(fs);
}
```

**IronPDF:**
```csharp
using IronPdf;

var pdf1 = PdfDocument.FromFile("file1.pdf");
var pdf2 = PdfDocument.FromFile("file2.pdf");

var merged = PdfDocument.Merge(pdf1, pdf2);
merged.SaveAs("merged.pdf");

// または既存に追加
pdf1.AppendPdf(pdf2);
pdf1.SaveAs("merged.pdf");
```

### 例7：セキュリティとパスワード保護

**Tall Components:**
```csharp
using TallComponents.PDF.Kit;
using TallComponents.PDF.Kit.Security;

Document document = a new Document();
// ... コンテンツの作成 ...

// セキュリティ設定
document.Security.OwnerPassword = "owner456";
document.Security.UserPassword = "user123";
document.Security.AllowPrinting = false;
document.Security.AllowCopying = false;
document.Security.EncryptionLevel = EncryptionLevel.AES256;

document.Write("protected.pdf");
```

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(htmlContent);

// セキュリティ設定
pdf.SecuritySettings.OwnerPassword = "owner456";
pdf.SecuritySettings.UserPassword = "user123";
pdf.SecuritySettings.AllowUserPrinting = PdfPrintSecurity.NoPrint;
pdf.SecuritySettings.AllowUserCopyPasteContent = false;
pdf.SecuritySettings.AllowUserEdits = PdfEditSecurity.NoEdit;
pdf.SecuritySettings.AllowUserAnnotations = false;

pdf.SaveAs("protected.pdf");
```

### 例8：URLからPDFへの変換（Tall Componentsでは不可能）

**Tall Components:** サポートされていません - HTMLレンダリング機能がありません

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

// 動的ページのための完全なJavaScriptサポート
renderer.RenderingOptions.EnableJavaScript = true;
renderer.RenderingOptions.WaitFor.RenderDelay(3000);

var pdf = renderer.RenderUrlAsPdf("https://example.com/dashboard");
pdf.SaveAs("dashboard.pdf");
```

### 例9：デジタル署名

**Tall Components:**
```csharp
using TallComponents.PDF.Kit;
using TallComponents.PDF.Kit.Signing;

Document document = new Document("unsigned.pdf");

// 証明書の読み込み
X509Certificate2 cert = new X509Certificate2("certificate.pfx", "password");

// 署名の作成
SignatureHandler handler = new SignatureHandler(cert);
document.Sign(handler);

document.Write("signed.pdf");
```

**IronPDF:**
```csharp
using IronPdf;
using IronPdf.Signing;

var pdf = PdfDocument.FromFile("unsigned.pdf");

// 証明書で署名
var signature = new PdfSignature("certificate.pfx", "password")
{
    SigningContact = "support@company.com",
    SigningLocation = "New York",
    Signing