---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [gotenberg/README.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/gotenberg/README.md)
🇯🇵 **日本語:** [gotenberg/README.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/gotenberg/README.md)

---

# Gotenberg vs IronPDF: Docker PDF生成 vs プロセス内C#ライブラリ

**C# PDF生成のためのGotenbergの代替をお探しですか？** Gotenbergは、REST API呼び出しを介してHTMLをPDFに変換するDockerベースのマイクロサービスです。このアーキテクチャは柔軟ではありますが、大幅なインフラストラクチャのオーバーヘッド、ネットワーク遅延、および運用の複雑さを導入します。C#開発者にとって、よりシンプルなアプローチがあります：**IronPDF**はプロセス内NuGetパッケージとして同じChromiumベースのレンダリングを提供します—Dockerコンテナ、ネットワーク呼び出し、管理するインフラストラクチャはありません。

## Gotenbergアーキテクチャの問題

Gotenbergでは以下が必要です：

1. **Dockerコンテナのデプロイと管理** - Kubernetes、Docker Compose、または手動のコンテナ管理
2. **ネットワーク通信の処理** - すべてのPDFリクエストは10-100ms+の遅延を伴うHTTP呼び出しです
3. **コールドスタートの管理** - コンテナの起動は最初のリクエストに2-5秒を追加します
4. **水平スケーリング** - より多くのPDF容量が必要ですか？より多くのコンテナをデプロイします
5. **サービスの健全性の監視** - コンテナのクラッシュ、ネットワーク障害、タイムアウト
6. **multipart/form-dataの構築** - すべてのリクエストに対する冗長なAPI

**C#アプリケーションにとって、これは不必要なオーバーヘッドです。** IronPDFはプロセス内で実行され、これらの複雑さをすべて排除します。

---

## Gotenberg vs IronPDF: 完全な比較

| 要素 | Gotenberg | IronPDF |
|--------|-----------|---------|
| **デプロイメント** | Dockerコンテナ + オーケストレーション | 単一のNuGetパッケージ |
| **アーキテクチャ** | マイクロサービス（REST API） | プロセス内ライブラリ |
| **リクエストごとの遅延** | 10-100ms+（ネットワーク往復） | < 1msのオーバーヘッド |
| **コールドスタート** | 2-5秒（コンテナ初期化） | 1-2秒（最初のレンダリングのみ） |
| **インフラストラクチャ** | Docker、Kubernetes、ロードバランサー | 必要なし |
| **ネットワーク依存性** | 必要 | なし |
| **障害モード** | ネットワーク、コンテナ、サービスの障害 | 標準的な.NET例外 |
| **APIスタイル** | REST multipart/form-data | ネイティブC#メソッド呼び出し |
| **スケーリング** | 水平（より多くのコンテナ） | 垂直（プロセス内） |
| **デバッグ** | 分散トレーシング | 標準デバッガー |
| **メモリ管理** | 別のコンテナ（512MB-2GB） | アプリケーションメモリ共有 |
| **バージョン管理** | コンテナイメージタグ | NuGetパッケージバージョン |
| **ヘルスチェック** | HTTPエンドポイント | N/A（プロセス内） |
| **CI/CDの複雑さ** | コンテナビルド、レジストリプッシュ | 標準的な.NETビルド |
| **コスト** | 無料（MITライセンス） | 商用 |
| **サポート** | コミュニティ | プロフェッショナルサポート |

---

## GotenbergのDockerアーキテクチャがC#アプリケーションに悪影響を与える理由

### 1. すべてのリクエストにネットワーク遅延

```csharp
// ❌ Gotenberg - すべてのPDFリクエストにはネットワーク往復が必要です
public async Task<byte[]> GeneratePdfGotenberg(string html)
{
    using var client = new HttpClient();
    using var content = new MultipartFormDataContent();
    content.Add(new StringContent(html), "files", "index.html");

    // ネットワーク呼び出し：すべてのリクエストに10-100ms+が追加されます
    var response = await client.PostAsync("http://gotenberg:3000/forms/chromium/convert/html", content);
    return await response.Content.ReadAsByteArrayAsync();
}

// ✅ IronPDF - プロセス内、ネットワークオーバーヘッドなし
public byte[] GeneratePdfIronPdf(string html)
{
    var renderer = new ChromePdfRenderer();
    return renderer.RenderHtmlAsPdf(html).BinaryData;
}
```

### 2. コールドスタートのペナルティ

```csharp
// ❌ Gotenberg - コンテナ開始後の最初のリクエスト：2-5+秒
// すべてのポッド再起動、スケールアップイベント、デプロイメント = コールドスタート

// ✅ IronPDF - 最初のレンダリング：1-2秒、その後のレンダリング：<200ミリ秒
// アプリケーションの寿命に一度だけコールドスタート
```

### 3. インフラストラクチャの複雑さ

**Gotenberg Docker Compose:**
```yaml
# ❌ Gotenberg - コンテナ管理が必要です
version: '3.8'
services:
  app:
    depends_on:
      - gotenberg
    environment:
      - GOTENBERG_URL=http://gotenberg:3000

  gotenberg:
    image: gotenberg/gotenberg:8
    ports:
      - "3000:3000"
    deploy:
      resources:
        limits:
          memory: 2G
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:3000/health"]
      interval: 30s
```

**IronPDF:**
```yaml
# ✅ IronPDF - 追加のサービスは必要ありません
version: '3.8'
services:
  app:
    environment:
      - IRONPDF_LICENSE_KEY=${IRONPDF_LICENSE_KEY}
# それだけです。Gotenbergサービスなし。ヘルスチェックなし。リソース制限なし。
```

### 4. 冗長なMultipart API

```csharp
// ❌ Gotenberg - すべてのリクエストにmultipart/form-dataの構築が必要です
using var content = new MultipartFormDataContent();
content.Add(new StringContent(html), "files", "index.html");
content.Add(new StringContent("0.5"), "marginTop");
content.Add(new StringContent("0.5"), "marginBottom");
content.Add(new StringContent("0.5"), "marginLeft");
content.Add(new StringContent("0.5"), "marginRight");
content.Add(new StringContent("3s"), "waitDelay");
content.Add(new StringContent("true"), "printBackground");
content.Add(new StringContent("true"), "landscape");

// ✅ IronPDF - クリーンで型指定されたC#プロパティ
var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.MarginTop = 12.7;     // mm
renderer.RenderingOptions.MarginBottom = 12.7;
renderer.RenderingOptions.RenderDelay = 3000;   // ms
renderer.RenderingOptions.PrintHtmlBackgrounds = true;
renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;
```

### 5. エラーハンドリングの複雑さ

```csharp
// ❌ Gotenberg - ネットワーク障害、HTTPエラー、タイムアウトを処理する必要があります
try
{
    var response = await client.PostAsync(gotenbergUrl, content);
    response.EnsureSuccessStatusCode(); // 500？503？タイムアウトはどうですか？
}
catch (HttpRequestException ex) { /* ネットワークエラー */ }
catch (TaskCanceledException ex) { /* タイムアウト */ }
catch (Exception ex) { /* コンテナダウン？ */ }

// ✅ IronPDF - 標準的な.NET例外処理
try
{
    var pdf = renderer.RenderHtmlAsPdf(html);
}
catch (IronPdf.Exceptions.IronPdfException ex)
{
    // クリアで特定の例外
}
```

---

## C#コード例: Gotenberg vs IronPDF

### HTMLからPDFへの変換

**Gotenberg（複雑）:**
```csharp
using System.Net.Http;
using System.Threading.Tasks;

class GotenbergPdf
{
    private readonly HttpClient _client;
    private readonly string _gotenbergUrl = "http://gotenberg:3000";

    public async Task<byte[]> ConvertHtmlToPdf(string html)
    {
        using var content = new MultipartFormDataContent();

        // multipart form dataを構築する必要があります
        content.Add(new StringContent(html), "files", "index.html");

        // 文字列ベースの設定（エラーが発生しやすい）
        content.Add(new StringContent("8.5"), "paperWidth");
        content.Add(new StringContent("11"), "paperHeight");

        var response = await _client.PostAsync(
            $"{_gotenbergUrl}/forms/chromium/convert/html", content);

        // HTTPステータスを手動でチェックする必要があります
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Gotenbergに失敗しました: {error}");
        }

        return await response.Content.ReadAsByteArrayAsync();
    }
}
```

**IronPDF（シンプル）:**
```csharp
using IronPdf;

class IronPdfExample
{
    public byte[] ConvertHtmlToPdf(string html)
    {
        var renderer = new ChromePdfRenderer();
        renderer.RenderingOptions.PaperSize = PdfPaperSize.Letter;

        var pdf = renderer.RenderHtmlAsPdf(html);
        return pdf.BinaryData;
    }
}
```

### カスタムオプションを使用したURLからPDFへの変換

**Gotenberg:**
```csharp
public async Task<byte[]> UrlToPdfGotenberg(string url)
{
    using var content = new MultipartFormDataContent();

    content.Add(new StringContent(url), "url");
    content.Add(new StringContent("true"), "landscape");
    content.Add(new StringContent("1"), "marginTop");
    content.Add(new StringContent("1"), "marginBottom");
    content.Add(new StringContent("3s"), "waitDelay");
    content.Add(new StringContent("true"), "printBackground");

    var response = await _client.PostAsync(
        $"{_gotenbergUrl}/forms/chromium/convert/url", content);
    response.EnsureSuccessStatusCode();

    return await response.Content.ReadAsByteArrayAsync();
}
```

**IronPDF:**
```csharp
public byte[] UrlToPdfIronPdf(string url)
{
    var renderer = new ChromePdfRenderer();

    renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;
    renderer.RenderingOptions.MarginTop = 25.4;      // 1インチをmmで
    renderer.RenderingOptions.MarginBottom = 25.4;
    renderer.RenderingOptions.RenderDelay = 3000;    // 3秒をmsで
    renderer.RenderingOptions.PrintHtmlBackgrounds = true;

    return renderer.RenderUrlAsPdf(url).BinaryData;
}
```

### ヘッダーとフッターを含むPDF

**Gotenberg:**
```csharp
public async Task<byte[]> PdfWithHeaderFooterGotenberg(string html)
{
    using var content = new MultipartFormDataContent();

    content.Add(new StringContent(html), "files", "index.html");

    // ヘッダーとフッターのための別々のファイル
    var headerHtml = "<div style='font-size:10px;'>会社名</div>";
    content.Add(new StringContent(headerHtml), "files", "header.html");

    // ページ番号は特別なCSSクラスを使用します
    var footerHtml = @"<div style='font-size:9px; text-align:center;'>
        ページ <span class='pageNumber'></span> / <span class='totalPages'></span>
    </div>";
    content.Add(new StringContent(footerHtml), "files", "footer.html");

    var response = await _client.PostAsync(
        $"{_gotenbergUrl}/forms/chromium/convert/html", content);

    return await response.Content.ReadAsByteArrayAsync();
}
```

**IronPDF:**
```csharp
public byte[] PdfWithHeaderFooterIronPdf(string html)
{
    var renderer = new ChromePdfRenderer();

    renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter()
    {
        HtmlFragment = "<div style='font-size:10px;'>会社名</div>"
    };

    // ページ番号はプレースホルダーを使用 - よりクリーンな構文
    renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter()
    {
        HtmlFragment = @"<div style='font-size:9px; text-align:center;'>
            ページ {page} / {total-pages}
        </div>"
    };

    return renderer.RenderHtmlAsPdf(html).BinaryData;
}
```

### 複数のPDFのマージ

**Gotenberg:**
```csharp
public async Task<byte[]> MergePdfsGotenberg(List<string> filePaths)
{
    using var content = new MultipartFormDataContent();

    int i = 1;
    foreach (var path in filePaths)
    {
        var bytes = await File.ReadAllBytesAsync(path);
        var fileContent = new ByteArrayContent(bytes);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
        content.Add(fileContent, "files", $"{i++}.pdf");
    }

    var response = await _client.PostAsync(
        $"{_gotenbergUrl}/forms/pdfengines/merge", content);

    return await response.Content.ReadAsByteArrayAsync();
}
```

**IronPDF:**
```csharp
public byte[] MergePdfsIronPdf(List<string> filePaths)
{
    var pdfs = filePaths.Select(PdfDocument.FromFile).ToList();
    var merged = PdfDocument.Merge(pdfs);

    foreach (var pdf in pdfs) pdf.Dispose();

    return merged.BinaryData;
}
```

---

## パフォーマンス比較

### 操作ごとの遅延

| 操作 | Gotenberg (ウォームコンテナ) | Gotenberg (コールドスタート) | IronPDF (最初のレンダリング) | IronPDF (その後) |
|-----------|---------------------------|------------------------|------------------------|---------------------|
| シンプルなHTML | 150-300ms | 2-5秒 | 1-2秒 | 50-150ms |
| 複雑なHTML | 500-1500ms | 3-7秒 | 1.5-3秒 | 200-800ms |
| URLレンダリング | 1-5秒 | 3-10秒 | 1-5秒 | 500ms-3s |
| PDFマージ | 200-500ms | 2-5秒 | 100-300ms | 100-300ms |

### インフラストラクチャコスト

| リソース | Gotenberg | IronPDF |
|----------|-----------|---------|
| 必要なコンテナ | 1-N（スケーリング） | 0 |