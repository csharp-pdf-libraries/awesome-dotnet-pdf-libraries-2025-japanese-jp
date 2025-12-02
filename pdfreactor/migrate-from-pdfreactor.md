---
**  (Japanese Translation)**

 **English:** [pdfreactor/migrate-from-pdfreactor.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/pdfreactor/migrate-from-pdfreactor.md)
 **:** [pdfreactor/migrate-from-pdfreactor.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/pdfreactor/migrate-from-pdfreactor.md)

---
# PDFreactorからIronPDFへの移行方法は？

PDFreactorは、優れたCSS Paged Mediaサポートを備えた強力なJavaベースのHTMLからPDFへの変換サーバーです。高忠実度ドキュメントレンダリングに優れていますが、Javaの依存関係とサーバーアーキテクチャは.NET環境での複雑さを生み出します。このガイドでは、外部依存関係なしで同等のレンダリング機能を提供するネイティブ.NETライブラリであるIronPDFへの包括的な移行パスを提供します。

## 目次

1. [PDFreactorからIronPDFに移行する理由は？](#why-migrate-from-pdfreactor-to-ironpdf)
2. [アーキテクチャの違い](#architectural-differences)
3. [インストールとセットアップ](#installation-and-setup)
4. [コアAPIマッピング](#core-api-mappings)
5. [コード移行例](#code-migration-examples)
6. [設定の移行](#configuration-migration)
7. [CSS Paged Mediaの移行](#css-paged-media-migration)
8. [ヘッダーとフッター](#headers-and-footers)
9. [JavaScriptと非同期コンテンツ](#javascript-and-async-content)
10. [サーバー対ライブラリアーキテクチャ](#server-vs-library-architecture)
11. [パフォーマンスの最適化](#performance-optimization)
12. [トラブルシューティング](#troubleshooting)
13. [移行チェックリスト](#migration-checklist)

---

## PDFreactorからIronPDFに移行する理由は？

### Javaの依存関係問題

PDFreactorのアーキテクチャは、.NET環境でいくつかの課題を生み出します：

1. **Javaランタイムが必要**：すべてのサーバーにJRE/JDKのインストールが必要
2. **サーバーアーキテクチャ**：追加のインフラストラクチャを必要とする別のサービスとして実行
3. **複雑なデプロイメント**：.NET CI/CDパイプラインでのJava依存関係の管理
4. **プロセス間通信**：REST APIまたはソケット通信がレイテンシを追加
5. **別々のライセンス管理**：アプリケーションではなくサーバーインスタンスにライセンスがバインドされる
6. **リソースの分離**：別々のプロセスメモリとCPU管理
7. **運用上のオーバーヘッド**：2つのランタイムを維持、監視、更新する必要がある

### IronPDFの利点

| 項目 | PDFreactor | IronPDF |
|--------|-----------|---------|
| ランタイム | Java（外部） | ネイティブ.NET |
| アーキテクチャ | サーバーベースのサービス | プロセス内ライブラリ |
| デプロイメント | 複雑（Java + サービス） | NuGetパッケージ |
| 依存関係 | JRE + RESTクライアント | 自己完結型 |
| レイテンシ | ネットワーク/IPCオーバーヘッド | 直接メソッド呼び出し |
| スケーリング | サーバーごとのライセンス | 開発者ごとのライセンス |
| 統合 | REST API呼び出し | ネイティブ.NET API |
| メモリ | 別プロセス | プロセス内制御 |
| CSSサポート | 優れている（Paged Media） | 優れている（Chromium） |
| PDF操作 | 変換のみ | ライフサイクル全体 |

---

## アーキテクチャの違い

### PDFreactorアーキテクチャ

```
┌─────────────────┐     ┌─────────────────┐     ┌─────────────────┐
│   .NET App      │────▶│  PDFreactor     │────▶│   PDF Output    │
│  (REST Client)  │ HTTP│    Server       │     │                 │
└─────────────────┘     │   (Java)        │     └─────────────────┘
                        └─────────────────┘
```

### IronPDFアーキテクチャ

```
┌─────────────────────────────────────────┐
│              .NET Application           │
│  ┌─────────────────────────────────┐    │
│  │         IronPDF Library         │    │
│  │    (Embedded Chromium Engine)   │───▶│ PDF Output
│  └─────────────────────────────────┘    │
└─────────────────────────────────────────┘
```

---

## インストールとセットアップ

### PDFreactorの削除

```bash
# PDFreactor NuGetパッケージを削除
dotnet remove package PDFreactor.NET
dotnet remove package PDFreactor.Native.Windows.x64

# PDFreactorサーバーサービスを停止（ローカルで実行している場合）
# Windows: net stop PDFreactor
# Linux: sudo systemctl stop pdfreactor
```

### IronPDFのインストール

```bash
dotnet add package IronPdf
```

### ライセンス設定

**PDFreactor（サーバーベース）：**
```csharp
// サーバーで設定ファイルまたはコマンドライン経由でライセンスを設定
// クライアントはライセンス付きサーバーに接続
var pdfReactor = new PDFreactor("http://pdfreactor-server:9423");
```

**IronPDF（アプリケーションレベル）：**
```csharp
// アプリケーション起動時に一度だけセットアップ
IronPdf.License.LicenseKey = "YOUR-IRONPDF-LICENSE-KEY";

// またはappsettings.json経由で
{
  "IronPdf": {
    "LicenseKey": "YOUR-IRONPDF-LICENSE-KEY"
  }
}
```

---

## コアAPIマッピング

### クラスとオブジェクト

| PDFreactor | IronPDF | 備考 |
|------------|---------|-------|
| `PDFreactor` | `ChromePdfRenderer` | 主要な変換クラス |
| `Configuration` | `ChromePdfRenderOptions` | PDF設定 |
| `Result` | `PdfDocument` | 出力ドキュメント |
| `Configuration.Document` | `RenderHtmlAsPdf(html)` | HTML入力 |
| `Result.Document` (byte[]) | `pdf.BinaryData` | 生のバイト |
| `Configuration.BaseURL` | `RenderingOptions.BaseUrl` | リソースのベースURL |

### 設定プロパティ

| PDFreactor設定 | IronPDF RenderingOptions | 備考 |
|-------------------------|-------------------------|-------|
| `config.Document = html` | `renderer.RenderHtmlAsPdf(html)` | HTMLコンテンツ |
| `config.Document = url` | `renderer.RenderUrlAsPdf(url)` | URL変換 |
| `config.BaseURL` | `RenderingOptions.BaseUrl` | リソースのベースパス |
| `config.EnableJavaScript = true` | `RenderingOptions.EnableJavaScript = true` | JS実行 |
| `config.JavaScriptSettings.Timeout` | `RenderingOptions.WaitFor.RenderDelay` | JSタイムアウト |
| `config.PageFormat = PageFormat.A4` | `RenderingOptions.PaperSize = PdfPaperSize.A4` | 用紙サイズ |
| `config.PageOrientation = Orientation.LANDSCAPE` | `RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape` | 方向 |
| `config.PageMargins` | `RenderingOptions.MarginTop/Bottom/Left/Right` | 余白 |
| `config.UserStyleSheets.Add()` | `RenderingOptions.CssMediaType` | CSS設定 |
| `config.Encryption` | `pdf.SecuritySettings` | PDFセキュリティ |
| `config.Title` | `pdf.MetaData.Title` | メタデータ |
| `config.Author` | `pdf.MetaData.Author` | メタデータ |
| `config.ColorSpace` | `RenderingOptions.GrayScale` | 色オプション |
| `config.ConformanceType` | `RenderingOptions.PdfA = true` | PDF/A準拠 |
| `config.Cookies` | `RenderingOptions.CustomCookies` | HTTPクッキー |

### HTTP/接続設定

| PDFreactor | IronPDF | 備考 |
|------------|---------|-------|
| `new PDFreactor(serverUrl)` | N/A (プロセス内) | サーバー不要 |
| `pdfReactor.ServiceUrl` | N/A | サービスURLなし |
| `config.RequestTimeout` | `RenderingOptions.Timeout` | レンダリングタイムアウト |
| `config.HttpProxy` | `RenderingOptions.Proxy` | プロキシ設定 |
| `config.Authentication` | `RenderingOptions.HttpLogin*` | HTTP認証 |

### CSSとスタイルシート設定

| PDFreactor | IronPDF | 備考 |
|------------|---------|-------|
| `config.AddUserStyleSheet(css)` | HTML内に埋め込むか`CustomCssUrl`を使用 | CSS注入 |
| `config.AddUserScript(js)` | `RenderingOptions.Javascript` | JS注入 |
| `config.MediaTypes` | `RenderingOptions.CssMediaType` | 画面/印刷 |
| `config.MergeMode` | HTML内のCSS | HTML/CSS経由で制御 |
| `config.DocumentDefaultLanguage` | HTMLの`lang`属性 | ドキュメント言語 |

### 高度な変換オプション

| PDFreactor | IronPDF | 備考 |
|------------|---------|-------|
| `config.AddAttachment()` | `pdf.Attachments.Add()` | PDF添付ファイル |
| `config.EnableBookmarks = true` | `RenderingOptions.CreatePdfFormsFromHtml` | ブックマーク |
| `config.EnableLinks = true` | 自動 | リンクは常に有効 |
| `config.EnableOverprint = true` | N/A (自動) | オーバープリント処理 |
| `config.FullCompression = true` | N/A (最適化済み) | 圧縮 |
| `config.IntegrationMode` | N/A (直接) | 統合 |

---

## コード移行例

### 例1：基本的なHTMLからPDFへ

**PDFreactor:**
```csharp
using RealObjects.PDFreactor;
using System.IO;

var pdfReactor = new PDFreactor("http://localhost:9423");

var config = new Configuration
{
    Document = "<html><body><h1>こんにちは世界</h1></body></html>"
};

Result result = pdfReactor.Convert(config);
File.WriteAllBytes("output.pdf", result.Document);
```

**IronPDF:**
```csharp
using IronPdf;

IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<html><body><h1>こんにちは世界</h1></body></html>");
pdf.SaveAs("output.pdf");
```

### 例2：URLからPDFへ

**PDFreactor:**
```csharp
using RealObjects.PDFreactor;
using System.IO;

var pdfReactor = new PDFreactor("http://localhost:9423");

var config = new Configuration
{
    Document = "https://www.example.com",
    EnableJavaScript = true
};

Result result = pdfReactor.Convert(config);
File.WriteAllBytes("webpage.pdf", result.Document);
```

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.EnableJavaScript = true;

var pdf = renderer.RenderUrlAsPdf("https://www.example.com");
pdf.SaveAs("webpage.pdf");
```

### 例3：ページ設定

**PDFreactor:**
```csharp
using RealObjects.PDFreactor;

var config = new Configuration
{
    Document = htmlContent,
    PageFormat = PageFormat.LETTER,
    PageOrientation = Orientation.LANDSCAPE,
    PageMargins = new PageMargins
    {
        Top = "1in",
        Bottom = "1in",
        Left = "0.5in",
        Right = "0.5in"
    }
};

Result result = pdfReactor.Convert(config);
```

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.PaperSize = PdfPaperSize.Letter;
renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;
renderer.RenderingOptions.MarginTop = 25.4;      // 1インチをmmで
renderer.RenderingOptions.MarginBottom = 25.4;
renderer.RenderingOptions.MarginLeft = 12.7;    // 0.5インチをmmで
renderer.RenderingOptions.MarginRight = 12.7;

var pdf = renderer.RenderHtmlAsPdf(htmlContent);
pdf.SaveAs("document.pdf");
```

### 例4：CSSスタイルシート

**PDFreactor:**
```csharp
using RealObjects.PDFreactor;

var config = new Configuration
{
    Document = htmlContent
};

// CSS Paged Mediaスタイルシートを追加
config.AddUserStyleSheet(
    "@page { size: A4 landscape; margin: 2cm; }" +
    "@page :first { margin-top: 5cm; }" +
    "body { font-family: Arial; }"
);

Result result = pdfReactor.Convert(config);
```

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;
renderer.RenderingOptions.MarginTop = 20;
renderer.RenderingOptions.MarginBottom = 20;
renderer.RenderingOptions.MarginLeft = 20;
renderer.RenderingOptions.MarginRight = 20;
renderer.RenderingOptions.FirstPageNumber = 1;

// CSSを直接HTMLに埋め込むか、CustomCssを使用
renderer.RenderingOptions.CustomCssUrl = "styles.css";

// またはCSSを直接注入
string htmlWithCss = $@"
<html>
<head>
    <style>body {{ font-family: Arial; }}</style>
</head>
<body>{htmlContent}</body>
</html>";

var pdf = renderer.RenderHtmlAsPdf(htmlWithCss);
```

### 例5：ヘッダーとフッター

**PDFreactor (CSS Paged Media):**
```csharp
using RealObjects.PDFreactor;

var config = new Configuration
{
    Document = htmlContent
};

config.AddUserStyleSheet(@"
    @page {
        @top-center {
            content: '会社報告書';
            font-size: 10pt;
        }
        @bottom-left {
            content: '機密';
        }
        @bottom-center {
            content: 'ページ ' counter(page) ' の ' counter(pages);
        }
        @bottom-right {
            content: string(date);
        }
    }
");

Result result = pdfReactor.Convert(config);
```

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

// テキストベースのヘッダー/フッター
renderer.RenderingOptions.TextHeader = new TextHeaderFooter
{
    CenterText = "会社報告書",
    FontSize = 10
};

renderer.RenderingOptions.TextFooter = new TextHeaderFooter
{
    LeftText = "機密",
    CenterText = "ページ {page} の {total-pages}",
    RightText = "{date}"
};

var pdf = renderer.RenderHtmlAsPdf(htmlContent);
pdf.SaveAs("report.pdf");

// より制御が必要な場合はHTMLヘッダー/フッターを使用
renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
{
    HtmlFragment = "<div