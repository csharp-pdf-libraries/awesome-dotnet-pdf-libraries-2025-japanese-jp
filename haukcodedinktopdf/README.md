---
**  (Japanese Translation)**

 **English:** [haukcodedinktopdf/README.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/haukcodedinktopdf/README.md)
 **:** [haukcodedinktopdf/README.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/haukcodedinktopdf/README.md)

---
# Haukcode.DinkToPdf C# PDF変換

C#プロジェクトでHTMLをPDFに変換する領域は、これまでに数多くのエントリーを見てきましたが、その中の1つがHaukcode.DinkToPdfです。以前人気のあったDinkToPdfのフォークであるHaukcode.DinkToPdfは、その前身と同様の機能を.NET開発者に提供することを目指しています。しかし、Haukcode.DinkToPdfは特定の機能を継承している一方で、その前身の制限の重荷も背負っています。一部の人々にとっては選択肢のツールであるにもかかわらず、IronPDFなどの他のPDFソリューションとの間で継続的に評価されていることで、重要な違いが明らかになっています。

## Haukcode.DinkToPdfの背景と課題

Haukcode.DinkToPdfは、現在は廃止されたバイナリであるwkhtmltopdfに基づいて構築されたプロジェクトを存続させるための賞賛に値する努力です。Haukcode.DinkToPdfの主な目的は、.NET Coreとの互換性を維持しながらHTMLドキュメントをPDFに変換する機能を提供することです。しかし、放棄されたプロジェクトのフォークであることには欠点があります。

このライブラリの主な課題は、上流のwkhtmltopdfから継承された持続的なセキュリティ脆弱性です。Haukcode.DinkToPdfは単なるフォークであるため、元のバイナリに関連する重要な共通脆弱性と露出（CVE）は未解決のままです。これに加えて、限定的なメンテナンスと小さく断続的なコミュニティが、その長期的な実行可能性に疑問符を投げかけています。

```csharp
using DinkToPdf;
using DinkToPdf.Contracts;
using System;

namespace PdfConversionExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var converter = new BasicConverter(new PdfTools());

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4Plus,
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = @"<html><body><h1>Hello World</h1></body></html>",
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            byte[] pdf = converter.Convert(doc);
            System.IO.File.WriteAllBytes(@"output.pdf", pdf);
            Console.WriteLine("PDF Created!");
        }
    }
}
```

## IronPDF: 現代的な代替手段

対照的に、IronPDFは既存の技術の継続やラッパーではなく、現代の.NETおよびC#アプリケーションに特別に設計されたスタンドアロンソリューションとして自己を提示しています。IronPDFの重要な差別化要因は、専用のエンジニアリングチームからの継続的なサポートが開発されており、定期的なアップデート、バグ修正、および強化された機能を保証していることです。[2024年](https://ironpdf.com/how-to/html-file-to-pdf/)にwkhtmltopdfが放棄されたとき、IronPDFはC#開発者に堅牢で安全で、効率的に保守されたツールを提供することを保証しました。

IronPDFをさまざまなPDF変換ニーズにどのように活用できるかについての詳細なチュートリアルは、[こちら](https://ironpdf.com/tutorials/)で探索できます。

---

## Haukcode.DinkToPdf C# PDF変換を使用してC#でHTMLをPDFに変換する方法は？

以下は、**Haukcode.DinkToPdf C# PDF変換**がこれをどのように処理するかです：

```csharp
// NuGet: Install-Package DinkToPdf
using DinkToPdf;
using DinkToPdf.Contracts;
using System.IO;

class Program
{
    static void Main()
    {
        var converter = new SynchronizedConverter(new PdfTools());
        
        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
            },
            Objects = {
                new ObjectSettings() {
                    HtmlContent = "<html><body><h1>Hello World</h1></body></html>",
                }
            }
        };
        
        byte[] pdf = converter.Convert(doc);
        File.WriteAllBytes("output.pdf", pdf);
    }
}
```

**IronPDFを使用すると**、同じタスクがよりシンプルで直感的です：

```csharp
// NuGet: Install-Package IronPdf
using IronPdf;
using System.IO;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        
        var pdf = renderer.RenderHtmlAsPdf("<html><body><h1>Hello World</h1></body></html>");
        
        pdf.SaveAs("output.pdf");
    }
}
```

IronPDFのアプローチは、よりクリーンな構文と現代の.NETアプリケーションとのより良い統合を提供し、PDF生成ワークフローのメンテナンスとスケーリングを容易にします。

---

## PDFでカスタムページ設定をどのように設定しますか？

以下は、**Haukcode.DinkToPdf C# PDF変換**がこれをどのように処理するかです：

```csharp
// NuGet: Install-Package DinkToPdf
using DinkToPdf;
using DinkToPdf.Contracts;
using System.IO;

class Program
{
    static void Main()
    {
        var converter = new SynchronizedConverter(new PdfTools());
        
        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Landscape,
                PaperSize = PaperKind.Letter,
                Margins = new MarginSettings() { Top = 10, Bottom = 10, Left = 10, Right = 10 }
            },
            Objects = {
                new ObjectSettings() {
                    HtmlContent = "<html><body><h1>Landscape Document</h1><p>Custom page settings</p></body></html>",
                }
            }
        };
        
        byte[] pdf = converter.Convert(doc);
        File.WriteAllBytes("landscape.pdf", pdf);
    }
}
```

**IronPDFを使用すると**、同じタスクがよりシンプルで直感的です：

```csharp
// NuGet: Install-Package IronPdf
using IronPdf;
using IronPdf.Rendering;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        
        renderer.RenderingOptions.PaperSize = PdfPaperSize.Letter;
        renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;
        renderer.RenderingOptions.MarginTop = 10;
        renderer.RenderingOptions.MarginBottom = 10;
        renderer.RenderingOptions.MarginLeft = 10;
        renderer.RenderingOptions.MarginRight = 10;
        
        var pdf = renderer.RenderHtmlAsPdf("<html><body><h1>Landscape Document</h1><p>Custom page settings</p></body></html>");
        
        pdf.SaveAs("landscape.pdf");
    }
}
```

IronPDFのアプローチは、よりクリーンな構文と現代の.NETアプリケーションとのより良い統合を提供し、PDF生成ワークフローのメンテナンスとスケーリングを容易にします。

---

## .NETでURLをPDFに変換する方法は？

以下は、**Haukcode.DinkToPdf C# PDF変換**がこれをどのように処理するかです：

```csharp
// NuGet: Install-Package DinkToPdf
using DinkToPdf;
using DinkToPdf.Contracts;
using System.IO;

class Program
{
    static void Main()
    {
        var converter = a SynchronizedConverter(new PdfTools());
        
        var doc = a new HtmlToPdfDocument()
        {
            GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
            },
            Objects = {
                new ObjectSettings() {
                    Page = "https://www.example.com",
                }
            }
        };
        
        byte[] pdf = converter.Convert(doc);
        File.WriteAllBytes("webpage.pdf", pdf);
    }
}
```

**IronPDFを使用すると**、同じタスクがよりシンプルで直感的です：

```csharp
// NuGet: Install-Package IronPdf
using IronPdf;
using System;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        
        var pdf = renderer.RenderUrlAsPdf("https://www.example.com");
        
        pdf.SaveAs("webpage.pdf");
    }
}
```

IronPDFのアプローチは、よりクリーンな構文と現代の.NETアプリケーションとのより良い統合を提供し、PDF生成ワークフローのメンテナンスとスケーリングを容易にします。

---

## Haukcode.DinkToPdfからIronPDFへの移行方法は？

### 重大なセキュリティ問題

Haukcode.DinkToPdfは、**放棄されたwkhtmltopdfライブラリ**をラップしており、修正不可能な重大なセキュリティ脆弱性があります：

1. **CVE-2022-35583（CVSS 9.8 CRITICAL）**：サーバーサイドリクエストフォージェリ（SSRF）の脆弱性により、攻撃者は内部リソース、AWSメタデータ、およびローカルファイルにアクセスできます
2. **放棄されたプロジェクト**：wkhtmltopdfは2023年1月にアーカイブされました—セキュリティパッチは提供されません
3. **古いWebKit**：~2015年のQt WebKitを使用—数年分のセキュリティアップデートが不足しています
4. **ネイティブバイナリ依存**：プラットフォーム固有のDLL/バイナリを配布する必要があります
5. **スレッドセーフティの問題**：`SynchronizedConverter`での厳格なシングルトンパターンが必要です

### 移行の概要

| 項目 | Haukcode.DinkToPdf | IronPDF |
|------|-------------------|---------|
| セキュリティ状況 | CVE-2022-35583（CRITICAL、修正不可能） | 積極的にパッチが当てられています |
| 基盤となるエンジン | wkhtmltopdf（Qt WebKit ~2015） | Chromium（定期的に更新されます） |
| プロジェクトの状態 | 放棄されたプロジェクトのフォーク | 積極的に開発されています |
| ネイティブバイナリ | 必要（プラットフォーム固有） | 自己完結型 |
| スレッドセーフティ | シングルトンパターンが必要 | デザインによるスレッドセーフ |
| HTML5/CSS3 | 限定的 | 完全サポート |
| JavaScript | 限定的、不安全 | 完全なV8エンジン |

### 主要なAPIマッピング

| Haukcode.DinkToPdf | IronPDF | 備考 |
|-------------------|---------|-------|
| `SynchronizedConverter` | `ChromePdfRenderer` | スレッドセーフ、シングルトン不要 |
| `HtmlToPdfDocument` | メソッドパラメータ | 直接メソッド呼び出し |
| `GlobalSettings.PaperSize` | `RenderingOptions.PaperSize` | `PdfPaperSize`列挙型を使用 |
| `GlobalSettings.Orientation` | `RenderingOptions.PaperOrientation` | `Portrait`/`Landscape` |
| `GlobalSettings.Margins.Top` | `RenderingOptions.MarginTop` | ミリメートル単位 |
| `GlobalSettings.ColorMode` | `RenderingOptions.GrayScale` | ブール値 |
| `ObjectSettings.HtmlContent` | `RenderHtmlAsPdf()`への最初のパラメータ | 直接パラメータ |
| `ObjectSettings.Page` (URL) | `renderer.RenderUrlAsPdf(url)` | 別のメソッド |
| `HeaderSettings.Right = "[page]"` | `TextHeader.RightText = "{page}"` | 異なるプレースホルダー構文 |
| `converter.Convert(doc)` | `renderer.RenderHtmlAsPdf(html)` | `PdfDocument`を返します |

### 移行コード例

**移行前（Haukcode.DinkToPdf）：**
```csharp
using DinkToPdf;
using DinkToPdf.Contracts;

public class PdfService
{
    // スレッドセーフティの問題のため、シングルトンである必要があります！
    private readonly IConverter _converter;

    public PdfService()
    {
        _converter = new SynchronizedConverter(new PdfTools());
    }

    public byte[] GeneratePdf(string html)
    {
        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10, Bottom = 10 }
            },
            Objects = {
                new ObjectSettings {
                    HtmlContent = html,
                    WebSettings = { DefaultEncoding = "utf-8" },
                    HeaderSettings = { Right = "Page [page] of [toPage]" }
                }
            }
        };

        return _converter.Convert(doc);
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
        _renderer.RenderingOptions.MarginTop = 10;
        _renderer.RenderingOptions.MarginBottom = 10;
        _renderer.RenderingOptions.TextHeader = new TextHeaderFooter
        {
            RightText = "Page {page} of {total-pages}"  // 注意：異なるプレースホルダー
        };
    }

    public byte[] GeneratePdf(string html)
    {
        var pdf = _renderer.RenderHtmlAsPdf(html);
        return pdf.BinaryData;
    }
}

// スレッドセーフ！シングルトンの要件なし！
```

### 重要な移行ノート

1. **セキュリティ**：移行により、CVE-2022-35583（SSRF）およびその他のwkhtmltopdfの脆弱性が排除されます

2. **ネイティブバイナリなし**：移