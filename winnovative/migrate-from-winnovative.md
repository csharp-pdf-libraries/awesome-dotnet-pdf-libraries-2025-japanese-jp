---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [winnovative/migrate-from-winnovative.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/winnovative/migrate-from-winnovative.md)
🇯🇵 **日本語:** [winnovative/migrate-from-winnovative.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/winnovative/migrate-from-winnovative.md)

---

# WinnovativeからIronPDFへの移行方法は？

## Winnovativeから移行する理由は？

### 古いレンダリングエンジンの問題

Winnovativeは、**2016年のWebKitエンジン**に依存しており、現代のWebアプリケーションに深刻な問題を引き起こします：

1. **CSS Gridのサポートなし**：Bootstrap 5、Tailwind CSS、および現代のレイアウトが完全に崩壊します
2. **Flexboxの実装がバグっている**：現代のブラウザと比較して一貫性のないレンダリング
3. **ES5 JavaScriptのみ**：現代のES6+ JavaScript（アロー関数、async/await、クラス）が静かに失敗します
4. **開発の停滞**："Winnovative"が革新を示唆しているにもかかわらず、近年は最小限の更新
5. **フォントレンダリングの問題**：Webフォントとカスタムタイポグラフィがしばしば正しくレンダリングされません
6. **セキュリティ上の懸念**：2016年のWebKitは、数年分のセキュリティパッチを欠いています

### 実際の影響

```html
<!-- この現代のCSSはWinnovativeで壊れます -->
<div style="display: grid; grid-template-columns: repeat(3, 1fr); gap: 20px;">
  <div>列 1</div>
  <div>列 2</div>
  <div>列 3</div>
</div>

<!-- 現代のJavaScriptが静かに失敗します -->
<script>
const items = data.map(item => item.name); // アロー関数：失敗
const result = await fetchData(); // Async/await：失敗
class Report { } // クラス：失敗
</script>
```

### 移行比較の概要

| 項目 | Winnovative | IronPDF |
|--------|-------------|---------|
| レンダリングエンジン | WebKit (2016) | Chromium (現在) |
| CSS Grid | サポートされていない | 完全サポート |
| Flexbox | バグがある | 完全サポート |
| JavaScript | ES5のみ | ES2024 |
| Bootstrap 5 | 壊れている | 完全サポート |
| Tailwind CSS | サポートされていない | 完全サポート |
| React/Vue SSR | 問題がある | 完璧に動作 |
| Webフォント | 信頼できない | 完全サポート |
| アップデート | 不定期 | 月次 |
| 価格 | $750-$1,600 | 競争力のある |

---

## 5分でWinnovativeからIronPDFへのクイックスタート

### ステップ 1: NuGetパッケージの置き換え

```bash
# Winnovativeを削除
dotnet remove package Winnovative.WebKitHtmlToPdf
dotnet remove package Winnovative.HtmlToPdf
dotnet remove package Winnovative.WebToPdfConverter

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ 2: 名前空間の更新

```csharp
// 以前
using Winnovative;
using Winnovative.WebKit;

// 以降
using IronPdf;
```

### ステップ 3: 基本コードの変換

**Winnovative:**
```csharp
var converter = new HtmlToPdfConverter();
converter.LicenseKey = "your-winnovative-key";
byte[] pdfBytes = converter.ConvertUrl("https://example.com");
File.WriteAllBytes("output.pdf", pdfBytes);
```

**IronPDF:**
```csharp
IronPdf.License.LicenseKey = "YOUR-IRONPDF-KEY";
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderUrlAsPdf("https://example.com");
pdf.SaveAs("output.pdf");
```

---

## 完全なAPIリファレンス

### コアクラスのマッピング

| Winnovativeクラス | IronPDF相当 | メモ |
|-------------------|-------------------|-------|
| `HtmlToPdfConverter` | `ChromePdfRenderer` | 主要な変換クラス |
| `PdfDocument` | `PdfDocument` | PDFの操作 |
| `PdfPage` | `PdfDocument.Pages[]` | ページアクセス |
| `PdfDocumentOptions` | `RenderingOptions` | 設定 |
| `PdfHeaderOptions` | `HtmlHeaderFooter` | ヘッダー |
| `PdfFooterOptions` | `HtmlHeaderFooter` | フッター |
| `TextElement` | `HtmlFragment`内のHTML | テキストの位置決め |
| `ImageElement` | HTMLの`<img>` | 画像の配置 |
| `PdfSecurityOptions` | `SecuritySettings` | セキュリティ |

### メソッドのマッピング

| Winnovativeメソッド | IronPDFメソッド |
|-------------------|----------------|
| `ConvertUrl(url)` | `RenderUrlAsPdf(url)` |
| `ConvertUrlToFile(url, path)` | `RenderUrlAsPdf(url).SaveAs(path)` |
| `ConvertHtml(html, baseUrl)` | `RenderHtmlAsPdf(html)` |
| `ConvertHtmlToFile(html, path)` | `RenderHtmlAsPdf(html).SaveAs(path)` |
| `ConvertHtmlFile(path)` | `RenderHtmlFileAsPdf(path)` |
| `MergePdf(streams)` | `PdfDocument.Merge(pdfs)` |
| `AppendPdf(pdf)` | `pdf1.AppendPdf(pdf2)` |
| `AddElement(element)` | HTMLベースのヘッダー/フッター |

### オプションのマッピング

| Winnovativeオプション | IronPDFオプション |
|-------------------|----------------|
| `PdfPageSize.A4` | `PaperSize = PdfPaperSize.A4` |
| `PdfPageSize.Letter` | `PaperSize = PdfPaperSize.Letter` |
| `PdfPageOrientation.Portrait` | `PaperOrientation = PdfPaperOrientation.Portrait` |
| `PdfPageOrientation.Landscape` | `PaperOrientation = PdfPaperOrientation.Landscape` |
| `TopMargin = 20` | `MarginTop = 20` |
| `BottomMargin = 20` | `MarginBottom = 20` |
| `LeftMargin = 15` | `MarginLeft = 15` |
| `RightMargin = 15` | `MarginRight = 15` |
| `ShowHeader = true` | `HtmlHeader`プロパティを設定 |
| `ShowFooter = true` | `HtmlFooter`プロパティを設定 |
| `LiveUrlsEnabled = true` | リンクはデフォルトで機能 |
| `JavaScriptEnabled = true` | `EnableJavaScript = true` |
| `InternalLinksEnabled = true` | リンクはデフォルトで機能 |
| `EmbedFonts = true` | フォントはデフォルトで埋め込まれる |

---