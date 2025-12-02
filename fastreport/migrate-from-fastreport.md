---
**  (Japanese Translation)**

 **English:** [fastreport/migrate-from-fastreport.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/fastreport/migrate-from-fastreport.md)
 **:** [fastreport/migrate-from-fastreport.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/fastreport/migrate-from-fastreport.md)

---
# FastReport.NETからIronPDFへの移行方法は？

## 目次
1. [IronPDFへの移行理由](#IronPDFへの移行理由)
2. [開始前に](#開始前に)
3. [クイックスタート移行](#クイックスタート移行)
4. [完全なAPIリファレンス](#完全なAPIリファレンス)
5. [コード例](#コード例)
6. [高度なシナリオ](#高度なシナリオ)
7. [パフォーマンスに関する考慮事項](#パフォーマンスに関する考慮事項)
8. [トラブルシューティング](#トラブルシューティング)
9. [移行チェックリスト](#移行チェックリスト)

---

## IronPDFへの移行理由

### FastReport.NETの課題

FastReport.NETは強力なレポーティングツールですが、現代のPDF生成には以下のような重要な制限があります：

1. **レポートデザイナー依存**：複雑なレイアウトを作成するにはビジュアルデザイナーまたは.frxファイル構造の深い知識が必要です。コードファースト開発には適していません
2. **学習曲線が急**：FastReportのバンドベースのアーキテクチャ（DataBand、PageHeaderBandなど）は、レポート固有の概念を理解する必要があります
3. **限定的なCSSサポート**：Web標準のスタイリングはネイティブにサポートされておらず、スタイリングはFastReportの独自フォーマットを通じて行われます
4. **複雑なデータバインディング**：RegisterData()やDataSource接続により、シンプルなPDF生成のためのボイラープレートが追加されます
5. **断片化されたパッケージ**：全機能を利用するためには複数のNuGetパッケージ（FastReport.OpenSource、FastReport.OpenSource.Export.PdfSimpleなど）が必要です
6. **ライセンスの複雑さ**：オープンソース版は機能が限定されており、PDF暗号化、デジタル署名、フォント埋め込みには商用版が必要です

### IronPDFの利点

| 項目 | FastReport.NET | IronPDF |
|------|----------------|---------|
| デザインアプローチ | ビジュアルデザイナー + .frxファイル | HTML/CSS（Web技術） |
| 学習曲線 | 急（バンドベースの概念） | 緩やか（HTML/CSSの知識） |
| データバインディング | RegisterData(), DataBand | 文字列補間、Razor、テンプレートエンジン |
| CSSサポート | 限定的 | 完全なCSS3サポート、Flexbox/Gridを含む |
| パッケージモデル | 複数のパッケージ | 単一のパッケージ（全機能） |
| レンダリングエンジン | カスタム | 最新のChromium |
| PDF操作 | エクスポートに焦点 | 完全な操作（マージ、分割、セキュリティ、フォーム） |
| 現代の.NET | .NET Standard 2.0 | .NET 6/7/8/9+ ネイティブ |

---

## 開始前に

### 前提条件

1. **.NET環境**：IronPDFは.NET Framework 4.6.2+、.NET Core 3.1+、.NET 5/6/7/8/9+をサポートしています
2. **NuGetアクセス**：NuGetからパッケージをインストールできることを確認してください
3. **ライセンスキー**：本番用途でのIronPDFライセンスキーを取得してください

### プロジェクトのバックアップ

```bash
# バックアップブランチを作成
git checkout -b pre-ironpdf-migration
git add .
git commit -m "FastReport.NETからIronPDFへの移行前のバックアップ"
```

### FastReportの使用箇所をすべて特定

```bash
# すべてのFastReport参照を検索
grep -r "FastReport\|\.frx\|PDFExport\|PDFSimpleExport\|DataBand\|RegisterData" --include="*.cs" --include="*.csproj" .
```

### レポートテンプレートを文書化

移行前に、すべての`.frx`ファイルとその目的をカタログ化してください：
- レポート名と目的
- 使用されるデータソース
- ヘッダー/フッターの設定
- ページ番号の要件
- 特別なフォーマットやスタイリング

---

## クイックスタート移行

### ステップ1：NuGetパッケージを更新

```bash
# すべてのFastReportパッケージを削除
dotnet remove package FastReport.OpenSource
dotnet remove package FastReport.OpenSource.Export.PdfSimple
dotnet remove package FastReport.OpenSource.Web
dotnet remove package FastReport.OpenSource.Data.MsSql

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ2：名前空間を更新

```csharp
// 以前
using FastReport;
using FastReport.Export.Pdf;
using FastReport.Export.PdfSimple;
using FastReport.Data;
using FastReport.Utils;

// 以降
using IronPdf;
using IronPdf.Rendering;
```

### ステップ3：IronPDFを初期化

```csharp
// アプリケーションの起動時にライセンスキーを設定
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

// オプション：ログの設定
IronPdf.Logging.Logger.LoggingMode = IronPdf.Logging.Logger.LoggingModes.Custom;
```

### ステップ4：基本的な変換パターン

```csharp
// 以前（FastReport.NET）
using (Report report = new Report())
{
    report.Load("report.frx");
    report.RegisterData(dataSet, "MyData");
    report.Prepare();

    using (var export = new PDFSimpleExport())
    {
        report.Export(export, "output.pdf");
    }
}

// 以降（IronPDF）
var renderer = new ChromePdfRenderer();
var html = GenerateHtmlFromData(dataSet);  // テンプレートロジック
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("output.pdf");
```

---

## 完全なAPIリファレンス

### 名前空間マッピング

| FastReport.NET名前空間 | IronPDF相当 | 備考 |
|------------------------|-------------|------|
| `FastReport` | `IronPdf` | メイン名前空間 |
| `FastReport.Utils` | `IronPdf.Rendering` | レンダリングユーティリティ |
| `FastReport.Export.Pdf` | `IronPdf` | PDFエクスポート（内蔵） |
| `FastReport.Export.PdfSimple` | `IronPdf` | 簡易エクスポート（内蔵） |
| `FastReport.Data` | N/A | 標準の.NETデータアクセスを使用 |
| `FastReport.Engine` | N/A | 不要（直接レンダリング） |
| `FastReport.Matrix` | HTML `<table>` | HTMLテーブルを使用 |
| `FastReport.Barcode` | IronBarCode（別途） | またはHTML/SVGバーコードを使用 |

### コアクラスマッピング

| FastReport.NETクラス | IronPDF相当 | 備考 |
|----------------------|-------------|------|
| `Report` | `ChromePdfRenderer` | メインレンダリングクラス |
| `PDFExport` | `ChromePdfRenderer` + `SecuritySettings` | レンダリング + セキュリティ |
| `PDFSimpleExport` | `ChromePdfRenderer` | 簡易エクスポート |
| `ReportPage` | HTML `<body>`または`<div>` | ページコンテンツ |
| `TextObject` | HTML `<p>`, `<span>`, `<div>` | テキスト要素 |
| `TableObject` | HTML `<table>` | テーブル |
| `DataBand` | テンプレート内のループ | データ反復 |
| `PageHeaderBand` | `HtmlHeaderFooter` | ページヘッダー |
| `PageFooterBand` | `HtmlHeaderFooter` | ページフッター |
| `HTMLObject` | 直接HTMLレンダリング | HTMLコンテンツ |
| `PictureObject` | HTML `<img>` | 画像 |
| `ShapeObject` | HTML/SVG/CSS | 形状と境界線 |
| `LineObject` | CSSボーダー/HR | 線 |
| `BarcodeObject` | IronBarCodeまたはSVG | バーコード |
| `CheckBoxObject` | HTML `<input type="checkbox">` | チェックボックス |
| `ZipCodeObject` | CSSスタイル要素 | 郵便番号 |

### レポートクラスメソッド

| FastReportメソッド | IronPDF相当 | 備考 |
|---------------------|-------------|------|
| `report.Load(path)` | HTMLテンプレートファイルを読み込む | またはコード内でHTMLを生成 |
| `report.Load(stream)` | ストリームからHTMLを読み込む | `File.ReadAllText()`またはストリームを使用 |
| `report.RegisterData(data, name)` | HTML内での直接データバインディング | 文字列補間/Razorを使用 |
| `report.RegisterData(ds, name, maxNesting)` | テンプレート内でのネストループ | 階層を手動で処理 |
| `report.GetDataSource(name)` | N/A | .NETコレクションを直接使用 |
| `report.Prepare()` | N/A | 不要（直接レンダリング） |
| `report.Export(export, path)` | `pdf.SaveAs(path)` | ファイルに保存 |
| `report.Export(export, stream)` | `pdf.Stream`または`pdf.BinaryData` | ストリーム/バイトとして取得 |
| `report.FindObject(name)` | N/A | DOMまたはテンプレートロジックを使用 |
| `report.Designer` | N/A | HTML/CSSエディターを使用 |
| `report.Refresh()` | 新しいデータで再レンダリング | 新しい`PdfDocument`を作成 |
| `report.Print()` | `pdf.Print()` | デフォルトプリンターに印刷 |
| `report.PrintWithDialog()` | ダイアログ付き`pdf.Print()` | 印刷ダイアログを設定 |
| `report.Dispose()` | `pdf.Dispose()`（オプション） | IDisposableサポート |

### PDFExportクラスプロパティ

| FastReport PDFExportプロパティ | IronPDF相当 | 備考 |
|--------------------------------|-------------|------|
| `Author` | `pdf.MetaData.Author` | ドキュメントの著者 |
| `Title` | `pdf.MetaData.Title` | ドキュメントのタイトル |
| `Subject` | `pdf.MetaData.Subject` | ドキュメントの主題 |
| `Keywords` | `pdf.MetaData.Keywords` | ドキュメントのキーワード |
| `Creator` | `pdf.MetaData.Creator` | 作成アプリケーション |
| `Producer` | `pdf.MetaData.Producer` | 製作者名 |
| `OwnerPassword` | `pdf.SecuritySettings.OwnerPassword` | オーナーパスワード |
| `UserPassword` | `pdf.SecuritySettings.UserPassword` | ユーザーパスワード |
| `AllowPrint` | `pdf.SecuritySettings.AllowUserPrinting` | 印刷許可 |
| `AllowCopy` | `pdf.SecuritySettings.AllowUserCopyPasteContent` | コピー許可 |
| `AllowModify` | `pdf.SecuritySettings.AllowUserEdits` | 変更許可 |
| `AllowAnnotate` | `pdf.SecuritySettings.AllowUserAnnotations` | 注釈許可 |
| `Compressed` | 常に圧縮 | IronPDFはデフォルトで圧縮 |
| `Background` | `RenderingOptions.PrintHtmlBackgrounds` | 背景レンダリング |
| `Outline` | `pdf.BookMarks` | PDFアウトライン/ブックマーク |
| `Transparency` | 自動 | 自動的に処理 |
| `HideToolbar` | `pdf.SecuritySettings.AllowUserFormData` | ビューア設定 |
| `CenterWindow` | HTML内のJavaScriptを介して | ビューア設定 |
| `PdfCompliance` | `RenderingOptions.PdfA` | PDF/A準拠 |
| `DigitalSignCertificate` | `pdf.SignWithFile()` | デジタル署名 |

### レンダリングオプション

| FastReportオプション | IronPDF相当 | 備考 |
|----------------------|-------------|------|
| ページサイズ | `RenderingOptions.PaperSize` | 標準サイズまたはカスタム |
| ページの向き | `RenderingOptions.PaperOrientation` | 縦/横 |
| 余白 | `RenderingOptions.MarginTop/Bottom/Left/Right` | mm単位 |
| 最初のページ番号 | `RenderingOptions.FirstPageNumber` | 開始ページ番号 |
| ズーム | `RenderingOptions.Zoom` | デフォルトのズームレベル |
| 背景の印刷 | `RenderingOptions.PrintHtmlBackgrounds` | 背景色/画像 |
| カスタム用紙サイズ | `RenderingOptions.SetCustomPaperSize()` | カスタム寸法 |

### ページ番号プレースホルダー

| FastReportプレースホルダー | IronPDF相当 | 備考 |
|-----------------------------|-------------|------|
| `[Page]` | `{page}` | 現在のページ番号 |
| `[TotalPages]` | `{total-pages}` | 総ページ数 |
| `[Page#]` | `{page}` | 代替構文 |
| `[TotalPages#]` | `{total-pages}` | 代替構文 |
| `[PageN]` | `{page}` | 変形 |
| `[PageNofM]` | `{page} of {total-pages}` | 組み合わせ |
| `[Date]` | `{date}`またはJavaScript | 現在の日付 |
| `[Time]` | `{time}`またはJavaScript | 現在の時刻 |

---

## コード例

### 例1：シンプルなHTMLからPDFへ

**以前（FastReport.NET）：**
```csharp
using FastReport;
using FastReport.Export.PdfSimple;
using System.IO;

class Program
{
    static void Main()
    {
        using (Report report = new Report())
        {
            // HTMLオブジェクトを作成
            FastReport.HTMLObject htmlObject =