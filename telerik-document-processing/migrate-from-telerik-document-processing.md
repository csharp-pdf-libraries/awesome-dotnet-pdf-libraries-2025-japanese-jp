---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [telerik-document-processing/migrate-from-telerik-document-processing.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/telerik-document-processing/migrate-from-telerik-document-processing.md)
🇯🇵 **日本語:** [telerik-document-processing/migrate-from-telerik-document-processing.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/telerik-document-processing/migrate-from-telerik-document-processing.md)

---

# Telerik Document ProcessingからIronPDFへの移行方法は？

## 目次
1. [Telerik Document Processingから移行する理由](#telerik-document-processingから移行する理由)
2. [CSS/HTMLレンダリングの問題](#csshtmlレンダリングの問題)
3. [フロードキュメントの問題](#フロードキュメントの問題)
4. [クイックスタート移行（5分）](#クイックスタート移行5分)
5. [完全なAPIリファレンス](#完全なapiリファレンス)
6. [コード移行例](#コード移行例)
7. [機能比較](#機能比較)
8. [ライセンス比較](#ライセンス比較)
9. [トラブルシューティングガイド](#トラブルシューティングガイド)
10. [移行チェックリスト](#移行チェックリスト)
11. [追加リソース](#追加リソース)

---

## Telerik Document Processingから移行する理由

### 重大な技術的制限

Telerik Document Processingは、現代のHTML/CSSを扱う際に根本的な問題があります：

| 問題 | 影響 | IronPDFの解決策 |
|-------|--------|------------------|
| **CSS解析の制限** | 現代のCSSフレームワーク（Bootstrapなど）が失敗する | 完全なChromium CSSサポート |
| **Divから段落への変換** | HTML構造が平坦化され、レイアウトが壊れる | 直接HTMLレンダリング |
| **フロードキュメントモデル** | 中間変換を強制される | ネイティブHTMLからPDFへ |
| **外部CSSの問題** | 複雑なセレクターが無視される | 完全なCSSファイルサポート |
| **メモリ問題** | 大規模なドキュメントでOutOfMemoryExceptionが発生する | 効率的なストリーミング |

### 核心的な問題：HTMLが正しくレンダリングされない

Telerik Document Processingは、PDFを生成する前にHTMLを中間の「フロードキュメント」モデルに変換します。このプロセスは：

1. **HTML構造を平坦化する** - `<div>`が段落になる
2. **現代のCSSを無視する** - Flexbox、Gridレイアウトが失敗する
3. **Bootstrapが壊れる** - 列システムが機能しない
4. **書式設定が失われる** - 複雑なセレクターが無視される

```html
<!-- この現代のHTML/CSSはTelerikで壊れます -->
<div class="container">
    <div class="row">
        <div class="col-md-6">列1</div>
        <div class="col-md-6">列2</div>
    </div>
</div>

<div style="display: flex; gap: 20px;">
    <div style="flex: 1;">フレックスアイテム1</div>
    <div style="flex: 1;">フレックスアイテム2</div>
</div>

<div style="display: grid; grid-template-columns: repeat(3, 1fr);">
    <div>グリッドアイテム1</div>
    <div>グリッドアイテム2</div>
    <div>グリッドアイテム3</div>
</div>
```

### 機能比較概要

| 機能 | Telerik Document Processing | IronPDF |
|---------|---------------------------|---------|
| **HTMLレンダリング** | フロードキュメント変換 | 直接Chromiumレンダリング |
| **CSS3サポート** | 限定的、多くの機能が失敗する | 完全なCSS3 |
| **Flexbox** | サポートされていない | 完全サポート |
| **CSS Grid** | サポートされていない | 完全サポート |
| **Bootstrap** | 壊れる（div平坦化） | 完全サポート |
| **外部CSS** | 部分的 | 完全サポート |
| **JavaScript** | サポートされていない | 完全サポート |
| **大規模なドキュメント** | メモリ問題 | 効率的なストリーミング |
| **APIの複雑さ** | 複雑（プロバイダー、モデル） | シンプル（1クラス） |

---

## CSS/HTMLレンダリングの問題

### Telerikで起こること

Telerikの`HtmlFormatProvider`はHTMLを`RadFlowDocument`に変換します：

```csharp
// ❌ Telerik - HTMLはフロードキュメントモデルに変換されます
HtmlFormatProvider htmlProvider = new HtmlFormatProvider();
RadFlowDocument document = htmlProvider.Import(htmlContent);

// インポート中に起こること：
// 1. <div class="container"> → 段落
// 2. <div class="row"> → 段落
// 3. <div class="col-md-6"> → 段落
// 4. すべてのレイアウトCSSが無視される
// 5. Bootstrapグリッドが連続する段落になる
// 6. Flexbox/Gridレイアウトは完全に失敗する
```

### Telerikで失敗するCSS機能

```css
/* ❌ これらのCSS機能はTelerikでは機能しません */

/* Flexbox - サポートされていない */
.container { display: flex; }
.item { flex: 1; }

/* CSS Grid - サポートされていない */
.grid { display: grid; grid-template-columns: repeat(3, 1fr); }

/* Bootstrap列 - 段落に変換される */
.col-md-6 { /* 無視され、線形テキストになる */ }

/* CSS変数 - サポートされていない */
:root { --primary: #007bff; }
.btn { color: var(--primary); }

/* 複雑なセレクター - よく無視される */
.container > .row:first-child { }
.item:hover { }
.content::before { }

/* 現代の単位 - 限定的なサポート */
.box { width: calc(100% - 20px); }
.text { font-size: 1.2rem; }
```

### IronPDFのChromiumエンジン

```csharp
// ✅ IronPDF - ブラウザと同じようにレンダリングします
var renderer = new ChromePdfRenderer();

var html = @"
<style>
    .container { display: flex; gap: 20px; }
    .item { flex: 1; padding: 20px; background: #f0f0f0; }
    .grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 10px; }
</style>
<div class='container'>
    <div class='item'>列1</div>
    <div class='item'>列2</div>
</div>
<div class='grid'>
    <div>グリッド1</div>
    <div>グリッド2</div>
    <div>グリッド3</div>
</div>";

var pdf = renderer.RenderHtmlAsPdf(html);
// すべてのCSSが正しくレンダリングされます - Chromeと全く同じように！
```

---

## フロードキュメントの問題

### Telerikの複雑なアーキテクチャ

Telerikでは、複数の概念を理解する必要があります：

```csharp
// ❌ Telerik - 複雑なプロバイダー/モデルアーキテクチャ

// 1. HTMLをフロードキュメントにインポート
HtmlFormatProvider htmlProvider = new HtmlFormatProvider();
RadFlowDocument document = htmlProvider.Import(htmlContent);

// 2. ドキュメントモデルを手動で変更
RadFlowDocumentEditor editor = new RadFlowDocumentEditor(document);
Section section = document.Sections.First();
Paragraph para = section.Blocks.AddParagraph();
para.Inlines.AddText("追加テキスト");

// 3. エクスポート設定を構成
PdfExportSettings exportSettings = new PdfExportSettings();
exportSettings.ImageQuality = ImageQuality.High;

// 4. 設定でPDFプロバイダーを作成
PdfFormatProvider pdfProvider = new PdfFormatProvider();
pdfProvider.ExportSettings = exportSettings;

// 5. バイトにエクスポート
byte[] pdfBytes = pdfProvider.Export(document);

// 6. ファイルに保存
File.WriteAllBytes("output.pdf", pdfBytes);
```

### IronPDFのシンプルなアプローチ

```csharp
// ✅ IronPDF - 直接レンダリング、中間モデルなし

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;

var pdf = renderer.RenderHtmlAsPdf(htmlContent);
pdf.SaveAs("output.pdf");

// それだけです - 15行以上のコードに対して3行！
```

---

## クイックスタート移行（5分）

### ステップ1：NuGetパッケージを更新

```bash
# Telerikパッケージを削除
dotnet remove package Telerik.Documents.Core
dotnet remove package Telerik.Documents.Flow
dotnet remove package Telerik.Documents.Flow.FormatProviders.Pdf
dotnet remove package Telerik.Documents.Fixed

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ2：Usingステートメントを更新

```csharp
// 以前
using Telerik.Windows.Documents.Flow.FormatProviders.Html;
using Telerik.Windows.Documents.Flow.FormatProviders.Pdf;
using Telerik.Windows.Documents.Flow.Model;
using Telerik.Documents.Primitives;

// 以降
using IronPdf;
```

### ステップ3：ライセンスキーを追加

```csharp
// アプリケーションの起動時に追加
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

### ステップ4：コードを更新

**以前（Telerik）：**
```csharp
using Telerik.Windows.Documents.Flow.FormatProviders.Html;
using Telerik.Windows.Documents.Flow.FormatProviders.Pdf;
using Telerik.Windows.Documents.Flow.Model;

HtmlFormatProvider htmlProvider = new HtmlFormatProvider();
RadFlowDocument document = htmlProvider.Import(htmlContent);

PdfFormatProvider pdfProvider = new PdfFormatProvider();
byte[] pdfBytes = pdfProvider.Export(document);

File.WriteAllBytes("output.pdf", pdfBytes);
```

**以降（IronPDF）：**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(htmlContent);
pdf.SaveAs("output.pdf");
```

---