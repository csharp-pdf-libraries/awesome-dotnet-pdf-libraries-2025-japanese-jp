---
**  (Japanese Translation)**

 **English:** [playwright/migrate-from-playwright.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/playwright/migrate-from-playwright.md)
 **:** [playwright/migrate-from-playwright.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/playwright/migrate-from-playwright.md)

---
# Playwright for .NETからIronPDFへの移行方法は？

## PlaywrightからIronPDFへ移行する理由

Playwright for .NETは、PDF生成が二次的な機能であるブラウザの自動化およびテストフレームワークです。Playwrightを使用してPDFを生成すると、以下のような取り扱いが必要になります：

- **最初の使用前に必要な400MB以上のブラウザのダウンロード**
- **ブラウザのコンテキストとページ管理に関する複雑な非同期パターン**
- **文書生成に最適化されていないテストファーストアーキテクチャ**
- **Ctrl+Pブラウザ印刷に相当するPrint-to-PDFの制限**
- **アクセシビリティ準拠のためのPDF/AまたはPDF/UAサポートがない**
- **完全なブラウザインスタンスを必要とするリソース集約型の操作**

**IronPDFはPDF生成のために特別に設計されており**、よりシンプルなAPI、優れたパフォーマンス、ブラウザの自動化オーバーヘッドなしでプロフェッショナルなドキュメント機能を提供します。

---

## テストフレームワークの問題

Playwrightはエンドツーエンドのテスト用に設計されており、文書生成用ではありません。これはPDFを使用する際に根本的な問題を引き起こします：

| 項目 | Playwright | IronPDF |
|--------|------------|---------|
| **主な目的** | ブラウザテスト | PDF生成 |
| **ブラウザダウンロード** | 400MB+ (Chromium, Firefox, WebKit) | 組み込みの最適化エンジン |
| **APIの複雑さ** | 非同期ブラウザ/コンテキスト/ページライフサイクル | 同期的なワンライナー |
| **初期化** | `playwright install` + CreateAsync + LaunchAsync | `new ChromePdfRenderer()` |
| **メモリ使用量** | 変換ごとに280-420MB | 変換ごとに80-120MB |
| **コールドスタート** | 4.5秒 | 2.8秒 |
| **続くレンダリング** | 3.8-4.1秒 | 0.8-1.2秒 |
| **PDF/Aサポート** | ❌ 利用不可 | ✅ 完全サポート |
| **PDF/UAアクセシビリティ** | ❌ 利用不可 | ✅ 完全サポート |
| **デジタル署名** | ❌ 利用不可 | ✅ 完全サポート |
| **PDF編集** | ❌ 利用不可 | ✅ マージ、分割、スタンプ、編集 |
| **プロフェッショナルサポート** | コミュニティ | SLA付き商用 |

---

## NuGetパッケージの変更

```bash
# Playwrightを削除
dotnet remove package Microsoft.Playwright

# ブラウザバイナリを削除（約400MBのディスクスペースを回収）
# プロジェクトまたはユーザーディレクトリの.playwrightフォルダを削除

# IronPDFを追加
dotnet add package IronPdf
```

**IronPDFでは`playwright install`が不要** - レンダリングエンジンは自動的にバンドルされます。

---

## 名前空間のマッピング

| Playwright | IronPDF |
|------------|---------|
| `Microsoft.Playwright` | `IronPdf` |
| `IPlaywright` | 不要 |
| `IBrowser` | 不要 |
| `IBrowserContext` | 不要 |
| `IPage` | 不要 |
| `PagePdfOptions` | `ChromePdfRenderOptions` |
| `Margin` | `RenderingOptions.Margin*`プロパティ |

---

## APIマッピング

| Playwright API | IronPDF API | 備考 |
|----------------|-------------|-------|
| `Playwright.CreateAsync()` | `new ChromePdfRenderer()` | 非同期不要 |
| `playwright.Chromium.LaunchAsync()` | 不要 | ブラウザ管理不要 |
| `browser.NewPageAsync()` | 不要 | ページコンテキスト不要 |
| `browser.NewContextAsync()` | 不要 | コンテキスト管理不要 |
| `page.GotoAsync(url)` | `renderer.RenderUrlAsPdf(url)` | 直接URLレンダリング |
| `page.SetContentAsync(html)` + `page.PdfAsync()` | `renderer.RenderHtmlAsPdf(html)` | 単一メソッド |
| `page.PdfAsync(options)` | `renderer.RenderHtmlAsPdf()` | RenderingOptions経由のオプション |
| `page.CloseAsync()` | 不要 | 自動クリーンアップ |
| `browser.CloseAsync()` | 不要 | 自動クリーンアップ |
| `playwright.Dispose()` | 不要 | 自動クリーンアップ |
| `PagePdfOptions.Format` | `RenderingOptions.PaperSize` | 用紙サイズ |
| `PagePdfOptions.Margin` | `RenderingOptions.MarginTop/Bottom/Left/Right` | 個別のマージン |
| `PagePdfOptions.PrintBackground` | `RenderingOptions.PrintHtmlBackgrounds` | 背景印刷 |
| `PagePdfOptions.HeaderTemplate` | `RenderingOptions.HtmlHeader` | HTMLヘッダー |
| `PagePdfOptions.FooterTemplate` | `RenderingOptions.HtmlFooter` | HTMLフッター |
| `PagePdfOptions.Scale` | `RenderingOptions.Zoom` | ページズーム |
| `PagePdfOptions.Landscape` | `RenderingOptions.PaperOrientation` | 方向 |
| `page.SetViewportSizeAsync()` | `RenderingOptions.ViewPortWidth/Height` | ビューポート制御 |
| N/A | `pdf.Merge()` | PDFのマージ |
| N/A | `pdf.ApplyStamp()` | ウォーターマークの追加 |
| N/A | `pdf.SecuritySettings` | PDFの暗号化 |
| N/A | `pdf.SignWithFile()` | デジタル署名 |

---

## コード例

### 例1: 基本的なHTMLからPDFへ

**Before (Playwright) - ブラウザライフサイクルでの複雑な非同期:**
```csharp
using Microsoft.Playwright;

public class PlaywrightPdfGenerator
{
    public async Task GeneratePdfAsync()
    {
        // Playwrightの初期化（最初に'playwright install'が必要）
        using var playwright = await Playwright.CreateAsync();

        // ブラウザインスタンスの起動
        await using var browser = await playwright.Chromium.LaunchAsync();

        // ページコンテキストの作成
        var page = await browser.NewPageAsync();

        // HTMLコンテンツの設定
        await page.SetContentAsync("<h1>Hello World</h1>");

        // PDFの生成
        await page.PdfAsync(new PagePdfOptions { Path = "output.pdf" });

        // クリーンアップ（await usingがこれを処理するが、明示的なクリーンアップがよく必要）
        await page.CloseAsync();
        await browser.CloseAsync();
    }
}
```

**After (IronPDF) - シンプルな同期呼び出し:**
```csharp
using IronPdf;

public class IronPdfGenerator
{
    public void GeneratePdf()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderHtmlAsPdf("<h1>Hello World</h1>");
        pdf.SaveAs("output.pdf");
    }
}
```

### 例2: URLからPDFへ

**Before (Playwright):**
```csharp
using Microsoft.Playwright;

public async Task UrlToPdfAsync(string url)
{
    using var playwright = await Playwright.CreateAsync();
    await using var browser = await playwright.Chromium.LaunchAsync();
    var page = await browser.NewPageAsync();

    // URLへの移動
    await page.GotoAsync(url);

    // ネットワークアイドルの待機（一般的なパターン）
    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

    // PDFの生成
    await page.PdfAsync(new PagePdfOptions
    {
        Path = "output.pdf",
        Format = "A4"
    });

    await browser.CloseAsync();
}
```

**After (IronPDF):**
```csharp
using IronPdf;

public void UrlToPdf(string url)
{
    IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

    var renderer = new ChromePdfRenderer();
    renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;

    var pdf = renderer.RenderUrlAsPdf(url);
    pdf.SaveAs("output.pdf");
}
```

### 例3: カスタムページ設定とマージン

**Before (Playwright):**
```csharp
using Microsoft.Playwright;

public async Task CustomPdfAsync()
{
    using var playwright = await Playwright.CreateAsync();
    await using var browser = await playwright.Chromium.LaunchAsync();
    var page = await browser.NewPageAsync();

    // 一貫したレンダリングのためのビューポート設定
    await page.SetViewportSizeAsync(1920, 1080);

    await page.SetContentAsync("<h1>Custom Document</h1>");

    await page.PdfAsync(new PagePdfOptions
    {
        Path = "custom.pdf",
        Format = "Letter",
        Landscape = true,
        Margin = new Margin
        {
            Top = "1in",
            Bottom = "1in",
            Left = "0.75in",
            Right = "0.75in"
        },
        PrintBackground = true,
        Scale = 0.8f
    });

    await browser.CloseAsync();
}
```

**After (IronPDF):**
```csharp
using IronPdf;

public void CustomPdf()
{
    IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

    var renderer = new ChromePdfRenderer();

    // ビューポート制御
    renderer.RenderingOptions.ViewPortWidth = 1920;
    renderer.RenderingOptions.ViewPortHeight = 1080;

    // ページ設定
    renderer.RenderingOptions.PaperSize = PdfPaperSize.Letter;
    renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;

    // ミリメートル単位のマージン（25.4mm = 1インチ、19mm ≈ 0.75インチ）
    renderer.RenderingOptions.MarginTop = 25;
    renderer.RenderingOptions.MarginBottom = 25;
    renderer.RenderingOptions.MarginLeft = 19;
    renderer.RenderingOptions.MarginRight = 19;

    // 印刷オプション
    renderer.RenderingOptions.PrintHtmlBackgrounds = true;
    renderer.RenderingOptions.Zoom = 80; // パーセンテージ

    var pdf = renderer.RenderHtmlAsPdf("<h1>Custom Document</h1>");
    pdf.SaveAs("custom.pdf");
}
```

### 例4: ヘッダーとフッター

**Before (Playwright) - 限定的なテンプレート構文:**
```csharp
using Microsoft.Playwright;

public async Task HeaderFooterPdfAsync()
{
    using var playwright = await Playwright.CreateAsync();
    await using var browser = await playwright.Chromium.LaunchAsync();
    var page = await browser.NewPageAsync();

    await page.SetContentAsync("<h1>Document with Header/Footer</h1>");

    await page.PdfAsync(new PagePdfOptions
    {
        Path = "output.pdf",
        DisplayHeaderFooter = true,
        // 限定的なPlaywrightテンプレートクラス：date, title, url, pageNumber, totalPages
        HeaderTemplate = @"
            <div style='font-size:10px; width:100%; text-align:center;'>
                <span class='title'></span>
            </div>",
        FooterTemplate = @"
            <div style='font-size:10px; width:100%; text-align:center;'>
                Page <span class='pageNumber'></span> of <span class='totalPages'></span>
            </div>",
        Margin = new Margin { Top = "100px", Bottom = "80px" }
    });

    await browser.CloseAsync();
}
```

**After (IronPDF) - 完全なHTML/CSSサポート:**
```csharp
using IronPdf;

public void HeaderFooterPdf()
{
    IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

    var renderer = new ChromePdfRenderer();

    // スタイリング付きの完全なHTMLヘッダー
    renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
    {
        HtmlFragment = @"
            <div style='text-align: center; font-size: 12px; color: #333;'>
                <img src='logo.png' style='height: 30px;' />
                <span style='margin-left: 20px;'>{html-title}</span>
            </div>",
        DrawDividerLine = true
    };

    // 動的プレースホルダー付きの完全なHTMLフッター
    renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
    {
        HtmlFragment = @"
            <div style='text-align: center; font-size: 10px; color: #666;'>
                Page {page} of {total-pages} | Generated: {date} {time}
            </div>",
        DrawDividerLine = true
    };

    renderer.RenderingOptions.MarginTop = 35;
    renderer.RenderingOptions.MarginBottom = 25;

    var pdf = renderer.RenderHtmlAsPdf("<h1>Document with Header/Footer</h1>");
    pdf.SaveAs("output.pdf");
}
```

**IronPDFヘッダー/フッタープレースホルダー:**
- `{page}` - 現在のページ番号
- `{total-pages}` - 総ページ数
- `{date}` - 現在の日付
- `{time}` - 現在の時間
- `{html-title}` - HTMLのドキュメントタイトル
- `{url}` - ソースURL

### 例5: 動的コンテンツの待機

**Before (Playwright) - 複雑な待機戦略:**
```csharp
using Microsoft.Playwright;

public async Task WaitForContentAsync()
{
    using var playwright = await Playwright.CreateAsync();
    await using var browser = await playwright.Chromium.LaunchAsync();
    var page = await browser.NewPageAsync();

    await page.GotoAsync("https://example.com/dynamic");

    // 複数の待機戦略がしばしば必要
    await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

    // 特定の要素の待機
    await page.WaitForSelectorAsync("#content-loaded");

    // アニメーションのための追加遅延
    await Task.Delay(1000);

    await page.PdfAsync(new PagePdfOptions { Path = "output.pdf" });
    await browser.CloseAsync();
}
```

**After (IronPDF) - シンプルなレンダリング遅延:**
```csharp
using IronPdf;

public void WaitForContent()
{
    IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

    var renderer = new ChromePdfRenderer();

    // JavaScript/AJAXコンテンツの待機
    renderer.RenderingOptions.WaitFor.RenderDelay = 2000; // ミリ秒

    // 特定のJavaScript条件の待機
    renderer.RenderingOptions.WaitFor.JavaScript = "window.contentLoaded === true";

    // HTML要素の待機
    renderer.RenderingOptions.WaitFor.HtmlElementId = "content-loaded";

    var pdf = renderer.RenderUrlAsPdf("https://example.com/dynamic");
    pdf.SaveAs("output.pdf");
}
```

### 例6: JavaScriptの実行

**Before (Playwright):**
```csharp
using Microsoft.Playwright;

public async Task ExecuteJavaScriptAsync()
{
    using var playwright = await Playwright.CreateAsync();
    await using var browser = await playwright.Chromium.LaunchAsync();
    var page = await browser.NewPageAsync();

    await page.SetContentAsync("<div id='target'>Original