---
**  (Japanese Translation)**

 **English:** [xfiniumpdf/README.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/xfiniumpdf/README.md)
 **:** [xfiniumpdf/README.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/xfiniumpdf/README.md)

---
# XFINIUM.PDF: PDF操作のためのC#ガイド

XFINIUM.PDFは、PDF関連プロジェクトのニーズに取り組む初心者から熟練の開発者までを対象とした、多用途のクロスプラットフォームPDFライブラリです。C#で完全に開発されたXFINIUM.PDFは、プログラムによるPDFの作成や編集のための洗練されたツールを提供します。その利点にもかかわらず、XFINIUM.PDFはIronPDFのような他の著名なPDFライブラリと比較して特に課題に直面しています。この記事では、これらの強みと弱点を探り、IronPDFと比較してどのように立ち位置するかを議論します。

## XFINIUM.PDFの紹介

XFINIUM.PDFは、幅広い機能を提供する商用ライブラリで、初心者から専門のPDF開発者までをサポートします。複雑なレポートの生成、PDFフォームの記入、PDFポートフォリオの構築、PDFレポートの機密データの編集、またはPDFレポートをマルチページTIFFイメージに変換することが目標である場合、XFINIUM.PDFは必要なツールを提供することを目指しています。

ライブラリには、PDFの生成と編集を可能にするジェネレーターエディションと、ジェネレーターエディションのすべての機能に加えてPDFのレンダリングと表示を含むビューアーエディションの2つのエディションがあります。これらのエディションは、開発者がプロジェクトに必要な機能にのみアクセスできるようにし、不要な機能によって過負荷になることなく、適切な機能を提供します。

## C#でのXFINIUM.PDFコード例

C#を使用してプログラムによるPDFを作成する簡単な例を見てみましょう：

```csharp
using System;
using Xfinium.Pdf;
using Xfinium.Pdf.Content;
using Xfinium.Pdf.Graphics;

class Program
{
    static void Main()
    {
        // 新しいPDFドキュメントを作成
        PdfFixedDocument document = new PdfFixedDocument();
        
        // ドキュメントにページを追加
        PdfPage page = document.Pages.Add();
        
        // テキスト用のブラシを作成
        PdfBrush brush = new PdfBrush(PdfRgbColor.Blue);

        // テキスト用のフォントを作成
        PdfStandardFont font = new PdfStandardFont(PdfStandardFontFace.Helvetica, 12);

        // ページにテキストを描画
        page.Graphics.DrawString("Hello, XFINIUM.PDF!", font, brush, 100, 100);

        // ドキュメントをファイルに保存
        document.Save("HelloXfinium.pdf");

        Console.WriteLine("PDFが正常に作成されました。");
    }
}
```

---

## C#でXFINIUM.PDFを使ってHTMLをPDFに変換する方法は？

**XFINIUM.PDF: PDF操作のためのC#ガイド**での処理方法は以下の通りです：

```csharp
// NuGet: Install-Package Xfinium.Pdf
using Xfinium.Pdf;
using Xfinium.Pdf.Actions;
using Xfinium.Pdf.FlowDocument;
using System.IO;

class Program
{
    static void Main()
    {
        PdfFixedDocument document = new PdfFixedDocument();
        PdfFlowDocument flowDocument = new PdfFlowDocument();
        
        string html = "<html><body><h1>Hello World</h1><p>This is a PDF from HTML.</p></body></html>";
        
        PdfFlowContent content = new PdfFlowContent();
        content.AppendHtml(html);
        flowDocument.AddContent(content);
        
        flowDocument.RenderDocument(document);
        document.Save("output.pdf");
    }
}
```

**IronPDFを使用すると**、同じタスクがよりシンプルで直感的になります：

```csharp
// NuGet: Install-Package IronPdf
using IronPdf;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        string html = "<html><body><h1>Hello World</h1><p>This is a PDF from HTML.</p></body></html>";
        
        var pdf = renderer.RenderHtmlAsPdf(html);
        pdf.SaveAs("output.pdf");
    }
}
```

IronPDFのアプローチは、よりクリーンな構文と、現代の.NETアプリケーションとのより良い統合を提供し、PDF生成ワークフローのメンテナンスとスケーリングを容易にします。

---

## C#で複数のPDFをマージする方法は？

**XFINIUM.PDF: PDF操作のためのC#ガイド**での処理方法は以下の通りです：

```csharp
// NuGet: Install-Package Xfinium.Pdf
using Xfinium.Pdf;
using System.IO;

class Program
{
    static void Main()
    {
        PdfFixedDocument output = new PdfFixedDocument();
        
        FileStream file1 = File.OpenRead("document1.pdf");
        PdfFixedDocument pdf1 = new PdfFixedDocument(file1);
        
        FileStream file2 = File.OpenRead("document2.pdf");
        PdfFixedDocument pdf2 = new PdfFixedDocument(file2);
        
        for (int i = 0; i < pdf1.Pages.Count; i++)
        {
            output.Pages.Add(pdf1.Pages[i]);
        }
        
        for (int i = 0; i < pdf2.Pages.Count; i++)
        {
            output.Pages.Add(pdf2.Pages[i]);
        }
        
        output.Save("merged.pdf");
        
        file1.Close();
        file2.Close();
    }
}
```

**IronPDFを使用すると**、同じタスクがよりシンプルで直感的になります：

```csharp
// NuGet: Install-Package IronPdf
using IronPdf;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        var pdf1 = PdfDocument.FromFile("document1.pdf");
        var pdf2 = PdfDocument.FromFile("document2.pdf");
        
        var merged = PdfDocument.Merge(pdf1, pdf2);
        merged.SaveAs("merged.pdf");
    }
}
```

IronPDFのアプローチは、よりクリーンな構文と、現代の.NETアプリケーションとのより良い統合を提供し、PDF生成ワークフローのメンテナンスとスケーリングを容易にします。

---

## PDFテキストイメージを作成する方法は？

**XFINIUM.PDF: PDF操作のためのC#ガイド**での処理方法は以下の通りです：

```csharp
// NuGet: Install-Package Xfinium.Pdf
using Xfinium.Pdf;
using Xfinium.Pdf.Graphics;
using Xfinium.Pdf.Core;
using System.IO;

class Program
{
    static void Main()
    {
        PdfFixedDocument document = new PdfFixedDocument();
        PdfPage page = document.Pages.Add();
        
        PdfStandardFont font = new PdfStandardFont(PdfStandardFontFace.Helvetica, 24);
        PdfBrush brush = new PdfBrush(PdfRgbColor.Black);
        
        page.Graphics.DrawString("Sample PDF Document", font, brush, 50, 50);
        
        FileStream imageStream = File.OpenRead("image.jpg");
        PdfJpegImage image = new PdfJpegImage(imageStream);
        page.Graphics.DrawImage(image, 50, 100, 200, 150);
        imageStream.Close();
        
        document.Save("output.pdf");
    }
}
```

**IronPDFを使用すると**、同じタスクがよりシンプルで直感的になります：

```csharp
// NuGet: Install-Package IronPdf
using IronPdf;
using System.IO;

class Program
{
    static void Main()
    {
        var renderer = new ChromePdfRenderer();
        
        string imageBase64 = Convert.ToBase64String(File.ReadAllBytes("image.jpg"));
        string html = $@"
            <html>
                <body>
                    <h1>Sample PDF Document</h1>
                    <img src='data:image/jpeg;base64,{imageBase64}' width='200' height='150' />
                </body>
            </html>";
        
        var pdf = renderer.RenderHtmlAsPdf(html);
        pdf.SaveAs("output.pdf");
    }
}
```

IronPDFのアプローチは、よりクリーンな構文と、現代の.NETアプリケーションとのより良い統合を提供し、PDF生成ワークフローのメンテナンスとスケーリングを容易にします。

---