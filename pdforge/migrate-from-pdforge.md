---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [pdforge/migrate-from-pdforge.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/pdforge/migrate-from-pdforge.md)
🇯🇵 **日本語:** [pdforge/migrate-from-pdforge.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/pdforge/migrate-from-pdforge.md)

---

# pdforgeからIronPDFへの移行方法は？

## なぜpdforgeから移行するのか？

pdforgeは、ドキュメントを外部サーバーで処理するクラウドベースのPDF生成APIです。これにより初期設定が簡素化されますが、本番アプリケーションにとっては大きな制限が生じます：

### クラウドAPI依存問題

1. **外部サーバー処理**：生成するPDFごとにHTML/データをpdforgeのサーバーに送信する必要があります。ドキュメントがインフラから離れます
2. **プライバシー＆コンプライアンスリスク**：機密データ（契約書、財務報告書、個人情報）がインターネット経由で第三者のサーバーに転送されます
3. **継続的なサブスクリプションコスト**：資産所有権がないまま、月額料金が無期限に蓄積します
4. **インターネット依存**：ネットワークが利用不可の場合、PDFの生成ができません
5. **レート制限**：APIの使用制限が高容量アプリケーションを制限する可能性があります
6. **ネットワーク遅延**：往復時間がPDF生成ごとに数秒追加されます
7. **ベンダーロックイン**：アプリケーションがpdforgeのサービス可用性とAPIの安定性に依存します

### なぜIronPDFなのか？

IronPDFは、アプリケーション内のローカルで全てを処理します：

- **完全なデータプライバシー**：ドキュメントがサーバーを離れることはありません
- **一回限りのライセンス**：永久ライセンスが繰り返しのコストを排除します
- **オフライン機能**：初期設定後はインターネットなしで動作します
- **レート制限なし**：無制限のPDF生成
- **低遅延**：ネットワーク往復なし
- **完全なコントロール**：処理環境を所有します
- **モダンなレンダリング**：完全なCSS3/JSサポートを備えたChromiumベースのエンジン
- **豊富な機能セット**：PDF操作、マージ、テキスト抽出、セキュリティ

---

## 移行概要

| 項目 | pdforge | IronPDF |
|--------|---------|---------|
| 処理場所 | 外部クラウドサーバー | ローカル（あなたのサーバー） |
| 認証 | リクエストごとにAPIキー | 一度だけのライセンスキー |
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
// 以前：pdforge
using PdForge;
using PdForge.Client;
using PdForge.Models;

// 以後：IronPDF
using IronPdf;
using IronPdf.Rendering;
```

---

## APIマッピング参照

### コアクラス

| pdforge | IronPDF | 備考 |
|---------|---------|-------|
| `PdfClient` | `ChromePdfRenderer` | 主要なPDFジェネレータ |
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
| 戻り値タイプ: `byte[]` | `pdf.BinaryData` | 生のバイトを取得 |
| 戻り値タイプ: `Task<byte[]>` | `pdf.Stream` | ストリームとして取得 |

### 設定オプション

| pdforge | IronPDF (RenderingOptions) | 備考 |
|---------|---------------------------|-------|
| `PageSize = PageSize.A4` | `.PaperSize = PdfPaperSize.A4` | 用紙サイズ |
| `PageSize = PageSize.Letter` | `.PaperSize = PdfPaperSize.Letter` | USレター |
| `Orientation = Orientation.Landscape` | `.PaperOrientation = PdfPaperOrientation.Landscape` | 方向 |
| `Orientation = Orientation.Portrait` | `.PaperOrientation = PdfPaperOrientation.Portrait` | 縦向き |
| `MarginTop = 20` | `.MarginTop = 20` | 上マージン |
| `MarginBottom = 20` | `.MarginBottom = 20` | 下マージン |
| `MarginLeft = 15` | `.MarginLeft = 15` | 左マージン |
| `MarginRight = 15` | `.MarginRight = 15` | 右マージン |
| `Header = "text"` | `.TextHeader = new TextHeaderFooter { CenterText = "text" }` | ヘッダー |
| `Footer = "text"` | `.TextFooter = new TextHeaderFooter { CenterText = "text" }` | フッター |
| `HeaderHtml = "<div>..."` | `.HtmlHeader = new HtmlHeaderFooter { HtmlFragment = "..." }` | HTMLヘッダー |
| `FooterHtml = "<div>..."` | `.HtmlFooter = new HtmlHeaderFooter { HtmlFragment = "..." }` | HTMLフッター |
| `JavascriptDelay = 2000` | `.RenderDelay = 2000` | JS待機時間 (ms) |
| `PrintBackground = true` | `.PrintHtmlBackgrounds = true` | 背景レンダリング |
| `Scale = 1.5` | `.Zoom = 150` | ズームパーセンテージ |

### 認証比較

| pdforge | IronPDF |
|---------|---------|
| `new PdfClient("your-api-key")` | `IronPdf.License.LicenseKey = "YOUR-KEY"` |
| リクエストごとの認証 | 起動時に一度だけ |
| コンストラクタ内のAPIキー | グローバルプロパティ |

---