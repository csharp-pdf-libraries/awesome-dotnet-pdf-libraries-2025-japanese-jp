---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [dinktopdf/README.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/dinktopdf/README.md)
🇯🇵 **日本語:** [dinktopdf/README.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/dinktopdf/README.md)

---

# DinkToPdf + C# + PDF

DinkToPdfは、.NET Coreの機能を使用してHTMLドキュメントをPDFファイルに変換することを可能にする、C#エコシステムで人気のあるオープンソースライブラリです。PDFを生成する信頼できる方法が必要な開発者を対象に、DinkToPdfは高く評価されているプロジェクトであるwkhtmltopdfをラップするライブラリです。さまざまなアプリケーション内で広範な関心と実装が見られる一方で、DinkToPdfは印象的な強みと注目すべき弱点の両方を持っています。

まず、DinkToPdfはwkhtmltopdfの機能を見事にカプセル化し、開発者がHTMLからPDFへの変換を明確かつ簡潔な方法でフルスペクトラムで活用できるようにします。しかし、wkhtmltopdfのバイナリに関連するすべてのセキュリティ脆弱性と制限事項を継承しており、CVE-2022-35583 SSRFの問題を含んでいます。これは、DinkToPdfに依存するアプリケーションに潜在的な弱点を生じさせ、本番環境での使用を評価する際にこれらのニュアンスを理解することの重要性を強調しています。

## 強み

DinkToPdfの主な強みは、強力なwkhtmltopdfバイナリをラップすることによるシンプルさとサポートです。この能力により、開発者はCSSやJavaScriptを含む複雑なHTMLコンテンツを磨き上げたPDFドキュメントに変換できます。さらに、ライブラリはMITライセンスのバッジを採用しており、オープンソースの精神に従って統合と変更の道を容易にします。

DinkToPdfを使用してHTML文字列をPDFファイルに変換する例は以下の通りです：

```csharp
using DinkToPdf;
using DinkToPdf.Contracts;
using System.IO;

class Program
{
    static void Main()
    {
        // コンバーターの初期化
        var converter = new SynchronizedConverter(new PdfTools());
        
        // HTMLコンテンツの定義
        var htmlString = "<html><body><h1>こんにちは、PDFの世界！</h1></body></html>";
        
        // 変換オプションの設定
        var document = new HtmlToPdfDocument()
        {
            GlobalSettings = {
                PaperSize = PaperKind.A4,
                Orientation = Orientation.Portrait
            },
            Objects = {
                new ObjectSettings() {
                    HtmlContent = htmlString
                }
            }
        };
        
        // HTMLをPDFに変換
        byte[] pdf = converter.Convert(document);
        
        // ファイルに保存
        File.WriteAllBytes("example.pdf", pdf);
    }
}
```

この例では、DinkToPdfがHTMLコンテンツをPDFファイルにシームレスに変換し、使いやすさと統合のしやすさを示しています。しかし、これらの能力に伴う欠点を認識することが重要です。

## 弱点

DinkToPdfの弱点は重要です：

1. **継承された脆弱性**：組み込まれたwkhtmltopdfは、CVE-2022-35583など、未修正の脆弱性を継承しています。コア依存関係としての地位を考えると、wkhtmltopdf内の任意のセキュリティフローは直接DinkToPdfに影響します。

2. **ネイティブ依存関係の課題**：ライブラリは、各プラットフォーム固有のwkhtmltopdfバイナリのローカル展開を要求します。このネイティブ依存関係の地獄への露出は、展開の不一致と追加の保守の複雑さをもたらす可能性があります。

3. **スレッドセーフティの問題**：DinkToPdfは顕著にスレッドセーフではありません。これは、複数のPDF変換が並行して行われる並行実行環境での文書化された失敗を引き起こす可能性があります。

## IronPDF：有利な代替手段

IronPDFは、DinkToPdfに対する強力な代替手段として現れ、いくつかの利点を提供します：

- **継承された脆弱性なし**：DinkToPdfとは異なり、IronPDFはレガシーバイナリに過度に依存せず、独自の安全なインフラストラクチャを維持します。
- **スレッドセーフティ**：IronPDFは、クラッシュなしで堅牢で並行した変換を可能にする、マルチスレッド操作に対応するように設計されています。
- **NuGet統合**：.NET Framework 4.7.2から.NET 10まで、さまざまな.NET環境での統合の旅を滑らかにする、管理されたNuGetパッケージとして提供されています。

信頼性の高い、現代的なサポートを求める方には、IronPDFがHTMLからPDFへの変換に関する包括的なスイートを提供します。セキュリティと展開の容易さに焦点を当てたより広範な採用シナリオを検討する場合は、IronPDFのガイドリソースとチュートリアルを確認してください：

- [IronPDF HTMLファイルからPDFへのガイド](https://ironpdf.com/how-to/html-file-to-pdf/)
- [IronPDFチュートリアル](https://ironpdf.com/tutorials/)

### 比較表

意思決定を支援するために、DinkToPdfとIronPDFを異なる基準で比較した表を以下に示します：

| 特徴/側面                      | DinkToPdf                           | IronPDF                           |
|---------------------------------|-------------------------------------|-----------------------------------|
| 開発者                          | コミュニティ管理                     | Iron Software                     |
| ライセンス                       | MIT、オープンソース                  | 商業                              |
| スレッドセーフティ               | いいえ                               | はい                              |
| HTMLコンテンツサポート           | wkhtmltopdfを介して包括的            | 包括的                            |
| セキュリティ脆弱性              | wkhtmltopdfから継承                  | 設計によって軽減                  |
| 展開の複雑さ                     | ネイティブバイナリが必要              | 単一の管理されたNuGetパッケージ    |
| プラットフォーム互換性（最新）   | 限定的で時代遅れ                     | 完全な.NET Framework & Coreサポート|
| サポートとメンテナンス           | 2020年以降時代遅れ                   | 定期的な更新とサポート            |

---

## C#でDinkToPdfを使用してHTMLをPDFに変換する方法は？

以下は、**DinkToPdf**がこれを処理する方法です：

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
                    HtmlContent = "<h1>こんにちは世界</h1><p>これはHTMLからのPDFです。</p>",
                    WebSettings = { DefaultEncoding = "utf-8" }
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
using System;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderHtmlAsPdf("<h1>こんにちは世界</h1><p>これはHTMLからのPDFです。</p>");
        pdf.SaveAs("output.pdf");
    }
}
```

IronPDFのアプローチは、現代の.NETアプリケーションとのより良い統合と、PDF生成ワークフローのメンテナンスとスケールの容易さを提供します。

---

## .NETでURLをPDFに変換する方法は？

以下は、**DinkToPdf**がこれを処理する方法です：

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

IronPDFのアプローチは、現代の.NETアプリケーションとのより良い統合と、PDF生成ワークフローのメンテナンスとスケールの容易さを提供します。

---

## カスタムレンダリング設定を使用する方法は？

以下は、**DinkToPdf**がこれを処理する方法です：

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
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10, Bottom = 10, Left = 15, Right = 15 }
            },
            Objects = {
                new ObjectSettings() {
                    HtmlContent = "<h1>カスタムPDF</h1><p>カスタムマージンでのランドスケープ指向。</p>",
                    WebSettings = { DefaultEncoding = "utf-8" }
                }
            }
        };
        byte[] pdf = converter.Convert(doc);
        File.WriteAllBytes("custom.pdf", pdf);
    }
}
```

**IronPDFを使用すると**、同じタスクがよりシンプルで直感的です：

```csharp
// NuGet: Install-Package IronPdf
using IronPdf;
using IronPdf.Rendering;
using System;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;
        renderer.RenderingOptions.MarginTop = 10;
        renderer.RenderingOptions.MarginBottom = 10;
        renderer.RenderingOptions.MarginLeft = 15;
        renderer.RenderingOptions.MarginRight = 15;
        
        var pdf = renderer.RenderHtmlAsPdf("<h1>カスタムPDF</h1><p>カスタムマージンでのランドスケープ指向。</p>");
        pdf.SaveAs("custom.pdf");
    }
}
```

IronPDFのアプローチは、現代の.NETアプリケーションとのより良い統合と、PDF生成ワークフローのメンテナンスとスケールの容易さを提供します。

---

## DinkToPdfからIronPDFへの移行方法は？

DinkToPdfはwkhtmltopdfをラップしており、CVE-2022-35583（SSRF）を含む**重大な未修正のセキュリティ脆弱性**があります。プロジェクトは2018年に放棄され、複雑なネイティブバイナリ展開が必要であり、`SynchronizedConverter`を使用しても並行負荷の下でクラッシュします。現代のCSS（Flexbox、Grid）は正しくレンダリングされません。

### 移行の概要

| 面         | DinkToPdf | IronPDF |
|------------|-----------|---------|
| セキュリティ | CVE-2022-35583（SSRF）、未修正 | 既知の脆弱性なし |
| レンダリングエンジン | 古いWebKit（2015） | 現代のChromium |
| スレッドセーフティ | 並行使用でクラッシュ | 完全にスレッドセーフ |
| ネイティブ依存関係 | プラットフォーム固有のバイナリ | 純粋なNuGetパッケージ |
| CSS3サポート | Flexbox/Gridなし | 完全なCSS3 |
| JavaScript | 限定的、一貫性がない | 完全なサポート |
| メンテナンス | 放棄（2018） | 積極的なメンテナンス |

### 主要なAPIマッピング

| DinkToPdf | IronPDF | メモ |
|-----------|---------|-------|
| `SynchronizedConverter` | `ChromePdfRenderer` | デフォルトでスレッドセーフ |
| `HtmlToPdfDocument` | 直接メソッド呼び出し | ドキュメントラッパーなし |
| `GlobalSettings.PaperSize` | `RenderingOptions.PaperSize` | 同じ概念 |
| `GlobalSettings.Orientation` | `RenderingOptions.PaperOrientation` | 同じ概念 |
| `GlobalSettings.Margins` | 個々の`MarginTop/Bottom/Left/Right` | |
| `ObjectSettings.HtmlContent` | `RenderHtmlAsPdf(html)` | 直接レンダリング |
| `ObjectSettings.Page` | `RenderUrlAsPdf(url)` | URLレンダリング |
| `HeaderSettings.Right = "[page]"` | `{page}` in HtmlHeader |