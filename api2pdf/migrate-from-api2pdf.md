---
**  (Japanese Translation)**

 **English:** [api2pdf/migrate-from-api2pdf.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/api2pdf/migrate-from-api2pdf.md)
 **:** [api2pdf/migrate-from-api2pdf.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/api2pdf/migrate-from-api2pdf.md)

---
# Api2pdfからIronPDFへの移行方法は？

## 目次
1. [Api2pdfからIronPDFへ移行する理由](#api2pdfからironpdfへ移行する理由)
2. [開始する前に](#開始する前に)
3. [クイックスタート（5分）](#クイックスタート5分)
4. [完全なAPIリファレンス](#完全なapiリファレンス)
5. [コード移行例](#コード移行例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
8. [トラブルシューティングガイド](#トラブルシューティングガイド)
9. [移行チェックリスト](#移行チェックリスト)
10. [追加リソース](#追加リソース)

---

## Api2pdfからIronPDFへ移行する理由

### Api2pdfとのセキュリティおよびコンプライアンスリスク

Api2pdfは、**機密HTMLおよびドキュメントをサードパーティのサーバーに送信**して処理するクラウドベースのサービスとして運用されています。これにより重大な懸念が生じます：

| リスク | Api2pdf | IronPDF |
|------|---------|---------|
| **データ転送** | すべてのコンテンツが外部サーバーに送信される | あなたのインフラストラクチャ上でローカルに処理される |
| **GDPRコンプライアンス** | データが管轄区域を越える | データはあなたの環境を離れない |
| **HIPAAコンプライアンス** | PHIが外部に送信される | PHIはあなたのシステム内に留まる |
| **SOC 2** | サードパーティ依存 | データ処理の完全な制御 |
| **PCI DSS** | カードデータが潜在的に露出する | 外部への転送なし |

### コスト比較

Api2pdfは**変換ごとに**無期限で課金されるのに対し、IronPDFは**一度きりの永久ライセンス**を提供します：

| ボリューム | Api2pdf（年間） | IronPDF（一回きり） |
|--------|-----------------|-------------------|
| 月10,000 PDF | 約$600/年 | $749（ライト） |
| 月50,000 PDF | 約$3,000/年 | $749（ライト） |
| 月100,000 PDF | 約$6,000/年 | $1,499（プラス） |
| 月500,000 PDF | 約$30,000/年 | $2,999（プロフェッショナル） |

*Api2pdfの価格：約$0.005/変換。数ヶ月でIronPDFが元を取る。*

### IronPDFの技術的利点

1. **ネットワーク遅延なし**：API呼び出しの秒単位ではなく、ミリ秒単位でPDFを生成
2. **外部サービスへの依存なし**：オフライン、エアギャップ環境で動作
3. **最新のChromiumエンジン**：完全なCSS3、JavaScript、Flexbox、Gridサポート
4. **完全な制御**：ヘッダー、フッター、ウォーターマーク、暗号化をローカルで設定可能
5. **同期＆非同期**：アプリに合ったプログラミングモデルを選択
6. **1000万以上のNuGetダウンロード**：世界中のプロダクション環境で実戦テスト済み

---

## 開始する前に

### 前提条件

- **.NET Framework 4.6.2+** または **.NET Core 3.1+** または **.NET 5/6/7/8/9**
- **Visual Studio 2019+** または **VS Code** にC#拡張機能があること
- **NuGetパッケージマネージャー** へのアクセス
- 移行する**既存のApi2pdfコードベース**

### すべてのApi2pdf参照を見つける

移行する前に、コードベース内のすべてのApi2pdfの使用を特定します：

```bash
# すべてのApi2pdf usingステートメントを検索
grep -r "using Api2Pdf" --include="*.cs" .

# Api2PdfClientのインスタンス化を検索
grep -r "Api2PdfClient\|new Api2Pdf" --include="*.cs" .

# すべての非同期API呼び出しを検索
grep -r "FromHtmlAsync\|FromUrlAsync\|MergePdfs\|LibreOffice" --include="*.cs" .
```

### 予想される変更点

| Api2pdfパターン | 必要な変更 |
|-----------------|-----------------|
| APIキー認証 | 完全に削除—IronPDFはローカルで実行 |
| 非同期HTTP呼び出し | デフォルトで同期（非同期はオプション） |
| URLベースのPDF取得 | 直接のインメモリPDFオブジェクト |
| 通話ごとの価格/メータリング | メータリング不要 |
| 複数のレンダリングエンジン | 単一のChromiumエンジン（品質向上） |
| クラウドスケーリング | 代わりにあなたのインフラストラクチャがスケール |

---

## クイックスタート（5分）

### ステップ1：NuGetパッケージを置き換える

```bash
# Api2pdfパッケージを削除
dotnet remove package Api2Pdf

# IronPDFをインストール
dotnet add package IronPdf
```

またはパッケージマネージャーコンソール経由で：
```powershell
Uninstall-Package Api2Pdf
Install-Package IronPdf
```

### ステップ2：名前空間を更新する

```csharp
// ❌ これらを削除
using Api2Pdf;
using Api2Pdf.DotNet;

// ✅ これらを追加
using IronPdf;
using IronPdf.Rendering;
```

### ステップ3：最初のPDFを変換する

**変換前（Api2pdf）：**
```csharp
var a2pClient = new Api2PdfClient("YOUR_API_KEY");
var response = await a2pClient.HeadlessChrome.FromHtmlAsync("<h1>こんにちは</h1>");
var pdfUrl = response.Pdf;
// その後、URLからPDFをダウンロード...
```

**変換後（IronPDF）：**
```csharp
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1>こんにちは</h1>");
pdf.SaveAs("output.pdf");
// PDFはすぐに準備完了！
```

### ステップ4：ライセンスキーを設定する（開発用はオプション）

```csharp
// アプリケーションの起動時に一度設定
IronPdf.License.LicenseKey = "YOUR-IRONPDF-LICENSE-KEY";

// またはappsettings.jsonで：
// { "IronPdf.LicenseKey": "YOUR-IRONPDF-LICENSE-KEY" }
```

---

## 完全なAPIリファレンス

### 名前空間マッピング

| Api2pdf名前空間 | IronPDF名前空間 | 目的 |
|-------------------|-------------------|---------|
| `Api2Pdf` | `IronPdf` | コアPDF機能 |
| `Api2Pdf.DotNet` | `IronPdf` | .NET固有のクラス |
| N/A | `IronPdf.Rendering` | レンダリングオプション |
| N/A | `IronPdf.Editing` | PDF編集機能 |
| N/A | `IronPdf.Forms` | PDFフォーム処理 |
| N/A | `IronPdf.Signing` | デジタル署名 |

### コアクラスマッピング

| Api2pdfクラス | IronPDF相当 | 備考 |
|---------------|-------------------|-------|
| `Api2PdfClient` | `ChromePdfRenderer` | メインのレンダリングクラス |
| `Api2PdfResult` | `PdfDocument` | PDFを表す |
| `HeadlessChromeOptions` | `ChromePdfRenderOptions` | レンダリング設定 |
| `WkhtmlHtmlToPdfRequest` | `ChromePdfRenderOptions` | IronPDFはすべてのレンダリングにChromiumを使用 |
| N/A | `HtmlToPdf` | レガシーレンダラー（ChromePdfRendererを使用） |

### 完全なメソッドマッピング

#### HTMLからPDFへの変換

| Api2pdfメソッド | IronPDFメソッド | 備考 |
|----------------|----------------|-------|
| `HeadlessChrome.FromHtmlAsync(html)` | `renderer.RenderHtmlAsPdf(html)` | デフォルトで同期 |
| `HeadlessChrome.FromHtmlAsync(html, options)` | `renderer.RenderHtmlAsPdf(html)` | 最初にレンダラーにオプションを設定 |
| `WkHtml.FromHtmlAsync(html)` | `renderer.RenderHtmlAsPdf(html)` | wkhtmltopdfの代わりにChromiumを使用 |
| `WkHtml.FromHtmlAsync(html, options)` | `renderer.RenderHtmlAsPdf(html)` | RenderingOptions経由で設定 |

#### URLからPDFへの変換

| Api2pdfメソッド | IronPDFメソッド | 備考 |
|----------------|----------------|-------|
| `HeadlessChrome.FromUrlAsync(url)` | `renderer.RenderUrlAsPdf(url)` | フルページキャプチャ |
| `HeadlessChrome.FromUrlAsync(url, options)` | `renderer.RenderUrlAsPdf(url)` | RenderingOptions経由で設定 |
| `WkHtml.FromUrlAsync(url)` | `renderer.RenderUrlAsPdf(url)` | より良い結果のためにChromiumを使用 |

#### ドキュメント変換（LibreOffice）

| Api2pdfメソッド | IronPDFメソッド | 備考 |
|----------------|----------------|-------|
| `LibreOffice.ConvertAsync(url)` | `PdfDocument.FromFile(path)` | 既存のPDFを開く |
| N/A | `renderer.RenderHtmlAsPdf(html)` | HTMLテーブルをPDFに変換 |

*注：Officeドキュメントの変換には、IronWord、IronXLを検討するか、Api2pdf呼び出しのサブセットを維持してください。*

#### PDFのマージ

| Api2pdfメソッド | IronPDFメソッド | 備考 |
|----------------|----------------|-------|
| `PdfSharp.MergePdfsAsync(urls)` | `PdfDocument.Merge(pdfs)` | PdfDocumentオブジェクトをマージ |
| `PdfSharp.MergePdfsAsync(request)` | `PdfDocument.Merge(pdf1, pdf2, ...)` | 複数のPDFを受け入れる |

#### PDF出力

| Api2pdfプロパティ | IronPDFメソッド | 備考 |
|------------------|----------------|-------|
| `response.Pdf` (URL) | `pdf.SaveAs(path)` | ファイルに保存 |
| `response.Pdf` (ダウンロード) | `pdf.BinaryData` | バイト配列として取得 |
| N/A | `pdf.Stream` | MemoryStreamとして取得 |
| N/A | `pdf.SaveAsAsync(path)` | 非同期ファイル保存 |

#### PDFセキュリティ

| Api2pdfメソッド | IronPDFプロパティ | 備考 |
|----------------|------------------|-------|
| `PdfSharp.SetPasswordAsync(url, password)` | `pdf.SecuritySettings.OwnerPassword` | オーナーパスワード |
| N/A | `pdf.SecuritySettings.UserPassword` | ユーザーパスワード |
| N/A | `pdf.SecuritySettings.AllowUserPrinting` | 印刷許可 |
| N/A | `pdf.SecuritySettings.AllowUserCopyPasteContent` | コピー許可 |
| N/A | `pdf.SecuritySettings.AllowUserEdits` | 編集許可 |

#### 画像変換

| Api2pdfメソッド | IronPDFメソッド | 備考 |
|----------------|----------------|-------|
| `HeadlessChrome.HtmlToImageAsync(html)` | `pdf.RasterizeToImageFiles(path)` | ページを画像としてレンダリング |
| `HeadlessChrome.UrlToImageAsync(url)` | `pdf.ToBitmap()` | System.Drawing.Bitmapを取得 |

#### レンダリングオプション

| Api2pdfオプション | IronPDFオプション | 備考 |
|----------------|----------------|-------|
| `Landscape = true` | `RenderingOptions.PaperOrientation = Landscape` | ページの向き |
| `PageSize = "A4"` | `RenderingOptions.PaperSize = PdfPaperSize.A4` | 用紙サイズ |
| `PrintBackground = true` | `RenderingOptions.PrintHtmlBackgrounds = true` | 背景色 |
| `MarginTop`, etc. | `RenderingOptions.MarginTop`, etc. | mm単位のマージン |
| `Delay = 5000` | `RenderingOptions.WaitFor.RenderDelay(5000)` | レンダリング前の待機 |
| `Scale = 0.8` | `RenderingOptions.Zoom = 80` | ズーム率 |
| N/A | `RenderingOptions.CssMediaType = Print` | CSSメディアタイプ |
| N/A | `RenderingOptions.EnableJavaScript = true` | JS実行 |

---

この移行ガイドは、Api2pdfからIronPDFへの移行をカバーしています。追加のサポートが必要な場合は、IronPDFサポートに連絡するか、公式ドキュメントを参照してください。