# Sumatra PDFからIronPDFへの移行方法は？

## Sumatra PDFから移行する理由は？

Sumatra PDFは**デスクトップPDFビューアアプリケーション**であり、開発ライブラリではありません。もし.NETアプリケーションでSumatra PDFを使用している場合、以下のようなことを行っている可能性があります：

1. 外部プロセスとして起動してPDFを表示
2. コマンドライン経由でPDFを印刷
3. ユーザーが別途インストールする必要がある依存関係

### Sumatra PDF統合の主な問題点

| 問題 | 影響 |
|---------|--------|
| **ライブラリではない** | プログラムでPDFを作成または編集できない |
| **外部プロセス** | 別のプロセスを生成する必要がある |
| **GPLライセンス** | 商用ソフトウェアに制限がある |
| **ユーザー依存** | ユーザーがSumatraを別途インストールする必要がある |
| **APIなし** | コマンドライン引数に限定される |
| **閲覧のみ** | PDFを作成、編集、操作できない |
| **Webサポートなし** | デスクトップ専用アプリケーション |

### 代わりにIronPDFが提供するもの

| 機能 | Sumatra PDF | IronPDF |
|------------|-------------|---------|
| PDFを作成 | いいえ | はい |
| PDFを編集 | いいえ | はい |
| HTMLからPDFへ | いいえ | はい |
| マージ/分割 | いいえ | はい |
| ウォーターマーク | いいえ | はい |
| デジタル署名 | いいえ | はい |
| フォーム入力 | いいえ | はい |
| テキスト抽出 | いいえ | はい |
| .NET統合 | なし | ネイティブ |
| Webアプリケーション | いいえ | はい |
| 商用ライセンス | GPL | はい |

---

## クイックスタート：Sumatra PDFの置き換え

### ステップ1：IronPDFのインストール

```bash
dotnet add package IronPdf
```

### ステップ2：Sumatraの依存関係を削除

```csharp
// このパターンを削除
Process.Start("SumatraPDF.exe", "document.pdf");

// IronPDFに置き換え
using IronPdf;
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

---

## Sumatra PDFの一般的な使用パターン → IronPDF

### パターン1：ユーザー用にPDFを開く

**Sumatra（外部プロセス）:**
```csharp
using System.Diagnostics;

// ユーザーのマシンにSumatraがインストールされている必要がある
Process.Start(new ProcessStartInfo
{
    FileName = "SumatraPDF.exe",
    Arguments = "\"report.pdf\"",
    UseShellExecute = true
});
```

**IronPDF（生成して開く）:**
```csharp
using IronPdf;
using System.Diagnostics;

// プログラムでPDFを作成
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1>Report</h1><p>Content here</p>");
pdf.SaveAs("report.pdf");

// デフォルトのPDFビューア（システムに関連付けられた）で開く
Process.Start(new ProcessStartInfo
{
    FileName = "report.pdf",
    UseShellExecute = true
});
```

### パターン2：PDFの印刷

**Sumatra（コマンドライン印刷）:**
```csharp
using System.Diagnostics;

// Sumatraのコマンドラインを使用して印刷
Process.Start(new ProcessStartInfo
{
    FileName = "SumatraPDF.exe",
    Arguments = "-print-to-default \"document.pdf\"",
    CreateNoWindow = true
});
```

**IronPDF（ネイティブ印刷）:**
```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// デフォルトプリンターに印刷
pdf.Print();

// オプション付きで印刷
pdf.Print(new PrintOptions
{
    PrinterName = "HP LaserJet",
    NumberOfCopies = 2,
    DPI = 300
});
```

### パターン3：PDFの作成（Sumatraでは不可能）

**Sumatra：** 不可能 - Sumatraは閲覧のみ

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

// HTML文字列から
var pdf = renderer.RenderHtmlAsPdf(@"
    <html>
    <head><style>body { font-family: Arial; }</style></head>
    <body>
        <h1>Invoice #12345</h1>
        <p>Thank you for your purchase.</p>
    </body>
    </html>");

pdf.SaveAs("invoice.pdf");
```

### パターン4：URLをPDFに変換（Sumatraでは不可能）

**Sumatra：** 不可能

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.EnableJavaScript = true;
renderer.RenderingOptions.WaitFor.JavaScript(3000);

var pdf = renderer.RenderUrlAsPdf("https://example.com/report");
pdf.SaveAs("webpage.pdf");
```

### パターン5：PDFのマージ（Sumatraでは不可能）

**Sumatra：** 不可能

**IronPDF:**
```csharp
using IronPdf;

var pdf1 = PdfDocument.FromFile("chapter1.pdf");
var pdf2 = PdfDocument.FromFile("chapter2.pdf");
var pdf3 = PdfDocument.FromFile("chapter3.pdf");

var book = PdfDocument.Merge(pdf1, pdf2, pdf3);
book.SaveAs("complete_book.pdf");
```

### パターン6：ウォーターマークの追加（Sumatraでは不可能）

**Sumatra：** 不可能

**IronPDF:**
```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

pdf.ApplyWatermark(@"
    <div style='
        font-size: 60pt;
        color: rgba(255, 0, 0, 0.3);
        transform: rotate(-45deg);
    '>
        CONFIDENTIAL
    </div>");

pdf.SaveAs("watermarked.pdf");
```

### パターン7：テキストの抽出（Sumatraではプログラム的に不可能）

**Sumatra：** プログラム的に不可能

**IronPDF:**
```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");
string allText = pdf.ExtractAllText();

// またはページごと
for (int i = 0; i < pdf.PageCount; i++)
{
    string pageText = pdf.ExtractTextFromPage(i);
    Console.WriteLine($"Page {i + 1}: {pageText}");
}
```

### パターン8：パスワード保護（Sumatraでは不可能）

**Sumatra：** 閲覧のみ、保護を追加できない

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1>機密データ</h1>");

pdf.SecuritySettings.OwnerPassword = "owner123";
pdf.SecuritySettings.UserPassword = "user456";
pdf.SecuritySettings.AllowUserCopyPasteContent = false;
pdf.SecuritySettings.AllowUserPrinting = PdfPrintSecurity.NoPrint;

pdf.SaveAs("protected.pdf");
```

---

## 完全な機能比較

| 機能 | Sumatra PDF | IronPDF |
|---------|-------------|---------|
| **作成** | | |
| HTMLからPDFへ | いいえ | はい |
| URLからPDFへ | いいえ | はい |
| テキストからPDFへ | いいえ | はい |
| 画像からPDFへ | いいえ | はい |
| **操作** | | |
| PDFのマージ | いいえ | はい |
| PDFの分割 | いいえ | はい |
| ページの回転 | いいえ | はい |
| ページの削除 | いいえ | はい |
| ページの並び替え | いいえ | はい |
| **コンテンツ** | | |
| ウォーターマークの追加 | いいえ | はい |
| ヘッダー/フッターの追加 | いいえ | はい |
| テキストのスタンプ | いいえ | はい |
| 画像のスタンプ | いいえ | はい |
| **セキュリティ** | | |
| パスワード保護 | いいえ | はい |
| デジタル署名 | いいえ | はい |
| 暗号化 | いいえ | はい |
| 権限設定 | いいえ | はい |
| **抽出** | | |
| テキストの抽出 | いいえ | はい |
| 画像の抽出 | いいえ | はい |
| **フォーム** | | |
| フォームの入力 | いいえ | はい |
| フォームの作成 | いいえ | はい |
| フォームデータの読み取り | いいえ | はい |
| **表示** | | |
| PDFの表示 | はい（外部） | ブラウザー/ビューア経由 |
| PDFの印刷 | はい（CLI） | ネイティブAPI |
| **プラットフォーム** | | |
| Windows | はい | はい |
| Linux | いいえ | はい |
| macOS | いいえ | はい |
| Webアプリ | いいえ | はい |
| Azure/AWS | いいえ | はい |

---

## 移行シナリオ

### シナリオ1：デスクトップアプリケーションでPDFを表示

**旧アプローチ：** Sumatraを外部プロセスとして起動
```csharp
Process.Start("SumatraPDF.exe", pdfPath);
```

**新アプローチ：** IronPDFの組み込みビューアまたはシステムビューアを使用
```csharp
using IronPdf;
using IronPdf.Viewing;

// オプション1：IronPDFの組み込みビューア（WinForms/WPF）
var pdf = PdfDocument.FromFile("document.pdf");
var viewer = new PdfViewer();
viewer.LoadPdf(pdf);

// オプション2：PDFを作成してシステムビューアで開く
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(htmlContent);
var tempPath = Path.GetTempFileName() + ".pdf";
pdf.SaveAs(tempPath);
Process.Start(new ProcessStartInfo(tempPath) { UseShellExecute = true });
```

### シナリオ2：WebアプリケーションでのPDF生成

**旧アプローチ：** Sumatraでは不可能

**新アプローチ：**
```csharp
// ASP.NET Core コントローラー
public IActionResult GenerateReport()
{
    var renderer = new ChromePdfRenderer();
    var pdf = renderer.RenderHtmlAsPdf(GetReportHtml());

    return File(pdf.BinaryData, "application/pdf", "report.pdf");
}
```

### シナリオ3：バッチPDF処理

**旧アプローチ：** Sumatraでは不可能

**新アプローチ：**
```csharp
var htmlFiles = Directory.GetFiles("input", "*.html");
var renderer = new ChromePdfRenderer();

foreach (var htmlFile in htmlFiles)
{
    var pdf = renderer.RenderHtmlFileAsPdf(htmlFile);
    var outputPath = Path.ChangeExtension(htmlFile, ".pdf");
    pdf.SaveAs(outputPath);
}
```

---

## 移行時の一般的な問題点

### 問題1：ユーザーがSumatraをインストールする必要がある

**問題：** アプリがユーザーにSumatraのインストールを要求する。

**解決策：** IronPDFはアプリにバンドルされるため、外部依存関係は不要。

### 問題2：GPLライセンスの制約

**問題：** SumatraのGPLライセンスが独自のソフトウェアと競合する可能性がある。

**解決策：** IronPDFには独自のソフトウェアと互換性のある商用ライセンスがある。

### 問題3：PDFを作成できない

**問題：** Sumatraは閲覧のみで、PDFを作成できない。

**解決策：** IronPDFはHTML、URL、画像などからPDFを作成する。

### 問題4：プロセス管理のオーバーヘッド

**問題：** 外部Sumatraプロセスの管理が複雑。

**解決策：** IronPDFはプロセス内ライブラリで、シンプルなAPI呼び出しを提供する。

---

## 移行チェックリスト

```markdown
## 移行チェックリスト

### 移行前

- [ ] **コードベース内のすべてのSumatra PDFの使用を調査**
  ```bash
  grep -r "SumatraPDF" --include="*.cs" .
  grep -r "Process.Start" --include="*.cs" .
  ```
  **理由：** Sumatra PDFからIronPDFへの完全な移行カバレッジを確実にするために、すべての使用箇所を特定します。

- [ ] **現在のPDF処理設定を文書化**
  ```csharp
  // 以下のようなパターンを探す：
  var process = new ProcessStartInfo {
      FileName = "SumatraPDF.exe",
      Arguments = "-print-to-default file.pdf"
  };
  ```
  **理由：** これらの設定はIronPDFの機能にマッピングされます。移行後に一貫した動作を確保するために、今それらを文書化します。

- [ ] **IronPDFライセンスキーを取得**
  **理由：** IronPDFは本番使用にライセンスキーが必要です。無料試用版はhttps://ironpdf.com/で入手可能です。

### コード変更

- [ ] **Sumatra PDFのプロセス処理を削除し、IronPdfをインストール**
  ```bash
  // 前（Sumatra PDF）
  // 削除するパッケージはないが、プロセス処理コードを削除

  // 後（IronPDF）
  dotnet add package IronPdf
  ```
  **理由：** 外部プロセス処理からネイティブ.NET PDF処理への移行を行います。

- [ ] **ライセンスキーの初期化を追加**
  ```csharp
  // アプリケーション起動時に追加
  IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
  ```
  **理由：** PDF操作を行う前にライセン