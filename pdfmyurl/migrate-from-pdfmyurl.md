---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [pdfmyurl/migrate-from-pdfmyurl.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/pdfmyurl/migrate-from-pdfmyurl.md)
🇯🇵 **日本語:** [pdfmyurl/migrate-from-pdfmyurl.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/pdfmyurl/migrate-from-pdfmyurl.md)

---

# PDFmyURLからIronPDFへの移行方法は？

## なぜPDFmyURLから移行するのか？

PDFmyURLは、ドキュメントを外部サーバーに送信して処理するクラウドベースのAPIサービスです。クイックインテグレーションには便利ですが、このアーキテクチャは本番アプリケーションに対して重大な懸念を生み出します：

### クラウド処理の問題

1. **プライバシー & データセキュリティ**: 変換するすべてのドキュメントがPDFmyURLのサーバーを経由して移動します。機密契約、財務報告、個人データなどが外部で処理されます。
2. **継続的なサブスクリプションコスト**: 月額$39から始まり、所有権なしで年間コストが$468を超えます。
3. **インターネット依存**: すべての変換にはネットワーク接続が必要です。オフライン機能はありません。
4. **レート制限 & スロットリング**: ピーク使用時にAPI呼び出しがスロットルされる可能性があります。
5. **レイテンシ**: ネットワークの往復がすべての変換に数秒を追加します。
6. **サービスの可用性**: アプリケーションは第三者のサービスがオンラインであることに依存しています。
7. **ベンダーロックイン**: APIの変更により、通知なしに統合が壊れる可能性があります。

### IronPDFの利点

IronPDFは、アプリケーション内でローカルにすべてを処理します：

- **完全なプライバシー**: ドキュメントはサーバーを離れません。
- **一回限りのコスト**: 永続ライセンスオプションで繰り返しの料金がなくなります。
- **オフライン対応**: 初期設定後はインターネットなしで動作します。
- **レート制限なし**: 無制限にドキュメントを処理します。
- **低レイテンシ**: ネットワークのオーバーヘッドがありません。
- **完全なコントロール**: 処理環境を自分で制御します。
- **モダンなエンジン**: Chromiumベースのレンダリングで完全なCSS3/JSサポート。

---

## 移行の概要

| 項目 | PDFmyURL | IronPDF |
|--------|----------|---------|
| 処理場所 | 外部サーバー | ローカル (あなたのサーバー) |
| 認証 | リクエストごとのAPIキー | 一回限りのライセンスキー |
| ネットワーク必要 | すべての変換に必要 | 初期設定時のみ |
| 価格モデル | 月額サブスクリプション ($39+) | 永続ライセンス利用可能 |
| レート制限 | あり (プランに依存) | なし |
| データプライバシー | データが外部に送信される | データはローカルに留まる |
| HTML/CSS/JSサポート | W3C準拠 | 完全なChromiumエンジン |
| 非同期パターン | 必須 (非同期のみ) | 同期および非同期オプション |
| PDF操作 | 限定的 | フルスイート (マージ、分割、編集) |

---

## NuGetパッケージの変更

```bash
# PDFmyURLパッケージを削除
dotnet remove package PdfMyUrl
dotnet remove package Pdfcrowd

# IronPDFをインストール
dotnet add package IronPdf
```

---

## 名前空間の変更

```csharp
// 以前: PDFmyURL
using PdfMyUrl;
using Pdfcrowd;

// 以降: IronPDF
using IronPdf;
using IronPdf.Rendering;
```

---

## APIマッピングリファレンス

### コアクラス

| PDFmyURL | IronPDF | 備考 |
|----------|---------|-------|
| `PdfMyUrlClient` | `ChromePdfRenderer` | 主要な変換クラス |
| `HtmlToPdfClient` | `ChromePdfRenderer` | Pdfcrowd SDKと同等 |
| `ConvertOptions` | `ChromePdfRenderOptions` | レンダリング設定 |
| APIレスポンスオブジェクト | `PdfDocument` | 結果のPDFオブジェクト |

### メソッド

| PDFmyURL | IronPDF | 備考 |
|----------|---------|-------|
| `client.ConvertUrl(url)` | `renderer.RenderUrlAsPdf(url)` | URLからPDFへ |
| `client.ConvertUrlAsync(url)` | `await Task.Run(() => renderer.RenderUrlAsPdf(url))` | 非同期URL変換 |
| `client.ConvertHtml(html)` | `renderer.RenderHtmlAsPdf(html)` | HTML文字列からPDFへ |
| `client.ConvertHtmlAsync(html)` | `await Task.Run(() => renderer.RenderHtmlAsPdf(html))` | 非同期HTML変換 |
| `client.ConvertHtmlFile(path)` | `renderer.RenderHtmlFileAsPdf(path)` | HTMLファイルからPDFへ |
| `convertUrlToFile(url, file)` | `renderer.RenderUrlAsPdf(url).SaveAs(file)` | Pdfcrowdスタイル |
| `convertStringToFile(html, file)` | `renderer.RenderHtmlAsPdf(html).SaveAs(file)` | Pdfcrowdスタイル |
| `convertFileToFile(input, output)` | `renderer.RenderHtmlFileAsPdf(input).SaveAs(output)` | ファイルからファイルへ |
| `response.Save(filename)` | `pdf.SaveAs(filename)` | ディスクに保存 |
| `response.GetBytes()` | `pdf.BinaryData` | 生のバイトを取得 |
| `response.GetStream()` | `pdf.Stream` | ストリームとして取得 |

### 設定オプション

| PDFmyURL (setXxxメソッド) | IronPDF (RenderingOptions) | 備考 |
|---------------------------|---------------------------|-------|
| `setPageSize("A4")` | `.PaperSize = PdfPaperSize.A4` | 用紙サイズ |
| `setPageSize("Letter")` | `.PaperSize = PdfPaperSize.Letter` | USレター |
| `setOrientation("landscape")` | `.PaperOrientation = PdfPaperOrientation.Landscape` | 横向き |
| `setOrientation("portrait")` | `.PaperOrientation = PdfPaperOrientation.Portrait` | 縦向き |
| `setMarginTop("10mm")` | `.MarginTop = 10` | 上の余白 (mm) |
| `setMarginBottom("10mm")` | `.MarginBottom = 10` | 下の余白 (mm) |
| `setMarginLeft("10mm")` | `.MarginLeft = 10` | 左の余白 (mm) |
| `setMarginRight("10mm")` | `.MarginRight = 10` | 右の余白 (mm) |
| `setNoMargins(true)` | `.MarginTop/Bottom/Left/Right = 0` | 余白なし |
| `setPageWidth("8.5in")` | `.SetCustomPaperSizeInInches(8.5, 11)` | カスタム幅 |
| `setPageHeight("11in")` | `.SetCustomPaperSizeInInches(8.5, 11)` | カスタム高さ |
| `setHeaderHtml(html)` | `.HtmlHeader = new HtmlHeaderFooter { HtmlFragment = html }` | ヘッダー |
| `setFooterHtml(html)` | `.HtmlFooter = new HtmlHeaderFooter { HtmlFragment = html }` | フッター |
| `setHeaderHeight("20mm")` | `.HtmlHeader.MaxHeight = 20` | ヘッダーの高さ |
| `setFooterHeight("20mm")` | `.HtmlFooter.MaxHeight = 20` | フッターの高さ |
| `setZoomFactor(1.5)` | `.Zoom = 150` | ズーム率 |
| `setJavascriptDelay(500)` | `.RenderDelay = 500` | JS待機時間 (ms) |
| `setDisableJavascript(true)` | `.EnableJavaScript = false` | JS無効化 |
| `setUsePrintMedia(true)` | `.CssMediaType = PdfCssMediaType.Print` | 印刷用CSS |
| `setUseScreenMedia(true)` | `.CssMediaType = PdfCssMediaType.Screen` | 画面用CSS |
| `setEncrypt(true)` | `pdf.SecuritySettings.MakeDocumentPrintOnly()` | 暗号化 |
| `setUserPassword("pass")` | `pdf.SecuritySettings.UserPassword = "pass"` | ユーザーパスワード |
| `setOwnerPassword("pass")` | `pdf.SecuritySettings.OwnerPassword = "pass"` | オーナーパスワード |

### 認証の比較

| PDFmyURL | IronPDF |
|----------|---------|
| `new HtmlToPdfClient("username", "apikey")` | `IronPdf.License.LicenseKey = "LICENSE-KEY"` |
| リクエストごとのAPIキー | 起動時に一度設定 |
| すべての呼び出しに必要 | グローバルに一度設定 |

---

## コード移行例

### 例1: 基本的なURLからPDFへ

**以前 (PDFmyURL/Pdfcrowd):**
```csharp
using Pdfcrowd;
using System;

class Program
{
    static void Main()
    {
        try
        {
            // すべての変換にAPI認証が必要
            var client = new HtmlToPdfClient("username", "apikey");

            // URLが外部サーバーに送信されて処理される
            client.convertUrlToFile("https://example.com", "output.pdf");

            Console.WriteLine("PDFが作成されました");
        }
        catch (Error why)
        {
            Console.WriteLine("エラー: " + why);
        }
    }
}
```

**以降 (IronPDF):**
```csharp
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        // 一度だけのライセンス設定
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

        // すべての処理がローカルで行われる
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderUrlAsPdf("https://example.com");
        pdf.SaveAs("output.pdf");

        Console.WriteLine("PDFが作成されました");
    }
}
```

### 例2: オプション付きのHTML文字列

**以前 (PDFmyURL/Pdfcrowd):**
```csharp
using Pdfcrowd;
using System;

class Program
{
    static void Main()
    {
        try
        {
            var client = new HtmlToPdfClient("username", "apikey");

            // setterメソッドを介して設定
            client.setPageSize("A4");
            client.setOrientation("landscape");
            client.setMarginTop("15mm");
            client.setMarginBottom("15mm");
            client.setMarginLeft("20mm");
            client.setMarginRight("20mm");

            string html = @"
                <html>
                <head><style>body { font-family: Arial; }</style></head>
                <body><h1>レポート</h1><p>ここにコンテンツ</p></body>
                </html>";

            client.convertStringToFile(html, "report.pdf");
        }
        catch (Error why)
        {
            Console.WriteLine("エラー: " + why);
        }
    }
}
```

**以降 (IronPDF):**
```csharp
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

        var renderer = new ChromePdfRenderer();

        // RenderingOptionsプロパティを介して設定
        renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
        renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;
        renderer.RenderingOptions.MarginTop = 15;
        renderer.RenderingOptions.MarginBottom = 15;
        renderer.RenderingOptions.MarginLeft = 20;
        renderer.RenderingOptions.MarginRight = 20;

        string html = @"
            <html>
            <head><style>body { font-family: Arial; }</style></head>
            <body><h1>レポート</h1><p>ここにコンテンツ</p></body>
            </html>";

        var pdf = renderer.RenderHtmlAsPdf(html);
        pdf.SaveAs("report.pdf");
    }
}
```

### 例3: ヘッダーとフッター

**以前 (PDFmyURL/Pdfcrowd):**
```csharp
using Pdfcrowd;
using System;

class Program
{
    static void Main()
    {
        try
        {
            var client = new HtmlToPdfClient("username", "apikey");

            // ヘッダーを設定
            client.setHeaderHtml("<div style='text-align:center;'>会社レポート</div>");
            client.setHeaderHeight("25mm");

            // ページ番号付きのフッターを設定
            client.setFooterHtml("<div style='text-align:center;'>ページ {page_number} / {total_pages}</div>");
            client.setFooterHeight("20mm");

            client.convertStringToFile("<h1>メインコンテンツ</h1>", "report.pdf");
        }
        catch (Error why)
        {
            Console.WriteLine("エラー: " + why);
        }
    }
}
```

**以降 (IronPDF):**
```csharp
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

        var renderer = new ChromePdfRenderer();

        // ヘッダーを設定
        renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
        {
            HtmlFragment = "<div style='text-align:center;'>会社レポート</div>",
            MaxHeight = 25
        };

        // ページ番号付きのフッターを設定（異なるプレースホルダー）
        renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
        {
            HtmlFragment = "<div style='text-align:center;'>ページ {page} / {total-pages}</div>",
            MaxHeight = 20
        };

        var pdf = renderer.RenderHtmlAsPdf("<h1>メインコンテンツ</h1>");
        pdf.SaveAs("report.pdf");
    }
}
```

### 例4: 非同期Webアプリケーション

**以前 (PDFmyURL):**
```csharp
using PdfMyUrl;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class ReportController : Controller
{
    private readonly string _apiKey = "your-api-key";

    [HttpGet]
    public async Task<IActionResult> GenerateReport(string url)
    {
        try
        {
            var client = new PdfMyUrlClient(_apiKey);

            // 外部サービスへの非同期呼び出し（APIに必須）
            var response = await client.ConvertUrlAsync(url);
            byte[] pdfBytes = response.GetBytes();

            return File(pdfBytes, "application/pdf", "report.pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "PDF生成に失敗しました: " + ex.Message);
        }
    }
}
```

**以降 (IronPDF):**
```csharp
using IronPdf;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class ReportController : Controller
{
    public ReportController()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
    }

    [HttpGet]
    public async Task<IActionResult> GenerateReport(string url)