---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [rotativa/migrate-from-rotativa.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/rotativa/migrate-from-rotativa.md)
🇯🇵 **日本語:** [rotativa/migrate-from-rotativa.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/rotativa/migrate-from-rotativa.md)

---

# RotativaからIronPDFへの移行方法は？

## ⚠️ 重要なセキュリティアドバイザリ

**Rotativaはwkhtmltopdfをラップしており、CRITICAL UNPATCHED SECURITY VULNERABILITIESが存在します。**

### CVE-2022-35583 — サーバーサイドリクエストフォージェリ（SSRF）

| 属性 | 値 |
|-----------|-------|
| **CVE ID** | CVE-2022-35583 |
| **重大度** | **CRITICAL (9.8/10)** |
| **攻撃ベクトル** | ネットワーク |
| **ステータス** | **修正されることはありません** |
| **影響を受ける** | すべてのRotativaバージョン |

**wkhtmltopdfは2022年12月に正式に放棄されました。** メンテナはセキュリティの脆弱性を修正しないと明言しています。Rotativaを使用しているすべてのアプリケーションは永久に露出します。

### 攻撃の仕組み

```html
<!-- 攻撃者がMVCモデル経由でこのコンテンツを送信 -->
<iframe src="http://169.254.169.254/latest/meta-data/iam/security-credentials/"></iframe>
<img src="http://internal-database:5432/admin" />
```

**影響：**
- AWS/Azure/GCPクラウドメタデータエンドポイントへのアクセス
- 内部APIデータと認証情報の盗難
- 内部ネットワークのポートスキャン
- 機密設定の流出

---

## 目次
1. [なぜ今すぐ移行するのか](#なぜ今すぐ移行するのか)
2. [Rotativaの重大な問題](#rotativaの重大な問題)
3. [クイックスタート移行（5分）](#クイックスタート移行5分)
4. [完全なAPIリファレンス](#完全なapiリファレンス)
5. [コード移行例](#コード移行例)
6. [ASP.NET Coreへの移行](#aspnet-coreへの移行)
7. [Rotativaにはない機能](#rotativaにはない機能)
8. [デプロイメントの比較](#デプロイメントの比較)
9. [トラブルシューティングガイド](#トラブルシューティングガイド)
10. [移行チェックリスト](#移行チェックリスト)
11. [追加リソース](#追加リソース)

---

## なぜ今すぐ移行するのか

### セキュリティ危機

| リスク | Rotativa | IronPDF |
|------|----------|---------|
| **CVE-2022-35583 (SSRF)** | ❌ 脆弱 | ✅ 保護されている |
| **ローカルファイルアクセス** | ❌ 脆弱 | ✅ サンドボックス化 |
| **内部ネットワークアクセス** | ❌ 脆弱 | ✅ 制限されている |
| **セキュリティパッチ** | ❌ なし | ✅ 定期的な更新 |
| **アクティブな開発** | ❌ 放棄された | ✅ 月次リリース |

### テクノロジー危機

Rotativaはwkhtmltopdfをラップしており、以下を使用しています：
- **Qt WebKit 4.8** (2012年から)
- **Flexboxサポートなし**
- **CSS Gridサポートなし**
- **JavaScript実行が壊れている**
- **ES6+サポートなし**

### 機能比較

| 機能 | Rotativa | IronPDF |
|---------|----------|---------|
| **セキュリティ** | ❌ 重大なCVE | ✅ 脆弱性なし |
| **HTMLレンダリング** | ⚠️ 古いWebKit | ✅ モダンなChromium |
| **CSS3** | ❌ 部分的 | ✅ 完全サポート |
| **Flexbox/Grid** | ❌ サポートされていない | ✅ 完全サポート |
| **JavaScript** | ⚠️ 信頼性が低い | ✅ 完全なES6+ |
| **ASP.NET Core** | ⚠️ 限定的なポート | ✅ ネイティブサポート |
| **Razorページ** | ❌ サポートされていない | ✅ 完全サポート |
| **Blazor** | ❌ サポートされていない | ✅ 完全サポート |
| **PDF操作** | ❌ 利用不可 | ✅ 完全 |
| **デジタル署名** | ❌ 利用不可 | ✅ 完全 |
| **PDF/A準拠** | ❌ 利用不可 | ✅ 完全 |
| **Async/Await** | ❌ 同期のみ | ✅ 完全な非同期 |
| **アクティブメンテナンス** | ❌ 放棄された | ✅ 週次更新 |

---

## Rotativaの重大な問題

### 問題1：MVCのみのアーキテクチャ

RotativaはASP.NET MVC 5およびそれ以前用に設計されていました：

```csharp
// ❌ Rotativa - 古典的なMVCパターンでのみ動作
public class InvoiceController : Controller
{
    public ActionResult InvoicePdf(int id)
    {
        var model = GetInvoice(id);
        return new ViewAsPdf("Invoice", model);  // MVCビューに結びつけられている
    }
}

// 問題点：
// - Razorページサポートなし
// - Blazorサポートなし
// - 最小限のAPIサポートなし
// - ASP.NET Coreネイティブ統合なし
```

### 問題2：wkhtmltopdfバイナリの管理

```csharp
// ❌ Rotativa - 手動でのバイナリ管理が必要
// プロジェクトにwkhtmltopdf.exeを含める必要がある
// x86/x64/Linux/Macの異なるバイナリ
// すべてのバージョンにセキュリティ脆弱性が存在

// デプロイメントの頭痛の種：
// 1. wkhtmltopdf.exeをデプロイメントにコピー
// 2. 正しい権限を設定
// 3. 異なるプラットフォームを処理
// 4. PATH環境変数を更新
```

### 問題3：同期のみ

```csharp
// ❌ Rotativa - スレッドをブロックする
public ActionResult GeneratePdf()
{
    return new ViewAsPdf("Report");
    // これはリクエストスレッドをPDFが完成するまでブロックする
    // 負荷下でのスケーラビリティが低い
}

// ✅ IronPDF - 完全な非同期サポート
public async Task<IActionResult> GeneratePdf()
{
    var renderer = new ChromePdfRenderer();
    var pdf = await renderer.RenderHtmlAsPdfAsync(html);
    return File(pdf.BinaryData, "application/pdf");
    // ブロックせず、より良いスケーラビリティ
}
```

---

## クイックスタート移行（5分）

### ステップ1：NuGetパッケージを更新

```bash
# Rotativaを削除
dotnet remove package Rotativa
dotnet remove package Rotativa.AspNetCore

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ2：wkhtmltopdfバイナリを削除

プロジェクトからこれらのファイルを削除します：
- `wkhtmltopdf.exe`
- `wkhtmltox.dll`
- 任意の`Rotativa/`フォルダ

### ステップ3：Usingステートメントを更新

```csharp
// 以前
using Rotativa;
using Rotativa.Options;

// 以降
using IronPdf;
```

### ステップ4：ライセンスキーを追加

```csharp
// Program.csまたはStartup.csに追加
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

### ステップ5：コントローラーコードを更新

**以前 (Rotativa):**
```csharp
using Rotativa;

public class ReportController : Controller
{
    public ActionResult DownloadReport(int id)
    {
        var model = GetReportData(id);
        return new ViewAsPdf("Report", model)
        {
            FileName = "Report.pdf",
            PageOrientation = Orientation.Portrait,
            PageSize = Size.A4
        };
    }
}
```

**以降 (IronPDF):**
```csharp
using IronPdf;

public class ReportController : Controller
{
    public async Task<IActionResult> DownloadReport(int id)
    {
        var model = GetReportData(id);

        // まずビューをHTMLにレンダリング
        var html = await RenderViewToStringAsync("Report", model);

        var renderer = new ChromePdfRenderer();
        renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Portrait;
        renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;

        var pdf = await renderer.RenderHtmlAsPdfAsync(html);
        return File(pdf.BinaryData, "application/pdf", "Report.pdf");
    }
}
```

---

## 完全なAPIリファレンス

### 名前空間マッピング

| Rotativa 名前空間 | IronPDF 名前空間 |
|-------------------|-------------------|
| `Rotativa` | `IronPdf` |
| `Rotativa.Options` | `IronPdf.Rendering` |
| `Rotativa.AspNetCore` | `IronPdf` |

### コアクラスマッピング

| Rotativa クラス | IronPDF 同等 | メモ |
|----------------|-------------------|-------|
| `ViewAsPdf` | `ChromePdfRenderer` | HTMLをレンダリング |
| `ActionAsPdf` | `ChromePdfRenderer.RenderUrlAsPdf()` | URLをレンダリング |
| `UrlAsPdf` | `ChromePdfRenderer.RenderUrlAsPdf()` | URLをレンダリング |
| `Orientation` enum | `PdfPaperOrientation` enum | 方向 |
| `Size` enum | `PdfPaperSize` enum | 用紙サイズ |

### プロパティマッピング

| Rotativa プロパティ | IronPDF プロパティ |
|-------------------|------------------|
| `FileName` | `File()`メソッドの名前パラメータを使用 |
| `PageOrientation` | `RenderingOptions.PaperOrientation` |
| `PageSize` | `RenderingOptions.PaperSize` |
| `PageWidth` | `SetCustomPaperSizeInMillimeters()` |
| `PageHeight` | `SetCustomPaperSizeInMillimeters()` |
| `PageMargins.Top` | `RenderingOptions.MarginTop` |
| `PageMargins.Bottom` | `RenderingOptions.MarginBottom` |
| `PageMargins.Left` | `RenderingOptions.MarginLeft` |
| `PageMargins.Right` | `RenderingOptions.MarginRight` |
| `CustomSwitches` | 様々な`RenderingOptions`プロパティ |

### CustomSwitches移行

| Rotativa CustomSwitch | IronPDF 同等 |
|----------------------|-------------------|
| `--page-offset 0` | `RenderingOptions.FirstPageNumber = 1` |
| `--footer-center [page]` | `{page}`を含む`HtmlFooter` |
| `--footer-font-size 8` | フッターHTMLのCSS |
| `--header-html ...` | `RenderingOptions.HtmlHeader` |
| `--footer-html ...` | `RenderingOptions.HtmlFooter` |
| `--javascript-delay 500` | `WaitFor.JavaScript(500)` |
| `--no-background` | `PrintHtmlBackgrounds = false` |
| `--print-media-type` | `CssMediaType = Print` |
| `--disable-smart-shrinking` | `FitToPaperMode`設定 |

---

## コード移行例

### 例1：基本的なビューからPDFへ

**以前 (Rotativa):**
```csharp
using Rotativa;

public class InvoiceController : Controller
{
    public ActionResult InvoicePdf(int id)
    {
        var invoice = _invoiceService.GetInvoice(id);
        return new ViewAsPdf("Invoice", invoice)
        {
            FileName = "Invoice.pdf"
        };
    }
}
```

**以降 (IronPDF):**
```csharp
using IronPdf;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;

public class InvoiceController : Controller
{
    private readonly IViewRenderService _viewRenderService;

    public InvoiceController(IViewRenderService viewRenderService)
    {
        _viewRenderService = viewRenderService;
    }

    public async Task<IActionResult> InvoicePdf(int id)
    {
        var invoice = _invoiceService.GetInvoice(id);

        // RazorビューをHTML文字列にレンダリング
        var html = await _viewRenderService.RenderToStringAsync("Invoice", invoice);

        var renderer = new ChromePdfRenderer();
        var pdf = await renderer.RenderHtmlAsPdfAsync(html);

        return File(pdf.BinaryData, "application/pdf", "Invoice.pdf");
    }
}
```

### 例2：URLからPDFへ

**以前 (Rotativa):**
```csharp
using Rotativa;

public ActionResult DownloadWebPage()
{
    return new UrlAsPdf("https://example.com/report")
    {
        FileName = "Report.pdf",
        PageSize = Size.Letter
    };
}
```

**以降 (IronPDF):**
```csharp
using IronPdf;

public async Task<IActionResult> DownloadWebPage()
{
    var renderer = new ChromePdfRenderer();
    renderer.RenderingOptions.PaperSize = PdfPaperSize.Letter;

    var pdf = await renderer.RenderUrlAsPdfAsync("https://example.com/report");
    return File(pdf.BinaryData, "application/pdf", "Report.pdf");
}
```

### 例3：カスタムページサイズとマージン

**以前 (Rotativa):**
```csharp
using Rotativa;
using Rotativa.Options;

public ActionResult CustomPdf()
{
    var model = GetData();
    return new ViewAsPdf("Report", model)
    {
        FileName = "Report.pdf",
        PageOrientation = Orientation.Landscape,
        PageSize = Size.A4,
        PageMargins = new Margins(20, 15, 20, 15)
    };
}
```

**以降 (IronPDF):**
```csharp
using IronPdf;

public async Task<IActionResult> CustomPdf()
{
    var model = GetData();
    var html = await RenderViewToStringAsync("Report", model);

    var renderer = new ChromePdfRenderer();
    renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;
    renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
    renderer.RenderingOptions.MarginTop = 20;
    renderer.RenderingOptions.MarginBottom = 20;
    renderer.RenderingOptions.MarginLeft = 15;
    renderer.RenderingOptions.MarginRight = 15;

    var pdf = await renderer.RenderHtmlAsPdfAsync(html);
    return File(pdf.BinaryData, "application/pdf", "Report.pdf");
}
```

### 例4：ページ番号付きのヘッダーとフッター

**以前 (Rotativa):**
```csharp
using Rotativa;

public ActionResult ReportWithHeaders()
{
    return new ViewAsPdf("Report", model)
    {
        FileName = "Report.pdf",
        CustomSwitches = @"
            --header-html ""C:\Templates\header.html""
            --footer-center ""Page [page] of [topage]""
            --footer-font-size 9
            --header-spacing 5
            --footer-spacing 5"
    };
}
```

**以降 (IronPDF):**
```csharp
using IronPdf;

public async Task<IActionResult> ReportWithHeaders()
{
    var html = await RenderViewToStringAsync("Report", model);

    var renderer = new Chrome