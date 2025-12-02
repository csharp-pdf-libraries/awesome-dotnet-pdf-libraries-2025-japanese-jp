# PDFBoltからIronPDFへの移行方法は？

## なぜPDFBoltからIronPDFへ移行するのか？

PDFBoltは、ドキュメントを外部サーバーで処理するクラウド専用のSaaSサービスです。クイックプロトタイプには便利ですが、このアーキテクチャは本番アプリケーションにおいて重大な課題を生み出します：

### PDFBoltの重大な制限

1. **クラウド専用処理**：すべてのドキュメントが外部サーバーを経由します。自己ホストオプションはありません
2. **データプライバシーリスク**：外部に送信される機密ドキュメント（契約書、医療記録、財務データ）
3. **使用制限**：無料枠は月100ドキュメントまで。ドキュメントごとの課金はすぐに高額になります
4. **ネットワーク依存性**：インターネットの停止やPDFBoltのダウンタイム = PDF生成が停止します
5. **遅延**：ネットワークの往復で毎回数秒が加算されます
6. **コンプライアンスの問題**：GDPR、HIPAA、SOC2の監査が外部処理により複雑化します
7. **APIキーセキュリティ**：漏洩したキー = あなたのアカウントに請求される不正使用
8. **ベンダーロックイン**：PDFBoltが条件を変更したりサービスを停止した場合、あなたのアプリケーションは失敗します

### IronPDFの利点

| 懸念事項 | PDFBolt | IronPDF |
|---------|---------|---------|
| **データの場所** | 外部サーバー | あなたのサーバーのみ |
| **使用制限** | 100無料、その後はドキュメントごと | 無制限 |
| **インターネットが必要** | はい、常に | いいえ |
| **遅延** | ネットワーク往復 | ミリ秒 |
| **コンプライアンス** | 複雑（外部処理） | 単純（ローカル処理） |
| **コストモデル** | ドキュメントごと | 一回または年間 |
| **オフライン操作** | 不可能 | 完全にサポートされています |
| **APIキーリスク** | 漏洩 = 請求される | ライセンスキー、請求リスクなし |

---

## NuGetパッケージの変更

```bash
# PDFBoltを削除
dotnet remove package PDFBolt

# IronPDFをインストール
dotnet add package IronPdf
```

---

## 名前空間の変更

```csharp
// PDFBolt
using PDFBolt;
using PDFBolt.Models;
using PDFBolt.Options;

// IronPDF
using IronPdf;
using IronPdf.Editing;
using IronPdf.Rendering;
```

---

## 完全なAPIリファレンス

### コアクラスのマッピング

| PDFBolt | IronPDF | 備考 |
|---------|---------|-------|
| `new Client(apiKey)` | `new ChromePdfRenderer()` | APIキーは不要 |
| `new HtmlToPdfConverter()` | `new ChromePdfRenderer()` | すべてに同じレンダラー |
| `new PdfOptions()` | `renderer.RenderingOptions` | プロパティベースの設定 |
| `PdfResult` | `PdfDocument` | 豊富なドキュメントオブジェクト |

### 変換メソッド

| PDFBolt | IronPDF | 備考 |
|---------|---------|-------|
| `await client.HtmlToPdf(html)` | `renderer.RenderHtmlAsPdf(html)` | デフォルトで同期 |
| `await client.HtmlToPdf(html, options)` | `renderer.RenderHtmlAsPdf(html)` | レンダラー上のオプション |
| `await client.UrlToPdf(url)` | `renderer.RenderUrlAsPdf(url)` | デフォルトで同期 |
| `await client.UrlToPdf(url, options)` | `renderer.RenderUrlAsPdf(url)` | レンダラー上のオプション |
| `await client.FileToPdf(path)` | `renderer.RenderHtmlFileAsPdf(path)` | HTMLファイル |
| `converter.ConvertHtmlString(html)` | `renderer.RenderHtmlAsPdf(html)` | PdfDocumentを返す |
| `converter.ConvertUrl(url)` | `renderer.RenderUrlAsPdf(url)` | PdfDocumentを返す |

### 出力メソッド

| PDFBolt | IronPDF | 備考 |
|---------|---------|-------|
| `await result.SaveToFile(path)` | `pdf.SaveAs(path)` | 同期メソッド |
| `result.GetBytes()` | `pdf.BinaryData` | プロパティアクセス |
| `await result.GetBytesAsync()` | `pdf.BinaryData` | すでに利用可能 |
| `result.GetStream()` | `pdf.Stream` | ストリームプロパティ |

### ページ設定

| PDFBolt | IronPDF | 備考 |
|---------|---------|-------|
| `options.PageSize = PageSize.A4` | `renderer.RenderingOptions.PaperSize = PdfPaperSize.A4` | 列挙名が異なる |
| `options.PageSize = PageSize.Letter` | `renderer.RenderingOptions.PaperSize = PdfPaperSize.Letter` | 標準サイズ |
| `options.Orientation = Orientation.Landscape` | `renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape` | ランドスケープモード |
| `options.Orientation = Orientation.Portrait` | `renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Portrait` | デフォルト |
| `options.Width = 210` | `renderer.RenderingOptions.SetCustomPaperSizeinMillimeters(210, 297)` | カスタムサイズ |

### 余白設定

| PDFBolt | IronPDF | 備考 |
|---------|---------|-------|
| `options.MarginTop = 20` | `renderer.RenderingOptions.MarginTop = 20` | ミリメートルで |
| `options.MarginBottom = 20` | `renderer.RenderingOptions.MarginBottom = 20` | 個別のプロパティ |
| `options.MarginLeft = 15` | `renderer.RenderingOptions.MarginLeft = 15` | 余白オブジェクトなし |
| `options.MarginRight = 15` | `renderer.RenderingOptions.MarginRight = 15` | 直接割り当て |
| `options.Margins = new Margins(t,r,b,l)` | 個別のプロパティ | 上記参照 |

### レンダリングオプション

| PDFBolt | IronPDF | 備考 |
|---------|---------|-------|
| `options.PrintBackground = true` | `renderer.RenderingOptions.PrintHtmlBackgrounds = true` | CSSの背景 |
| `options.WaitForNetworkIdle = true` | `renderer.RenderingOptions.WaitFor.NetworkIdle()` | 待機戦略 |
| `options.WaitTime = 2000` | `renderer.RenderingOptions.WaitFor.RenderDelay(2000)` | ミリ秒 |
| `options.Scale = 1.0` | `renderer.RenderingOptions.Zoom = 100` | パーセンテージ |
| `options.PreferCssPageSize = true` | `renderer.RenderingOptions.UseCssPageSize = true` | CSS @page ルール |

### ヘッダーとフッター

| PDFBolt | IronPDF | 備考 |
|---------|---------|-------|
| `options.Header = "Header text"` | `renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter { HtmlFragment = "<div>Header</div>" }` | HTMLベース |
| `options.Footer = "Footer text"` | `renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter { HtmlFragment = "<div>Footer</div>" }` | 完全なCSSサポート |
| `options.DisplayHeaderFooter = true` | HtmlHeader/HtmlFooter設定時に自動 | 暗黙的 |
| `{pageNumber}` | `{page}` | 現在のページ |
| `{totalPages}` | `{total-pages}` | 総ページ数 |
| `{date}` | `{date}` | 同じ |
| `{title}` | `{html-title}` | ドキュメントタイトル |

### PDF操作（IronPDFの新機能）

| PDFBolt | IronPDF | 備考 |
|---------|---------|-------|
| _(利用不可)_ | `PdfDocument.Merge(pdf1, pdf2)` | PDFをマージ |
| _(利用不可)_ | `pdf.CopyPages(start, end)` | ページを抽出 |
| _(利用不可)_ | `pdf.RemovePages(indices)` | ページを削除 |
| _(利用不可)_ | `pdf.InsertPdf(other, index)` | PDFを挿入 |
| _(利用不可)_ | `pdf.RotatePage(index, degrees)` | ページを回転 |
| _(利用不可)_ | `pdf.ApplyWatermark(html)` | ウォーターマークを追加 |
| _(利用不可)_ | `pdf.ExtractAllText()` | テキストを抽出 |
| _(利用不可)_ | `pdf.RasterizeToImageFiles()` | PDFを画像に変換 |
| _(利用不可)_ | `pdf.SecuritySettings` | 暗号化 |

---

## コード移行例

### 例1：APIキーパターンの削除

**移行前（PDFBolt）：**
```csharp
using PDFBolt;

public class PdfService
{
    private readonly Client _client;

    public PdfService(IConfiguration config)
    {
        // 漏洩した場合のセキュリティリスクがある設定からAPIキーを取得
        var apiKey = config["PDFBolt:ApiKey"];
        _client = new Client(apiKey);
    }

    public async Task<byte[]> GeneratePdfAsync(string html)
    {
        try
        {
            var result = await _client.HtmlToPdf(html);
            return result.GetBytes();
        }
        catch (PDFBoltException ex) when (ex.Code == "RATE_LIMIT")
        {
            throw new Exception("月間制限を超えました");
        }
        catch (PDFBoltException ex) when (ex.Code == "INVALID_API_KEY")
        {
            throw new Exception("APIキーが無効または取り消されました");
        }
    }
}
```

**移行後（IronPDF）：**
```csharp
using IronPdf;

public class PdfService
{
    private readonly ChromePdfRenderer _renderer;

    public PdfService()
    {
        // ライセンスキーは起動時に一度設定（リクエストごとではない）
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
        _renderer = new ChromePdfRenderer();
    }

    public byte[] GeneratePdf(string html)
    {
        // 制限なし、APIキーの検証なし
        // ネットワーク不要、外部処理なし
        var pdf = _renderer.RenderHtmlAsPdf(html);
        return pdf.BinaryData;
    }
}
```

### 例2：非同期から同期への変換

**移行前（PDFBolt）：**
```csharp
using PDFBolt;

public async Task<ActionResult> GenerateInvoice(int orderId)
{
    var order = await _orderService.GetOrderAsync(orderId);
    var html = await _templateService.RenderInvoiceAsync(order);

    var client = new Client(_apiKey);
    var options = new PdfOptions
    {
        PageSize = PageSize.A4,
        MarginTop = 20,
        MarginBottom = 20
    };

    // PDFBoltサーバーへのネットワーク往復
    var result = await client.HtmlToPdf(html, options);
    var bytes = result.GetBytes();

    return File(bytes, "application/pdf", $"invoice-{orderId}.pdf");
}
```

**移行後（IronPDF）：**
```csharp
using IronPdf;

public ActionResult GenerateInvoice(int orderId)
{
    var order = _orderService.GetOrder(orderId);
    var html = _templateService.RenderInvoice(order);

    var renderer = new ChromePdfRenderer();
    renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
    renderer.RenderingOptions.MarginTop = 20;
    renderer.RenderingOptions.MarginBottom = 20;

    // ローカル処理 - ネットワーク不要、外部サーバー不要
    var pdf = renderer.RenderHtmlAsPdf(html);

    return File(pdf.BinaryData, "application/pdf", $"invoice-{orderId}.pdf");
}
```

### 例3：ヘッダー/フッター付きのURLからPDFへの変換

**移行前（PDFBolt）：**
```csharp
using PDFBolt;
using PDFBolt.Models;

public async Task<byte[]> CaptureWebpageAsync(string url)
{
    var client = new Client(_apiKey);
    var options = new PdfOptions
    {
        PageSize = PageSize.A4,
        Orientation = Orientation.Portrait,
        DisplayHeaderFooter = true,
        Header = "Captured from: " + url,
        Footer = "Page {pageNumber} of {totalPages}",
        WaitForNetworkIdle = true,
        PrintBackground = true
    };

    var result = await client.UrlToPdf(url, options);
    return result.GetBytes();
}
```

**移行後（IronPDF）：**
```csharp
using IronPdf;

public byte[] CaptureWebpage(string url)
{
    var renderer = new ChromePdfRenderer();
    renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
    renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Portrait;
    renderer.RenderingOptions.PrintHtmlBackgrounds = true;
    renderer.RenderingOptions.WaitFor.NetworkIdle();

    // HTMLベースのヘッダーで完全なCSSサポート
    renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
    {
        HtmlFragment = $"<div style='font-size:10px; color:#666;'>Captured from: {url}</div>",
        MaxHeight = 20
    };

    // 組み込みのプレースホルダー：{page}, {total-pages}, {date}, {time}
    renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
    {
        HtmlFragment = "<div style='font-size:10px; text-align:center;'>Page {page} of {total-pages}</div>",
        MaxHeight = 20
    };

    var pdf = renderer.RenderUrlAsPdf(url);
    return pdf.BinaryData;
}
```

### 例4：制限なしのバッチ処理

**移行前（PDFBolt）：**
```csharp
using PDFBolt;

public async Task GenerateMonthlyReports(List<Report> reports)
{
    var client = new Client(_apiKey);
    int processed = 0;

    foreach (var report in reports)
    {
        // PDFBoltの無料枠の制限をチェック - PDFBolt無料枠 = 月100
        if (processed >= 100)
        {
            throw new Exception("PDFBoltの月間制限に達しました！");
        }

        try
        {
            var html = RenderReport(report);
            var result = await client.HtmlToPdf(html);
            await result.SaveToFile($"report-{report.Id}.pdf");
            processed++;

            // 制限に達しないように遅延を追加
            await Task.Delay(1000);
        }
        catch (PDFBoltException ex) when (ex.Code == "RATE_LIMIT")
        {
            throw new Exception($"レポート{processed}件後にレート制限に達しました");
        }
    }
}
```

**移行後（IronPDF）：**
```csharp
using IronPdf;

public void GenerateMonthlyReports(List<Report> reports)
{
    var renderer = new ChromePdfRenderer();

    // 制限なし、月間キャップなし、遅延不要
    foreach (var report in reports)
    {
        var html = RenderReport(report);
        var pdf = renderer.RenderHtmlAsPdf(html);
        pdf.SaveAs($"report-{report.Id}.pdf");
    }

    // 速度向上のために並列処理：
    Parallel