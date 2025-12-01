---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [selectpdf/migrate-from-selectpdf.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/selectpdf/migrate-from-selectpdf.md)
🇯🇵 **日本語:** [selectpdf/migrate-from-selectpdf.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/selectpdf/migrate-from-selectpdf.md)

---

# 移行ガイド: SelectPdf → IronPDF

## 目次
1. [SelectPdfから移行する理由](#selectpdfから移行する理由)
2. [Windowsのみの問題](#windowsのみの問題)
3. [古いレンダリングエンジン](#古いレンダリングエンジン)
4. [5ページ無料版の罠](#5ページ無料版の罠)
5. [クイックスタート移行（5分）](#クイックスタート移行5分)
6. [完全なAPIリファレンス](#完全なapiリファレンス)
7. [コード移行例](#コード移行例)
8. [機能比較](#機能比較)
9. [ライセンス比較](#ライセンス比較)
10. [プラットフォームサポート比較](#プラットフォームサポート比較)
11. [トラブルシューティングガイド](#トラブルシューティングガイド)
12. [移行チェックリスト](#移行チェックリスト)
13. [追加リソース](#追加リソース)

---

## SelectPdfから移行する理由

### 重大な制限

| 問題 | 影響 | IronPDFの解決策 |
|-------|--------|------------------|
| **Windowsのみ** | Linux、Docker、Azure Functionsにデプロイできない | 完全なクロスプラットフォームサポート |
| **古いレンダリングエンジン** | 最新のCSSが失敗し、レイアウトが崩れる | 最新のChromium |
| **5ページ無料版の制限** | 5ページ後に強力なウォーターマーキング | 寛大なトライアル |
| **.NET 10のサポートなし** | 将来の互換性の問題 | 完全な.NET 10サポート |
| **クラウドデプロイメントがブロックされる** | AWS Lambda、Azure Functionsを使用できない | クラウドネイティブ |

### 核心的な問題

SelectPdfは堅牢なHTMLからPDFへのソリューションとして自己を宣伝していますが、根本的な制限があります：

1. **偽のクロスプラットフォームの主張**：マーケティング言語にもかかわらず、SelectPdfは**Windows上でのみ機能します**
2. **古いChromiumフォーク**：古いBlink/WebKitフォークを使用しており、最新のCSSでは失敗します
3. **制限的な無料版**：ウォーターマーキング前に5ページのみ
4. **クラウドサポートなし**：Azure Functions、AWS Lambda、またはDockerにデプロイできません

### 機能比較の概要

| 機能 | SelectPdf | IronPDF |
|---------|-----------|---------|
| **Windows** | ✅ | ✅ |
| **Linux** | ❌ サポートされていません | ✅ 10+ディストリビューション |
| **macOS** | ❌ サポートされていません | ✅ 完全サポート |
| **Docker** | ❌ サポートされていません | ✅ 公式イメージ |
| **Azure Functions** | ❌ サポートされていません | ✅ 完全サポート |
| **AWS Lambda** | ❌ サポートされていません | ✅ 完全サポート |
| **CSS Grid** | ⚠️ 限定 | ✅ 完全サポート |
| **Flexbox** | ⚠️ 限定 | ✅ 完全サポート |
| **CSS変数** | ❌ サポートされていません | ✅ 完全サポート |
| **.NET 10** | ❌ サポートされていません | ✅ 完全サポート |
| **無料版の制限** | 5ページ | 寛大なトライアル |

---

## Windowsのみの問題

### SelectPdfのプラットフォーム制限

マーケティング主張にかかわらず、SelectPdfは明示的に**サポートしていません**：

- Linux（どのディストリビューションでも）
- macOS
- Dockerコンテナ
- Azure Functions
- AWS Lambda
- Google Cloud Functions
- 任意のARMベースのシステム

これは根本的なアーキテクチャの制限です—SelectPdfはWindows固有のライブラリに依存しており、移植できません。

### あなたのアプリケーションにとってこれが意味すること

```csharp
// ❌ SelectPdf - このコードはLinux/Dockerで失敗します
using SelectPdf;

// Azure App Service（Linux）へのデプロイメント - 失敗
// Dockerコンテナへのデプロイメント - 失敗
// AWS Lambdaへのデプロイメント - 失敗
// GitHub Actions on ubuntu-latest - 失敗

var converter = new HtmlToPdf();
var doc = converter.ConvertHtmlString("<h1>Hello</h1>");
// 例外: SelectPdfはWindows上でのみ機能します
```

### IronPDFのクロスプラットフォームサポート

```csharp
// ✅ IronPDF - どこでも動作します
using IronPdf;

// Azure App Service（Linux）- 動作します
// Dockerコンテナ - 動作します
// AWS Lambda - 動作します
// GitHub Actions on ubuntu-latest - 動作します
// macOS開発 - 動作します

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1>Hello</h1>");
pdf.SaveAs("output.pdf");
```

### サポートされているプラットフォーム

| プラットフォーム | SelectPdf | IronPDF |
|----------|-----------|---------|
| Windows Server 2019+ | ✅ | ✅ |
| Windows 10/11 | ✅ | ✅ |
| Ubuntu 20.04+ | ❌ | ✅ |
| Debian 10+ | ❌ | ✅ |
| CentOS 7+ | ❌ | ✅ |
| Alpine Linux | ❌ | ✅ |
| Amazon Linux 2 | ❌ | ✅ |
| macOS 10.15+ | ❌ | ✅ |
| Azure App Service (Linux) | ❌ | ✅ |
| Azure Functions | ❌ | ✅ |
| AWS Lambda | ❌ | ✅ |
| Docker (Linux) | ❌ | ✅ |
| Kubernetes | ❌ | ✅ |

---

## 古いレンダリングエンジン

### SelectPdfのChromiumフォーク

SelectPdfは**古いBlink/WebKitフォーク**を使用しており、現代のウェブ標準に追いついていません：

```html
<!-- この現代のCSSはSelectPdfで失敗するか、正しくレンダリングされません -->

<!-- CSS Grid - 壊れている -->
<div style="display: grid; grid-template-columns: repeat(3, 1fr); gap: 20px;">
    <div>アイテム 1</div>
    <div>アイテム 2</div>
    <div>アイテム 3</div>
</div>

<!-- 高度なFlexbox - 壊れている -->
<div style="display: flex; gap: 20px; flex-wrap: wrap;">
    <div style="flex: 1 1 300px;">Flexアイテム</div>
</div>

<!-- CSS変数 - サポートされていません -->
<style>
:root { --primary-color: #007bff; }
h1 { color: var(--primary-color); }
</style>

<!-- 現代の@mediaクエリ - 限定 -->
<style>
@media (prefers-color-scheme: dark) { ... }
@media print { ... }
</style>
```

### IronPDFの現代的なChromium

```csharp
// ✅ IronPDF - 最新の安定したChromiumを使用
var renderer = new ChromePdfRenderer();

var html = @"
<style>
    :root { --primary: #007bff; --gap: 20px; }
    .grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(200px, 1fr)); gap: var(--gap); }
    .flex { display: flex; gap: var(--gap); flex-wrap: wrap; }
</style>
<div class='grid'>
    <div style='background: var(--primary); color: white; padding: 1rem;'>アイテム 1</div>
    <div style='background: var(--primary); color: white; padding: 1rem;'>アイテム 2</div>
    <div style='background: var(--primary); color: white; padding: 1rem;'>アイテム 3</div>
</div>";

var pdf = renderer.RenderHtmlAsPdf(html);
// すべての現代的なCSS機能が正しくレンダリングされます！
```

### CSS機能サポート比較

| CSS機能 | SelectPdf | IronPDF |
|-------------|-----------|---------|
| CSS Grid | ⚠️ 部分的/壊れている | ✅ 完全 |
| Flexbox (基本) | ✅ | ✅ |
| Flexbox (gapプロパティ) | ❌ | ✅ |
| CSS変数 | ❌ | ✅ |
| CSS calc() | ⚠️ 限定 | ✅ |
| @media print | ⚠️ 限定 | ✅ |
| @font-face | ⚠️ 限定 | ✅ |
| Webフォント | ⚠️ 限定 | ✅ |
| SVG | ⚠️ 基本 | ✅ 完全 |
| CSS変形 | ⚠️ 限定 | ✅ |
| CSSアニメーション | ❌ | ✅ |

---

## 5ページ無料版の罠

### SelectPdfの厳しい制限

SelectPdfの無料版には重大な制限があります：

```csharp
// ❌ SelectPdf 無料版
// - PDFあたり最大5ページ
// - 5ページを超えると、すべてのページに強力なウォーターマーキング
// - 「SelectPdfで作成されました」のウォーターマーク
// - 購入しない限り削除できません
// - 評価も制限されています

var converter = new HtmlToPdf();
var doc = converter.ConvertHtmlString(longHtmlContent);
// コンテンツが5ページを超える場合：
// - ページ1-5：あなたのコンテンツ
// - ページ6+：大量のウォーターマーク
```

### 商用価格比較

| 項目 | SelectPdf | IronPDF |
|--------|-----------|---------|
| **開始価格** | $499 | $749 |
| **無料トライアルページ** | 最大5ページ | 寛大なトライアル |
| **ウォーターマークの挙動** | 5ページ後に強力 | トライアルウォーターマーク |
| **ライセンスタイプ** | サブスクリプションオプション | 永久利用可能 |
| **価格の透明性** | 複雑な階層 | 明確な価格設定 |

---

## クイックスタート移行（5分）

### ステップ1: NuGetパッケージを更新

```bash
# SelectPdfを削除
dotnet remove package Select.HtmlToPdf

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ2: Usingステートメントを更新

```csharp
// 以前
using SelectPdf;

// 以後
using IronPdf;
```

### ステップ3: ライセンスキーを追加（トライアル用はオプション）

```csharp
// アプリケーション起動時に追加（ライセンス版用）
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

### ステップ4: コードを更新

**以前 (SelectPdf):**
```csharp
using SelectPdf;

var converter = new HtmlToPdf();
converter.Options.PdfPageSize = PdfPageSize.A4;
converter.Options.MarginTop = 20;
converter.Options.MarginBottom = 20;

PdfDocument doc = converter.ConvertHtmlString("<h1>Hello World</h1>");
doc.Save("output.pdf");
doc.Close();
```

**以後 (IronPDF):**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
renderer.RenderingOptions.MarginTop = 20;
renderer.RenderingOptions.MarginBottom = 20;

var pdf = renderer.RenderHtmlAsPdf("<h1>Hello World</h1>");
pdf.SaveAs("output.pdf");
// Close()は不要
```

---

## 完全なAPIリファレンス

### 名前空間マッピング

| SelectPdf 名前空間 | IronPDF 名前空間 | 備考 |
|---------------------|-------------------|-------|
| `SelectPdf` | `IronPdf` | メイン名前空間 |
| `SelectPdf.HtmlToPdf` | `IronPdf.ChromePdfRenderer` | レンダラー |
| `SelectPdf.PdfDocument` | `IronPdf.PdfDocument` | 同じ名前 |
| N/A | `IronPdf.Rendering` | 列挙型 |
| N/A | `IronPdf.Editing` | スタンパー |

### コアクラスマッピング

| SelectPdf | IronPDF | 備考 |
|-----------|---------|-------|
| `HtmlToPdf` | `ChromePdfRenderer` | メインコンバーター |
| `PdfDocument` | `PdfDocument` | 同じ名前、異なるAPI |
| `PdfPageSize` | `PdfPaperSize` | ページサイズ列挙型 |
| `PdfPageOrientation` | `PdfPaperOrientation` | 方向列挙型 |
| `SelectPdf.HtmlToPdfOptions` | `ChromePdfRenderOptions` | RenderingOptions経由 |

### メソッドマッピング

| SelectPdf メソッド | IronPDF メソッド | 備考 |
|------------------|----------------|-------|
| `converter.ConvertUrl(url)` | `renderer.RenderUrlAsPdf(url)` | URLからPDFへ |
| `converter.ConvertHtmlString(html)` | `renderer.RenderHtmlAsPdf(html)` | HTMLからPDFへ |
| `converter.ConvertHtmlString(html, baseUrl)` | `renderer.RenderHtmlAsPdf(html, baseUrl)` | ベースURL付き |
| `doc.Save(path)` | `pdf.SaveAs(path)` | ファイルを保存 |
| `doc.Save(stream)` | `pdf.Stream` または `pdf.BinaryData` | バイトを取得 |
| `doc.Close()` | 不要 | IDisposable |
| `doc.Pages.Count` | `pdf.PageCount` | ページ数 |

### オプションマッピング

| SelectPdf オプション | IronPDF オプション | 備考 |
|------------------|----------------|-------|
| `Options.PdfPageSize` | `RenderingOptions.PaperSize` | ページサイズ |
| `Options.PdfPageOrientation` | `RenderingOptions.PaperOrientation` | 方向 |
| `Options.MarginTop` | `RenderingOptions.MarginTop` | 上マージン（mm） |
| `Options.MarginBottom` |