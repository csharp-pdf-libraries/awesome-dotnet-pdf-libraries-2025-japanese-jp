# Kaizen.io HTML-to-PDFからIronPDFへの移行方法は？

## 目次
1. [Kaizen.ioから移行する理由](#kaizenioから移行する理由)
2. [始める前に](#始める前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全なAPIリファレンス](#完全なapiリファレンス)
5. [コード移行例](#コード移行例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
8. [トラブルシューティング](#トラブルシューティング)
9. [移行チェックリスト](#移行チェックリスト)

---

## Kaizen.ioから移行する理由

### クラウドベースAPIの課題

Kaizen.io HTML-to-PDFは、他のクラウドベースのPDFサービスと同様に、以下の制限があります：

1. **クラウド依存**：常時インターネット接続と外部サービスの可用性が必要
2. **データプライバシーの懸念**：機密HTMLコンテンツが第三者のサーバーに送信される
3. **ネットワークレイテンシ**：PDF生成ごとにネットワーク往復遅延が発生
4. **リクエストごとの価格設定**：使用量に直接比例するコスト
5. **レート制限**：高トラフィック期間中のAPIスロットリング
6. **カスタマイズの限定**：クラウドAPIが公開するものに制約される
7. **ベンダーロックイン**：APIの変更やサービスの中止リスク

### IronPDFの利点

| 機能 | Kaizen.io | IronPDF |
|---------|-----------|---------|
| 処理 | クラウド（外部サーバー） | ローカル（プロセス内） |
| データプライバシー | データが外部に送信される | データはインフラ内に留まる |
| レイテンシ | ネットワーク往復（100-500ms+） | ローカル処理（50-200ms） |
| 可用性 | 外部サービスに依存 | 100%自分のコントロール下 |
| 価格 | リクエストごとまたはサブスクリプション | 一回払いまたは年間ライセンス |
| オフラインモード | 不可能 | 完全機能 |
| カスタマイズ | APIオプションに限定 | 完全なChrome/レンダリング制御 |
| JavaScript | 限定サポート | 完全なChromium実行 |

### 移行の利点

- **クラウド依存の排除**：PDF生成にインターネットは不要
- **完全なデータプライバシー**：機密文書がネットワークを離れることはない
- **低レイテンシ**：ネットワークオーバーヘッドなしで2-10倍高速
- **予測可能なコスト**：使用量ベースの価格設定ではない固定ライセンス
- **完全なコントロール**：レンダリングの各側面を設定可能
- **オフライン機能**：ネットワーク接続なしで動作
- **レート制限なし**：ハードウェアが許す限り多くのPDFを生成可能

---

## 始める前に

### 前提条件

1. **.NET環境**：.NET Framework 4.6.2+または.NET Core 3.1+ / .NET 5+
2. **NuGetアクセス**：NuGetパッケージをインストールできること
3. **IronPDFライセンス**：無料トライアルまたは購入したライセンスキー

### インストール

```bash
# Kaizen.ioパッケージを削除（パッケージ名は異なる場合があります）
dotnet remove package Kaizen.HtmlToPdf
dotnet remove package Kaizen.IO.HtmlToPdf

# IronPDFをインストール
dotnet add package IronPdf
```

### ライセンス設定

```csharp
// アプリケーション起動時に追加（Program.csまたはStartup.cs）
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

### Kaizen.ioの使用箇所を特定

```bash
# すべてのKaizen.io参照を検索
grep -r "using Kaizen\|HtmlToPdfConverter\|ConversionOptions" --include="*.cs" .
grep -r "ConvertUrl\|ConvertHtml\|Kaizen" --include="*.cs" .
```

---

## クイックスタート移行

### 最小変更例

**移行前（Kaizen.io）：**
```csharp
using Kaizen.IO;

public class KaizenPdfService
{
    private readonly HtmlToPdfConverter _converter;
    private readonly string _apiKey;

    public KaizenPdfService(string apiKey)
    {
        _apiKey = apiKey;
        _converter = new HtmlToPdfConverter(apiKey);
    }

    public byte[] GeneratePdf(string html)
    {
        var options = new ConversionOptions
        {
            PageSize = PageSize.A4,
            Orientation = Orientation.Portrait,
            MarginTop = 20,
            MarginBottom = 20
        };

        return _converter.Convert(html, options);
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
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
        _renderer = new ChromePdfRenderer();
        _renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
        _renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Portrait;
        _renderer.RenderingOptions.MarginTop = 20;
        _renderer.RenderingOptions.MarginBottom = 20;
    }

    public byte[] GeneratePdf(string html)
    {
        var pdf = _renderer.RenderHtmlAsPdf(html);
        return pdf.BinaryData;
    }
}
```

---

## 完全なAPIリファレンス

### クラスマッピング

| Kaizen.ioクラス | IronPDF相当 | 備考 |
|-----------------|-------------------|-------|
| `HtmlToPdfConverter` | `ChromePdfRenderer` | メインコンバーター |
| `ConversionOptions` | `ChromePdfRenderOptions` | `RenderingOptions`経由 |
| `HeaderOptions` | `HtmlHeaderFooter` | HTMLヘッダー |
| `FooterOptions` | `HtmlHeaderFooter` | HTMLフッター |
| `PageSize` | `PdfPaperSize` | 用紙サイズenum |
| `Orientation` | `PdfPaperOrientation` | 方向enum |

### メソッドマッピング

| Kaizen.ioメソッド | IronPDF相当 | 備考 |
|------------------|-------------------|-------|
| `converter.Convert(html)` | `renderer.RenderHtmlAsPdf(html)` | PdfDocumentを返す |
| `converter.Convert(html, options)` | `RenderingOptions`を設定して`RenderHtmlAsPdf()` | レンダラー上のオプション |
| `converter.ConvertUrl(url)` | `renderer.RenderUrlAsPdf(url)` | 直接URLサポート |
| `converter.ConvertUrl(url, options)` | `RenderingOptions`を設定して`RenderUrlAsPdf()` | レンダラー上のオプション |
| `converter.ConvertFile(path)` | `renderer.RenderHtmlFileAsPdf(path)` | ファイルベースの変換 |
| `converter.ConvertAsync(...)` | `renderer.RenderHtmlAsPdfAsync(...)` | 非同期バージョン |

### ConversionOptionsプロパティマッピング

| Kaizen.ioプロパティ | IronPDF相当 | 備考 |
|-------------------|-------------------|-------|
| `PageSize` | `RenderingOptions.PaperSize` | Enum値 |
| `Orientation` | `RenderingOptions.PaperOrientation` | 縦/横 |
| `MarginTop` | `RenderingOptions.MarginTop` | ミリメートル単位 |
| `MarginBottom` | `RenderingOptions.MarginBottom` | ミリメートル単位 |
| `MarginLeft` | `RenderingOptions.MarginLeft` | ミリメートル単位 |
| `MarginRight` | `RenderingOptions.MarginRight` | ミリメートル単位 |
| `Header` | `RenderingOptions.HtmlHeader` | HTMLベース |
| `Footer` | `RenderingOptions.HtmlFooter` | HTMLベース |
| `Header.HtmlContent` | `HtmlHeader.HtmlFragment` | ヘッダーHTML |
| `Footer.HtmlContent` | `HtmlFooter.HtmlFragment` | フッターHTML |
| `BaseUrl` | `RenderingOptions.BaseUrl` | 相対リソース用 |
| `Timeout` | `RenderingOptions.Timeout` | ミリ秒単位 |
| `EnableJavaScript` | `RenderingOptions.EnableJavaScript` | デフォルトtrue |
| `WaitForComplete` | `RenderingOptions.WaitFor` | 待機戦略 |
| `PrintBackground` | `RenderingOptions.PrintHtmlBackgrounds` | 背景印刷 |
| `Scale` | `RenderingOptions.Zoom` | ズームパーセンテージ |

### PageSizeマッピング

| Kaizen.io PageSize | IronPDF PaperSize |
|-------------------|-------------------|
| `PageSize.A4` | `PdfPaperSize.A4` |
| `PageSize.Letter` | `PdfPaperSize.Letter` |
| `PageSize.Legal` | `PdfPaperSize.Legal` |
| `PageSize.A3` | `PdfPaperSize.A3` |
| `PageSize.A5` | `PdfPaperSize.A5` |
| カスタム寸法 | `SetCustomPaperSizeInMillimeters()` |

### プレースホルダーマッピング

| Kaizen.ioプレースホルダー | IronPDFプレースホルダー | 備考 |
|----------------------|-------------------|-------|
| `{page}` | `{page}` | 現在のページ |
| `{total}` | `{total-pages}` | 合計ページ数 |
| `{date}` | `{date}` | 現在の日付 |
| `{time}` | `{time}` | 現在の時間 |
| `{title}` | `{html-title}` | 文書のタイトル |
| `{url}` | `{url}` | 文書のURL |

---