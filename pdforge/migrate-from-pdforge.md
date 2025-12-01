---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [pdforge/migrate-from-pdforge.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/pdforge/migrate-from-pdforge.md)
🇯🇵 **日本語:** [pdforge/migrate-from-pdforge.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/pdforge/migrate-from-pdforge.md)

---

# 移行ガイド: pdforge → IronPDF

## なぜpdforgeから移行するのか？

pdforgeはクラウドベースのPDF生成APIで、ドキュメントを外部サーバーで処理します。これにより初期設定が簡素化されますが、本番アプリケーションには重大な制限が伴います：

### クラウドAPI依存問題

1. **外部サーバー処理**: 生成するPDFごとにHTML/データをpdforgeのサーバーに送信する必要があります。ドキュメントがインフラから離れます
2. **プライバシーおよびコンプライアンスリスク**: 機密データ（契約書、財務報告書、個人情報）がインターネット経由で第三者のサーバーに送信されます
3. **継続的なサブスクリプションコスト**: 月額料金が無期限に蓄積し、資産の所有権がありません
4. **インターネット依存**: ネットワークが利用不可の場合、PDFの生成ができません
5. **レート制限**: APIの使用制限が高容量アプリケーションを制限する可能性があります
6. **ネットワーク遅延**: 往復時間がPDF生成に秒単位で追加されます
7. **ベンダーロックイン**: アプリケーションはpdforgeのサービス可用性とAPI安定性に依存します

### なぜIronPDFか？

IronPDFはアプリケーション内で全てをローカルで処理します：

- **完全なデータプライバシー**: ドキュメントはサーバーを離れません
- **一回限りのライセンス**: 永久ライセンスが繰り返しのコストを排除します
- **オフライン機能**: 初期設定後はインターネットなしで動作します
- **レート制限なし**: 無制限のPDFを生成
- **低遅延**: ネットワーク往復がありません
- **完全な制御**: 処理環境を所有します
- **最新のレンダリング**: ChromiumベースのエンジンでCSS3/JSを完全サポート
- **豊富な機能セット**: PDF操作、マージ、テキスト抽出、セキュリティ

---

## 移行の概要

| 項目 | pdforge | IronPDF |
|--------|---------|---------|
| 処理場所 | 外部クラウドサーバー | ローカル（あなたのサーバー） |
| 認証 | リクエストごとにAPIキー | 一回限りのライセンスキー |
| ネットワーク要件 | 生成ごとに必要 | 初期設定時のみ |
| 価格モデル | 月額サブスクリプション | 永久ライセンス利用可能 |
| レート制限 | あり（プランに依存） | なし |
| データプライバシー | データが外部に送信される | データがローカルに留まる |
| オフラインサポート | なし | あり |
| PDF操作 | 限定的 | フルスイート（マージ、分割、編集） |
| テキスト抽出 | なし | あり |
| 非同期パターン | 必須 | オプション（同期/非同期） |

---

## NuGetパッケージの変更

```bash
# pdforgeパッケージを削除
dotnet remove package pdforge
dotnet remove package PdfForge

# IronPDFをインストール
dotnet add package IronPdf
```

---

## 名前空間の変更

```csharp
// 以前: pdforge
using PdForge;
using PdForge.Client;
using PdForge.Models;

// 以降: IronPDF
using IronPdf;
using IronPdf.Rendering;
```

---

## APIマッピングリファレンス

### コアクラス

| pdforge | IronPDF | 備考 |
|---------|---------|-------|
| `PdfClient` | `ChromePdfRenderer` | 主要なPDFジェネレーター |
| `HtmlToPdfRequest` | `ChromePdfRenderOptions` | HTML変換設定 |
| `UrlToPdfRequest` | `ChromePdfRenderOptions` | URL変換設定 |
| `HtmlToPdfConverter` | `ChromePdfRenderer` | 代替クラス名 |
| APIレスポンス (byte[]) | `PdfDocument` | 結果オブジェクト |

### メソッド

| pdforge | IronPDF | 備考 |
|---------|---------|-------|
| `client.GenerateAsync(request)` | `renderer.RenderHtmlAsPdf(html)` | HTMLからPDFへ |
| `client.GenerateAsync(urlRequest)` | `renderer.RenderUrlAsPdf(url)` | URLからPDFへ |
| `converter.ConvertHtmlString(html)` | `renderer.RenderHtmlAsPdf(html)` | HTML文字列 |
| `converter.ConvertUrl(url)` | `renderer.RenderUrlAsPdf(url)` | URL変換 |
| `converter.ConvertFile(path)` | `renderer.RenderHtmlFileAsPdf(path)` | HTMLファイル |
| `File.WriteAllBytes(path, bytes)` | `pdf.SaveAs(path)` | ディスクに保存 |
| 戻り値: `byte[]` | `pdf.BinaryData` | 生データを取得 |
| 戻り値: `Task<byte[]>` | `pdf.Stream` | ストリームとして取得 |

### 設定オプション

| pdforge | IronPDF (RenderingOptions) | 備考 |
|---------|---------------------------|-------|
| `PageSize = PageSize.A4` | `.PaperSize = PdfPaperSize.A4` | 用紙サイズ |
| `PageSize = PageSize.Letter` | `.PaperSize = PdfPaperSize.Letter` | USレター |
| `Orientation = Orientation.Landscape` | `.PaperOrientation = PdfPaperOrientation.Landscape` | 方向 |
| `Orientation = Orientation.Portrait` | `.PaperOrientation = PdfPaperOrientation.Portrait` | 縦向き |
| `MarginTop = 20` | `.MarginTop = 20` | 上余白 |
| `MarginBottom = 20` | `.MarginBottom = 20` | 下余白 |
| `MarginLeft = 15` | `.MarginLeft = 15` | 左余白 |
| `MarginRight = 15` | `.MarginRight = 15` | 右余白 |
| `Header = "text"` | `.TextHeader = new TextHeaderFooter { CenterText = "text" }` | ヘッダー |
| `Footer = "text"` | `.TextFooter = new TextHeaderFooter { CenterText = "text" }` | フッター |
| `HeaderHtml = "<div>..."` | `.HtmlHeader = new HtmlHeaderFooter { HtmlFragment = "..." }` | HTMLヘッダー |
| `FooterHtml = "<div>..."` | `.HtmlFooter = new HtmlHeaderFooter { HtmlFragment = "..." }` | HTMLフッター |
| `JavascriptDelay = 2000` | `.RenderDelay = 2000` | JS待機時間（ms） |
| `PrintBackground = true` | `.PrintHtmlBackgrounds = true` | 背景レンダリング |
| `Scale = 1.5` | `.Zoom = 150` | ズームパーセンテージ |

### 認証比較

| pdforge | IronPDF |
|---------|---------|
| `new PdfClient("your-api-key")` | `IronPdf.License.LicenseKey = "YOUR-KEY"` |
| リクエストごとに認証 | 起動時に一度 |
| コンストラクタ内のAPIキー | グローバルプロパティ |

---

## コード移行例

### 例1: 基本的なHTMLからPDFへ

**以前 (pdforge):**
```csharp
using PdForge;
using PdForge.Client;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        // クライアントインスタンスごとにAPIキーが必要
        var client = new PdfClient("your-api-key");

        var request = new HtmlToPdfRequest
        {
            Html = "<html><body><h1>こんにちは世界</h1><p>PDFコンテンツ</p></body></html>"
        };

        // 外部サーバーにデータを送信
        byte[] pdfBytes = await client.GenerateAsync(request);
        File.WriteAllBytes("output.pdf", pdfBytes);
    }
}
```

**以降 (IronPDF):**
```csharp
using IronPdf;

class Program
{
    static void Main()
    {
        // ライセンス設定は一度だけ
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

        // 全ての処理はローカルで行われます
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderHtmlAsPdf("<html><body><h1>こんにちは世界</h1><p>PDFコンテンツ</p></body></html>");
        pdf.SaveAs("output.pdf");
    }
}
```

### 例2: オプション付きのURLからPDFへ

**以前 (pdforge):**
```csharp
using PdForge;
using PdForge.Client;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var client = new PdfClient("your-api-key");

        var request = new UrlToPdfRequest
        {
            Url = "https://example.com",
            PageSize = PageSize.A4,
            Orientation = Orientation.Landscape,
            MarginTop = 20,
            MarginBottom = 20,
            MarginLeft = 15,
            MarginRight = 15
        };

        byte[] pdfBytes = await client.GenerateAsync(request);
        File.WriteAllBytes("webpage.pdf", pdfBytes);
    }
}
```

**以降 (IronPDF):**
```csharp
using IronPdf;

class Program
{
    static void Main()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

        var renderer = new ChromePdfRenderer();
        renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
        renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;
        renderer.RenderingOptions.MarginTop = 20;
        renderer.RenderingOptions.MarginBottom = 20;
        renderer.RenderingOptions.MarginLeft = 15;
        renderer.RenderingOptions.MarginRight = 15;

        var pdf = renderer.RenderUrlAsPdf("https://example.com");
        pdf.SaveAs("webpage.pdf");
    }
}
```

### 例3: ページ番号付きのヘッダーとフッター

**以前 (pdforge):**
```csharp
using PdForge;
using PdForge.Client;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var client = new PdfClient("your-api-key");

        var request = new HtmlToPdfRequest
        {
            Html = "<h1>レポート</h1><p>ここにコンテンツ...</p>",
            Header = "会社レポート",
            Footer = "ページ {page} / {totalPages}"
        };

        byte[] pdfBytes = await client.GenerateAsync(request);
        File.WriteAllBytes("report.pdf", pdfBytes);
    }
}
```

**以降 (IronPDF):**
```csharp
using IronPdf;

class Program
{
    static void Main()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

        var renderer = new ChromePdfRenderer();

        renderer.RenderingOptions.TextHeader = new TextHeaderFooter
        {
            CenterText = "会社レポート",
            DrawDividerLine = true,
            FontSize = 12
        };

        renderer.RenderingOptions.TextFooter = new TextHeaderFooter
        {
            CenterText = "ページ {page} / {total-pages}",  // ハイフンを含む{total-pages}に注意
            DrawDividerLine = true
        };

        var pdf = renderer.RenderHtmlAsPdf("<h1>レポート</h1><p>ここにコンテンツ...</p>");
        pdf.SaveAs("report.pdf");
    }
}
```

### 例4: HTMLヘッダーとフッター

**以前 (pdforge):**
```csharp
using PdForge;
using PdForge.Client;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var client = new PdfClient("your-api-key");

        var request = new HtmlToPdfRequest
        {
            Html = "<h1>レポートコンテンツ</h1>",
            HeaderHtml = "<div style='text-align:center;'><img src='logo.png'/> 会社名</div>",
            FooterHtml = "<div style='text-align:center;'>機密 - ページ {page}</div>",
            HeaderHeight = 50,
            FooterHeight = 30
        };

        byte[] pdfBytes = await client.GenerateAsync(request);
        File.WriteAllBytes("branded-report.pdf", pdfBytes);
    }
}
```

**以降 (IronPDF):**
```csharp
using IronPdf;

class Program
{
    static void Main()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

        var renderer = new ChromePdfRenderer();

        renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
        {
            HtmlFragment = "<div style='text-align:center;'><img src='logo.png'/> 会社名</div>",
            MaxHeight = 50
        };

        renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
        {
            HtmlFragment = "<div style='text-align:center;'>機密 - ページ {page}</div>",
            MaxHeight = 30
        };

        var pdf = renderer.RenderHtmlAsPdf("<h1>レポートコンテンツ</h1>");
        pdf.SaveAs("branded-report.pdf");
    }
}
```

### 例5: 非同期Webアプリケーション

**以前 (pdforge):**
```csharp
using PdForge;
using PdForge.Client;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class ReportController : Controller
{
    private readonly PdfClient _pdfClient;

    public ReportController()
    {
        _pdfClient = new PdfClient("your-api-key");
    }

    [HttpGet]
    public async Task<IActionResult> GenerateReport()
    {
        var request = new HtmlToPdfRequest
        {
            Html = "<h1>売上レポート</h1><p>Q4結果</p>"
        };

        try
        {
            // 外部APIへの非同期呼び出し（ネットワーク遅延）
            byte[] pdfBytes = await _pdfClient.GenerateAsync(request);
            return File(pdfBytes, "application/pdf", "report.pdf");
        }
        catch (ApiException ex)
        {
            return StatusCode(500, $"PDF生成に失敗しました: {ex.Message}");
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
    public async Task<IActionResult> GenerateReport()
    {
        try
        {
            // ローカル処理 - 必要に応じてTask.Runで非同期にラップ
            var pdf = await Task.Run(() =>
            {
                var renderer = new ChromePdfRenderer();
                return renderer.RenderHtmlAsPdf("<h1>売上レポート</h1><p>Q4結果</p>");
            });

            return File(pdf.BinaryData, "application/pdf", "report.pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"PDF生成に失敗しました: {ex.Message}");
        }
    }
}
```

### 例6: 動的コンテン