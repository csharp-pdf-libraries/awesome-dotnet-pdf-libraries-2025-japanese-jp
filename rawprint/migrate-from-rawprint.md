---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [rawprint/migrate-from-rawprint.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/rawprint/migrate-from-rawprint.md)
🇯🇵 **日本語:** [rawprint/migrate-from-rawprint.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/rawprint/migrate-from-rawprint.md)

---

# 移行ガイド: RawPrint → IronPDF

## RawPrintから移行する理由は？

RawPrintは、生のバイトをプリンタースプーラーに送信する低レベルの印刷ユーティリティです。PDFライブラリではありません - データをプリンターにプッシュするだけです。主な制限事項:

1. **PDF作成不可**: PDFを作成または生成できない
2. **Windows専用**: Windowsの印刷サブシステムに依存
3. **低レベルAPI**: 手動でのプリンターハンドル管理
4. **文書処理不可**: バイト伝送のみ
5. **限定的な制御**: 最小限の印刷ジョブ設定
6. **クロスプラットフォーム非対応**: Windowsスプーラーに依存

### RawPrintができること vs. 必要なこと

| タスク | RawPrint | IronPDF |
|------|----------|---------|
| HTMLからPDFを作成 | いいえ | はい |
| URLからPDFを作成 | いいえ | はい |
| PDFの編集/修正 | いいえ | はい |
| PDFの結合/分割 | いいえ | はい |
| 既存のPDFを印刷 | はい（生のバイト） | はい（高レベルAPI） |
| 印刷制御 | 基本 | 完全なオプション |
| クロスプラットフォーム | Windowsのみ | はい |

---

## クイックスタート: RawPrintからIronPDFへ

### ステップ1: NuGetパッケージの置き換え

```bash
# RawPrintを削除
dotnet remove package RawPrint

# IronPDFをインストール
dotnet add package IronPdf
```

### ステップ2: ライセンスの初期化

```csharp
IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";
```

### ステップ3: 印刷コードの置き換え

**RawPrint:**
```csharp
using RawPrint;

byte[] pdfBytes = File.ReadAllBytes("document.pdf");
Printer.SendBytesToPrinter("HP LaserJet", pdfBytes, pdfBytes.Length);
```

**IronPDF:**
```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");
pdf.Print("HP LaserJet");
```

---

## APIマッピングリファレンス

| RawPrint | IronPDF | 備考 |
|----------|---------|-------|
| `Printer.SendBytesToPrinter()` | `pdf.Print()` | 高レベルの印刷 |
| `Printer.OpenPrinter()` | 該当なし | 不要 |
| `Printer.ClosePrinter()` | 該当なし | 自動 |
| `Printer.StartDocPrinter()` | 該当なし | 自動 |
| `Printer.WritePrinter()` | 該当なし | 自動 |
| `Printer.EndDocPrinter()` | 該当なし | 自動 |
| 該当なし | `ChromePdfRenderer` | PDFの作成 |
| 該当なし | `PdfDocument.Merge()` | PDFの結合 |
| 該当なし | `pdf.ApplyWatermark()` | ウォーターマークの追加 |

---

## コード例

### 例1: 既存のPDFを印刷

**RawPrint:**
```csharp
using RawPrint;
using System.IO;

byte[] pdfBytes = File.ReadAllBytes("document.pdf");
bool success = Printer.SendBytesToPrinter(
    "Brother HL-L2340D",
    pdfBytes,
    pdfBytes.Length
);

if (!success)
{
    throw new Exception("Print failed");
}
```

**IronPDF:**
```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// シンプルな印刷
pdf.Print();

// またはプリンターを指定
pdf.Print("Brother HL-L2340D");
```

### 例2: PDFを作成して印刷する（RawPrintでは不可能）

**RawPrint:**
```csharp
// 不可能 - RawPrintはPDFを作成できない
// 最初にPDFを作成するために別のライブラリが必要
```

**IronPDF:**
```csharp
using IronPdf;

// HTMLからPDFを作成
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(@"
    <h1>Invoice #12345</h1>
    <p>Customer: John Doe</p>
    <p>Amount: $150.00</p>
");

// 直接印刷
pdf.Print("HP LaserJet");

// または最初に保存
pdf.SaveAs("invoice.pdf");
pdf.Print();
```

### 例3: 設定付きで印刷

**RawPrint:**
```csharp
using RawPrint;
using System;

// 手動でのプリンターハンドル管理
IntPtr printerHandle = IntPtr.Zero;
try
{
    Printer.OpenPrinter("HP LaserJet", out printerHandle);
    Printer.StartDocPrinter(printerHandle, 1, "Document");

    byte[] pdfBytes = File.ReadAllBytes("report.pdf");
    Printer.WritePrinter(printerHandle, pdfBytes, pdfBytes.Length);

    Printer.EndDocPrinter(printerHandle);
}
finally
{
    if (printerHandle != IntPtr.Zero)
        Printer.ClosePrinter(printerHandle);
}
```

**IronPDF:**
```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("report.pdf");

// 完全な設定で印刷
pdf.Print(new PrintOptions
{
    PrinterName = "HP LaserJet",
    NumberOfCopies = 2,
    DPI = 300,
    GrayScale = false
});
```

### 例4: サイレント/バックグラウンド印刷

**RawPrint:**
```csharp
using RawPrint;

// RawPrintは本質的にサイレントだが、ダイアログオプションはない
byte[] data = File.ReadAllBytes("document.pdf");
Printer.SendBytesToPrinter("Microsoft Print to PDF", data, data.Length);
```

**IronPDF:**
```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("document.pdf");

// サイレント印刷（ダイアログなし）
pdf.Print("Microsoft Print to PDF");

// またはダイアログ付きで印刷
pdf.PrintWithDialog();
```

### 例5: 生成されたレポートを印刷

**RawPrint:**
```csharp
// PDFを作成するために外部ライブラリが必要、その後：
using RawPrint;

// 何か他のものを使ってpdfBytesを作成したと仮定
byte[] pdfBytes = SomeOtherLibrary.CreatePdf(data);
Printer.SendBytesToPrinter("Printer Name", pdfBytes, pdfBytes.Length);
```

**IronPDF:**
```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

// ページオプションの設定
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;

// ヘッダーとフッター
renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
{
    HtmlFragment = "<div style='text-align:center'>Monthly Report</div>",
    MaxHeight = 25
};

renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
{
    HtmlFragment = "<div style='text-align:center'>Page {page} of {total-pages}</div>",
    MaxHeight = 25
};

// 生成して印刷
var pdf = renderer.RenderHtmlAsPdf(reportHtml);
pdf.Print("HP LaserJet Pro");
```

### 例6: バッチ印刷

**RawPrint:**
```csharp
using RawPrint;

foreach (var filePath in pdfFiles)
{
    byte[] bytes = File.ReadAllBytes(filePath);
    Printer.SendBytesToPrinter("Printer", bytes, bytes.Length);
}
```

**IronPDF:**
```csharp
using IronPdf;

// オプション1: 各ファイルを印刷
foreach (var filePath in pdfFiles)
{
    var pdf = PdfDocument.FromFile(filePath);
    pdf.Print("Printer");
}

// オプション2: 一度に結合して印刷
var pdfs = pdfFiles.Select(f => PdfDocument.FromFile(f)).ToList();
var merged = PdfDocument.Merge(pdfs);
merged.Print("Printer");
```

---

## 機能比較

| 機能 | RawPrint | IronPDF |
|---------|----------|---------|
| **PDF作成** | | |
| HTMLからPDF | いいえ | はい |
| URLからPDF | いいえ | はい |
| ゼロから作成 | いいえ | はい |
| **PDF操作** | | |
| PDFの結合 | いいえ | はい |
| PDFの分割 | いいえ | はい |
| ウォーターマークの追加 | いいえ | はい |
| 既存の編集 | いいえ | はい |
| **印刷** | | |
| PDF印刷 | はい（生） | はい（高レベル） |
| 印刷ダイアログ | いいえ | はい |
| 複数コピー | 限定的 | はい |
| DPI制御 | いいえ | はい |
| 両面印刷 | いいえ | はい |
| **プラットフォーム** | | |
| Windows | はい | はい |
| Linux | いいえ | はい |
| macOS | いいえ | はい |
| Docker | いいえ | はい |
| **その他** | | |
| セキュリティ | いいえ | はい |
| デジタル署名 | いいえ | はい |
| PDF/A | いいえ | はい |

---

## 一般的な移行シナリオ

### シナリオ1: レポートを印刷

**以前:** PDFを他の場所で作成し、その後RawPrintを使用
```csharp
// ステップ1: 何らかのライブラリを使用してPDFを作成
byte[] pdf = CreatePdfSomehow(reportData);
// ステップ2: RawPrint
Printer.SendBytesToPrinter("Printer", pdf, pdf.Length);
```

**その後:** IronPDFですべてを一つに
```csharp
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(reportHtml);
pdf.Print("Printer");
```

### シナリオ2: 印刷キュー処理

**以前:**
```csharp
while (queue.TryDequeue(out var job))
{
    var bytes = File.ReadAllBytes(job.PdfPath);
    Printer.SendBytesToPrinter(job.PrinterName, bytes, bytes.Length);
}
```

**その後:**
```csharp
while (queue.TryDequeue(out var job))
{
    var pdf = PdfDocument.FromFile(job.PdfPath);
    pdf.Print(new PrintOptions
    {
        PrinterName = job.PrinterName,
        NumberOfCopies = job.Copies
    });
}
```

---

## 移行チェックリスト

### 移行前
- [ ] RawPrintの使用箇所を特定
- [ ] 使用されているプリンター名を文書化
- [ ] 外部PDF作成コードをメモ
- [ ] IronPDFライセンスを取得

### コードの更新
- [ ] RawPrintパッケージを削除
- [ ] IronPdfパッケージをインストール
- [ ] 生の印刷をpdf.Print()に置き換え
- [ ] PDF作成と印刷を統合
- [ ] ライセンス初期化を追加
- [ ] 手動ハンドル管理を削除

### テスト
- [ ] 対象プリンターへの印刷をテスト
- [ ] 印刷品質を確認
- [ ] 複数コピーをテスト
- [ ] サイレント印刷をテスト
- [ ] 必要に応じてクロスプラットフォームをテスト

---

## 追加リソース

- [IronPDF Documentation](https://ironpdf.com/docs/)
- [IronPDF Printing Guide](https://ironpdf.com/how-to/csharp-print-pdf/)
- [IronPDF Tutorials](https://ironpdf.com/tutorials/)

---

*この移行ガイドは、[Awesome .NET PDF Libraries](../README.md) コレクションの一部です。*