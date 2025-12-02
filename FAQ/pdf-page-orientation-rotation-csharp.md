# C#でIronPDFを使用してPDFページの向きと回転を制御する方法は？

PDFが常に正しい向きであることを保証する必要がありますか、または同じドキュメント内で縦向きと横向きのページを混在させたいですか？.NET用のIronPDFは、新しいレポートの生成、スキャンされたファイルの整理、レイアウトが混在したコンテンツのマージなど、PDFページの向きと回転を正確に制御する機能を提供します。ここでは、最も一般的な（およびいくつかの高度な）シナリオをカバーする実用的なFAQと、行き詰まった際にコピー＆ペーストできるコードを紹介します。

---

## PDFのページの向きと回転の違いは何ですか？

PDFを調整する前に、違いを知ることが重要です：

- **向き**は、PDFが作成されるときのレイアウトの形状—縦向き（高い）または横向き（広い）—を決定します。コンテンツがレンダリングされる前に設定されます。
- **回転**は、作成後のページの表示角度を変更します。たとえば、横向きになってしまったスキャンされたドキュメントを回転させることがあります。

向きを印刷設定と考え、回転を間違った向きになってしまったページの修正と考えてください。

| 概念       | 使用するタイミング  | 使用例                      |
|-------------|---------------------|-------------------------------|
| 向き        | PDF作成時           | 広いチャートのために横向きを設定  |
| 回転        | 既存のPDFで         | 上下逆のスキャンを修正          |

特に、さまざまなソースからのPDFを組み合わせたり、修復したりする場合には、両方が便利です。

---

## IronPDFで向きと回転のタスクを設定するにはどうすればよいですか？

まず、NuGetからIronPDFがインストールされていることを確認してください。これは、これらの操作を簡素化する.NET用の包括的なPDFライブラリです。

```bash
// Install-Package IronPdf
```

C#ファイルで参照してください：

```csharp
using IronPdf;
// ファイルおよびディレクトリの処理用：
using System.IO;
```

より高度なPDFの使用例については、[IronPDFのドキュメント](https://ironpdf.com)および[Iron Softwareの開発者ツール](https://ironsoftware.com)を参照してください。

---

## PDFを縦向きまたは横向きで作成するにはどうすればよいですか？

IronPDFを使用すると、HTMLやその他のコンテンツをPDFにレンダリングする際に、ページの向きを明示的に設定できます。

```csharp
using IronPdf;
// Install-Package IronPdf

var pdfRenderer = new ChromePdfRenderer();
pdfRenderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;

var htmlContent = "<h1>Quarterly Dashboard</h1><table style='width:100%;'><tr><td>Data</td></tr></table>";

var document = pdfRenderer.RenderHtmlAsPdf(htmlContent);
document.SaveAs("dashboard-landscape.pdf");
```

**ヒント：** レイアウトが予測可能になるように、向きを常に指定してください（たとえ縦向きがデフォルトであっても）。

### 縦向きと横向きはいつ使用するべきですか？

- **縦向き：** 手紙、法的文書、またはテキストが多いコンテンツに最適。
- **横向き：** スプレッドシート、広いチャート、またはダッシュボードに理想的。

PDFの構造とページネーションを制御する方法については、[C#でPDFページネーションを制御する方法は？](html-to-pdf-page-breaks-csharp.md)を参照してください。

---

## 既存のPDFファイルのページを回転させるにはどうすればよいですか？

IronPDFは、任意のPDFのページを一度に全て、または選択的に回転させることができます。

### ドキュメント内のすべてのページを回転させるにはどうすればよいですか？

```csharp
using IronPdf;
// Install-Package IronPdf

var myPdf = PdfDocument.FromFile("input.pdf");
myPdf.SetAllPageRotations(PdfPageRotation.Clockwise90);
myPdf.SaveAs("rotated-all-pages.pdf");
```

- `PdfPageRotation.Clockwise90`は、ページを右に90°回転させます。
- 他の角度には`Clockwise180`または`Clockwise270`を使用します。

### 特定のページのみを回転させるにはどうすればよいですか？

```csharp
using IronPdf;

var pdfDoc = PdfDocument.FromFile("multi-orientation.pdf");

// 最初のページのみを回転
pdfDoc.SetPageRotation(0, PdfPageRotation.Clockwise90);

// 2、3、および5ページを上下逆に回転
pdfDoc.SetPageRotations(new[] { 1, 2, 4 }, PdfPageRotation.Clockwise180);

pdfDoc.SaveAs("some-pages-rotated.pdf");
```

**注記：** ページはゼロベースインデックスです（最初のページは0）。

---

## 上下逆のページを自動的に検出して修正するにはどうすればよいですか？

しばしば一部のページが上下逆になっているスキャンされたPDFのバッチを正規化するには、各ページの回転をチェックしてループで修正します。

```csharp
using IronPdf;

public void FixUpsideDownPages(string source, string destination)
{
    var doc = PdfDocument.FromFile(source);

    for (int i = 0; i < doc.PageCount; i++)
    {
        if (doc.GetPageRotation(i) == PdfPageRotation.Clockwise180)
            doc.SetPageRotation(i, PdfPageRotation.None);
    }

    doc.SaveAs(destination);
}
```

任意の回転シナリオにこのパターンを拡張できます。

---

## 単一のPDF内で縦向きと横向きのページを混在させるにはどうすればよいですか？

たとえば、縦向きの表紙と横向きのチャートを同じファイルにしたい場合、希望する向きで各ページをレンダリングしてからマージします。

```csharp
using IronPdf;

var portrait = new ChromePdfRenderer();
portrait.RenderingOptions.PaperOrientation = PdfPaperOrientation.Portrait;

var landscape = new ChromePdfRenderer();
landscape.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;

var coverPage = portrait.RenderHtmlAsPdf("<h1>Yearly Report</h1>");
var chartPage = landscape.RenderHtmlAsPdf("<h2>Growth Chart</h2><img src='https://placehold.co/800x200/chart.png' style='width:100%;'/>");

var finalPdf = PdfDocument.Merge(coverPage, chartPage);
finalPdf.SaveAs("mixed-orientation.pdf");
```

**参考情報：** 各ページは、レンダリングされたときの向きを保持します。マージについての詳細は、[C#とJavaを使用してPDFを比較およびマージする方法は？](compare-csharp-to-java.md)を参照してください。

---

## 既存のPDF（縦向きから横向き）の向きを変更できますか？

すべてのコンテンツを回転させることはできますが、ページの寸法は変わりません—縦向きのページは横になりますが、真の横向きにはなりません。

```csharp
using IronPdf;

var doc = PdfDocument.FromFile("portrait.pdf");
doc.SetAllPageRotations(PdfPageRotation.Clockwise90);
doc.SaveAs("rotated-to-landscape.pdf");
```

実際の横向きページサイズ（単に回転したコンテンツではなく）が必要な場合は、正しい向きで再レンダリングしてください。

---

## 向きとともにカスタム用紙サイズを設定するにはどうすればよいですか？

IronPDFは、標準およびカスタムサイズを向きと組み合わせてサポートしています。

```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;

// カスタムサイズ：297mm x 210mm（A4横向き）
renderer.RenderingOptions.SetCustomPaperSizeInMillimeters(297, 210);

var pdf = renderer.RenderHtmlAsPdf("<h2>Custom Size Example</h2>");
pdf.SaveAs("custom-paper.pdf");
```

---

## ライブWebページを横向きのPDFとしてレンダリングするにはどうすればよいですか？

ダッシュボードや広いレポートに最適—URLをレンダリングする前に向きと用紙サイズを設定します。

```csharp
using IronPdf;

var chromeRenderer = new ChromePdfRenderer();
chromeRenderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;
chromeRenderer.RenderingOptions.PaperSize = PdfPaperSize.A4;

var webPdf = chromeRenderer.RenderUrlAsPdf("https://example.com/dashboard");
webPdf.SaveAs("webpage-landscape.pdf");
```

HTML内のページブレークを処理する必要がある場合は、[C#でPDFページネーションを制御する方法は？](html-to-pdf-page-breaks-csharp.md)を参照してください。

---

## 多くのPDFを一括で回転させる最良の方法は何ですか？

一度に数百のPDFを修正するプロセスを自動化します：

```csharp
using IronPdf;
using System.IO;

public void BatchRotatePdfs(string inDir, string outDir, PdfPageRotation rotation)
{
    foreach (var file in Directory.GetFiles(inDir, "*.pdf"))
    {
        using var doc = PdfDocument.FromFile(file);
        doc.SetAllPageRotations(rotation);
        doc.SaveAs(Path.Combine(outDir, Path.GetFileName(file)));
    }
}
```

メモリリークを避けるために、各`PdfDocument`オブジェクトをループ内で破棄することを忘れないでください。

---

## すべてのページを自動的に縦向きに正規化できますか？

はい！横向きのページを検出して縦向きに回転させます：

```csharp
using IronPdf;

public void NormalizeToPortrait(string input, string output)
{
    var pdf = PdfDocument.FromFile(input);

    for (int i = 0; i < pdf.PageCount; i++)
    {
        var info = pdf.GetPageInfo(i);
        if (info.Width > info.Height)
            pdf.SetPageRotation(i, PdfPageRotation.Clockwise270);
    }

    pdf.SaveAs(output);
}
```

---

## マージされたPDFは元のページの向きを保持しますか？

絶対にそうです。IronPDFは、異なるファイルをマージする際に、各ページの元の向きを維持します。

```csharp
using IronPdf;

var doc1 = PdfDocument.FromFile("portrait.pdf");
var doc2 = PdfDocument.FromFile("landscape.pdf");

var mergedPdf = PdfDocument.Merge(doc1, doc2);
mergedPdf.SaveAs("merged.pdf");
```

ページ番号やその他の機能を追加する必要がある場合は、[C#でPDFにページ番号を追加する方法は？](add-page-numbers-pdf-csharp.md)を参照してください。

---

## コンテンツに基づいてページを自動的に回転させることはできますか？

はい、テキストを抽出して特定のキーワードが表示された場合にページを回転させることができます：

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("input.pdf");

for (int i = 0; i < pdf.PageCount; i++)
{
    var pageText = pdf.ExtractTextFromPage(i);
    if (pageText.Contains("CONFIDENTIAL"))
        pdf.SetPageRotation(i, PdfPageRotation.Clockwise90);
}

pdf.SaveAs("rotated-by-content.pdf");
```

XMLベースのワークフローについては、[C#でXMLをPDFに変換する方法は？](xml-to-pdf-csharp.md)をチェックしてください。

---

## 向きと回転に関する最も一般的な落とし穴は何ですか？

- **回転してもページサイズは変わらない：** 縦向きのページを回転させると、コンテンツが横向きになりますが、ページの形はそのままです。完全な横向きにするには、再レンダリングしてください。
- **マージされたPDFの不一致：** ページサイズと向きを混在させると、一部のPDFビューアで奇妙に見えることがあります—マージされたファイルを常にテストしてください。
- **プリンタの特性：** 一部のプリンタは回転を無視します。代わりに正しい向きでPDFを作成してみてください。
- **ゼロベースのインデックス：** IronPDFでは、ページインデックスは0から始まることを覚えておいてください。
- **メモリ管理：** 大量のバッチ処理、特にループ内で`PdfDocument`オブジェクトを破棄してください。
- **ライセンス：** IronPDFは商用ソフトウェアです—[ウォーターマーク](https://ironpdf.com/how-to/pdf-memory-stream/)を削除するにはライセンスを取得してください。

---

## さらに助けや高度な例を得るにはどこに行けばよいですか？

- 高度なレンダリング、デジタル署名などについては、[IronPDFドキュメント](https://ironpdf.com)を参照してください。
- デジタル署名については、[C#でPDFにデジタル署名をする方法は？](digitally-sign-pdf