# PDFPrinting.NETからIronPDFへの移行方法は？

## なぜPDFPrinting.NETから移行するのか？

PDFPrinting.NETは、Windows環境内でのサイレントPDF印刷に特化したライブラリです。その狭い目的には優れていますが、アプリケーションが印刷を超えてPDFの作成、編集、または操作が必要になった場合には対応できません。

### 印刷のみの制限

PDFPrinting.NETは、以下の1つのタスクにのみ焦点を当てています：

1. **印刷のみ**：PDFドキュメントを作成、編集、または操作できません
2. **Windowsのみ**：Windows印刷インフラに依存しています—Linux/macOSはサポートされていません
3. **PDF生成不可**：HTML、URL、またはデータをPDFに変換できません
4. **ドキュメント操作不可**：PDFをマージ、分割、透かしを入れる、またはセキュリティを強化することができません
5. **テキスト抽出不可**：PDFからコンテンツを読み取ったり抽出したりすることができません
6. **フォーム処理不可**：PDFフォームを入力または平滑化することができません
7. **モダンなWebコンテンツ不可**：JavaScriptや複雑なCSSレイアウトをレンダリングできません

### なぜIronPDFを選ぶのか？

IronPDFは、完全なPDFライフサイクル管理に加えて印刷を提供します：

- **完全なPDF生成**：HTML、URL、画像などからPDFを作成
- **クロスプラットフォーム**：Windows、Linux、macOSで動作
- **PDF操作**：PDFをマージ、分割、回転、修正
- **セキュリティ機能**：パスワード、暗号化、デジタル署名
- **テキスト抽出**：PDFコンテンツの読み取りと検索
- **フォーム処理**：PDFフォームの入力と平滑化
- **印刷**：PDFPrinting.NETの機能に加えてさらに多くの機能
- **モダンなレンダリング**：完全なCSS3/JSサポートを備えたChromiumベースのエンジン

---

## 移行の概要

| 項目 | PDFPrinting.NET | IronPDF |
|--------|-----------------|---------|
| 主な焦点 | サイレントPDF印刷 | 完全なPDFライフサイクル |
| PDF作成 | サポートされていません | 包括的 |
| HTMLからPDFへ | サポートされていません | 完全なChromiumエンジン |
| PDF操作 | サポートされていません | マージ、分割、回転 |
| テキスト抽出 | サポートされていません | 完全サポート |
| プラットフォームサポート | Windowsのみ | クロスプラットフォーム |
| プリンター統合 | Windows Print API | クロスプラットフォーム印刷 |
| サイレント印刷 | はい | はい |
| 印刷設定 | 基本 | 包括的 |
| ライセンス | 商用 | 商用 |

---

## NuGetパッケージの変更

```bash
# PDFPrinting.NETを削除
dotnet remove package PDFPrinting.NET
dotnet remove package PDFPrintingNET

# IronPDFをインストール
dotnet add package IronPdf
```

---

## 名前空間の変更

```csharp
// 以前：PDFPrinting.NET
using PDFPrintingNET;
using PDFPrinting;
using PDFPrinting.NET;

// 以降：IronPDF
using IronPdf;
using IronPdf.Printing;
```

---

## APIマッピングリファレンス

### コアクラス

| PDFPrinting.NET | IronPDF | メモ |
|-----------------|---------|-------|
| `PDFPrinter` | `PdfDocument` | コアPDFオブジェクト |
| `PDFPrintDocument` | `PdfDocument` | 代替クラス名 |
| `HtmlToPdfConverter` | `ChromePdfRenderer` | PDF生成 |
| `WebPageToPdfConverter` | `ChromePdfRenderer` | URL変換 |
| 印刷設定プロパティ | `PrintSettings` | 印刷設定 |

### 印刷メソッド

| PDFPrinting.NET | IronPDF | メモ |
|-----------------|---------|-------|
| `printer.Print(filePath)` | `pdf.Print()` | デフォルトプリンターへ印刷 |
| `printer.Print(filePath, printerName)` | `pdf.Print(printerName)` | 特定のプリンターへ印刷 |
| `printer.PrinterName = "..."` | `pdf.Print("...")` | プリンター指定 |
| `printer.GetPrintDocument(path)` | `pdf.GetPrintDocument()` | PrintDocumentを取得 |
| `printer.PageScaling` | `printSettings.PrinterResolution` | スケーリングオプション |
| `printer.Copies = n` | `printSettings.NumberOfCopies = n` | コピー数 |
| `printer.Duplex` | `printSettings.DuplexMode` | 両面印刷 |
| `printer.CollatePages` | `printSettings.Collate` | 並べ替え |
| `printer.PrintInColor` | `printSettings.GrayscaleOutput` | 色設定 |
| `printer.PaperSource` | `printSettings.PaperTray` | 紙のソース |

### PDF生成（IronPDFでの新機能）

| 機能 | IronPDFメソッド | メモ |
|---------|----------------|-------|
| HTMLからPDFへ | `renderer.RenderHtmlAsPdf(html)` | 完全なHTML/CSS/JS |
| URLからPDFへ | `renderer.RenderUrlAsPdf(url)` | Webページキャプチャ |
| ファイルからPDFへ | `renderer.RenderHtmlFileAsPdf(path)` | HTMLファイル |
| 画像からPDFへ | `ImageToPdfConverter.ImageToPdf(paths)` | 画像 |

### PDF操作（IronPDFでの新機能）

| 機能 | IronPDFメソッド | メモ |
|---------|----------------|-------|
| PDF読み込み | `PdfDocument.FromFile(path)` | 既存の読み込み |
| PDFマージ | `PdfDocument.Merge(pdfs)` | 結合 |
| PDF分割 | `pdf.CopyPages(start, end)` | ページ抽出 |
| ウォーターマーク追加 | `pdf.ApplyWatermark(html)` | オーバーレイ |
| パスワード追加 | `pdf.SecuritySettings.UserPassword` | セキュリティ |
| テキスト抽出 | `pdf.ExtractAllText()` | コンテンツ読み取り |

---

## コード移行例

### 例1：基本的なサイレント印刷

**以前（PDFPrinting.NET）：**
```csharp
using PDFPrintingNET;
using System;

class Program
{
    static void Main()
    {
        string filePath = "document.pdf";

        var printer = new PDFPrinter();
        printer.Print(filePath);

        Console.WriteLine("PDFが正常に印刷されました。");
    }
}
```

**以降（IronPDF）：**
```csharp
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

        string filePath = "document.pdf";

        var pdf = PdfDocument.FromFile(filePath);
        pdf.Print();

        Console.WriteLine("PDFが正常に印刷されました。");
    }
}
```

### 例2：特定のプリンターへ印刷

**以前（PDFPrinting.NET）：**
```csharp
using PDFPrintingNET;
using System;

class Program
{
    static void Main()
    {
        var printer = new PDFPrinter();
        printer.PrinterName = "HP LaserJet Pro";
        printer.Print("document.pdf");

        Console.WriteLine("PDFがHP LaserJet Proに送信されました。");
    }
}
```

**以降（IronPDF）：**
```csharp
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

        var pdf = PdfDocument.FromFile("document.pdf");
        pdf.Print("HP LaserJet Pro");

        Console.WriteLine("PDFがHP LaserJet Proに送信されました。");
    }
}
```

### 例3：カスタム設定で印刷

**以前（PDFPrinting.NET）：**
```csharp
using PDFPrintingNET;
using System;

class Program
{
    static void Main()
    {
        var printer = new PDFPrinter();
        printer.PrinterName = "Office Printer";
        printer.Copies = 3;
        printer.PageScaling = PDFPageScaling.FitToPrintableArea;
        printer.Duplex = true;
        printer.CollatePages = true;
        printer.PrintInColor = false;

        printer.Print("report.pdf");

        Console.WriteLine("3枚のグレースケール両面コピーが印刷されました。");
    }
}
```

**以降（IronPDF）：**
```csharp
using IronPdf;
using IronPdf.Printing;
using System;

class Program
{
    static void Main()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

        var pdf = PdfDocument.FromFile("report.pdf");

        var printSettings = new PrintSettings
        {
            PrinterName = "Office Printer",
            NumberOfCopies = 3,
            DuplexMode = System.Drawing.Printing.Duplex.Vertical,
            Collate = true,
            GrayscaleOutput = true
        };

        pdf.Print(printSettings);

        Console.WriteLine("3枚のグレースケール両面コピーが印刷されました。");
    }
}
```

### 例4：高度な制御のためのPrintDocumentへのアクセス

**以前（PDFPrinting.NET）：**
```csharp
using PDFPrintingNET;
using System.Drawing.Printing;
using System;

class Program
{
    static void Main()
    {
        var printer = new PDFPrinter();
        var printDoc = printer.GetPrintDocument("document.pdf");

        // System.Drawing.Printing.PrintDocumentへのアクセス
        printDoc.PrinterSettings.PrinterName = "Network Printer";
        printDoc.PrinterSettings.Copies = 2;
        printDoc.PrinterSettings.FromPage = 1;
        printDoc.PrinterSettings.ToPage = 5;

        printDoc.Print();
    }
}
```

**以降（IronPDF）：**
```csharp
using IronPdf;
using System.Drawing.Printing;
using System;

class Program
{
    static void Main()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

        var pdf = PdfDocument.FromFile("document.pdf");
        var printDoc = pdf.GetPrintDocument();

        // 同じSystem.Drawing.Printing.PrintDocumentへのアクセス
        printDoc.PrinterSettings.PrinterName = "Network Printer";
        printDoc.PrinterSettings.Copies = 2;
        printDoc.PrinterSettings.FromPage = 1;
        printDoc.PrinterSettings.ToPage = 5;

        printDoc.Print();
    }
}
```

### 例5：ページ範囲の印刷

**以前（PDFPrinting.NET）：**
```csharp
using PDFPrintingNET;
using System;

class Program
{
    static void Main()
    {
        var printer = new PDFPrinter();
        printer.FromPage = 2;
        printer.ToPage = 5;

        printer.Print("document.pdf");
    }
}
```

**以降（IronPDF）：**
```csharp
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

        var pdf = PdfDocument.FromFile("document.pdf");
        var printDoc = pdf.GetPrintDocument();

        printDoc.PrinterSettings.FromPage = 2;
        printDoc.PrinterSettings.ToPage = 5;
        printDoc.PrinterSettings.PrintRange = System.Drawing.Printing.PrintRange.SomePages;

        printDoc.Print();
    }
}
```

### 例6：PDFを作成してから印刷する（新しい機能）

**以前（PDFPrinting.NET）：**
```csharp
// 不可能
// PDFPrinting.NETは既存のPDFのみを印刷できます
// 最初にPDFを作成するために別のライブラリが必要です
```

**以降（IronPDF）：**
```csharp
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

        // HTMLからPDFを作成
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderHtmlAsPdf(@"
            <html>
            <head>
                <style>
                    body { font-family: Arial; }
                    h1 { color: navy; }
                </style>
            </head>
            <body>
                <h1>Invoice #12345</h1>
                <p>Amount Due: $1,234.56</p>
            </body>
            </html>
        ");

        // 即時印刷
        pdf.Print("Invoice Printer");

        // または後で印刷するために保存
        pdf.SaveAs("invoice.pdf");

        Console.WriteLine("PDFが作成され、印刷されました。");
    }
}
```

### 例7：URLから印刷する（新しい機能）

**以前（PDFPrinting.NET）：**
```csharp
// 不可能
// PDFPrinting.NETはURLをPDFに変換できません
// 最初にキャプチャして変換するために別のライブラリが必要です
```

**以降（IronPDF）：**
```csharp
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

        var renderer = new ChromePdfRenderer();

        // レンダリングを設定
        renderer.RenderingOptions.RenderDelay = 2000;  // JSの待機
        renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;

        // WebページをPDFとしてキャプチャ
        var pdf = renderer.RenderUrlAsPdf("https://example.com/report");

        // 直接印刷
        pdf.Print();

        Console.WriteLine("Webページがキャプチャされ、印刷されました。");
    }
}
```

### 例8：複数のPDFをバッチ印刷

**以前（PDFPrinting.NET）：**
```csharp
using PDFPrintingNET;
using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var printer = new PDFPrinter();
        printer.PrinterName = "Office Printer";

        var files = new List<string>
        {
            "document1.pdf",
            "document2.pdf",
            "document3.pdf"
        };

        foreach (var file in files)
        {
            printer.Print(file);
            Console.WriteLine($"印刷されました：{file}");
        }
    }
}
```

**以降（IronPDF）：**
```csharp
using IronPdf;
using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        IronPdf.License.LicenseKey = "YOUR-LICENSE-KEY";

        var files = new List<string>
        {
            "document1.pdf",
            "document2.pdf",
            "document3.pdf"
        };

        foreach (var file in files)
        {
            var pdf = PdfDocument.FromFile(file);
            pdf.Print("Office Printer");
            Console.WriteLine($"印刷されました：{file}");
        }

        // または1つのジョブとしてマージしてから印刷
        var merged = PdfDocument.Merge(files