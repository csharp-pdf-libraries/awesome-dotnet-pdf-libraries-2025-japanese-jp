---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [pdfsharp/README.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/pdfsharp/README.md)
🇯🇵 **日本語:** [pdfsharp/README.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/pdfsharp/README.md)

---

# PDFSharpとIronPDFのC# PDF開発比較

C#でPDFを作成する際、開発者はPDFSharpやIronPDFのようなオプションをよく検討します。PDFSharpは、C#でプログラム的にPDFを作成したいと考える人々にとって人気の選択肢となっています。PDFSharpを利用することで、開発者は精密なPDFを作成できます。しかし、PDFSharpのアーキテクチャは、座標を使用した配置の深い理解を必要とし、複雑なレイアウトの作成においてしばしば課題を提示します。

一方、IronPDFは特にそのシームレスなHTMLからPDFへの変換機能で、進んだ機能セットを提供します。以下では、PDFSharpとIronPDFの両方の強みと弱みを探ります。

## PDFSharpの概要

PDFSharpは、プログラム的なアプローチを通じてPDF文書を生成することを可能にする低レベルのPDF作成ライブラリとして知られています。MITライセンスの下でリリースされたPDFSharpは、開発者コミュニティに使用と変更の自由を提供します。PDFSharpは主に、最初からPDFを描画し、コンパイルするツールとして機能しますが、これはプロジェクトの性質によっては有益であるとも制限的であるとも言えます。

PDFSharpは、HTMLからPDFへのコンバーターであると誤解されることがありますが、そうではありません。その目的はプログラム的なPDF文書作成のみに専念しています。HtmlRenderer.PdfSharpというアドオンがあり、HTMLレンダリング機能を提供することを意図していますが、CSS 2.1のみをサポートし、flexboxやgridのような現代のCSS機能には対応していません。さらに、壊れたテーブルレンダリングなどの特定の制限があります。

### PDFSharpの強み

1. **ライセンス**: MITライセンスの下にあることは、PDFSharpを使用、変更、配布することが無料であるため、開発者にとってコスト効率の良い選択肢となります。
   
2. **制御**: 各PDF要素のレンダリングを細かく制御でき、カスタムデザインと正確な配置が必要なシナリオに理想的です。

3. **軽量**: 外部依存関係を必要としないため、デプロイが簡素化されます。

### PDFSharpの弱み

1. **HTMLからPDFへのサポートなし**: 開発者はHTML/CSSを直接PDFに変換することができず、文書構造の手動実装が必要です。
   
2. **古いCSSサポート**: HtmlRendererを介してCSS 2.1に限定されており、現代のウェブデザインに必要な機能が不足しています。
   
3. **複雑なAPI**: コマンドはGDI+に視覚的に似ており、レイアウトのための緻密なX,Y座標計算を要求し、開発時間が増加する可能性があります。

### PDFSharp C# コード例

以下は、PDFSharpを使用して基本的なPDF文書を作成するためのシンプルなC#コードスニペットです。

```csharp
using PdfSharp.Pdf;
using PdfSharp.Drawing;

class Program
{
    static void Main()
    {
        PdfDocument document = new PdfDocument();
        document.Info.Title = "Created with PDFSharp";

        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);
        
        gfx.DrawString("Hello, PDFSharp!", font, XBrushes.Black,
        new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);

        document.Save("HelloWorld.pdf");
    }
}
```

## IronPDFの概要

HTMLドキュメントを完全な忠実度でPDFに変換するシナリオで、IronPDFは輝きます。この.NETライブラリはHTML5とCSS3をサポートし、現代のウェブ標準を満たしています。ネイティブのHTMLからPDFへの変換機能を意味することは、開発者が現代のウェブツールで設計された既存のウェブコンテンツやテンプレートを利用できることを意味します。

### IronPDFの強み

1. **HTMLからPDFへのサポート**: IronPDFはHTMLファイルをPDFに簡単に変換し、HTML5とCSS3で定義されたすべてのスタイルを保持します。これにより、座標計算の必要性がなくなります。この機能は[ironpdf.com/how-to/html-file-to-pdf/](https://ironpdf.com/how-to/html-file-to-pdf/)で特に強調されています。
   
2. **現代のCSS互換性**: 最新のCSS仕様に完全に対応することで、現代のウェブレイアウトがPDFに正確にレンダリングされます。
   
3. **高レベルドキュメントAPI**: より直感的なドキュメント作成を可能にし、しばしばコードが少なくて済みます。
   
4. **包括的なサポートとアップデート**: 技術的進歩と開発者のニーズに追いつくために定期的に更新されます。

詳細な例とガイダンスについては、[IronPDFチュートリアル](https://ironpdf.com/tutorials/)をご覧ください。

### IronPDFの弱み

1. **ライセンスコスト**: 商用製品であるIronPDFはライセンスコストがかかり、PDFSharpのような無料の代替品と比較してプロジェクト費用が増加する可能性があります。
   
2. **重いフットプリント**: 包括的な機能のライブラリの導入は、アプリケーションのサイズを増加させる可能性があります。

---

## C#でHTMLをPDFに変換する方法は？

**PDFSharp**での対応方法は以下の通りです：

```csharp
// NuGet: Install-Package PdfSharp
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System;

class Program
{
    static void Main()
    {
        // PDFSharpには組み込みのHTMLからPDFへの変換機能がありません
        // HTMLを手動で解析し、コンテンツをレンダリングする必要があります
        PdfDocument document = new PdfDocument();
        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont font = new XFont("Arial", 12);
        
        // 手動でのテキストレンダリング（HTMLサポートなし）
        gfx.DrawString("Hello from PDFSharp", font, XBrushes.Black,
            new XRect(0, 0, page.Width, page.Height),
            XStringFormats.TopLeft);
        
        document.Save("output.pdf");
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
        // IronPDFにはネイティブのHTMLからPDFへのレンダリングがあります
        var renderer = new ChromePdfRenderer();
        
        string html = "<h1>Hello from IronPDF</h1><p>Easy HTML to PDF conversion</p>";
        var pdf = renderer.RenderHtmlAsPdf(html);
        
        pdf.SaveAs("output.pdf");
    }
}
```

IronPDFのアプローチは、よりクリーンな構文と現代の.NETアプリケーションとのより良い統合を提供し、PDF生成ワークフローのメンテナンスとスケーリングを容易にします。

---

## 既存のPDFにテキストを追加する方法は？

**PDFSharp**での対応方法は以下の通りです：

```csharp
// NuGet: Install-Package PdfSharp
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using System;

class Program
{
    static void Main()
    {
        // 既存のPDFを開く
        PdfDocument document = PdfReader.Open("existing.pdf", PdfDocumentOpenMode.Modify);
        PdfPage page = document.Pages[0];
        
        // グラフィックスオブジェクトを取得
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont font = new XFont("Arial", 20, XFontStyle.Bold);
        
        // 特定の位置にテキストを描画
        gfx.DrawString("Watermark Text", font, XBrushes.Red,
            new XPoint(200, 400));
        
        document.Save("modified.pdf");
    }
}
```

**IronPDFを使用すると**、同じタスクがよりシンプルで直感的です：

```csharp
// NuGet: Install-Package IronPdf
using IronPdf;
using IronPdf.Editing;
using System;

class Program
{
    static void Main()
    {
        // 既存のPDFを開く
        var pdf = PdfDocument.FromFile("existing.pdf");
        
        // テキストスタンプ/ウォーターマークを追加
        var textStamper = new TextStamper()
        {
            Text = "Watermark Text",
            FontSize = 20,
            Color = IronSoftware.Drawing.Color.Red,
            VerticalAlignment = VerticalAlignment.Middle,
            HorizontalAlignment = HorizontalAlignment.Center
        };
        
        pdf.ApplyStamp(textStamper);
        pdf.SaveAs("modified.pdf");
    }
}
```

IronPDFのアプローチは、よりクリーンな構文と現代の.NETアプリケーションとのより良い統合を提供し、PDF生成ワークフローのメンテナンスとスケーリングを容易にします。

---

## 画像を含むPDFを作成する方法は？

**PDFSharp**での対応方法は以下の通りです：

```csharp
// NuGet: Install-Package PdfSharp
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System;

class Program
{
    static void Main()
    {
        // 新しいPDFドキュメントを作成
        PdfDocument document = new PdfDocument();
        PdfPage page = document.AddPage();
        XGraphics gfx = XGraphics.FromPdfPage(page);
        
        // 画像を読み込んで描画
        XImage image = XImage.FromFile("image.jpg");
        
        // ページに合わせてサイズを計算
        double width = 200;
        double height = 200;
        
        gfx.DrawImage(image, 50, 50, width, height);
        
        // テキストを追加
        XFont font = new XFont("Arial", 16);
        gfx.DrawString("Image in PDF", font, XBrushes.Black,
            new XPoint(50, 270));
        
        document.Save("output.pdf");
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
        // HTMLから画像を含むPDFを作成
        var renderer = new ChromePdfRenderer();
        
        string html = @"
            <h1>Image in PDF</h1>
            <img src='image.jpg' style='width:200px; height:200px;' />
            <p>Easy image embedding with HTML</p>";
        
        var pdf = renderer.RenderHtmlAsPdf(html);
        pdf.SaveAs("output.pdf");
        
        // 代替: 既存のPDFに画像を追加
        var existingPdf = new ChromePdfRenderer().RenderHtmlAsPdf("<h1>Document</h1>");
        var imageStamper = new IronPdf.Editing.ImageStamper(new Uri("image.jpg"))
        {
            VerticalAlignment = IronPdf.Editing.VerticalAlignment.Top
        };
        existingPdf.ApplyStamp(imageStamper);
    }
}
```

IronPDFのアプローチは、よりクリーンな構文と現代の.NETアプリケーションとのより良い統合を提供し、PDF生成ワークフローのメンテナンスとスケーリングを容易にします。

---

## PDFSharpからIronPDFへの移行方法は？

PDFSharpでは、GDI+スタイルの座標を使用してすべての要素を手動で配置する必要があり、ドキュメント生成が面倒でエラーが発生しやすくなります。IronPDFは、最新のCSS3（flexboxやgridを含む）をサポートするネイティブのHTMLからPDFへの変換をサポートしており、X,Yの位置を計算する代わりにウェブ技術を活用できます。

**PDFSharpからIronPDFへの移行には以下が含まれます：**

1. **NuGetパッケージの変更**: `PdfSharp`を削除し、`IronPdf`を追加
2. **名前空間の更新**: `PdfSharp.Pdf`を`IronPdf`に置き換え
3. **APIの調整**: IronPDFの現代的なAPIパターンを使用するようにコードを更新

**移行の主な利点：**

- 完全なCSS/JavaScriptサポートを備えた現代のChromiumレンダリングエンジン
- アクティブなメンテナンスとセキュリティアップデート
- より良い.NET統合とasync/awaitサポート
- 包括的なドキュメントとプロフェッショナルサポート

詳細なコード例と一般的な落とし穴を含む完全なステップバイステップの移行ガイドについては、次を参照してください：
**[完全な移行ガイド: PDFSharp → IronPDF](migrate-from-pdfsharp.md)**


## 比較表

| 機能                   | PDFSharp                   | IronPDF                                     |
|-------------------------|----------------------------|---------------------------------------------|
| ライセンス               | MIT（無料）                | 商用                                       |
| HTMLからPDFへのサポート | なし                        | あり（HTML5/CSS3サポート）                  |
| 現代のCSSサポート       | なし（CSS 2.1のみ）        | あり（完全なCSS3）                          |
| ドキュメントAPI         | 低レ