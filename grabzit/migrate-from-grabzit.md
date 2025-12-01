---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [grabzit/migrate-from-grabzit.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/grabzit/migrate-from-grabzit.md)
🇯🇵 **日本語:** [grabzit/migrate-from-grabzit.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/grabzit/migrate-from-grabzit.md)

---

# GrabzItからIronPDFへの移行方法は？

## 目次

1. [GrabzItからIronPDFへの移行理由](#grabzitからironpdfへの移行理由)
2. [開始する前に](#開始する前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全なAPIリファレンス](#完全なapiリファレンス)
5. [コード移行例](#コード移行例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンス比較](#パフォーマンス比較)
8. [トラブルシューティング](#トラブルシューティング)
9. [移行チェックリスト](#移行チェックリスト)

---

## GrabzItからIronPDFへの移行理由

### GrabzItアーキテクチャの問題

GrabzItは、クラウドベースのスクリーンショットおよびPDFキャプチャサービスです。迅速な統合には便利ですが、根本的なアーキテクチャ上の制限があります：

1. **画像ベースのPDF**：GrabzItは、テキストが選択できないスクリーンショットベースのPDFを作成します。基本的にはPDF形式でラップされた画像です
2. **外部処理**：すべてのコンテンツはGrabzItのサーバーに送信されて処理されます。機密データに対するプライバシーとコンプライアンスの懸念
3. **ネットワーク依存**：PDF生成ごとに外部サーバーへのHTTP呼び出しが必要です。レイテンシ、可用性、およびレート制限の問題
4. **コールバックの複雑さ**：非同期コールバックモデルは、Webhook処理インフラストラクチャを必要とします
5. **キャプチャごとの価格設定**：使用量に応じたモデルは、規模に応じて高価になる可能性があります
6. **テキスト検索なし**：PDFが画像ベースであるため、OCRなしではテキスト検索と抽出が機能しません
7. **ファイルサイズが大きい**：画像ベースのPDFは、ベクターベースのPDFよりも大幅に大きくなります
8. **オフライン機能なし**：インターネット接続なしではPDFを生成できません

### 代わりにIronPDFが提供するもの

| 項目 | GrabzIt | IronPDF |
|--------|---------|---------|
| PDFタイプ | 画像ベース（スクリーンショット） | 真のベクターPDF |
| テキスト選択 | 不可能 | テキスト全選択可能 |
| テキスト検索 | OCRが必要 | ネイティブ検索可能 |
| 処理場所 | 外部サーバー | ローカル/インプロセス |
| プライバシー | データが外部に送信される | データはローカルに留まる |
| レイテンシ | ネットワーク往復（500ms-5s） | ローカル処理（約100ms） |
| 価格モデル | キャプチャごと | 開発者ライセンスごと |
| オフライン機能 | なし | あり |
| ファイルサイズ | 大（画像データ） | 小（ベクターデータ） |
| コールバック必要 | はい（非同期） | いいえ（同期/非同期） |
| CSS/JSサポート | 限定的 | 完全なChromiumエンジン |

---

## 開始する前に

### 前提条件

1. **.NETバージョン**：IronPDFは.NET Framework 4.6.2+および.NET Core 3.1+ / .NET 5+をサポートしています
2. **ライセンスキー**：[IronPDFウェブサイト](https://ironpdf.com/licensing/)から入手してください
3. **GrabzItインフラストラクチャの削除**：コールバックハンドラーとAPIキー設定を削除する計画を立ててください

### GrabzItの使用状況を特定する

コードベース内のすべてのGrabzIt API呼び出しを見つけます：

```bash
# GrabzItクライアントの使用を検索
grep -r "GrabzItClient\|GrabzIt\." --include="*.cs" .

# コールバックハンドラーを検索
grep -r "GrabzIt\|grabzit" --include="*.ashx" --include="*.aspx" --include="*.cs" .

# 設定を検索
grep -r "APPLICATION_KEY\|APPLICATION_SECRET\|grabzit" --include="*.config" --include="*.json" .
```

### 依存関係の監査

現在のGrabzItパッケージを確認します：

```bash
# NuGetパッケージを確認
grep -r "GrabzIt" --include="*.csproj" .
```

---

## クイックスタート移行

### ステップ1：IronPDFのインストール

```bash
# GrabzItを削除
dotnet remove package GrabzIt

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ2：コードの更新

**以前（GrabzIt）：**
```csharp
using GrabzIt;
using GrabzIt.Parameters;

public class PdfService
{
    private readonly GrabzItClient _grabzIt;

    public PdfService()
    {
        _grabzIt = new GrabzItClient("APP_KEY", "APP_SECRET");
    }

    public void GeneratePdf(string html, string callbackUrl)
    {
        var options = new PDFOptions();
        options.MarginTop = 20;
        options.MarginBottom = 20;

        _grabzIt.HTMLToPDF(html, options);
        _grabzIt.Save(callbackUrl);  // 非同期 - コールバックが結果を受け取る
    }
}

// コールバックハンドラー（別のエンドポイント）
public class GrabzItHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        string id = context.Request.QueryString["id"];
        GrabzItClient grabzIt = new GrabzItClient("APP_KEY", "APP_SECRET");
        GrabzItFile file = grabzIt.GetResult(id);
        file.Save("output.pdf");
    }
}
```

**その後（IronPDF）：**
```csharp
using IronPdf;

public class PdfService
{
    private readonly ChromePdfRenderer _renderer;

    public PdfService()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
        _renderer = new ChromePdfRenderer();
        _renderer.RenderingOptions.MarginTop = 20;
        _renderer.RenderingOptions.MarginBottom = 20;
    }

    public byte[] GeneratePdf(string html)
    {
        // 同期 - コールバックは不要！
        var pdf = _renderer.RenderHtmlAsPdf(html);
        return pdf.BinaryData;
    }

    public void GeneratePdfToFile(string html, string outputPath)
    {
        var pdf = _renderer.RenderHtmlAsPdf(html);
        pdf.SaveAs(outputPath);
    }
}

// コールバックハンドラーは不要 - GrabzItHandlerを削除！
```

### ステップ3：インフラストラクチャの削除

- コールバックハンドラーファイル（`.ashx`、ハンドラーエンドポイント）を削除
- 設定からGrabzIt APIキーを削除
- Webhook URL設定を削除
- GrabzItステータスチェックコードを削除

---

## 完全なAPIリファレンス

### GrabzItClientからIronPDFへのマッピング

| GrabzItメソッド | IronPDF相当 | 備考 |
|---------------|-------------------|-------|
| `new GrabzItClient(key, secret)` | `new ChromePdfRenderer()` | 認証不要 |
| `HTMLToPDF(html)` | `renderer.RenderHtmlAsPdf(html)` | PDFを直接返す |
| `HTMLToPDF(html, options)` | `RenderingOptions`を先に設定 | レンダリング前にオプションを設定 |
| `URLToPDF(url)` | `renderer.RenderUrlAsPdf(url)` | PDFを直接返す |
| `URLToPDF(url, options)` | `RenderingOptions`を先に設定 | レンダリング前にオプションを設定 |
| `FileToPDF(path)` | `renderer.RenderHtmlFileAsPdf(path)` | ローカルHTMLファイル |
| `HTMLToImage(html)` | `pdf.ToBitmap()` | レンダリング後に変換 |
| `URLToImage(url)` | `pdf.ToBitmap()` | レンダリング後に変換 |
| `Save(callbackUrl)` | `pdf.SaveAs(path)`または`pdf.BinaryData` | 即時結果 |
| `SaveTo(filePath)` | `pdf.SaveAs(filePath)` | 同じ機能 |
| `GetResult(id)` | 該当なし | コールバック不要 |
| `GetStatus(id)` | 該当なし | 同期操作 |

### PDFOptionsからRenderingOptionsへのマッピング

| GrabzIt PDFOptions | IronPDFプロパティ | 備考 |
|-------------------|------------------|-------|
| `MarginTop` | `RenderingOptions.MarginTop` | 同じ単位（mm） |
| `MarginBottom` | `RenderingOptions.MarginBottom` | 同じ単位（mm） |
| `MarginLeft` | `RenderingOptions.MarginLeft` | 同じ単位（mm） |
| `MarginRight` | `RenderingOptions.MarginRight` | 同じ単位（mm） |
| `PageSize`（A4、Letterなど） | `RenderingOptions.PaperSize` | `PdfPaperSize`列挙型を使用 |
| `Orientation` | `RenderingOptions.PaperOrientation` | `Portrait`または`Landscape` |
| `BrowserWidth` | `RenderingOptions.ViewPortWidth` | ビューポート幅（ピクセル） |
| `BrowserHeight` | `RenderingOptions.ViewPortHeight` | ビューポート高さ（ピクセル） |
| `CSSMediaType` | `RenderingOptions.CssMediaType` | `Screen`または`Print` |
| `Delay` | `RenderingOptions.RenderDelay` | ミリ秒単位 |
| `ClickElement` | 代わりにJavaScriptを使用 | `WaitFor.JavaScript()` |
| `HideElement` | CSS/JavaScriptを使用 | CSSを注入して非表示 |
| `TemplateId` | `RenderingOptions.HtmlHeader/Footer` | HTMLテンプレートを使用 |
| `CustomWaterMark` | `pdf.ApplyWatermark()` | レンダリング後 |
| `Password` | `pdf.SecuritySettings.UserPassword` | レンダリング後 |
| `IncludeBackground` | `RenderingOptions.PrintHtmlBackgrounds` | ブール値 |
| `IncludeLinks` | 常に含まれる | IronPDFはリンクを保持 |
| `CoverURL` | 別途レンダリングしてマージ | `PdfDocument.Merge()` |
| `TargetElement` | JavaScriptを使用 | 特定の要素をターゲット |

### ImageOptionsからIronPDFへのマッピング

| GrabzIt ImageOptions | IronPDF相当 | 備考 |
|---------------------|-------------------|-------|
| `Format`（png、jpg） | `bitmap.Save(path, ImageFormat.Png)` | `ToBitmap()`後 |
| `Width` | `RenderingOptions.ViewPortWidth` | またはビットマップのサイズ変更 |
| `Height` | `RenderingOptions.ViewPortHeight` | またはビットマップのサイズ変更 |
| `Quality` | `EncoderParameters` | JPEGを保存する際 |
| `HD` | `ToBitmap()`で高いDPI | DPIパラメータを使用 |

---

## コード移行例

### 例1：基本的なHTMLからPDFへ

**以前（GrabzIt、コールバックあり）：**
```csharp
using GrabzIt;
using GrabzIt.Parameters;

public class GrabzItService
{
    private readonly GrabzItClient _client;

    public GrabzItService()
    {
        _client = new GrabzItClient("APP_KEY", "APP_SECRET");
    }

    public void CreatePdf(string html)
    {
        _client.HTMLToPDF(html);
        _client.Save("https://myserver.com/grabzit-callback");
        // 結果は後でコールバック経由で到着...
    }
}

// コールバックハンドラー - 非同期で結果を受け取る
public class GrabzItCallback : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        string captureId = context.Request.QueryString["id"];
        var client = new GrabzItClient("APP_KEY", "APP_SECRET");
        var result = client.GetResult(captureId);
        result.Save(Server.MapPath("~/pdfs/" + captureId + ".pdf"));
    }
}
```

**その後（IronPDF）：**
```csharp
using IronPdf;

public class PdfService
{
    public byte[] CreatePdf(string html)
    {
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderHtmlAsPdf(html);
        return pdf.BinaryData;  // 即時結果！
    }

    public void CreatePdfToFile(string html, string path)
    {
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderHtmlAsPdf(html);
        pdf.SaveAs(path);  // 完了！
    }
}

// コールバックハンドラーは不要 - 削除する！
```

### 例2：オプション付きのURLからPDFへ

**以前（GrabzIt）：**
```csharp
using GrabzIt;
using GrabzIt.Parameters;

public void CaptureWebPage(string url)
{
    var client = new GrabzItClient("APP_KEY", "APP_SECRET");

    var options = new PDFOptions();
    options.PageSize = PageSize.A4;
    options.Orientation = PageOrientation.Landscape;
    options.MarginTop = 25;
    options.MarginBottom = 25;
    options.BrowserWidth = 1280;
    options.Delay = 3000;  // 3秒待機
    options.IncludeBackground = true;

    client.URLToPDF(url, options);
    client.SaveTo("webpage.pdf");
}
```

**その後（IronPDF）：**
```csharp
using IronPdf;

public void CaptureWebPage(string url)
{
    var renderer = new ChromePdfRenderer();

    renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
    renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;
    renderer.RenderingOptions.MarginTop = 25;
    renderer.RenderingOptions.MarginBottom = 25;
    renderer.RenderingOptions.ViewPortWidth = 1280;
    renderer.RenderingOptions.RenderDelay = 3000;  // 3秒待機
    renderer.RenderingOptions.PrintHtmlBackgrounds = true;

    var pdf = renderer.RenderUrlAsPdf(url);
    pdf.SaveAs("webpage.pdf");
}
```

### 例3：PDFへの透かし追加

**以前（GrabzIt）：**
```csharp
using GrabzIt;
using GrabzIt.Parameters;

public void CreateWatermarkedPdf(string html)
{
    var client = new GrabzItClient("APP