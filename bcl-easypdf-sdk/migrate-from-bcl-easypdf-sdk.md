---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [bcl-easypdf-sdk/migrate-from-bcl-easypdf-sdk.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/bcl-easypdf-sdk/migrate-from-bcl-easypdf-sdk.md)
🇯🇵 **日本語:** [bcl-easypdf-sdk/migrate-from-bcl-easypdf-sdk.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/bcl-easypdf-sdk/migrate-from-bcl-easypdf-sdk.md)

---

# 移行ガイド：BCL EasyPDF SDK → IronPDF

## 目次
1. [BCL EasyPDF SDKからIronPDFへの移行理由](#bcl-easypdf-sdkからironpdfへの移行理由)
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

## BCL EasyPDF SDKからIronPDFへの移行理由

### BCL EasyPDF SDKの問題点

BCL EasyPDF SDKは、展開の悪夢を引き起こすいくつかの問題技術に依存しています：

| 問題 | 影響 |
|-------|--------|
| **Windows専用** | Linux、macOS、Docker、クラウドコンテナに展開できない |
| **Microsoft Office必須** | ドキュメント変換のためにすべてのサーバーにOfficeをインストールする必要がある |
| **COM Interop** | DLLの読み込みエラー、クラッシュ、バージョンの競合 |
| **仮想プリンタードライバー** | サーバー上での対話型ユーザーセッションが必要 |
| **レガシーアーキテクチャ** | .NET Core/.NET 5+のサポートが限定的 |
| **複雑なインストール** | MSIインストーラー、GAC登録、レジストリの変更が必要 |

### BCL EasyPDF SDKの一般的なエラー

開発者は頻繁にこれらの問題に遭遇します：

- `bcl.easypdf.interop.easypdfprinter.dll error loading`
- `COM object that has been separated from its underlying RCW cannot be used`
- `Timeout expired waiting for print job to complete`
- `The printer operation failed because the service is not running`
- `Error: Access denied`（対話型セッションが必要）
- `Cannot find printer: BCL easyPDF Printer`

### IronPDFの利点

| 機能 | BCL EasyPDF SDK | IronPDF |
|---------|-----------------|---------|
| **プラットフォーム** | Windows専用 | Windows, Linux, macOS, Docker |
| **Office依存性** | 必須 | なし |
| **インストール** | 複雑なMSI + ドライバー | シンプルなNuGetパッケージ |
| **HTMLレンダリング** | 基本（Officeベース） | 完全なChromium（CSS3、JS、Flexbox） |
| **サーバー展開** | 対話型セッションが必要 | ヘッドレスで実行 |
| **.NETサポート** | .NET Coreが限定的 | .NET 5/6/7/8/9を完全サポート |
| **非同期サポート** | コールバックベース | ネイティブのasync/await |
| **コンテナサポート** | なし | Docker/Kubernetesを完全サポート |
| **ライセンス** | サーバーごと | 開発者ごと |

---

## 開始する前に

### 前提条件

- **.NET Framework 4.6.2+** または **.NET Core 3.1+** または **.NET 5/6/7/8/9**
- **Visual Studio 2019+** または **VS Code** にC#拡張機能がある
- **NuGetパッケージマネージャー**へのアクセス
- 移行する**既存のBCL EasyPDF SDKコードベース**

### すべてのBCL EasyPDF SDK参照を見つける

移行する前に、コードベース内のすべてのBCL EasyPDFの使用を特定します：

```bash
# すべてのBCL usingステートメントを検索
grep -r "using BCL" --include="*.cs" .

# Printer/PDFDocumentの使用を検索
grep -r "Printer\|PDFDocument\|PDFConverter\|HTMLConverter" --include="*.cs" .

# COM interop参照を検索
grep -r "easyPDF\|BCL.easyPDF" --include="*.csproj" .

# 設定を検索
grep -r "PageOrientation\|TimeOut\|PrintOffice" --include="*.cs" .
```

### 予想される変更点

| BCL EasyPDFパターン | 必要な変更 |
|---------------------|-----------------|
| `new Printer()` | `ChromePdfRenderer`を使用 |
| `PrintOfficeDocToPDF()` | Office変換が異なる方法で処理される |
| `RenderHTMLToPDF()` | `RenderHtmlAsPdf()` |
| COM interop参照 | 完全に削除 |
| プリンタードライバー設定 | 不要 |
| `BeginPrintToFile()`コールバック | ネイティブのasync/await |
| 対話型セッション要件 | ヘッドレスで実行 |

---

## クイックスタート（5分）

### ステップ1：BCL EasyPDF SDKを削除

BCL EasyPDF SDKは通常、以下の方法でインストールされます：
- MSIインストーラー
- 手動DLL参照
- GAC登録

すべての参照を削除します：
1. プログラムと機能からBCL EasyPDF SDKをアンインストール
2. プロジェクトからDLL参照を削除
3. COM interop参照を削除
4. 存在する場合はGACエントリをクリーンアップ

### ステップ2：IronPDFをインストール

```bash
# IronPDFをインストール
dotnet add package IronPdf
```

またはパッケージマネージャーコンソール経由で：
```powershell
Install-Package IronPdf
```

### ステップ3：名前空間を更新

```csharp
// ❌ これらを削除
using BCL.easyPDF;
using BCL.easyPDF.Interop;
using BCL.easyPDF.PDFConverter;
using BCL.easyPDF.Printer;

// ✅ これらを追加
using IronPdf;
using IronPdf.Rendering;
```

### ステップ4：最初のPDFを変換

**変換前（BCL EasyPDF SDK）：**
```csharp
using BCL.easyPDF;
using BCL.easyPDF.Interop;

Printer printer = new Printer();
printer.Configuration.TimeOut = 120;

try
{
    printer.RenderHTMLToPDF("<h1>Hello</h1>", "output.pdf");
}
finally
{
    printer.Dispose();
}
```

**変換後（IronPDF）：**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.Timeout = 120000; // ミリ秒

var pdf = renderer.RenderHtmlAsPdf("<h1>Hello</h1>");
pdf.SaveAs("output.pdf");
```

---

## 完全なAPIリファレンス

### 名前空間マッピング

| BCL EasyPDF名前空間 | IronPDF名前空間 | 目的 |
|-----------------------|-------------------|---------|
| `BCL.easyPDF` | `IronPdf` | コア機能 |
| `BCL.easyPDF.Interop` | `IronPdf` | Interop（不要） |
| `BCL.easyPDF.PDFConverter` | `IronPdf` | PDF変換 |
| `BCL.easyPDF.Printer` | `IronPdf` | プリンター不要 |
| `BCL.easyPDF.Office` | N/A | Office不要 |

### コアクラスマッピング

| BCL EasyPDFクラス | IronPDF同等クラス | 備考 |
|-------------------|-------------------|-------|
| `Printer` | `ChromePdfRenderer` | 主要な変換クラス |
| `PDFDocument` | `PdfDocument` | ドキュメント操作 |
| `HTMLConverter` | `ChromePdfRenderer` | HTML変換 |
| `PrinterConfiguration` | `ChromePdfRenderOptions` | レンダリングオプション |
| `PageOrientation` | `PdfPaperOrientation` | ページの向き |
| `PageSize` | `PdfPaperSize` | 用紙サイズ |
| `SecurityHandler` | `PdfDocument.SecuritySettings` | セキュリティオプション |
| `PDFProcessor` | `PdfDocument` | PDF処理 |

### 完全なメソッドマッピング

#### PDF作成

| BCL EasyPDFメソッド | IronPDFメソッド | 備考 |
|--------------------|----------------|-------|
| `printer.RenderHTMLToPDF(html, path)` | `renderer.RenderHtmlAsPdf(html).SaveAs(path)` | HTML文字列 |
| `printer.RenderUrlToPDF(url, path)` | `renderer.RenderUrlAsPdf(url).SaveAs(path)` | URL |
| `printer.RenderFileToPDF(file, path)` | `renderer.RenderHtmlFileAsPdf(file).SaveAs(path)` | HTMLファイル |
| `htmlConverter.ConvertHTML(html, doc)` | `renderer.RenderHtmlAsPdf(html)` | HTMLからPDFへ |
| `htmlConverter.ConvertURL(url, doc)` | `renderer.RenderUrlAsPdf(url)` | URLからPDFへ |

#### Officeドキュメント変換

| BCL EasyPDFメソッド | IronPDFメソッド | 備考 |
|--------------------|----------------|-------|
| `printer.PrintOfficeDocToPDF(doc, pdf)` | IronWordまたはHTMLワークフローを使用 | Office不要 |
| `printer.PrintWordToPDF()` | 最初にHTMLに変換してからPDFに | HTMLベースのワークフロー |
| `printer.PrintExcelToPDF()` | IronXLまたはHTMLワークフローを使用 | IronXLを検討 |

*注：OfficeなしでOfficeドキュメントを変換するには、.docx用のIronWord、.xlsx用のIronXLを使用するか、ドキュメントを最初にHTMLに変換してください。*

#### PDF操作

| BCL EasyPDFメソッド | IronPDFメソッド | 備考 |
|--------------------|----------------|-------|
| `doc.Append(doc2)` | `PdfDocument.Merge(pdf1, pdf2)` | PDFのマージ |
| `doc.ExtractPages(start, end)` | `pdf.CopyPages(start, end)` | ページの抽出 |
| `doc.DeletePage(index)` | `pdf.RemovePage(index)` | ページの削除 |
| `doc.RotatePage(index, angle)` | `pdf.RotatePage(index, angle)` | ページの回転 |
| `doc.GetPageCount()` | `pdf.PageCount` | ページ数 |
| `doc.Save(path)` | `pdf.SaveAs(path)` | PDFの保存 |
| `doc.Close()` | `pdf.Dispose()`または`using` | クリーンアップ |

#### 設定オプション

| BCL EasyPDFオプション | IronPDFオプション | 備考 |
|--------------------|----------------|-------|
| `config.TimeOut` | `RenderingOptions.Timeout` | タイムアウト（ミリ秒） |
| `config.PageOrientation = Landscape` | `RenderingOptions.PaperOrientation = Landscape` | 方向 |
| `config.PageSize = A4` | `RenderingOptions.PaperSize = PdfPaperSize.A4` | 用紙サイズ |
| `config.JobTitle` | `pdf.MetaData.Title` | ドキュメントタイトル |
| `config.PageWidth` | `RenderingOptions.SetCustomPaperSize()` | カスタム幅 |
| `config.PageHeight` | `RenderingOptions.SetCustomPaperSize()` | カスタム高さ |
| `config.MarginTop/Bottom/Left/Right` | `RenderingOptions.MarginTop`など | マージン |
| `config.BackgroundPrinting` | 常にtrue | 背景サポート |

#### セキュリティとメタデータ

| BCL EasyPDFメソッド | IronPDFメソッド | 備考 |
|--------------------|----------------|-------|
| `doc.SetPassword(pwd)` | `pdf.SecuritySettings.UserPassword` | パスワード |
| `doc.SetOwnerPassword(pwd)` | `pdf.SecuritySettings.OwnerPassword` | オーナーパスワード |
| `doc.SetPrintPermission(bool)` | `pdf.SecuritySettings.AllowUserPrinting` | 印刷許可 |
| `doc.SetCopyPermission(bool)` | `pdf.SecuritySettings.AllowUserCopyPasteContent` | コピー許可 |
| `doc.SetTitle(title)` | `pdf.MetaData.Title` | タイトル |
| `doc.SetAuthor(author)` | `pdf.MetaData.Author` | 著者 |
| `doc.SetSubject(subject)` | `pdf.MetaData.Subject` | 件名 |
| `doc.SetKeywords(keywords)` | `pdf.MetaData.Keywords` | キーワード |

#### テキスト抽出

| BCL EasyPDFメソッド | IronPDFメソッド | 備考 |
|--------------------|----------------|-------|
| `doc.ExtractText()` | `pdf.ExtractAllText()` | 全テキスト |
| `doc.ExtractTextFromPage(index)` | `pdf.ExtractTextFromPage(index)` | ページごと |

#### ライセンス

| BCL EasyPDFメソッド | IronPDFメソッド | 備考 |
|--------------------|----------------|-------|
| `Printer.SetLicenseKey(key)` | `IronPdf.License.LicenseKey = key` | ライセンスの設定 |
| ライセンスファイルパス | コードベースのみ | ファイル不要 |

---

## コード移行例

### 例1：HTML文字列からPDFへ

**変換前（BCL EasyPDF SDK）：**
```csharp
using BCL.easyPDF;
using BCL.easyPDF.Interop;

class Program
{
    static void Main()
    {
        Printer printer = new Printer();

        try
        {
            // 設定を設定
            printer.Configuration.TimeOut = 120;
            printer.Configuration.JobTitle = "Invoice";
            printer.Configuration.PageOrientation = PageOrientation.Portrait;
            printer.Configuration.PageSize = PageSize.Letter;

            string htmlContent = @"
                <html>
                <head>
                    <style>
                        body { font-family: Arial; }
                        h1 { color: navy; }
                    </style>
                </head>
                <body>
                    <h1>Invoice #12345</h1>
                    <p>Thank you for your order.</p>
                </body>
                </html>";

            printer.RenderHTMLToPDF(htmlContent, "invoice.pdf");
            Console.WriteLine("PDF created successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            printer.Dispose();
        }
    }
}
```

**変換後（IronPDF）：**
```csharp
using IronPdf;

class Program
{
    static void Main()
    {
        // オプション：ライセンスを設定
        License.LicenseKey = "YOUR-LICENSE-KEY";

        var renderer = new ChromePdfRenderer();

        // 設定を設定
        renderer.RenderingOptions.Timeout = 120000;
        renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Portrait;
        renderer.RenderingOptions.PaperSize = PdfPaperSize.Letter;

        string