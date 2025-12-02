# C#でIronPDFを使用してPDFにページ番号を追加する方法は？

.NETでPDFにページ番号を追加するのは複雑である必要はありません。IronPDFを使用すると、新しいPDFドキュメントや既存のPDFドキュメントにスタイルを適用したページ番号をすぐに挿入できます。座標計算やイベントハンドラは不要です。このFAQでは、ページ番号の付け方、特定のページをスキップする方法、スタイリングなど、実際のシナリオとコード例を紹介します。

---

## なぜPDFにページ番号を追加すべきなのか？

ページ番号は、PDFドキュメントでのナビゲーション、参照、プロフェッショナリズムの維持に不可欠です。ユーザーがコンテンツを引用したり、セクションにジャンプしたり、ドキュメントの長さをすばやく把握したりできるようにします。私の経験では、このような機能は契約書、請求書、技術文書の使いやすさを大幅に向上させ、ユーザーやサポートチームの時間を大いに節約します。

---

## C#でページ番号を付ける際の一般的な課題は何ですか？

多くのC# PDFライブラリ（iTextSharpなど）は、ページ番号を追加するためにサブクラス化と手動の座標計算が必要であり、これはエラーが発生しやすく、維持が難しい場合があります。例えば、iTextSharpで`PdfPageEventHelper`を使用する場合、複雑なオーバーライドが関与し、ドキュメントの構造が変更されると簡単に壊れる可能性があります。一方、IronPDFはプレースホルダーとシンプルなAPIを使用しており、プロセスをはるかに簡単にします。

---

## IronPDFでページ番号を付け始めるにはどうすればよいですか？

まず、IronPDF NuGetパッケージをインストールします：

```powershell
Install-Package IronPdf
```
または、.NET CLIで：
```bash
dotnet add package IronPdf
```

次に、プレースホルダーを使用して自動番号付けされたフッターを持つPDFを作成します：

```csharp
using IronPdf; // Install-Package IronPdf

var renderer = new ChromePdfRenderer();
var doc = renderer.RenderHtmlAsPdf(@"
    <h1>Sample Report</h1>
    <p>This is page 1.</p>
    <div style='page-break-after: always;'></div>
    <h2>Section 2</h2>
    <p>This is page 2.</p>
");

var footer = new TextHeaderFooter
{
    CenterText = "Page {page} of {total-pages}",
    FontSize = 10
};

doc.AddTextFooters(footer);
doc.SaveAs("numbered-report.pdf");
```

`{page}`と`{total-pages}`のプレースホルダーは自動的に置き換えられます。迅速なコードサンプルについては、[IronPDF Cookbook: Quick Examples](ironpdf-cookbook-quick-examples.md)を参照してください。

---

## HTMLやテンプレートからPDFを作成する際にページ番号を追加するにはどうすればよいですか？

PDFレンダリング中にヘッダーやフッターを設定することで、生成されるすべてのPDF（HTML、Razor、またはマークダウンから）に自動的にページ番号を含めることができます。これは、バッチジョブや自動化された請求書/レポート生成に最適です：

```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.TextFooter = new TextHeaderFooter
{
    RightText = "Page {page} of {total-pages}",
    FontSize = 9
};

var html = "<h1>Invoice</h1><div style='page-break-after: always;'></div><p>Page 2 content</p>";
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("invoice-with-numbers.pdf");
```

---

## 既存のPDFにページ番号を挿入するにはどうすればよいですか？

IronPDFを使用すると、既存のPDFを開いてページ番号をオーバーレイできます。これは、レガシードキュメント、スキャンされたファイル、またはサードパーティのエクスポートに便利です。方法は次のとおりです：

```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("old-report.pdf");
var footer = new TextHeaderFooter
{
    CenterText = "- {page} -",
    FontSize = 11
};
pdf.AddTextFooters(footer);
pdf.SaveAs("numbered-old-report.pdf");
```

ファイルをループ処理してフォルダ全体を一括処理する方法は、[Add Copy/Delete PDF Pages in C#](add-copy-delete-pdf-pages-csharp.md)で示されています。

---

## TextHeaderFooterとHtmlHeaderFooterの違いは何ですか？

**TextHeaderFooter**は、シンプルで高速な番号付けと最小限のスタイリングに最適です。  
**HtmlHeaderFooter**は、完全なHTML/CSSを許可します。ブランディング、ロゴ、色、画像などに理想的です。

**テキスト例：**
```csharp
var footer = new TextHeaderFooter
{
    CenterText = "Page {page}",
    FontSize = 8
};
pdf.AddTextFooters(footer);
```

**HTML例：**
```csharp
var htmlFooter = new HtmlHeaderFooter
{
    HtmlFragment = "<div style='color:#007acc;'>Page {page} of {total-pages}</div>"
};
pdf.AddHtmlFooters(htmlFooter);
```

会社のロゴやカスタムフォントを含めるには、`HtmlHeaderFooter`を使用します。フッターに画像を挿入する方法については、[Add Images to PDF in C#](add-images-to-pdf-csharp.md)を参照してください。

---

## 特定のページ（例：表紙）でページ番号をスキップすることはできますか？

はい。IronPDFを使用すると、どのページに番号を付けるかを選択し、シーケンスの開始番号も設定できます。例えば、最初のページの後に番号付けを開始するには：

```csharp
using IronPdf;
using System.Linq;

var footer = new HtmlHeaderFooter { HtmlFragment = "<center>Page {page}</center>" };
var skipCover = Enumerable.Range(0, pdf.PageCount).Skip(1);
pdf.AddHtmlFooters(footer, firstPageNumber: 1, pageIndices: skipCover);
```

これは、表紙、目次、付録セクションをスキップするのに便利です。類似のドキュメント操作タスクについては、[Add Attachments to PDF in C#](add-attachments-pdf-csharp.md)を参照してください。

---

## 奇数/偶数のページ、または特定のページのみにページ番号を適用するにはどうすればよいですか？

LINQを使用して、奇数、偶数、最初、または最後のページのインデックスを指定できます：

**奇数ページ：**
```csharp
var oddPages = Enumerable.Range(0, pdf.PageCount).Where(i => i % 2 == 0);
pdf.AddTextFooters(new TextHeaderFooter { CenterText = "Page {page}" }, pageIndices: oddPages);
```

**最初または最後のページ：**
```csharp
pdf.AddTextFooters(new TextHeaderFooter { CenterText = "Page {page}" }, pageIndices: new[] { 0 });
pdf.AddTextFooters(new TextHeaderFooter { CenterText = "Page {page}" }, pageIndices: new[] { pdf.PageCount - 1 });
```

---

## ページ番号の形式をカスタマイズすることはできますか（例：ローマ数字、セクションラベル）？

もちろんです。ページをループ処理して独自のロジックを注入することで、カスタムの番号付けスキームを作成できます。ローマ数字の場合：

```csharp
string Roman(int n) => n < 1 ? "" :
    n >= 1000 ? "M" + Roman(n - 1000) :
    n >= 900 ? "CM" + Roman(n - 900) :
    n >= 500 ? "D" + Roman(n - 500) :
    n >= 400 ? "CD" + Roman(n - 400) :
    n >= 100 ? "C" + Roman(n - 100) :
    n >= 90 ? "XC" + Roman(n - 90) :
    n >= 50 ? "L" + Roman(n - 50) :
    n >= 40 ? "XL" + Roman(n - 40) :
    n >= 10 ? "X" + Roman(n - 10) :
    n >= 9 ? "IX" + Roman(n - 9) :
    n >= 5 ? "V" + Roman(n - 5) :
    n >= 4 ? "IV" + Roman(n - 4) :
    "I" + Roman(n - 1);

foreach (int i in Enumerable.Range(0, 3))
    pdf.AddTextFooters(new TextHeaderFooter { CenterText = Roman(i + 1) }, pageIndices: new[] { i });
```

---

## 法的またはアーカイブPDFにベイツ番号を追加するにはどうすればよいですか？

法的文書の場合、各ページに一意の連続IDであるベイツ番号が必要になる場合があります：

```csharp
for (int i = 0; i < pdf.PageCount; i++)
{
    var bates = $"CASE24-{(i + 1):D5}";
    pdf.AddTextFooters(new TextHeaderFooter { RightText = bates, FontSize = 8 }, pageIndices: new[] { i });
}
```

---

## ページ番号付けに関する一般的な落とし穴とトラブルシューティングのヒントは何ですか？

- **コンテンツとの重複：** メインテキストとフッターが衝突しないように、`RenderingOptions`の`MarginBottom`を増やします。
- **マージ後の番号が間違っている：** `{total-pages}`が正確であることを確認するために、PDFをマージした*後*に番号を追加してください。
- **特定のページを選択：** `pageIndices`パラメータを使用し、手動でページを編集しないでください。
- **空白ページでヘッダー/フッターが表示されない：** ソースHTMLがそれらのページのレンダリングをスキップしていないことを確認してください。

別のライブラリから移行している場合は、[Migrate Telerik to IronPDF](migrate-telerik-to-ironpdf.md)を参照してください。

---

## IronPDFについてもっと学ぶか、助けを得るにはどこに行けばよいですか？

[IronPDF documentation](https://ironpdf.com)では、ヘッダー/フッターの配置、ページ番号付けなどを含む完全なガイドを提供しています。添付ファイル、画像の挿入、ページ管理など、より幅広いPDF操作については、[Iron Software](https://ironsoftware.com)および[Add Copy/Delete PDF Pages in C#](add-copy-delete-pdf-pages-csharp.md)などの他のFAQをチェックしてください。

---

*[Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/)、[Iron Software](https://ironsoftware.com)のCTO。2017年から開発者の生活を楽にするツールを構築しています。*
---

## 関連リソース

### 📚 チュートリアル & ガイド
- **[HTML to PDF Guide](../html-to-pdf-csharp.md)** — 完全な変換チュートリアル
- **[Best PDF Libraries 2025](../best-pdf-libraries-dotnet-2025.md)** — ライブラリ比較
- **[Beginner Tutorial](../csharp-pdf-tutorial-beginners.md)** — 最初のPDFを5分で
- **[Decision Flowchart](../choosing-a-pdf-library.md)** — 適切なライブラリを見つける

### 🔧 PDF操作
- **[Merge & Split PDFs](../merge-split-pdf-csharp.md)** — ドキュメントの結合
- **[Digital Signatures](../digital-signatures-pdf-csharp.md)** — 法的に署名
- **[Extract Text](../extract-text-from-pdf-csharp.md)** — テキスト抽出
- **[Fill PDF Forms](../fill-pdf-forms-csharp.md)** — フォーム自動化

### 🚀 フレームワーク統合
- **[ASP.NET Core PDF](../asp-net-core-pdf-reports.md)** — Webアプリ統合
- **[Blazor PDF Generation](../blazor-pdf-generation.md)** — Blazorサポート
- **[Cross-Platform Deployment](../cross-platform-pdf-dotnet.md)** — Docker、Linux、クラウド

### 📖 ライブラリドキュメント
- **[IronPDF](../ironpdf/)** — 完全なChromiumレンダリング
- **[QuestPDF](../questpdf/)** — コードファースト生成
- **[PDFSharp](../pdfsharp/)** — オープンソースオプション

---

*「[Awesome .NET PDF Libraries 2025](../README.md)」コレクションの一部 — 73のC#/.NET PDFライブラリを比較し、167のFAQ記事を提供しています。*

---

Jacob Mellor（CTO, Iron Software）は、41年間ドキュメント処理の課題を解決してきました。彼の哲学：「何でも名前をつけてください。私は常にそれを学びます。新しいプログラミング言語で最高の仕事をするのは、何が不可能かを知らないときです。」[LinkedIn](https://www.linkedin.com/in/jacob-mellor-iron-software/)でつながる。