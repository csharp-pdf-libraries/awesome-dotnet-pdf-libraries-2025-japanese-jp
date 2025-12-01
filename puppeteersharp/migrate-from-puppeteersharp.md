---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [puppeteersharp/migrate-from-puppeteersharp.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/puppeteersharp/migrate-from-puppeteersharp.md)
🇯🇵 **日本語:** [puppeteersharp/migrate-from-puppeteersharp.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/puppeteersharp/migrate-from-puppeteersharp.md)

---

# PuppeteerSharpからIronPDFへの移行方法は？

## PuppeteerSharpからIronPDFへ移行する理由

PuppeteerSharpは、GoogleのPuppeteerの.NETポートであり、ブラウザの自動化を目的としていますが、PDF生成は二次的な機能です。PuppeteerSharpをPDF生成に使用することは可能ですが、以下のような生産上の課題を生み出します：

- **初回使用前に300MB以上のChromiumダウンロードが必要**
- **負荷下でのメモリリークが発生し、手動でのブラウザリサイクルが必要**
- **ブラウザライフサイクル管理に複雑な非同期パターンが必要**
- **Print-to-PDF出力**（Ctrl+Pに相当、スクリーンキャプチャではない）
- **PDF/AやPDF/UAのサポートがない**（コンプライアンス要件に対応できない）
- **PDF操作ができない** - 生成のみ、マージ/分割/編集不可

**IronPDFはPDF生成のために特化して開発されており**、より軽量なフットプリント、自動メモリ管理、ブラウザ自動化のオーバーヘッドなしで包括的なPDF操作を提供します。

---

## ブラウザ自動化の問題

PuppeteerSharpは、Webテストやスクレイピングのために設計されており、ドキュメント生成には適していません。これにより、PDFを使用する際に根本的な問題が生じます：

| 項目 | PuppeteerSharp | IronPDF |
|------|----------------|---------|
| **主な目的** | ブラウザ自動化 | PDF生成 |
| **Chromium依存性** | 300MB以上の別ダウンロード | 組み込み最適化エンジン |
| **APIの複雑さ** | 非同期ブラウザ/ページライフサイクル | 同期的な一行コマンド |
| **初期化** | `BrowserFetcher.DownloadAsync()` + LaunchAsync | `new ChromePdfRenderer()` |
| **メモリ管理** | 手動でのブラウザリサイクルが必要 | 自動 |
| **負荷下のメモリ** | 500MB以上でリーク | 約50MBで安定 |
| **コールドスタート** | 45秒以上 | 約20秒 |
| **PDF/Aサポート** | ❌ 利用不可 | ✅ 完全サポート |
| **PDF/UAアクセシビリティ** | ❌ 利用不可 | ✅ 完全サポート |
| **PDF編集** | ❌ 利用不可 | ✅ マージ、分割、スタンプ、編集 |
| **デジタル署名** | ❌ 利用不可 | ✅ 完全サポート |
| **プロフェッショナルサポート** | コミュニティ | 商用SLA付き |

---

## メモリリークの問題

PuppeteerSharpは、持続的な負荷下でのメモリ蓄積で悪名高いです：

```csharp
// ❌ PuppeteerSharp - 操作ごとにメモリが増加
// N操作ごとに明示的なブラウザリサイクルが必要
for (int i = 0; i < 1000; i++)
{
    var page = await browser.NewPageAsync();
    await page.SetContentAsync($"<h1>Document {i}</h1>");
    await page.PdfAsync($"doc_{i}.pdf");
    await page.CloseAsync(); // メモリはまだ蓄積される！
}
// 定期的に: await browser.CloseAsync(); そして再起動が必要

// ✅ IronPDF - 安定したメモリ、レンダラーの再利用
var renderer = new ChromePdfRenderer();
for (int i = 0; i < 1000; i++)
{
    var pdf = renderer.RenderHtmlAsPdf($"<h1>Document {i}</h1>");
    pdf.SaveAs($"doc_{i}.pdf");
    // メモリは自動的に管理される
}
```

---

## NuGetパッケージの変更

```bash
# PuppeteerSharpを削除
dotnet remove package PuppeteerSharp

# ダウンロードしたChromiumバイナリを削除（約300MBが回復）
# .local-chromiumフォルダを削除

# IronPDFを追加
dotnet add package IronPdf
```

**IronPDFでは`BrowserFetcher.DownloadAsync()`が不要** - レンダリングエンジンは自動的にバンドルされます。

---

## 名前空間マッピング

| PuppeteerSharp | IronPDF |
|----------------|---------|
| `PuppeteerSharp` | `IronPdf` |
| `PuppeteerSharp.Media` | `IronPdf` |
| `BrowserFetcher` | 不要 |
| `LaunchOptions` | 不要 |
| `PdfOptions` | `ChromePdfRenderOptions` |
| `MarginOptions` | `RenderingOptions.Margin*` |
| N/A | `IronPdf.Editing` |
| N/A | `IronPdf.Signing` |

---

## APIマッピング

| PuppeteerSharp API | IronPDF API | 備考 |
|--------------------|-------------|-------|
| `new BrowserFetcher().DownloadAsync()` | 不要 | ブラウザのダウンロード不要 |
| `Puppeteer.LaunchAsync(options)` | 不要 | ブラウザ管理不要 |
| `browser.NewPageAsync()` | 不要 | ページコンテキスト不要 |
| `page.GoToAsync(url)` | `renderer.RenderUrlAsPdf(url)` | 直接レンダリング |
| `page.SetContentAsync(html)` | `renderer.RenderHtmlAsPdf(html)` | 直接レンダリング |
| `page.PdfAsync(path)` | `pdf.SaveAs(path)` | レンダリング後 |
| `page.PdfAsync(options)` | `renderer.RenderHtmlAsPdf()` | RenderingOptions経由でオプション |
| `await page.CloseAsync()` | 不要 | 自動クリーンアップ |
| `await browser.CloseAsync()` | 不要 | 自動クリーンアップ |
| `PdfOptions.Format` | `RenderingOptions.PaperSize` | 用紙サイズ |
| `PdfOptions.Landscape` | `RenderingOptions.PaperOrientation` | 方向 |
| `PdfOptions.MarginOptions` | `RenderingOptions.MarginTop/Bottom/Left/Right` | 個別のマージン |
| `PdfOptions.PrintBackground` | `RenderingOptions.PrintHtmlBackgrounds` | 背景印刷 |
| `PdfOptions.HeaderTemplate` | `RenderingOptions.HtmlHeader` | HTMLヘッダー |
| `PdfOptions.FooterTemplate` | `RenderingOptions.HtmlFooter` | HTMLフッター |
| `PdfOptions.Scale` | `RenderingOptions.Zoom` | ページズーム |
| `page.SetViewportAsync()` | `RenderingOptions.ViewPortWidth/Height` | ビューポート制御 |
| `page.WaitForSelectorAsync()` | `RenderingOptions.WaitFor.HtmlElementId` | 要素待ち |
| `page.WaitForNetworkIdleAsync()` | 自動 | 組み込みの賢い待機 |
| N/A | `PdfDocument.Merge()` | PDFのマージ |
| N/A | `pdf.ApplyStamp()` | ウォーターマークの追加 |
| N/A | `pdf.SecuritySettings` | PDFの暗号化 |
| N/A | `pdf.Sign()` | デジタル署名 |

---

## コード例

### 例1: 基本的なHTMLからPDFへ

**変更前（PuppeteerSharp）- ブラウザライフサイクルでの複雑な非同期:**
```csharp
using PuppeteerSharp;

public class PuppeteerPdfGenerator
{
    public async Task GeneratePdfAsync()
    {
        // Chromiumをダウンロード（約300MB）していない場合はダウンロード
        var browserFetcher = new BrowserFetcher();
        await browserFetcher.DownloadAsync();

        // ブラウザインスタンスを起動
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true
        });

        // ページコンテキストを作成
        await using var page = await browser.NewPageAsync();

        // HTMLコンテンツを設定
        await page.SetContentAsync("<h1>Hello World</h1>");

        // PDFを生成
        await page.PdfAsync("output.pdf");

        // await usingによるクリーンアップが行われるが、メモリはまだ蓄積される可能性がある
    }
}
```

**変更後（IronPDF）- 単純な同期呼び出し:**
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

### 例2: オプション付きのURLからPDFへ

**変更前（PuppeteerSharp）:**
```csharp
using PuppeteerSharp;
using PuppeteerSharp.Media;

public async Task UrlToPdfAsync(string url)
{
    var browserFetcher = new BrowserFetcher();
    await browserFetcher.DownloadAsync();

    await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
    {
        Headless = true
    });

    await using var page = await browser.NewPageAsync();

    // 待機戦略でナビゲート
    await page.GoToAsync(url, new NavigationOptions
    {
        WaitUntil = new[] { WaitUntilNavigation.NetworkIdle2 }
    });

    // オプション付きでPDFを生成
    await page.PdfAsync("output.pdf", new PdfOptions
    {
        Format = PaperFormat.A4,
        PrintBackground = true,
        MarginOptions = new MarginOptions
        {
            Top = "20mm",
            Bottom = "20mm",
            Left = "15mm",
            Right = "15mm"
        }
    });
}
```

**変更後（IronPDF）:**
```csharp
using IronPdf;

public void UrlToPdf(string url)
{
    IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

    var renderer = new ChromePdfRenderer();

    renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
    renderer.RenderingOptions.PrintHtmlBackgrounds = true;
    renderer.RenderingOptions.MarginTop = 20;
    renderer.RenderingOptions.MarginBottom = 20;
    renderer.RenderingOptions.MarginLeft = 15;
    renderer.RenderingOptions.MarginRight = 15;

    var pdf = renderer.RenderUrlAsPdf(url);
    pdf.SaveAs("output.pdf");
}
```

### 例3: カスタムページサイズと向き

**変更前（PuppeteerSharp）:**
```csharp
using PuppeteerSharp;
using PuppeteerSharp.Media;

public async Task CustomPagePdfAsync()
{
    var browserFetcher = new BrowserFetcher();
    await browserFetcher.DownloadAsync();

    await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
    {
        Headless = true
    });

    await using var page = await browser.NewPageAsync();

    // 一貫したレンダリングのためのビューポートを設定
    await page.SetViewportAsync(new ViewPortOptions
    {
        Width = 1920,
        Height = 1080
    });

    await page.SetContentAsync("<h1>Landscape Document</h1>");

    await page.PdfAsync("custom.pdf", new PdfOptions
    {
        Format = PaperFormat.Letter,
        Landscape = true,
        Scale = 0.8m,
        PrintBackground = true
    });
}
```

**変更後（IronPDF）:**
```csharp
using IronPdf;

public void CustomPagePdf()
{
    IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

    var renderer = new ChromePdfRenderer();

    // ビューポート制御
    renderer.RenderingOptions.ViewPortWidth = 1920;
    renderer.RenderingOptions.ViewPortHeight = 1080;

    // ページ設定
    renderer.RenderingOptions.PaperSize = PdfPaperSize.Letter;
    renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;
    renderer.RenderingOptions.Zoom = 80; // パーセンテージ（Scale 0.8と同じ）
    renderer.RenderingOptions.PrintHtmlBackgrounds = true;

    var pdf = renderer.RenderHtmlAsPdf("<h1>Landscape Document</h1>");
    pdf.SaveAs("custom.pdf");
}
```

### 例4: ヘッダーとフッター

**変更前（PuppeteerSharp）- 限定的なテンプレート構文:**
```csharp
using PuppeteerSharp;
using PuppeteerSharp.Media;

public async Task HeaderFooterPdfAsync()
{
    var browserFetcher = new BrowserFetcher();
    await browserFetcher.DownloadAsync();

    await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
    {
        Headless = true
    });

    await using var page = await browser.NewPageAsync();
    await page.SetContentAsync("<h1>Document with Header/Footer</h1>");

    await page.PdfAsync("output.pdf", new PdfOptions
    {
        DisplayHeaderFooter = true,
        // Puppeteerの限定的なテンプレートクラス
        HeaderTemplate = @"
            <div style='font-size:10px; width:100%; text-align:center;'>
                <span class='title'></span>
            </div>",
        FooterTemplate = @"
            <div style='font-size:10px; width:100%; text-align:center;'>
                Page <span class='pageNumber'></span> of <span class='totalPages'></span>
            </div>",
        MarginOptions = new MarginOptions
        {
            Top = "100px",
            Bottom = "80px"
        }
    });
}
```

**変更後（IronPDF）- 完全なHTML/CSSサポート:**
```csharp
using IronPdf;

public void HeaderFooterPdf()
{
    IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

    var renderer = new ChromePdfRenderer();

    // 画像やスタイリングを含む完全なHTMLヘッダー
    renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
    {
        HtmlFragment = @"
            <div style='text-align: center; font-size: 12px; color: #333;'>
                <img src='logo.png' style='height: 30px;' />
                <span style='margin-left: 20px;'>{html-title}</span>
            </div>",
        DrawDividerLine = true
    };

    // 動的プレースホルダーを含む完全なHTMLフッター
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

**IronPDFのヘッダー/フッタープレースホルダー:**
- `{page}` - 現在のページ番号
- `{total-pages}` - 総ページ数
- `{date}` - 現在の日付
- `{time}` - 現在の時刻
- `{html-title}` - HTMLのドキュメントタイトル
- `{url}` - ソースURL

### 例5: 動的コンテンツの待機

**変更前（PuppeteerSharp）- 複雑な待機戦略:**
```csharp
using PuppeteerSharp;

public async Task WaitForContentAsync()
{
    var browserFetcher = new BrowserFetcher();
    await browserFetcher.DownloadAsync();

    await using var