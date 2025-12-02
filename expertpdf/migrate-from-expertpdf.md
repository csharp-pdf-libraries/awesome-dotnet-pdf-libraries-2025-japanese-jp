---
**  (Japanese Translation)**

 **English:** [expertpdf/migrate-from-expertpdf.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/expertpdf/migrate-from-expertpdf.md)
 **:** [expertpdf/migrate-from-expertpdf.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/expertpdf/migrate-from-expertpdf.md)

---
# ExpertPdfからIronPDFへの移行方法は？

## 目次

1. [ExpertPdfからIronPDFへ移行する理由](#ExpertPdfからIronPDFへ移行する理由)
2. [始める前に](#始める前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全なAPIリファレンス](#完全なAPIリファレンス)
5. [コード例](#コード例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
8. [トラブルシューティング](#トラブルシューティング)
9. [移行チェックリスト](#移行チェックリスト)

---

## ExpertPdfからIronPDFへ移行する理由

### ExpertPdfの問題点

1. **2018年以降ドキュメントが更新されていない**: ExpertPdfのドキュメントは6年以上更新されておらず、現在の情報、例、ベストプラクティスを見つけることがますます困難になっています。

2. **古いChromeバージョンに依存**: ExpertPdfはレンダリングに古いChromeバージョンを使用しています。最新のCSS3機能（Flexbox、Grid、CSS変数）が正しくレンダリングされない可能性があり、セキュリティパッチが適用されません。

3. **レガシーテクノロジーに対するプレミアム価格**: ExpertPdfはライセンスごとに550〜1,200ドルのプレミアム価格を請求していますが、古いレンダリング技術を提供しています。

4. **断片化された製品スイート**: ExpertPdfは異なる機能のために別々のパッケージを販売しています：
   - HtmlToPdfコンバーター
   - PDFマージャー
   - PDFセキュリティ
   - PDFスプリッター
   - PDFからイメージへの変換

   各機能には別々のライセンスが必要です。

5. **限定的な現代の.NETサポート**: ExpertPdfには.NET Coreパッケージがありますが、最新の.NETバージョンやプラクティスには追いついていません。

### ExpertPdfとIronPDFの比較

| 項目 | ExpertPdf | IronPDF |
|--------|-----------|---------|
| **ドキュメント** | 2018年以降更新なし | 継続的に更新 |
| **レンダリングエンジン** | 古いChrome | 最新のChromium |
| **CSSサポート** | 限定的なCSS3 | 完全なCSS3（Flexbox、Grid） |
| **価格** | $550-$1,200 | 競争力のある価格 |
| **更新頻度** | 不定期 | 月次リリース |
| **製品モデル** | 断片化（5+ DLL） | オールインワンライブラリ |
| **現代の.NET** | 限定的 | .NET 6/7/8/9+ ネイティブ |
| **非同期サポート** | 限定的 | 完全なasync/await |
| **セキュリティ更新** | 不定期 | 定期的なパッチ |

### 主な移行の利点

1. **最新のレンダリング**: 最新のChromiumエンジンによるピクセル完璧な出力
2. **オールインワンパッケージ**: PDF生成、マージ、セキュリティ、抽出を1つのNuGetで
3. **アクティブな開発**: 新機能とセキュリティパッチを含む月次の更新
4. **より良いドキュメント**: 包括的なチュートリアルと例
5. **真のクロスプラットフォーム**: Windows、Linux、macOS、Dockerサポート
6. **現代の.NET**: .NET 6/7/8/9のネイティブサポート

---

## 始める前に

### 1. ExpertPdfの使用状況を調査する

使用中のExpertPdfコンポーネントをすべて特定します：

```bash
# ExpertPdfの参照をすべて見つける
grep -r "ExpertPdf\|PdfConverter\|PDFMerge\|PdfSecurityManager" --include="*.cs" .

# NuGetパッケージをチェック
dotnet list package | grep -i "ExpertPdf"
```

**一般的なExpertPdfパッケージ：**
- `ExpertPdf.HtmlToPdf` - HTMLからPDFへの変換
- `ExpertPdf.PDFMerge` - PDFのマージ
- `ExpertPdf.PDFSecurity` - 暗号化とパスワード
- `ExpertPdf.PDFSplit` - PDFの分割
- `ExpertPdf.PdfToImage` - PDFからイメージへの変換

### 2. 現在の機能を文書化する

使用しているExpertPdfの機能のチェックリストを作成します：

- [ ] HTMLからPDFへ（`PdfConverter`）
- [ ] URLからPDFへ（`GetPdfBytesFromUrl`）
- [ ] PDFのマージ（`PDFMerge`）
- [ ] PDFのセキュリティ（`PdfSecurityOptions`）
- [ ] ヘッダーとフッター（`PdfHeaderOptions`、`PdfFooterOptions`）
- [ ] ページ番号（`&p;` と `&P;` トークン）
- [ ] カスタムページサイズ
- [ ] PDFの分割
- [ ] PDFからイメージへの変換

### 3. IronPDFをセットアップする

```bash
# すべてのExpertPdfパッケージを削除
dotnet remove package ExpertPdf.HtmlToPdf
dotnet remove package ExpertPdf.PDFMerge
dotnet remove package ExpertPdf.PDFSecurity
dotnet remove package ExpertPdf.PDFSplit
dotnet remove package ExpertPdf.PdfToImage

# IronPDFをインストール（すべての機能を含む）
dotnet add package IronPdf
```

### 4. ライセンスを設定する

```csharp
// アプリケーションの起動時に
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

---

## クイックスタート移行

### 核心的なパターンの変更

ExpertPdfは`PdfConverter`を使用して直接ファイルメソッドを使用します。IronPDFは`ChromePdfRenderer`を使用して`PdfDocument`オブジェクトを返します。

**ExpertPdfのパターン：**
```csharp
PdfConverter pdfConverter = new PdfConverter();
pdfConverter.LicenseKey = "LICENSE";
pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
pdfConverter.SavePdfFromUrlToFile(url, "output.pdf");
```

**IronPDFのパターン：**
```csharp
IronPdf.License.LicenseKey = "LICENSE";
var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
var pdf = renderer.RenderUrlAsPdf(url);
pdf.SaveAs("output.pdf");
```

### 最小限の移行例

**移行前（ExpertPdf）：**
```csharp
using ExpertPdf.HtmlToPdf;

public class PdfService
{
    public void GenerateReport(string html)
    {
        PdfConverter pdfConverter = new PdfConverter();
        pdfConverter.LicenseKey = "EXPERTPDF-LICENSE";
        pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
        pdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Portrait;
        pdfConverter.PdfDocumentOptions.MarginTop = 20;
        pdfConverter.PdfDocumentOptions.MarginBottom = 20;

        byte[] pdfBytes = pdfConverter.GetPdfBytesFromHtmlString(html);
        File.WriteAllBytes("report.pdf", pdfBytes);
    }
}
```

**移行後（IronPDF）：**
```csharp
using IronPdf;

public class PdfService
{
    public PdfService()
    {
        IronPdf.License.LicenseKey = "IRONPDF-LICENSE";
    }

    public void GenerateReport(string html)
    {
        var renderer = new ChromePdfRenderer();
        renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
        renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Portrait;
        renderer.RenderingOptions.MarginTop = 20;
        renderer.RenderingOptions.MarginBottom = 20;

        var pdf = renderer.RenderHtmlAsPdf(html);
        pdf.SaveAs("report.pdf");
    }
}
```

---

## 完全なAPIリファレンス

### 名前空間のマッピング

| ExpertPdfの名前空間 | IronPDFの同等物 |
|--------------------|-------------------|
| `ExpertPdf.HtmlToPdf` | `IronPdf` |
| `ExpertPdf.PDFMerge` | `IronPdf` |
| `ExpertPdf.PDFSecurity` | `IronPdf` |
| `ExpertPdf.PDFSplit` | `IronPdf` |

### コアクラスのマッピング

| ExpertPdfのクラス | IronPDFの同等物 | 備考 |
|----------------|-------------------|-------|
| `PdfConverter` | `ChromePdfRenderer` | 主要な変換クラス |
| `PdfDocumentOptions` | `ChromePdfRenderOptions` | `RenderingOptions`を介して |
| `PdfSecurityOptions` | `PdfDocument.SecuritySettings` | |
| `PdfHeaderOptions` | `HtmlHeaderFooter` | HTMLを設定可能 |
| `PdfFooterOptions` | `HtmlHeaderFooter` | HTMLを設定可能 |
| `PDFMerge` | `PdfDocument.Merge()` | 静的メソッド |
| `PdfSecurityManager` | `PdfDocument.SecuritySettings` | |
| `ImgConverter` | `PdfDocument.RasterizeToImageFiles()` | |

### メソッドのマッピング

| ExpertPdfのメソッド | IronPDFのメソッド | 備考 |
|-----------------|----------------|-------|
| `pdfConverter.GetPdfBytesFromHtmlString(html)` | `renderer.RenderHtmlAsPdf(html).BinaryData` | |
| `pdfConverter.GetPdfBytesFromUrl(url)` | `renderer.RenderUrlAsPdf(url).BinaryData` | |
| `pdfConverter.GetPdfBytesFromHtmlFile(path)` | `renderer.RenderHtmlFileAsPdf(path).BinaryData` | |
| `pdfConverter.SavePdfFromUrlToFile(url, path)` | `renderer.RenderUrlAsPdf(url).SaveAs(path)` | 二段階 |
| `pdfConverter.SavePdfFromHtmlStringToFile(html, path)` | `renderer.RenderHtmlAsPdf(html).SaveAs(path)` | 二段階 |
| `pdfMerge.AppendPDFFile(path)` | `PdfDocument.Merge()` | 静的マージ |
| `pdfMerge.SaveMergedPDFToFile(path)` | `merged.SaveAs(path)` | |
| `pdfMerge.AppendImageFile(path)` | HTMLとして画像をレンダリング | |

### オプションのマッピング

| ExpertPdfのオプション | IronPDFのRenderingOptions | 備考 |
|-----------------|-------------------------|-------|
| `PdfDocumentOptions.PdfPageSize = PdfPageSize.A4` | `PaperSize = PdfPaperSize.A4` | |
| `PdfDocumentOptions.PdfPageSize = PdfPageSize.Letter` | `PaperSize = PdfPaperSize.Letter` | |
| `PdfDocumentOptions.PdfPageOrientation = Portrait` | `PaperOrientation = PdfPaperOrientation.Portrait` | |
| `PdfDocumentOptions.PdfPageOrientation = Landscape` | `PaperOrientation = PdfPaperOrientation.Landscape` | |
| `PdfDocumentOptions.MarginTop` | `MarginTop` | 同じプロパティ名 |
| `PdfDocumentOptions.MarginBottom` | `MarginBottom` | |
| `PdfDocumentOptions.MarginLeft` | `MarginLeft` | |
| `PdfDocumentOptions.MarginRight` | `MarginRight` | |
| `PdfDocumentOptions.ShowHeader = true` | `HtmlHeader`プロパティを設定 | HtmlHeaderFooterを使用 |
| `PdfDocumentOptions.ShowFooter = true` | `HtmlFooter`プロパティを設定 | HtmlHeaderFooterを使用 |
| `PdfDocumentOptions.PdfCompressionLevel` | `CompressImages` | 異なるアプローチ |
| `PdfDocumentOptions.LiveUrlsEnabled` | デフォルトで有効 | リンクは自動的に機能 |
| `PdfDocumentOptions.EmbedFonts` | `CustomCssUrl` で @font-face | またはシステムフォント |

### ヘッダー/フッタープレースホルダーのマッピング

| ExpertPdfのトークン | IronPDFのプレースホルダー | 備考 |
|----------------|---------------------|-------|
| `&p;` | `{page}` | 現在のページ番号 |
| `&P;` | `{total-pages}` | 総ページ数 |
| `&d;` | `{date}` | 現在の日付 |
| `&t;` | `{time}` | 現在の時刻 |
| `&u;` | `{url}` | ソースURL |

### セキュリティのマッピング

| ExpertPdfのセキュリティ | IronPDFの同等物 |
|-------------------|-------------------|
| `PdfSecurityOptions.UserPassword` | `pdf.SecuritySettings.UserPassword` |
| `PdfSecurityOptions.OwnerPassword` | `pdf.SecuritySettings.OwnerPassword` |
| `PdfSecurityOptions.CanPrint` | `pdf.SecuritySettings.AllowUserPrinting` |
| `PdfSecurityOptions.CanCopyContent` | `pdf.SecuritySettings.AllowUserCopyPasteContent` |
| `PdfSecurityOptions.CanEditContent` | `pdf.SecuritySettings.AllowUserEdits` |
| `PdfSecurityOptions.CanFillFormFields` | `pdf.SecuritySettings.AllowUserFormData` |

---

## コード例

### 例1：基本的なHTMLからPDFへの変換

**移行前（ExpertPdf）：**
```csharp
using ExpertPdf.HtmlToPdf;

PdfConverter pdfConverter = new PdfConverter();
pdfConverter.LicenseKey = "YOUR_LICENSE_KEY";

string html = "<html><body><h1>Hello World</h1><p>ExpertPdfで生成</p></body></html>";
byte[] pdfBytes = pdfConverter.GetPdfBytesFromHtmlString(html);

File.WriteAllBytes("output.pdf", pdfBytes);
```

**移行後（IronPDF）：**
```csharp
using IronPdf;

IronPdf.License.LicenseKey = "YOUR_LICENSE_KEY";

var renderer = new ChromePdfRenderer();
string html = "<html><body><h1>Hello World</h1><p>IronPDFで生成</p></body></html>";

var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("output.pdf");
```

### 例2：カスタムオプションを使用したURLからPDFへの変換

**移行前（ExpertPdf）：**
```csharp
using ExpertPdf.HtmlToPdf;

PdfConverter pdfConverter = new PdfConverter();
pdfConverter.LicenseKey = "YOUR_LICENSE_KEY";
pdfConverter.PdfDocumentOptions.PdfPageSize = PdfPageSize.A4;
pdfConverter.PdfDocumentOptions.PdfPageOrientation = PdfPageOrientation.Landscape;
pdfConverter.PdfDocumentOptions.MarginLeft = 15;
pdfConverter.PdfDocumentOptions.MarginRight = 15;
pdfConverter.PdfDocumentOptions.MarginTop = 20;
pdfConverter.PdfDocumentOptions.MarginBottom = 20;

pdfConverter.SavePdfFromUrlToFile("https://example.com", "webpage.pdf");
```

**移行後（IronPDF）：**
```csharp
using IronPdf;

IronPdf.License.LicenseKey = "YOUR_LICENSE_KEY";

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;
renderer.RenderingOptions.MarginLeft = 15;
renderer.RenderingOptions.MarginRight = 15;
renderer.RenderingOptions.MarginTop = 20;
renderer.RenderingOptions.MarginBottom = 20;

var pdf = renderer.RenderUrlAsPdf("https://example.com");
pdf.SaveAs("webpage.pdf");
```

###