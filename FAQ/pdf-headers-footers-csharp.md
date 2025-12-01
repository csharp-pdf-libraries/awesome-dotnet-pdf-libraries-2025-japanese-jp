---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [FAQ/pdf-headers-footers-csharp.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/FAQ/pdf-headers-footers-csharp.md)
🇯🇵 **日本語:** [FAQ/pdf-headers-footers-csharp.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/FAQ/pdf-headers-footers-csharp.md)

---

# C#でIronPDFを使用してPDFにヘッダーとフッターを追加する方法は？

ヘッダーとフッターを追加することで、あなたのPDFを平凡から会議室レベルに引き上げることができます。[Iron Software](https://ironsoftware.com)から提供されている強力な.NET PDFライブラリーであるIronPDFを使用すれば、ページ番号からカスタムHTMLレイアウトやロゴまで、簡単に追加することができます。このFAQでは、C#でプロフェッショナルなヘッダーとフッターを作成する方法、実用的なコード、ヒント、一般的な問題への解決策を説明します。

---
## なぜ私のPDFにヘッダーとフッターを使用すべきですか？

ヘッダーとフッターは、あなたのPDFを洗練されて整理された外観にするのに役立ちます。これらは特に以下のために重要です：
- ページ番号（特に複数ページのレポートで）
- 会社名やロゴによるブランディング
- 生成日付/タイムスタンプの追加
- 法的免責事項や機密通知の挿入
- 交互またはセクション固有のヘッダーの作成（本や付録のように）

IronPDFを使用すると、プレーンテキストを超えて、HTMLとCSSを使用した高度なレイアウトとビジュアルを使用できます。

---
## IronPDFで簡単にシンプルなヘッダーとフッターを追加するにはどうすればよいですか？

こちらが直接的な例です：上部に中央揃えのタイトルを追加し、各ページの下部にページ番号を追加します。

```csharp
using IronPdf; // Install-Package IronPdf

var renderer = new ChromePdfRenderer();

renderer.RenderingOptions.TextHeader = new TextHeaderFooter
{
    CenterText = "Financial Report",
    FontSize = 16,
    FontFamily = "Segoe UI"
};

renderer.RenderingOptions.TextFooter = new TextHeaderFooter
{
    CenterText = "Page {page} of {total-pages}",
    FontSize = 12
};

var pdfDoc = renderer.RenderHtmlAsPdf("<h1>PDF content goes here</h1>");
pdfDoc.SaveAs("simple-header-footer.pdf");
```
**ヒント：** `{page}` や `{total-pages}` のようなプレースホルダーを使用してください—IronPDFはこれらを実際のページ番号に置き換えます。

---
## テキストヘッダー/フッターとHTMLヘッダー/フッターを使用すべきですか？違いは何ですか？

IronPDFは、シンプルなテキストとリッチなHTML/CSSヘッダーとフッターの両方をサポートしています：

| タイプ  | 速度  | カスタマイズ      | 最適な用途                       |
|-------|--------|-------------------|--------------------------------|
| テキスト  | 速い   | 基本（フォント、サイズ、配置） | 簡単な情報、ページ番号      |
| HTML  | 遅い | 全てのHTML/CSS、画像 | ロゴ、ブランディング、複雑なレイアウト |

**テキストヘッダー/フッター** は、ページ番号や短い通知のような基本的なタスクに適しています。  
**HTMLに切り替える** ときは、画像、高度なレイアウト、CSSによるスタイリングを使用したい場合です。  
ヘッダーにテキスト、フッターにHTMLを使用するなど、組み合わせて使用することもできます。

---
## IronPDFヘッダーとフッターで使用できる動的プレースホルダーは何ですか？

IronPDFはいくつかの動的プレースホルダーを提供しています：

| プレースホルダー    | 置き換えられる内容                      |
|----------------|------------------------------------|
| `{page}`       | 現在のページ番号            |
| `{total-pages}`| ページの総数          |
| `{date}`       | 現在の日付                   |
| `{time}`       | 現在の時刻                   |
| `{title}`      | ドキュメントのタイトル                 |
| `{url}`        | ソースURL（URLからレンダリングする場合） |

**例：** フッターにタイムスタンプとページ番号を追加します。

```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

renderer.RenderingOptions.TextFooter = new TextHeaderFooter
{
    LeftText = "Created: {date} {time}",
    RightText = "Page {page} of {total-pages}",
    FontSize = 10
};

var pdf = renderer.RenderHtmlAsPdf("<p>Document body here.</p>");
pdf.SaveAs("footer-timestamp.pdf");
```

*日付/時刻の形式をカスタマイズする必要がある場合は、HTMLヘッダー/フッターを使用し、フォーマットされたC#値を代わりに挿入してください。*

---
## 画像やカスタムレイアウトを含む高度なHTMLヘッダーとフッターを作成するにはどうすればよいですか？

より洗練されたデザインには、`HtmlHeader` と `HtmlFooter` オプションを使用します。ロゴ、色、スタイル付きテキストを含めることができます。

```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();

renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
{
    HtmlFragment = @"
      <div style='display: flex; align-items: center; justify-content: space-between; width: 100%; font-family: Arial;'>
        <img src='logo.png' style='height: 36px;' alt='Logo'/>
        <span style='font-size: 18px; color: #333;'>Executive Summary</span>
      </div>",
    DrawDividerLine = true,
    DividerLineColor = "#0078D7"
};

var pdf = renderer.RenderHtmlAsPdf("<p>Main content here.</p>");
pdf.SaveAs("header-html.pdf");
```
**注意：** IronPDFはほとんどのHTMLとCSSをサポートしており、動的コンテンツ用のJavaScriptもサポートしています（ただし、速度のためにシンプルに保ってください）。

---
## ヘッダー/フッターの画像が常に表示されるようにするにはどうすればよいですか？

ロゴやQRコードなどの画像が常に表示されるようにするには、Base64エンコーディングを使用して埋め込みます：

```csharp
using IronPdf;
using System.IO;

var renderer = new ChromePdfRenderer();
var logoBytes = File.ReadAllBytes("logo.png");
var base64Logo = Convert.ToBase64String(logoBytes);

renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
{
    HtmlFragment = $@"
      <div style='display: flex; align-items: center;'>
        <img src='data:image/png;base64,{base64Logo}' style='height: 32px;' alt='Logo'/>
        <span style='font-weight: bold; font-size: 20px;'>My Company</span>
      </div>",
    MaxHeight = 40
};

var pdf = renderer.RenderHtmlAsPdf("<div>Body content here.</div>");
pdf.SaveAs("header-base64-logo.pdf");
```
**Base64を使用する理由は？** ファイルが見つからない心配やパスの問題がなく、クラウドやDockerで完璧です。

---
## ヘッダーまたはフッターの高さを調整するにはどうすればよいですか？

`MaxHeight` プロパティ（ミリメートル単位）を使用して、ヘッダー/フッターの高さを設定します。

```csharp
renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
{
    HtmlFragment = "<img src='banner.png' style='width:100%;' />",
    MaxHeight = 50
};

renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
{
    HtmlFragment = "<p style='text-align:center;'>Page {page}</p>",
    MaxHeight = 18
};
```
**リマインダー：** 実際のデータでテストしてください—大きすぎるヘッダー/フッターはメインコンテンツを圧迫する可能性があります。

---
## コンテンツとヘッダー/フッターの間に区切り線を追加するにはどうすればよいですか？

`DrawDividerLine` を使用して区切り線を追加するか、CSSのボーダー/`<hr>` タグを使用します。

```csharp
renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
{
    HtmlFragment = "<h2>Handbook</h2>",
    DrawDividerLine = true,
    DividerLineColor = "#e5e5e5"
};
```
HTML内で直接 `<hr>` やスタイル付きの `<div>` 要素を使用してカスタムラインを追加し、より多くの制御を得ることもできます。

---
## CSSでヘッダーとフッターをスタイルすることはできますか？

もちろんです！インラインスタイル、`<style>` ブロック、さらにはカスタムフォントもHTMLヘッダー/フッターでサポートされています。

```csharp
renderer.RenderingOptions.HtmlHeader = new HtmlHeaderFooter
{
    HtmlFragment = @"
      <style>
        .header-main { font-family: 'Georgia', serif; color: #2c3e50; }
      </style>
      <div class='header-main'>
        <span>Annual Report 2024</span> <span style='font-size:13px;'>{date}</span>
      </div>"
};
```
**注意：** 外部フォントを使用する場合は、PDF生成中にアクセス可能であることを確認してください。

---
## 既存のPDFにヘッダーやフッターを追加するにはどうすればよいですか？

IronPDFは、作成後のPDFの更新をサポートしています：

```csharp
using IronPdf;

var pdfDoc = new PdfDocument("existing.pdf");

pdfDoc.AddTextHeaders(new TextHeaderFooter
{
    CenterText = "CONFIDENTIAL",
    FontSize = 14,
    FontFamily = "Arial"
});

pdfDoc.AddHtmlFooters(new HtmlHeaderFooter
{
    HtmlFragment = "<div style='text-align:center;'>Updated 2024</div>"
});

pdfDoc.SaveAs("updated.pdf");
```
コンプライアンスワークフローやバッチ透かしに最適です。

---
## 特定のページやセクションに異なるヘッダー/フッターを適用するにはどうすればよいですか？

`pageIndices` パラメーターを使用して、特定のページにヘッダー/フッターを適用できます：

```csharp
using System.Linq;
using IronPdf;

var pdf = new PdfDocument("multi.pdf");

// 2-10ページ（ゼロベース）にヘッダー
pdf.AddTextHeaders(
    new TextHeaderFooter { CenterText = "Main Section" },
    pageIndices: Enumerable.Range(1, 9).ToArray()
);

// 付録ページ（11-13ページ）にフッター
pdf.AddTextFooters(
    new TextHeaderFooter { CenterText = "Appendix - Page {page}" },
    pageIndices: new[] { 10, 11, 12 }
);
```
ページインデックスはゼロから始まります。

---
## 奇数ページと偶数ページで異なるヘッダーを設定するにはどうすればよいですか？

書籍レイアウトや学術論文に最適です：

```csharp
using System.Linq;
using IronPdf;

var pdf = new PdfDocument("book.pdf");
var pageCount = pdf.PageCount;

var oddPages = Enumerable.Range(0, pageCount).Where(i => i % 2 == 0).ToArray();
pdf.AddTextHeaders(
    new TextHeaderFooter { RightText = "Book Title" },
    pageIndices: oddPages
);

var evenPages = Enumerable.Range(0, pageCount).Where(i => i % 2 == 1).ToArray();
pdf.AddTextHeaders(
    new TextHeaderFooter { LeftText = "Chapter 1" },
    pageIndices: evenPages
);
```
必要に応じて奇数/偶数のヘッダーとフッターを混在させてください。

---
## 法的通知や連絡先情報にHTMLフッターを使用できますか？

はい！HTMLフッターは、法的テキスト、メールリンク、またはスタイル付きレイアウトに最適です。

```csharp
renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
{
    HtmlFragment = @"
      <div style='font-size: 10px; text-align: center; color: #555;'>
        © 2024 ExampleCorp | <a href='mailto:info@example.com'>info@example.com</a> | Page {page} of {total-pages}
      </div>",
    DrawDividerLine = true
};
```
リンクは、ほとんどのPDFビューアで通常クリック可能です。

---
## ヘッダーとフッターに関する実際のパターンやレシピは何ですか？

- **多言語ヘッダー：** ユーザーのロケールに基づいてヘッダーテキストを設定します。
- **条件付きフッター：** 未公開のドキュメントにのみ「DRAFT」を表示します。
- **透かし：** 「CONFIDENTIAL」のような薄いマークにHTMLの不透明度を使用します。
- **バーコード/QRコード：** Base64画像として生成し、ヘッダーに埋め込みます。
- **動的データ：** ユーザー名、日付、またはカスタム値を挿入するために文字列補間を使用します。

動的ドキュメントコンテンツの処理についての詳細は、[C#でXMLをPDFに変換する方法は？](xml-to-pdf-csharp.md) と [.NET MAUIでXAMLをPDFに使用する方法は？](xaml-to-pdf-maui-csharp.md) をチェックしてください。

---
## 注意すべき一般的な問題は何ですか？

- **画像パスが失敗する？** 信頼性のためにBase64画像を使用してください。
- **コンテンツの重なり：** `MaxHeight`が正しく設定されていることを確認し、密度の高いドキュメントでテストしてください。
- **フォントが正しくレンダリングされない？** テキストヘッダー