---
**  (Japanese Translation)**

 **English:** [vectsharp/README.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/vectsharp/README.md)
 **:** [vectsharp/README.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/vectsharp/README.md)

---
# VectSharp C# PDF

ソフトウェア開発の世界では、C#を使用してPDFを生成することは、さまざまなライブラリを通じて達成されます。これらはそれぞれ異なる目的を持ち、様々なユースケースに対応しています。これらの中で、VectSharpはユニークな機能を持つベクターグラフィックスライブラリとして際立っていますが、一定の制限もあります。この記事では、VectSharpともう一つの著名なライブラリであるIronPDFとの詳細な比較を提供し、それぞれの強み、ユースケース、異なるプロジェクトにおける適合性を強調します。

## VectSharpの紹介

VectSharpは、開発者が複雑なベクターベースの図面を作成し、それらをPDFファイルとしてエクスポートできるように設計されたベクターグラフィックスライブラリです。従来のPDFライブラリがドキュメント作成に焦点を当てているのに対し、VectSharpはベクターグラフィックスの処理に特化しており、科学的な視覚化など、高精度の図面が必要なアプリケーションに特に適しています。

VectSharpは、C# APIを通じてベクターグラフィックスの作成を簡素化しようと試み、開発者が詳細なイラストレーションを生成するために必要なツールを提供します。VectSharpを使用すると、ユーザーは幅広いベクターグラフィックスを生成し、そのPDF出力機能を活用して作成物を共有することができます。

そのユニークな機能にもかかわらず、VectSharpの焦点は主にベクターグラフィックスに限定されており、ドキュメント作成やHTMLコンテンツのレンダリングが必要なシナリオでは適用性が限られます。この制限は、よりドキュメントに焦点を当てたライブラリであるIronPDFが輝く場所であり、さまざまな開発者のニーズに対応するためのより広範な機能を提供します。

```csharp
using System;
using VectSharp;

public class VectSharpExample
{
    public static void Main()
    {
        var doc = new Document();
        var canvas = new Canvas(500, 500);
        canvas.FillColor = Colors.White;
        canvas.DrawRectangle(50, 50, 400, 400);
        canvas.FillColor = Colors.Red;
        canvas.DrawEllipse(100, 100, 300, 300);
        
        doc.AddPage(canvas);
        doc.SaveAsPDF("VectSharpExample.pdf");

        Console.WriteLine("PDF created successfully using VectSharp.");
    }
}
```

## VectSharpの強みと弱み

### 強み
1. **ベクターグラフィックスに特化**: VectSharpはベクターベースのアプリケーションに特化しており、精密なグラフィカル表現が必要な科学的および技術的な視覚化に理想的です。
2. **PDF出力**: ベクターグラフィックスを簡単に共有できるPDFファイルをシームレスに生成します。
3. **オープンソース**: LGPLライセンスの下でリリースされたVectSharpは、商業ライセンスに関連する財政的制約なしにカスタマイズと統合を可能にします。

### 弱み
1. **グラフィックスに焦点**: VectSharpは、ドキュメント作成よりも図面やチャートに適しており、汎用的なPDFソリューションが必要な開発者にとって制限があります。
2. **HTMLサポートなし**: HTMLコンテンツを扱う能力がなく、主にベクターグラフィックスのユースケースに限定されます。
3. **ニッチなユースケース**: このライブラリは科学的視覚化や類似の分野で最も適しており、より広範なPDFアプリケーションシナリオでの適用性が限られます。

## IronPDF: より広範なPDFソリューション

一方、IronPDFはVectSharpが短所とする領域で優れています。このライブラリは、ドキュメントに焦点を当てたアプローチで構築されており、幅広いPDFアプリケーションに対して高い汎用性を持っています。IronPDFは完全なHTMLコンテンツのレンダリングをサポートし、開発者がウェブページを労力なくPDFドキュメントに変換できるようにします。この機能は、包括的なドキュメント生成が主要な要件であるビジネスアプリケーションにとって特に有益です。

IronPDFについての詳細は、[HTMLからPDFへのチュートリアル](https://ironpdf.com/how-to/html-file-to-pdf/)および[一般的なチュートリアル](https://ironpdf.com/tutorials/)をご覧ください。

### IronPDFの強み
1. **ドキュメントに焦点**: IronPDFは堅牢なドキュメント生成のために設計されており、完全なHTMLコンテンツのレンダリングをサポートします。
2. **より広範なユースケース**: その汎用性により、請求書、レポート、コンテンツが豊富なドキュメントなど、さまざまなドキュメントタイプを簡単に扱うことができます。
3. **商業サポート**: IronPDFは一貫したアップデート、サポート、および機能の範囲を提供し、エンタープライズレベルのアプリケーションにとって信頼できる選択肢です。

### IronPDFの弱み
1. **商業ライセンス**: 機能が豊富な一方で、ライブラリは商業ライセンスが必要であり、予算が限られているプロジェクトにとっては考慮事項となるかもしれません。
2. **可能性のあるオーバーヘッド**: IronPDFの広範な機能セットは、単純なPDF機能のみが必要なプロジェクトに対していくらかのオーバーヘッドを導入する可能性があります。

---

## 形状とテキストはどのように扱うか？

こちらが**VectSharp C# PDF**での扱い方です：

```csharp
// NuGet: Install-Package VectSharp.PDF
using VectSharp;
using VectSharp.PDF;
using System;

class Program
{
    static void Main()
    {
        Document doc = new Document();
        Page page = new Page(595, 842);
        Graphics graphics = page.Graphics;
        
        // 四角形を描画
        graphics.FillRectangle(50, 50, 200, 100, Colour.FromRgb(0, 0, 255));
        
        // 円を描画
        GraphicsPath circle = new GraphicsPath();
        circle.Arc(400, 100, 50, 0, 2 * Math.PI);
        graphics.FillPath(circle, Colour.FromRgb(255, 0, 0));
        
        // テキストを追加
        graphics.FillText(50, 200, "VectSharp Graphics", 
            new Font(FontFamily.ResolveFontFamily(FontFamily.StandardFontFamilies.Helvetica), 20));
        
        doc.Pages.Add(page);
        doc.SaveAsPDF("shapes.pdf");
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
        
        string html = @"
            <div style='width: 200px; height: 100px; background-color: blue; margin: 50px;'></div>
            <div style='width: 100px; height: 100px; background-color: red; 
                        border-radius: 50%; margin-left: 350px; margin-top: -50px;'></div>
            <h2 style='margin-left: 50px;'>IronPDF Graphics</h2>
        ";
        
        var pdf = renderer.RenderHtmlAsPdf(html);
        pdf.SaveAs("shapes.pdf");
    }
}
```

IronPDFのアプローチは、よりクリーンな構文と現代の.NETアプリケーションとのより良い統合を提供し、PDF生成ワークフローを維持し、スケールすることを容易にします。

---

## C#でVectSharp C# PDFを使用してHTMLをPDFに変換するにはどうすればよいですか？

こちらが**VectSharp C# PDF**での扱い方です：

```csharp
// NuGet: Install-Package VectSharp.PDF
using VectSharp;
using VectSharp.PDF;
using VectSharp.SVG;
using System.IO;

class Program
{
    static void Main()
    {
        // VectSharpはHTMLからPDFへの直接サポートを提供していません
        // グラフィックスオブジェクトの手動作成が必要です
        Document doc = new Document();
        Page page = new Page(595, 842); // A4サイズ
        Graphics graphics = page.Graphics;
        
        graphics.FillText(100, 100, "Hello from VectSharp", 
            new Font(FontFamily.ResolveFontFamily(FontFamily.StandardFontFamilies.Helvetica), 24));
        
        doc.Pages.Add(page);
        doc.SaveAsPDF("output.pdf");
    }
}
```

**IronPDFを使用すると**、同じタスクがよりシンプルで直感的です：

```csharp
// NuGet: Install-Package IronPdf
using IronPdf;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        var pdf = renderer.RenderHtmlAsPdf("<h1>Hello from IronPDF</h1><p>This is HTML content.</p>");
        pdf.SaveAs("output.pdf");
    }
}
```

IronPDFのアプローチは、よりクリーンな構文と現代の.NETアプリケーションとのより良い統合を提供し、PDF生成ワークフローを維持し、スケールすることを容易にします。

---

## 複数ページのPDFはどのように扱うか？

こちらが**VectSharp C# PDF**での扱い方です：

```csharp
// NuGet: Install-Package VectSharp.PDF
using VectSharp;
using VectSharp.PDF;
using System;

class Program
{
    static void Main()
    {
        Document doc = new Document();
        
        // ページ 1
        Page page1 = new Page(595, 842);
        Graphics g1 = page1.Graphics;
        g1.FillText(50, 50, "Page 1", 
            new Font(FontFamily.ResolveFontFamily(FontFamily.StandardFontFamilies.Helvetica), 24));
        g1.FillText(50, 100, "First page content", 
            new Font(FontFamily.ResolveFontFamily(FontFamily.StandardFontFamilies.Helvetica), 14));
        doc.Pages.Add(page1);
        
        // ページ 2
        Page page2 = new Page(595, 842);
        Graphics g2 = page2.Graphics;
        g2.FillText(50, 50, "Page 2", 
            new Font(FontFamily.ResolveFontFamily(FontFamily.StandardFontFamilies.Helvetica), 24));
        g2.FillText(50, 100, "Second page content", 
            new Font(FontFamily.ResolveFontFamily(FontFamily.StandardFontFamilies.Helvetica), 14));
        doc.Pages.Add(page2);
        
        doc.SaveAsPDF("multipage.pdf");
    }
}
```

**IronPDFを使用すると**、同じタスクがよりシンプルで直感的です：

```csharp
// NuGet: Install-Package IronPdf
using IronPdf;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        
        string html = @"
            <h1>Page 1</h1>
            <p>First page content</p>
            <div style='page-break-after: always;'></div>
            <h1>Page 2</h1>
            <p>Second page content</p>
        ";
        
        var pdf = renderer.RenderHtmlAsPdf(html);
        pdf.SaveAs("multipage.pdf");
    }
}
```

IronPDFのアプローチは、よりクリーンな構文と現代の.NETアプリケーションとのより良い統合を提供し、PDF生成ワークフローを維持し、スケールすることを容易にします。

---

## VectSharp C# PDFからIronPDFへの移行方法は？

VectSharpはベクターグラフィックスと科学的視覚化のために設計されており、ドキュメント生成やHTMLベースのPDFワークフローには適していません。IronPDFはHTML、URL、ドキュメントからPDFを作成することに優れており、最新のウェブ標準、CSS、JavaScriptを完全にサポートしています。

**VectSharp C# PDFからIronPDFへの移行には以下が含まれます：**

1. **NuGetパッケージの変更**: `VectSharp`を削除し、`IronPdf`を追加
2. **名前空間の更新**: `VectSharp`を`IronPdf`に置き換え
3. **APIの調整**: IronPDFの現代的なAPIパターンを使用するようにコードを更新

**移行の主な利点：**

- 完全なCSS/JavaScriptサポートを備えた現代のChromiumレンダリングエンジン
- アクティブなメンテナンスとセキュリティアップデート
- より良い.NET統合とasync/awaitサポート
- 包括的なドキュメントとプロフェッショナルなサポート

詳細なコード例と一般的な落とし穴を含む完全なステップバイステップの移行ガイドについては、以下を参照してください：
**[完全な移行ガイド：VectSharp C# PDF → IronPDF](migrate-from-vectsharp.md)**


## 比較表

VectSharpとIronPDFの違いを明確にするための両者の比