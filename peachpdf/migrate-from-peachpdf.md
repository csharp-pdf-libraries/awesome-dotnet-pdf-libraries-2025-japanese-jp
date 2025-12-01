---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [peachpdf/migrate-from-peachpdf.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/peachpdf/migrate-from-peachpdf.md)
🇯🇵 **日本語:** [peachpdf/migrate-from-peachpdf.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/peachpdf/migrate-from-peachpdf.md)

---

# PeachPDFからIronPDFへの移行方法は？

## PeachPDFから移行する理由は？

PeachPDFは比較的新しく、あまり知られていないPDFライブラリで、成熟度、機能、サポートが確立されたソリューションに欠けています。移行する主な理由は以下の通りです：

1. **限定された機能セット**：PeachPDFは、デジタル署名、PDF/A準拠、高度なテキスト抽出などの高度な機能がありません
2. **小さなコミュニティ**：限定されたドキュメント、例、コミュニティサポート
3. **不確実な未来**：確立された実績のない新しいライブラリは、採用リスクを伴います
4. **基本的なHTMLサポート**：限定されたCSSおよびJavaScriptレンダリング機能
5. **エンタープライズサポートなし**：プロフェッショナルサポートやSLAオプションがありません

### 簡単な比較

| 項目 | PeachPDF | IronPDF |
|------|----------|---------|
| 成熟度 | 新しい | 確立された (40M+ ダウンロード) |
| HTMLレンダリング | 基本的 | フルChromium |
| CSSサポート | 限定的 | フルCSS3 |
| JavaScript | 基本的 | フルES2024 |
| デジタル署名 | なし | あり |
| PDF/A準拠 | なし | あり |
| プロフェッショナルサポート | なし | あり |
| ドキュメント | 限定的 | 広範 |

---

## クイックスタート：PeachPDFからIronPDFへ

### ステップ1：NuGetパッケージを置き換える

```bash
# PeachPDFを削除
dotnet remove package PeachPDF

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ2：名前空間を更新

```csharp
// 以前
using PeachPDF;

// 以後
using IronPdf;
```

### ステップ3：ライセンスを初期化

```csharp
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

---

## APIマッピングリファレンス

| PeachPDF | IronPDF | 備考 |
|----------|---------|------|
| `PdfDocument.Create()` | `new ChromePdfRenderer()` | レンダラーを作成 |
| `document.AddHtmlContent(html)` | `renderer.RenderHtmlAsPdf(html)` | HTMLをPDFに |
| `document.Save(path)` | `pdf.SaveAs(path)` | ファイルを保存 |
| `document.ToByteArray()` | `pdf.BinaryData` | バイトを取得 |
| `PdfReader.LoadFromFile(path)` | `PdfDocument.FromFile(path)` | PDFを読み込み |
| `document.AddPage()` | `pdf.AddPdfPages(newPdf)` | ページを追加 |
| `document.SetMetadata()` | `pdf.MetaData` | プロパティを設定 |
| `document.MergeWith(other)` | `PdfDocument.Merge(pdfs)` | PDFを結合 |

---

## コード例

### 例1：基本的なHTMLからPDFへ

**PeachPDF:**
```csharp
using PeachPDF;

var document = PdfDocument.Create();
document.AddHtmlContent("<h1>Hello World</h1><p>Sample content</p>");
document.Save("output.pdf");
```

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1>Hello World</h1><p>Sample content</p>");
pdf.SaveAs("output.pdf");
```

### 例2：URLからPDFへ

**PeachPDF:**
```csharp
// URLサポートが限定的またはなし
var document = PdfDocument.Create();
// HTMLを手動で取得する必要がある
```

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.EnableJavaScript = true;
renderer.RenderingOptions.WaitFor.JavaScript(3000);

var pdf = renderer.RenderUrlAsPdf("https://example.com");
pdf.SaveAs("webpage.pdf");
```

### 例3：PDFの読み込みと変更

**PeachPDF:**
```csharp
using PeachPDF;

var document = PdfReader.LoadFromFile("input.pdf");
document.AddPage();
document.Save("modified.pdf");
```

**IronPDF:**
```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("input.pdf");

// 新しいコンテンツページを追加
var renderer = new ChromePdfRenderer();
var newPage = renderer.RenderHtmlAsPdf("<h1>New Page</h1>");
pdf.AppendPdf(newPage);

// ウォーターマークを追加
pdf.ApplyWatermark("<div style='color: red; font-size: 48pt;'>DRAFT</div>");

pdf.SaveAs("modified.pdf");
```

### 例4：複数のPDFを結合

**PeachPDF:**
```csharp
using PeachPDF;

var doc1 = PdfReader.LoadFromFile("doc1.pdf");
var doc2 = PdfReader.LoadFromFile("doc2.pdf");
doc1.MergeWith(doc2);
doc1.Save("merged.pdf");
```

**IronPDF:**
```csharp
using IronPdf;

var pdf1 = PdfDocument.FromFile("doc1.pdf");
var pdf2 = PdfDocument.FromFile("doc2.pdf");

var merged = PdfDocument.Merge(pdf1, pdf2);
merged.SaveAs("merged.pdf");
```

### 例5：ヘッダーとフッター付きのPDF

**PeachPDF:**
```csharp
// ヘッダー/フッターのサポートが限定的
var document = PdfDocument.Create();
document.AddHtmlContent("<div>Header</div><h1>Content</h1>");
document.Save("output.pdf");
```

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
{
    HtmlFragment = @"<div style='text-align:center; font-size:10pt;'>Company Report</div>",
    MaxHeight = 30
};

renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
{
    HtmlFragment = @"<div style='text-align:center; font-size:9pt;'>Page {page} of {total-pages}</div>",
    MaxHeight = 25
};

var pdf = renderer.RenderHtmlAsPdf("<h1>Report Content</h1>");
pdf.SaveAs("report.pdf");
```

### 例6：パスワード保護

**PeachPDF:**
```csharp
// サポートされていない可能性がある
```

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1>Confidential</h1>");

pdf.SecuritySettings.OwnerPassword = "owner123";
pdf.SecuritySettings.UserPassword = "user123";
pdf.SecuritySettings.AllowUserCopyPasteContent = false;
pdf.SecuritySettings.AllowUserPrinting = PdfPrintSecurity.NoPrint;

pdf.SaveAs("protected.pdf");
```

### 例7：デジタル署名

**PeachPDF:**
```csharp
// サポートされていない
```

**IronPDF:**
```csharp
using IronPdf;
using IronPdf.Signing;

var pdf = PdfDocument.FromFile("document.pdf");

var signature = new PdfSignature("certificate.pfx", "password")
{
    SigningReason = "Document Approval",
    SigningLocation = "New York"
};

pdf.Sign(signature);
pdf.SaveAs("signed.pdf");
```

### 例8：非同期操作

**PeachPDF:**
```csharp
// 同期のみ
var document = PdfDocument.Create();
document.AddHtmlContent(html);
```

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
var pdf = await renderer.RenderHtmlAsPdfAsync("<h1>Async PDF</h1>");
pdf.SaveAs("async_output.pdf");
```

---

## 機能比較

| 機能 | PeachPDF | IronPDF |
|------|----------|---------|
| HTMLからPDF | 基本的 | フルChromium |
| URLからPDF | 限定的 | あり |
| CSS Grid/Flexbox | なし | あり |
| JavaScript | 限定的 | フルES2024 |
| PDFの結合 | あり | あり |
| PDFの分割 | 限定的 | あり |
| ウォーターマーク | 限定的 | フルHTML |
| ヘッダー/フッター | 基本的 | フルHTML |
| デジタル署名 | なし | あり |
| PDF/A | なし | あり |
| フォーム入力 | 限定的 | あり |
| テキスト抽出 | 基本的 | あり |
| 画像抽出 | なし | あり |
| 非同期サポート | 限定的 | あり |
| クロスプラットフォーム | 不明 | あり |

---

## 共通の移行問題

### 問題1：異なるAPIパターン

**問題：** PeachPDFはビルダーパターンを使用していますが、IronPDFは別のレンダラーを使用します。

**解決策：**
```csharp
// PeachPDFパターン
var document = PdfDocument.Create();
document.AddHtmlContent(html);
document.Save(path);

// IronPDFパターン
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs(path);
```

### 問題2：ページ追加の違い

**問題：** PeachPDFは空ページを追加しますが、IronPDFはコンテンツページを追加します。

**解決策：**
```csharp
// IronPDFで空白ページを作成
var blankPage = renderer.RenderHtmlAsPdf("<div style='height: 100vh;'></div>");
pdf.AppendPdf(blankPage);
```

### 問題3：SaveとSaveAsの違い

**問題：** メソッド名が異なります。

**解決策：** IronPDFでは`Save()`の代わりに`SaveAs()`を使用します。

---

## 移行チェックリスト

### 移行前

- [ ] **コードベースでのPeachPDFの使用を監査**
  ```bash
  grep -r "using PeachPDF" --include="*.cs" .
  grep -r "PdfDocument\|AddHtmlContent" --include="*.cs" .
  ```
  **理由：** 完全な移行範囲を確保するためにすべての使用箇所を特定します。

- [ ] **カスタム設定を文書化**
  ```csharp
  // 以下のようなパターンを探す：
  var config = new PeachPdfOptions {
      CustomSetting1 = value1,
      CustomSetting2 = value2
  };
  ```
  **理由：** これらの設定はIronPDFのRenderingOptionsにマッピングされます。移行後に一貫した出力を確保するために、今それらを文書化します。

- [ ] **IronPDFライセンスキーを取得**
  **理由：** IronPDFは本番使用にライセンスキーが必要です。無料試用版はhttps://ironpdf.com/で入手可能です。

### コード更新

- [ ] **NuGetパッケージを置き換える**
  ```bash
  dotnet remove package PeachPDF
  dotnet add package IronPdf
  ```
  **理由：** IronPDFへのクリーンなパッケージ切り替え。

- [ ] **名前空間を更新**
  ```csharp
  // 以前 (PeachPDF)
  using PeachPDF;

  // 以後 (IronPDF)
  using IronPdf;
  ```
  **理由：** コードが正しいライブラリを参照するようにします。

- [ ] **API呼び出しを変換**
  ```csharp
  // 以前 (PeachPDF)
  var document = PdfDocument.Create();
  document.AddHtmlContent(html);
  document.Save("output.pdf");

  // 以後 (IronPDF)
  var renderer = new ChromePdfRenderer();
  var pdf = renderer.RenderHtmlAsPdf(html);
  pdf.SaveAs("output.pdf");
  ```
  **理由：** IronPDFはChromePdfRendererを使用し、現代のChromiumレンダリングで正確なHTML/CSSサポートを提供します。

- [ ] **ライセンス初期化を追加**
  ```csharp
  // アプリケーション起動時に追加
  IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
  ```
  **理由：** ライセンスキーはPDF操作の前に設定されなければなりません。

- [ ] **保存方法を更新**
  ```csharp
  // 以前 (PeachPDF)
  document.Save("output.pdf");

  // 以後 (IronPDF)
  pdf.SaveAs("output.pdf");
  ```
  **理由：** IronPDFでPDFを保存するための正しい方法を使用します。

### テスト

- [ ] **HTMLレンダリングをテスト**
  **理由：** IronPDFのChromiumエンジンを使用してHTMLコンテンツが正しくレンダリングされることを確認します。

- [ ] **PDF出力品質を検証**
  **理由：** IronPDFのChromiumエンジンは若干異なるレンダリングを行う場合があります。通常はより良いですが、重要なドキュメントを確認してください。

- [ ] **結合操作をテスト**
  ```csharp
  // 以前 (PeachPDF)
  document.MergeWith(otherDocument);

  // 以後 (IronPDF)
  var merged = PdfDocument.Merge(pdf1, pdf2);
  ```
  **理由：** IronPDFのAPIを使用してPDFの結合が正しく機能することを確認します。

- [ ] **セキュリティ設定を検証**
  ```csharp
  // 以前 (PeachPDF)
  document.SetPassword("password");

  // 以後 (IronPDF)
  pdf.SecuritySettings.UserPassword = "password";
  ```
  **理由：** IronPDFでセキュリティ設定が正しく適用されていることを確認します。

- [ ] **パフォーマンス比較**
  **理由：** PeachPDFとIronPDFのパフォーマンスを比較し、移行がアプリケーションのパフォーマンスに悪影響を与えないことを確認します。
---

## 追加リソース

- [IronPDFドキュメント](https://ironpdf.com/docs/)
- [IronPDFチュートリアル](https://ironpdf.com/tutorials/)
- [HTMLからPDFへのガイド](https://ironpdf.com/how-to/html-file-to-pdf/)

---

*この移行ガイドは、[Awesome .NET PDF Libraries](../README.md)コレクションの一部です。*