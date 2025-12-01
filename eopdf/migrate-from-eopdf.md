---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [eopdf/migrate-from-eopdf.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/eopdf/migrate-from-eopdf.md)
🇯🇵 **日本語:** [eopdf/migrate-from-eopdf.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/eopdf/migrate-from-eopdf.md)

---

# EO.PdfからIronPDFへの完全移行ガイド

## 目次

1. [EO.PdfからIronPDFへ移行する理由](#EO.PdfからIronPDFへ移行する理由)
2. [開始する前に](#開始する前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全なAPIリファレンス](#完全なAPIリファレンス)
5. [コード例](#コード例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
8. [トラブルシューティング](#トラブルシューティング)
9. [移行チェックリスト](#移行チェックリスト)

---

## EO.PdfからIronPDFへ移行する理由

### EO.Pdfの問題点

1. **巨大な126MBのパッケージサイズ**: EO.Pdfは独自のChromiumエンジンをバンドルしており、126MBのデプロイメントフットプリントをもたらします。これはDockerイメージを膨らませ、CI/CDパイプラインを遅くし、インフラストラクチャのコストを増加させます。

2. **レガシーアーキテクチャの荷物**: EO.Pdfは元々Internet Explorerのレンダリングエンジン上に構築され、その後Chromiumに移行しました。このレガシーは以下を導入します:
   - IE時代の互換性問題
   - API設計の技術的負債
   - バージョン間の一貫性のない振る舞い

3. **Windows中心の設計**: "クロスプラットフォーム"としてマーケティングされているにもかかわらず、EO.PdfのLinuxおよびmacOSサポートは限定的です。多くの開発者が非Windowsデプロイメントで問題を報告しています。

4. **1ライセンスあたり$799**: 類似またはより良い機能を提供する代替品と比較して、EO.Pdfは開発者ライセンスあたり$799と高価です。

5. **静的なグローバルオプション**: EO.Pdfは設定のために静的な`HtmlToPdf.Options`を使用しており、これはスレッドセーフではなく、マルチテナントWebアプリケーションで問題があります。

### EO.PdfとIronPDFの比較

| 項目 | EO.Pdf | IronPDF |
|------|--------|---------|
| **パッケージサイズ** | 126MB | 最適化 (~50MB) |
| **レガシー問題** | IE移行の荷物 | クリーンで現代的なコードベース |
| **プラットフォームサポート** | Windows中心 | 真のクロスプラットフォーム |
| **設定** | 静的/グローバル | インスタンスベース、スレッドセーフ |
| **価格** | $799/開発者 | 競争力のある価格設定 |
| **API設計** | 混合 (HtmlToPdf + ACM) | 統一された一貫性 |
| **ドキュメント** | 限定的 | 包括的なチュートリアル |
| **現代の.NET** | .NET Standard | .NET 6/7/8/9+ ネイティブ |
| **非同期サポート** | 限定的 | 完全なasync/await |

### 主な移行メリット

1. **50%小さなフットプリント**: IronPDFの最適化されたChromiumパッケージング
2. **真のクロスプラットフォーム**: Windows、Linux、macOS、Dockerで同一に動作
3. **スレッドセーフな設定**: インスタンスベースのレンダラーオプション
4. **現代的なAPI**: 一貫性があり直感的なメソッド名
5. **より良いドキュメント**: 豊富なチュートリアルと例
6. **アクティブな開発**: 定期的なアップデートとセキュリティパッチ

---

## 開始する前に

### 1. EO.Pdfの使用状況を調査

コードベース内のすべてのEO.Pdfパターンを特定します:

```bash
# EO.Pdfの参照をすべて見つける
grep -r "EO.Pdf\|HtmlToPdf\|AcmRender\|PdfDocument" --include="*.cs" .

# NuGetパッケージをチェック
dotnet list package | grep -i "EO.Pdf"
```

**一般的なEO.Pdfの名前空間:**
- `EO.Pdf` - HTMLからPDFへのコア
- `EO.Pdf.Acm` - 高度なコンテンツモデル (ACM)
- `EO.Pdf.Contents` - 低レベルのコンテンツ操作
- `EO.Pdf.Drawing` - グラフィック操作

### 2. 現在の機能を文書化

使用しているEO.Pdfの機能のチェックリストを作成します:

- [ ] HTMLからPDFへの変換 (`HtmlToPdf.ConvertHtml`)
- [ ] URLからPDFへの変換 (`HtmlToPdf.ConvertUrl`)
- [ ] ACMレンダリング (`AcmRender`, `AcmText`, `AcmBlock`)
- [ ] PDFのマージ (`PdfDocument.Merge`)
- [ ] ページ操作
- [ ] ヘッダーとフッター
- [ ] セキュリティ/暗号化
- [ ] フォームフィールド
- [ ] ウォーターマーク
- [ ] 印刷 (`PdfDocument.Print`)

### 3. IronPDFのセットアップ

```bash
# EO.Pdfを削除
dotnet remove package EO.Pdf

# IronPDFをインストール
dotnet add package IronPdf
```

### 4. ライセンスの設定

```csharp
// アプリケーションの起動時に
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

---

## クイックスタート移行

### コアパターンの変更

EO.Pdfは静的メソッドとグローバルオプションを使用します。IronPDFはインスタンスベースのレンダラーとローカルオプションを使用します。

**EO.Pdfパターン (静的、スレッドセーフではない):**
```csharp
// グローバルオプションはすべての変換に影響します
HtmlToPdf.Options.PageSize = PdfPageSizes.A4;
HtmlToPdf.Options.OutputArea = new RectangleF(0.5f, 0.5f, 7.5f, 10.5f);
HtmlToPdf.ConvertHtml(html, "output.pdf");
```

**IronPDFパターン (インスタンスベース、スレッドセーフ):**
```csharp
var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
renderer.RenderingOptions.MarginTop = 12.7;  // mm
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("output.pdf");
```

### 最小限の移行例

**移行前 (EO.Pdf):**
```csharp
using EO.Pdf;

public class PdfService
{
    public void GenerateReport(string html)
    {
        HtmlToPdf.Options.PageSize = PdfPageSizes.A4;
        HtmlToPdf.Options.OutputArea = new RectangleF(0.5f, 0.5f, 7.5f, 10.5f);
        HtmlToPdf.ConvertHtml(html, "report.pdf");
    }
}
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

public class PdfService
{
    public void GenerateReport(string html)
    {
        var renderer = new ChromePdfRenderer();
        renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
        renderer.RenderingOptions.MarginTop = 12.7;
        renderer.RenderingOptions.MarginBottom = 12.7;
        renderer.RenderingOptions.MarginLeft = 12.7;
        renderer.RenderingOptions.MarginRight = 12.7;

        var pdf = renderer.RenderHtmlAsPdf(html);
        pdf.SaveAs("report.pdf");
    }
}
```

---

## 完全なAPIリファレンス

### 名前空間マッピング

| EO.Pdfの名前空間 | IronPDFの同等物 |
|-----------------|-------------------|
| `EO.Pdf` | `IronPdf` |
| `EO.Pdf.Acm` | HTML/CSS (ACM不要) |
| `EO.Pdf.Contents` | `IronPdf.Editing` |
| `EO.Pdf.Drawing` | HTML/CSSまたは`IronPdf.Editing` |

### コアクラスマッピング

| EO.Pdfのクラス | IronPDFの同等物 | 備考 |
|-------------|-------------------|-------|
| `HtmlToPdf` | `ChromePdfRenderer` | インスタンスベース |
| `PdfDocument` | `PdfDocument` | 類似しているが異なるメソッド |
| `PdfPage` | `PdfDocument.Pages[i]` | ページコレクション経由 |
| `HtmlToPdfOptions` | `ChromePdfRenderOptions` | `RenderingOptions`経由 |
| `AcmRender` | 不要 | HTML/CSSを使用 |
| `AcmText` | HTML `<span>`, `<p>` | |
| `AcmBlock` | HTML `<div>` | |
| `AcmImage` | HTML `<img>` | |
| `AcmTable` | HTML `<table>` | |

### メソッドマッピング

| EO.Pdfのメソッド | IronPDFのメソッド | 備考 |
|--------------|----------------|-------|
| `HtmlToPdf.ConvertHtml(html, path)` | `renderer.RenderHtmlAsPdf(html)` その後 `SaveAs(path)` | IronPDFでは2ステップ |
| `HtmlToPdf.ConvertHtml(html, stream)` | `pdf.Stream` または `pdf.BinaryData` | レンダリング後にデータにアクセス |
| `HtmlToPdf.ConvertHtml(html, pdfDoc)` | `renderer.RenderHtmlAsPdf(html)` | PdfDocumentを返す |
| `HtmlToPdf.ConvertUrl(url, path)` | `renderer.RenderUrlAsPdf(url)` その後 `SaveAs(path)` | |
| `HtmlToPdf.ConvertUrl(url, stream)` | `pdf.BinaryData` または `pdf.Stream` | |
| `PdfDocument.Save(path)` | `pdf.SaveAs(path)` | |
| `PdfDocument.Save(stream)` | `pdf.Stream` | |
| `PdfDocument.Merge(docs)` | `PdfDocument.Merge(docs)` | 静的メソッド |
| `PdfDocument.Append(doc)` | `pdf.AppendPdf(other)` または `PdfDocument.Merge()` | |
| `PdfDocument.Print()` | `pdf.Print()` | 類似の機能 |
| `new PdfDocument(path)` | `PdfDocument.FromFile(path)` | 静的ファクトリ |
| `new PdfDocument(stream)` | `PdfDocument.FromStream(stream)` | |

### オプションマッピング

| EO.Pdfのオプション | IronPDFのRenderingOptions | 備考 |
|--------------|-------------------------|-------|
| `Options.PageSize = PdfPageSizes.A4` | `PaperSize = PdfPaperSize.A4` | |
| `Options.PageSize = PdfPageSizes.Letter` | `PaperSize = PdfPaperSize.Letter` | |
| `Options.OutputArea` (RectangleF) | `MarginTop`, `MarginBottom` など | 個別のプロパティ |
| `Options.BaseUrl` | `BaseUrl` | 同じ概念 |
| `Options.AutoFitX` | `FitToPaperMode` | |
| `Options.NoCacheImages` | 不要 | Chromeがキャッシュを処理 |
| `Options.NoLink` | CSSでリンクを無効化可能 | |
| `Options.RepeatTableHeader` | HTMLテーブルで自動 | |
| `Options.RepeatTableFooter` | HTMLテーブルで自動 | |

### ページサイズ定数

| EO.Pdf | IronPDF |
|--------|---------|
| `PdfPageSizes.A0` - `A10` | `PdfPaperSize.A0` - `A10` |
| `PdfPageSizes.Letter` | `PdfPaperSize.Letter` |
| `PdfPageSizes.Legal` | `PdfPaperSize.Legal` |
| `PdfPageSizes.Tabloid` | `PdfPaperSize.Tabloid` |
| `PdfPageSizes.B0` - `B10` | `PdfPaperSize.B0` - `B10` |
| カスタム via `new PdfPageSize(w, h)` | `SetCustomPaperSizeInMillimeters(w, h)` |

### セキュリティマッピング

| EO.Pdfのセキュリティ | IronPDFの同等物 |
|----------------|-------------------|
| `PdfDocumentSecurity.UserPassword` | `pdf.SecuritySettings.UserPassword` |
| `PdfDocumentSecurity.OwnerPassword` | `pdf.SecuritySettings.OwnerPassword` |
| `PdfDocumentSecurity.AllowPrint` | `pdf.SecuritySettings.AllowUserPrinting` |
| `PdfDocumentSecurity.AllowCopy` | `pdf.SecuritySettings.AllowUserCopyPasteContent` |
| `PdfDocumentSecurity.AllowModify` | `pdf.SecuritySettings.AllowUserEdits` |

---

## コード例

### 例1: 基本的なHTMLからPDFへ

**移行前 (EO.Pdf):**
```csharp
using EO.Pdf;

string html = "<html><body><h1>Hello World</h1><p>EO.Pdfで生成</p></body></html>";
HtmlToPdf.ConvertHtml(html, "output.pdf");
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

string html = "<html><body><h1>Hello World</h1><p>IronPDFで生成</p></body></html>";
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("output.pdf");
```

### 例2: オプション付きのURLからPDFへ

**移行前 (EO.Pdf):**
```csharp
using EO.Pdf;
using System.Drawing;

HtmlToPdf.Options.PageSize = PdfPageSizes.A4;
HtmlToPdf.Options.OutputArea = new RectangleF(0.5f, 0.5f, 7.5f, 10.5f);  // インチ
HtmlToPdf.Options.BaseUrl = "https://example.com";
HtmlToPdf.ConvertUrl("https://example.com/report", "report.pdf");
```

**移行後 (IronPDF):**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
renderer.RenderingOptions.MarginTop = 12.7;    // 0.5インチ = 12.7mm
renderer.RenderingOptions.MarginBottom = 12.7;
renderer.RenderingOptions.MarginLeft = 12.7;
renderer.RenderingOptions.MarginRight = 12.7;
renderer.RenderingOptions.BaseUrl = new Uri("https://example.com");

var pdf = renderer.RenderUrlAsPdf("https://example.com/report");
pdf.SaveAs("report.pdf");
```

### 例3: PDFのマージ

**移行前 (EO.Pdf):**
```csharp
using EO.Pdf;

PdfDocument doc1 = new PdfDocument("file1.pdf");
PdfDocument doc2 = new PdfDocument("file2.pdf");
PdfDocument doc3 = new PdfDocument("file3.pdf");

doc1.Append(doc2);
doc1.Append(doc