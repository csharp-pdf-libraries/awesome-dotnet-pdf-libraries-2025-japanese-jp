# Scryber.CoreからIronPDFへの移行方法は？

## なぜScryber.Coreから移行するのか？

Scryber.Coreは、カスタムパーシングエンジンを使用したXML/HTMLテンプレートを利用するオープンソースのPDFライブラリです。データバインディング機能を提供していますが、いくつかの制限が開発者を移行させる原因となっています：

1. **LGPLライセンスの懸念**：一部の商用シナリオでソースコードの開示が必要
2. **カスタムテンプレート構文**：独自のバインディング構文に学習曲線が必要
3. **限定的なCSSサポート**：完全なブラウザベースのレンダラーではない
4. **コミュニティが小さい**：ドキュメントとコミュニティの例が少ない
5. **JavaScriptの実行ができない**：静的レンダリングのみ
6. **複雑な設定**：XMLを多用した設定アプローチ

### 簡単な比較

| 項目 | Scryber.Core | IronPDF |
|--------|--------------|---------|
| ライセンス | LGPL（制限的） | 商用 |
| レンダリングエンジン | カスタム | Chromium |
| CSSサポート | 限定的 | 完全なCSS3 |
| JavaScript | なし | 完全なES2024 |
| テンプレートバインディング | 独自のXML | 標準（Razorなど） |
| 学習曲線 | カスタム構文 | 標準のHTML/CSS |
| 非同期サポート | 限定的 | 完全 |
| ドキュメント | 基本 | 詳細 |

---

## クイックスタート：Scryber.CoreからIronPDFへ

### ステップ1：NuGetパッケージの置き換え

```bash
# Scryber.Coreを削除
dotnet remove package Scryber.Core

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ2：名前空間の更新

```csharp
// 以前
using Scryber.Components;
using Scryber.Components.Pdf;
using Scryber.PDF;
using Scryber.Styles;

// 以後
using IronPdf;
```

### ステップ3：ライセンスの初期化

```csharp
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

---

## APIマッピングリファレンス

| Scryber.Core | IronPDF | 備考 |
|--------------|---------|-------|
| `Document.ParseDocument(html)` | `renderer.RenderHtmlAsPdf(html)` | HTMLレンダリング |
| `Document.ParseTemplate(path)` | `renderer.RenderHtmlFileAsPdf(path)` | ファイルレンダリング |
| `doc.SaveAsPDF(path)` | `pdf.SaveAs(path)` | ファイルに保存 |
| `doc.SaveAsPDF(stream)` | `pdf.Stream` または `pdf.BinaryData` | ストリーム/バイトを取得 |
| `doc.Info.Title` | `pdf.MetaData.Title` | メタデータ |
| `doc.Info.Author` | `pdf.MetaData.Author` | メタデータ |
| `PDFPage` | `pdf.Pages[i]` | ページアクセス |
| `PDFLayoutDocument` | `RenderingOptions` | レイアウト制御 |
| `PDFStyle` | HTML内のCSS | スタイリング |
| データバインディング（`{{value}}`） | Razor/文字列補間 | テンプレーティング |

---

## コード例

### 例1：基本的なHTMLからPDFへ

**Scryber.Core：**
```csharp
using Scryber.Components;

var html = @"<html xmlns='http://www.w3.org/1999/xhtml'>
    <body>
        <h1>Hello World</h1>
        <p>Scryberで生成</p>
    </body>
</html>";

using (var doc = Document.ParseDocument(new System.IO.StringReader(html), ParseSourceType.DynamicContent))
{
    doc.SaveAsPDF("output.pdf");
}
```

**IronPDF：**
```csharp
using IronPdf;

var html = @"
<html>
<body>
    <h1>Hello World</h1>
    <p>IronPDFで生成</p>
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("output.pdf");
```

### 例2：データバインディングを含むテンプレート

**Scryber.Core（独自のバインディング）：**
```csharp
using Scryber.Components;

// Scryber XMLテンプレートとバインディング
var template = @"<?xml version='1.0' encoding='utf-8' ?>
<pdf:Document xmlns:pdf='http://www.scryber.co.uk/schemas/core/release/v1/Scryber.Components.xsd'>
    <Pages>
        <pdf:Page>
            <Content>
                <pdf:H1 text='{{model.Title}}' />
                <pdf:Para text='顧客: {{model.CustomerName}}' />
                <pdf:Para text='金額: {{model.Amount}}' />
            </Content>
        </pdf:Page>
    </Pages>
</pdf:Document>";

var model = new { Title = "請求書", CustomerName = "John Doe", Amount = "$150.00" };

using (var doc = Document.ParseDocument(new System.IO.StringReader(template), ParseSourceType.DynamicContent))
{
    doc.Params["model"] = model;
    doc.SaveAsPDF("invoice.pdf");
}
```

**IronPDF（任意のテンプレートエンジンを使用）：**
```csharp
using IronPdf;

// 標準のC#文字列補間を使用
var model = new { Title = "請求書", CustomerName = "John Doe", Amount = "$150.00" };

var html = $@"
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; padding: 40px; }}
        h1 {{ color: #333; border-bottom: 2px solid #007bff; }}
        .amount {{ font-size: 24px; color: green; }}
    </style>
</head>
<body>
    <h1>{model.Title}</h1>
    <p><strong>顧客:</strong> {model.CustomerName}</p>
    <p class='amount'><strong>金額:</strong> {model.Amount}</p>
</body>
</html>";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("invoice.pdf");
```

### 例3：URLからPDFへ

**Scryber.Core：**
```csharp
using Scryber.Components;
using System.Net.Http;

// HTMLを手動で取得する必要がある
var client = new HttpClient();
var html = await client.GetStringAsync("https://example.com");

// パースして保存
using (var doc = Document.ParseDocument(new System.IO.StringReader(html), ParseSourceType.DynamicContent))
{
    doc.SaveAsPDF("webpage.pdf");
}
```

**IronPDF：**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.EnableJavaScript = true;
renderer.RenderingOptions.WaitFor.JavaScript(3000);

var pdf = renderer.RenderUrlAsPdf("https://example.com");
pdf.SaveAs("webpage.pdf");
```

### 例4：ページ設定とマージン

**Scryber.Core：**
```csharp
using Scryber.Components;
using Scryber.Styles;

var template = @"<?xml version='1.0' encoding='utf-8' ?>
<pdf:Document xmlns:pdf='http://www.scryber.co.uk/schemas/core/release/v1/Scryber.Components.xsd'>
    <Styles>
        <pdf:Style applied-type='pdf:Page'>
            <pdf:Size paper-size='A4' orientation='Portrait' />
            <pdf:Margins all='20pt' />
        </pdf:Style>
    </Styles>
    <Pages>
        <pdf:Page>
            <Content>
                <pdf:H1 text='レポート' />
            </Content>
        </pdf:Page>
    </Pages>
</pdf:Document>";
```

**IronPDF：**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

// ページ設定
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Portrait;

// マージン（mm単位）
renderer.RenderingOptions.MarginTop = 20;
renderer.RenderingOptions.MarginBottom = 20;
renderer.RenderingOptions.MarginLeft = 20;
renderer.RenderingOptions.MarginRight = 20;

var pdf = renderer.RenderHtmlAsPdf("<h1>レポート</h1>");
pdf.SaveAs("report.pdf");
```

### 例5：ヘッダーとフッター

**Scryber.Core：**
```csharp
// XMLベースのヘッダー/フッター定義が必要
var template = @"<?xml version='1.0' encoding='utf-8' ?>
<pdf:Document xmlns:pdf='http://www.scryber.co.uk/schemas/core/release/v1/Scryber.Components.xsd'>
    <Pages>
        <pdf:Section>
            <Header>
                <pdf:Para text='会社レポート' />
            </Header>
            <Footer>
                <pdf:Para text='ページ {{pagenum}} / {{pagetotal}}' />
            </Footer>
            <Content>
                <pdf:H1 text='ここにコンテンツ' />
            </Content>
        </pdf:Section>
    </Pages>
</pdf:Document>";
```

**IronPDF：**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

// CSSをフルサポートするHTMLヘッダー
renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
{
    HtmlFragment = @"
        <div style='width: 100%; text-align: center; font-size: 12pt; border-bottom: 1px solid #ccc;'>
            会社レポート
        </div>",
    MaxHeight = 30
};

// ページ番号を含むHTMLフッター
renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
{
    HtmlFragment = @"
        <div style='width: 100%; text-align: center; font-size: 10pt;'>
            ページ {page} / {total-pages}
        </div>",
    MaxHeight = 25
};

var pdf = renderer.RenderHtmlAsPdf("<h1>ここにコンテンツ</h1>");
pdf.SaveAs("report.pdf");
```

### 例6：PDFの結合

**Scryber.Core：**
```csharp
// 組み込みの結合サポートが限定的
// 外部ライブラリがしばしば必要
```

**IronPDF：**
```csharp
using IronPdf;

var pdf1 = PdfDocument.FromFile("chapter1.pdf");
var pdf2 = PdfDocument.FromFile("chapter2.pdf");
var pdf3 = PdfDocument.FromFile("chapter3.pdf");

var merged = PdfDocument.Merge(pdf1, pdf2, pdf3);
merged.SaveAs("complete_book.pdf");
```

### 例7：セキュリティとメタデータ

**Scryber.Core：**
```csharp
using (var doc = Document.ParseDocument(reader, ParseSourceType.DynamicContent))
{
    doc.Info.Title = "私のドキュメント";
    doc.Info.Author = "John Doe";
    // セキュリティオプションが限定的
    doc.SaveAsPDF("output.pdf");
}
```

**IronPDF：**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1>機密</h1>");

// メタデータ
pdf.MetaData.Title = "私のドキュメント";
pdf.MetaData.Author = "John Doe";
pdf.MetaData.Subject = "年次報告";
pdf.MetaData.Keywords = "報告, 年次, 機密";

// セキュリティ
pdf.SecuritySettings.OwnerPassword = "owner123";
pdf.SecuritySettings.UserPassword = "user456";
pdf.SecuritySettings.AllowUserCopyPasteContent = false;
pdf.SecuritySettings.AllowUserPrinting = PdfPrintSecurity.FullPrintRights;

pdf.SaveAs("protected.pdf");
```

---

## 機能比較

| 機能 | Scryber.Core | IronPDF |
|---------|--------------|---------|
| HTMLからPDFへ | 基本 | 完全なChromium |
| URLからPDFへ | 手動フェッチ | ネイティブサポート |
| CSSグリッド | 限定的 | 完全サポート |
| フレックスボックス | 限定的 | 完全サポート |
| JavaScript | なし | 完全なES2024 |
| データバインディング | 独自のXML | Razor/Handlebarsを使用 |
| ヘッダー/フッター | XMLベース | HTML/CSS |
| PDFの結合 | 限定的 | 組み込み |
| PDFの分割 | なし | あり |
| ウォーターマーク | 基本 | 完全なHTML |
| デジタル署名 | なし | あり |
| PDF/A | なし | あり |
| パスワード保護 | 基本 | 完全 |
| 非同期サポート | 限定的 | 完全 |
| クロスプラットフォーム | あり | あり |

---

## テンプレート移行パターン

### 独自のバインディングから標準テンプレートへの移行

**Scryberバインディング：**
```xml
<pdf:Para text='{{model.Name}}' />
<pdf:Para text='合計: {{model.Total:C}}' />
<pdf:ForEach on='{{model.Items}}'>
    <pdf:Para text='{{.Name}}: {{.Price}}' />
</pdf:ForEach>
```

**C#文字列補間を使用したIronPDF：**
```csharp
var items = model.Items.Select(i => $"<li>{i.Name}: {i.Price:C}</li>");

var html = $@"
<p>{model.Name}</p>
<p>合計: {model.Total:C}</p>
<ul>
    {string.Join("", items)}
</ul>";
```

**複雑なテンプレートにはRazorを推奨（IronPDF）：**
```csharp
// RazorLightなどを使用
var engine = new RazorLightEngineBuilder()
    .UseMemoryCachingProvider()
    .Build();

var html = await engine.CompileRenderStringAsync("template", template, model);
var pdf = renderer.RenderHtmlAsPdf(html);
```

---

## 移行チェックリスト

### 移行前

- [ ] **すべてのScryberテンプレートを監査**
  ```bash
  grep -r "<scryber:Document" --include="*.xml" .
  ```
  **理由：** IronPDF用にHTMLに変換するため、すべてのScryberテンプレートを特定します。

- [ ] **使用されているデータバインディングパターンを文書化**
  ```xml
  <!-- 以前（Scryber） -->
  <scryber:TextField Value="{{dataBinding}}" />

  <!-- これらのパターンを変換用に文書化 -->
  ```
  **理由：** Scryberは独自のデータバインディングを使用しており、Razorなどの標準テンプレーティングに変換する必要があります。

- [ ] **カスタムスタイルを特定**
  ```xml
  <!-- 以前（Scryber） -->
  <scryber:Style>
      <scryber:Font Size="12pt" />
  </scryber:Style>
  ```
  **理由：** IronPDFのHTMLベースのレンダリングに合わせて、カスタムスタイルをCSSに変換する必要があります。

- [ ] **IronPDFライセンスキーを取得**
  **理由：** Iron