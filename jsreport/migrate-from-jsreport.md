---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [jsreport/migrate-from-jsreport.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/jsreport/migrate-from-jsreport.md)
🇯🇵 **日本語:** [jsreport/migrate-from-jsreport.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/jsreport/migrate-from-jsreport.md)

---

# 移行ガイド: jsreport → IronPDF

## 目次
1. [jsreportから移行する理由](#jsreportから移行する理由)
2. [開始する前に](#開始する前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全なAPIリファレンス](#完全なAPIリファレンス)
5. [コード移行例](#コード移行例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
8. [トラブルシューティング](#トラブルシューティング)
9. [移行チェックリスト](#移行チェックリスト)

---

## jsreportから移行する理由

### jsreportの課題

jsreportは、純粋な.NET環境に属さない複雑さを導入します：

1. **Node.jsの依存関係**：Node.jsのランタイムとバイナリが必要で、インフラの複雑さが増します
2. **外部バイナリ管理**：プラットフォーム固有のバイナリ（Windows、Linux、OSX）をダウンロードして管理する必要があります
3. **別のサーバープロセス**：ユーティリティまたはWebサーバーとして実行される—追加のプロセス管理が必要です
4. **JavaScriptテンプレート**：Handlebars、JsRender、または他のJSテンプレートシステムを学ぶ必要があります
5. **複雑なリクエスト構造**：`RenderRequest`にネストされた`Template`オブジェクトを含む冗長なものです
6. **ライセンス制限**：無料層はテンプレート数を制限し、スケーリングには商用ライセンスが必要です
7. **ストリームベースの出力**：手動でのファイル操作が必要なストリームを返します

### IronPDFの利点

| 機能 | jsreport | IronPDF |
|---------|----------|---------|
| ランタイム | Node.js + .NET | 純粋な.NET |
| バイナリ管理 | 手動 (jsreport.Binaryパッケージ) | 自動 |
| サーバープロセス | 必要 (ユーティリティまたはWebサーバー) | プロセス内 |
| テンプレート | JavaScript (Handlebarsなど) | C# (Razor、文字列補間) |
| APIスタイル | 冗長なリクエストオブジェクト | クリーンな流暢なメソッド |
| 出力 | ストリーム | PdfDocumentオブジェクト |
| PDF操作 | 限定的 | 広範（マージ、分割、編集） |
| 非同期サポート | 主要 | 同期および非同期の両方 |

### 移行の利点

- **Node.jsを排除**：Node.jsのバイナリやバージョンを管理する必要がなくなります
- **よりシンプルなAPI**：20行以上を3-5行のコードに置き換えます
- **ネイティブC#**：JavaScriptの代わりにC#テンプレートを使用します
- **プロセス内**：サーバー管理や起動遅延がありません
- **豊富なPDF操作**：完全なマージ、分割、ウォーターマーク、フォーム、およびセキュリティサポート
- **より小さなフットプリント**：単一のNuGetパッケージ、プラットフォーム固有のバイナリなし

---

## 開始する前に

### 前提条件

1. **.NET環境**：.NET Framework 4.6.2+ または .NET Core 3.1+ / .NET 5+
2. **NuGetアクセス**：NuGetパッケージをインストールできること
3. **IronPDFライセンス**：無料トライアルまたは購入したライセンスキー

### インストール

```bash
# jsreportパッケージを削除
dotnet remove package jsreport.Binary
dotnet remove package jsreport.Binary.Linux
dotnet remove package jsreport.Binary.OSX
dotnet remove package jsreport.Local
dotnet remove package jsreport.Types
dotnet remove package jsreport.Client

# IronPDFをインストール
dotnet add package IronPdf
```

### ライセンス設定

```csharp
// アプリケーションの起動時に追加 (Program.cs または Startup.cs)
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

### jsreportの使用箇所の特定

```bash
# すべてのjsreport参照を検索
grep -r "using jsreport\|LocalReporting\|RenderRequest\|RenderAsync" --include="*.cs" .
grep -r "JsReportBinary\|Template\|Recipe\|Engine\." --include="*.cs" .
```

---

## クイックスタート移行

### 最小限の変更例

**移行前 (jsreport):**
```csharp
using jsreport.Binary;
using jsreport.Local;
using jsreport.Types;

public class JsReportPdfService
{
    private readonly ILocalUtilityReportingService _rs;

    public JsReportPdfService()
    {
        _rs = new LocalReporting()
            .UseBinary(JsReportBinary.GetBinary())
            .AsUtility()
            .Create();
    }

    public async Task<byte[]> GeneratePdfAsync(string html)
    {
        var report = await _rs.RenderAsync(new RenderRequest
        {
            Template = new Template
            {
                Recipe = Recipe.ChromePdf,
                Engine = Engine.None,
                Content = html
            }
        });

        using (var memoryStream = new MemoryStream())
        {
            await report.Content.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public class PdfService
{
    private readonly ChromePdfRenderer _renderer;

    public PdfService()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
        _renderer = new ChromePdfRenderer();
    }

    public byte[] GeneratePdf(string html)
    {
        var pdf = _renderer.RenderHtmlAsPdf(html);
        return pdf.BinaryData;
    }

    // 必要に応じて非同期バージョン
    の公開 async Task<byte[]> GeneratePdfAsync(string html)
    {
        var pdf = await _renderer.RenderHtmlAsPdfAsync(html);
        return pdf.BinaryData;
    }
}
```

---

## 完全なAPIリファレンス

### 名前空間マッピング

| jsreport 名前空間 | IronPDF相当 |
|-------------------|-------------------|
| `jsreport.Local` | `IronPdf` |
| `jsreport.Types` | `IronPdf` |
| `jsreport.Binary` | _(不要)_ |
| `jsreport.Client` | _(不要)_ |

### クラスマッピング

| jsreport クラス | IronPDF相当 | 備考 |
|---------------|-------------------|-------|
| `LocalReporting` | `ChromePdfRenderer` | メインレンダラー |
| `ReportingService` | `ChromePdfRenderer` | 同じクラス |
| `RenderRequest` | メソッドパラメータ | ラッパー不要 |
| `Template` | メソッドパラメータ | ラッパー不要 |
| `Chrome` | `RenderingOptions` | Chromeオプション |
| `Report` | `PdfDocument` | 結果オブジェクト |
| `Engine` | _(不要)_ | テンプレートにはC#を使用 |

### メソッドマッピング

| jsreport メソッド | IronPDF相当 | 備考 |
|----------------|-------------------|-------|
| `LocalReporting().UseBinary().AsUtility().Create()` | `new ChromePdfRenderer()` | ワンライナー |
| `LocalReporting().UseBinary().AsWebServer().Create()` | `new ChromePdfRenderer()` | サーバー不要 |
| `rs.RenderAsync(request)` | `renderer.RenderHtmlAsPdf(html)` | 直接呼び出し |
| `rs.StartAsync()` | _(不要)_ | プロセス内 |
| `rs.KillAsync()` | _(不要)_ | 自動クリーンアップ |
| `report.Content.CopyTo(stream)` | `pdf.SaveAs(path)` または `pdf.BinaryData` | 直接アクセス |

### RenderRequestプロパティマッピング

| jsreport テンプレートプロパティ | IronPDF相当 | 備考 |
|---------------------------|-------------------|-------|
| `Template.Content` | `RenderHtmlAsPdf()`への最初のパラメータ | 直接のHTML文字列 |
| `Template.Recipe` | _(不要)_ | 常にChromePdf |
| `Template.Engine` | _(不要)_ | C#テンプレートを使用 |
| `Template.Chrome.HeaderTemplate` | `RenderingOptions.HtmlHeader` | HTMLヘッダー |
| `Template.Chrome.FooterTemplate` | `RenderingOptions.HtmlFooter` | HTMLフッター |
| `Template.Chrome.DisplayHeaderFooter` | _(自動)_ | ヘッダー自動有効化 |
| `Template.Chrome.MarginTop` | `RenderingOptions.MarginTop` | ミリメートル単位 |
| `Template.Chrome.MarginBottom` | `RenderingOptions.MarginBottom` | ミリメートル単位 |
| `Template.Chrome.MarginLeft` | `RenderingOptions.MarginLeft` | ミリメートル単位 |
| `Template.Chrome.MarginRight` | `RenderingOptions.MarginRight` | ミリメートル単位 |
| `Template.Chrome.Format` | `RenderingOptions.PaperSize` | 列挙値 |
| `Template.Chrome.Landscape` | `RenderingOptions.PaperOrientation` | 列挙値 |
| `Template.Chrome.MediaType` | `RenderingOptions.CssMediaType` | 画面または印刷 |
| `Template.Chrome.WaitForNetworkIdle` | `RenderingOptions.WaitFor.NetworkIdle()` | 待機戦略 |
| `Template.Chrome.WaitForJS` | `RenderingOptions.WaitFor.JavaScript()` | JSの待機 |
| `Template.Chrome.PrintBackground` | `RenderingOptions.PrintHtmlBackgrounds` | 背景印刷 |
| `Template.Chrome.Scale` | `RenderingOptions.Zoom` | ズームパーセンテージ |
| `Template.Chrome.Url` | `renderer.RenderUrlAsPdf(url)` | 別のメソッド |

### プレースホルダーマッピング (ヘッダー/フッター)

| jsreport プレースホルダー | IronPDF プレースホルダー | 備考 |
|---------------------|-------------------|-------|
| `{#pageNum}` | `{page}` | 現在のページ |
| `{#numPages}` | `{total-pages}` | 合計ページ数 |
| `{#timestamp}` | `{date}` | 現在の日付 |
| `{#title}` | `{html-title}` | ドキュメントのタイトル |
| `{#url}` | `{url}` | ドキュメントのURL |

### 用紙サイズマッピング

| jsreport フォーマット | IronPDF PaperSize |
|-----------------|-------------------|
| `"A4"` | `PdfPaperSize.A4` |
| `"Letter"` | `PdfPaperSize.Letter` |
| `"Legal"` | `PdfPaperSize.Legal` |
| `"A3"` | `PdfPaperSize.A3` |
| `"A5"` | `PdfPaperSize.A5` |

---