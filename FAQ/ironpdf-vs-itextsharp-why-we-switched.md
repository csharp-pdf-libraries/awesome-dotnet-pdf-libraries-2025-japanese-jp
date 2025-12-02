# 開発者がiTextSharpをIronPDFに置き換えるべき理由は？

.NETプロジェクトをiTextSharpからIronPDFに移行するかどうか疑問に思っているなら、あなただけではありません。多くの開発者が、iTextSharpの時代遅れとHTMLからPDFへの新しい要件、現代的な機能、ライセンスの出現とともに、この決断に直面しています。以下は、この移行を検討している.NET開発者向けの実用的なFAQであり、なぜ、どのように、何を期待するかを説明しており、実際のコードと学んだ教訓が豊富に含まれています。

## なぜ多くの.NETアプリケーションがまだiTextSharpを使用しているのですか？

多くの.NETプロジェクトがJavaのiTextのC#ポートであるiTextSharpに依存しているのは、かつてPDF操作のデフォルトの選択肢だったからです。しかし、iTextSharp 5.xは2016年以降アップデートを受けておらず、バグ修正、新機能、そして重要なセキュリティパッチがありません。にもかかわらず、多くのレガシーアプリケーションがそれを使用し続けています。なぜなら、PDFソリューションの移行はそれが壊れるか、コンプライアンス担当者が連絡を取るまで「緊急」ではないからです。

これが問題である理由は？
- **セキュリティ：** パッチがないということは、脆弱性が残り続けることを意味します。
- **HTML/CSSの制限：** iTextSharpのHTMLレンダラーは過去に固執しており、現代のCSS、JS、フォントは機能しません。
- **ライセンスリスク：** AGPLライセンスは、コードをオープンソース化するか、高額な料金を支払うことを強制する可能性があります。

iTextSharpに固執している場合、あなただけではありませんが、より良い、現代的な代替手段があります。詳細については、[iTextSharpは放棄されましたか？ IronPDFにアップグレードすべきですか？](itextsharp-abandoned-upgrade-ironpdf.md)をご覧ください。

## IronPDFとは何か、iTextSharpとはどう違うのか？

IronPDFはChromiumエンジンに基づいて構築された.NET PDFライブラリであり、Google ChromeがWebページをレンダリングするのと同じようにPDFをレンダリングします。これは、HTML/CSSのレンダリングを模倣しようとするが、2000年以降に構築されたものには対応できないiTextSharpとは対照的です。

これが重要な理由は？
- **HTML5/CSS3サポート：** IronPDFは最新のWeb標準—Bootstrap、Tailwind、Google Fonts、Flexbox、Grid、JavaScriptなどをサポートしています。
- **現代の.NET互換性：** .NET Core、.NET 5–8+、Linux、Docker、Azureでシームレスに動作します。
- **シンプルさ：** HTMLをドロップして視覚的に正確なPDFを取得します。
- **継続的な開発：** IronPDFはアクティブに更新され、クリアで開発者に優しい商用ライセンスを提供しています。

HTMLからPDFへの変換がどれほど簡単か見たいですか？最小限の例をこちらです：

```csharp
using IronPdf; // NuGet経由でインストール：Install-Package IronPdf

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1>Hello, IronPDF!</h1>");
pdf.SaveAs("greeting.pdf");
```

HTMLからPDFへのシナリオについては、[HTML to PDF guide](https://ironpdf.com/how-to/html-string-to-pdf/)をご覧ください。

## 開発者がiTextSharpからIronPDFに移行する主な理由は何ですか？

### iTextSharpのAGPLライセンスは実際に問題ですか？

絶対にそうです。多くのチームが、iTextSharpのAGPLライセンスがSaaSやクローズドソースプロジェクトに「無料」ではないことを遅れて発見します。コードをオープンソース化しない場合、商用ライセンスのために年間$1,800以上の開発者ごとに支払う必要があります。コンプライアンス監査は高額な驚きを引き起こす可能性があります。

対照的にIronPDFは、前払いの商用ライセンス（開発者ごとに$749、永久）を提供し、驚きの監査はなく、明確な条件があります。ほとんどのチームにとって、これは大幅なコスト削減と安心を意味します。

詳細については、[iTextSharpからIronPDFへの移行方法は？](migrate-itextsharp-to-ironpdf.md)をご覧ください。

### iTextSharpでのセキュリティ脆弱性はリスクですか？

はい、そしてそれは悪化する一方です。例えば、CVE-2020-15522 XXEの脆弱性はiTextSharpでは決して修正されず、iText 7でのみ修正されましたが、それは別の、ドロップインライブラリではありません。

IronPDFはアクティブに維持され、月次のセキュリティ更新と新しい問題への迅速な対応があります。プロジェクトがPCI-DSS、SOC2、HIPAAのコンプライアンスが必要な場合、古いソフトウェアを実行する余裕はありません。

### IronPDFは本当にHTMLレンダリングの問題を解決しますか？

実際には、はい。iTextSharpのHTML/CSSレンダリングは基本的なHTMLを超えるものに苦労し、Bootstrapのレイアウトが壊れ、flexboxとgridが無視され、Google Fontsでさえ手動で埋め込む必要があります。私たちのチームは、iTextSharpで現代的な請求書を正しく見せるために数週間苦労しましたが、IronPDFを使って最初の試みで完璧にレンダリングされました。

PDFに現代のWeb技術を頼っている場合、IronPDFはゲームチェンジャーです。他の開発者がなぜ切り替えたのかを知るには、[なぜ開発者はIronPDFを選ぶのか？](why-developers-choose-ironpdf.md)を読んでください。

## iTextSharpとIronPDFの機能はどのように比較されますか？

一般的なPDFタスクのための簡単な機能比較はこちらです：

| 機能                      | iTextSharp 5.x      | IronPDF                        |
|--------------------------|---------------------|--------------------------------|
| HTML5/CSS3サポート       | いいえ（HTML 3.2のみ） | はい（Chromiumエンジン）        |
| Flexbox/Grid             | いいえ                | はい                            |
| JavaScript               | いいえ                | はい                            |
| Bootstrap互換性          | 不十分                | 優れている                      |
| Google Fonts             | 手動埋め込み          | 自動読み込み                    |
| .NET Core/5–8+           | いいえ                | 完全サポート                   |
| Linux/Docker             | Monoのみ             | ネイティブ/コンテナサポート     |
| セキュリティパッチ       | なし                  | 月次                            |
| ライセンス                | AGPL（高価）          | 永久商用                        |
| APIのシンプルさ           | 冗長                  | 現代的でシンプル                |

開発者がジャンプをする理由の要約をチェックするには、[なぜ開発者はIronPDFを選ぶのか？](why-developers-choose-ironpdf.md)をご覧ください。

## IronPDFとiTextSharpで一般的なPDFタスクはどのように異なりますか？

### HTMLをPDFに変換する方法は？

#### iTextSharpを使用して：
```csharp
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

var html = "<h1>Invoice</h1><p>Total: $500</p>";
var htmlStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(html));
using var pdfFile = new FileStream("invoice.pdf", FileMode.Create);
var doc = new Document();
var writer = PdfWriter.GetInstance(doc, pdfFile);
doc.Open();
XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, htmlStream, null);
doc.Close();
```
限られたCSS、JSなし、フォントがしばしば欠けています。

#### IronPDFを使用して：
```csharp
using IronPdf; // NuGet

var html = "<h1>Invoice</h1><p>Total: $500</p>";
var renderer = new ChromePdfRenderer();
var pdfDoc = renderer.RenderHtmlAsPdf(html);
pdfDoc.SaveAs("invoice.pdf");
```
ウェブページと同じように見えます—Bootstrap、フォント、その他すべて。

Razorベースのシナリオについては、[C#でRazorビューからPDFを生成する方法は？](razor-view-to-pdf-csharp.md)をご覧ください。

### IronPDFはBootstrap、Flexbox、Google Fontsを扱えますか？

はい、IronPDFはここで輝きます。そのままのWeb HTMLを使用してください：

```csharp
using IronPdf;

var html = @"
<link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
<div class='container'>
  <div class='row'>
    <div class='col'>Invoice #12345</div>
    <div class='col'>Date: Feb 2, 2025</div>
    <div class='col'>Total: $1,500</div>
  </div>
</div>";
var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("styled-invoice.pdf");
```

真のBootstrap列、Google Fonts、およびすべての現代的なCSSを取得します。

### ヘッダー、フッター、またはページ番号を追加する方法は？

iTextSharpでは、イベントハンドラをサブクラス化してテキストを手動で描画します。IronPDFでは、ヘッダー/フッターのHTMLを設定するだけです：

```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
renderer.RenderingOptions.HtmlFooter = new HtmlHeaderFooter
{
    HtmlFragment = "<div style='text-align:center;'>Page {page} of {total-pages}</div>"
};
var pdf = renderer.RenderHtmlAsPdf("<h1>Document</h1>");
pdf.SaveAs("with-footer.pdf");
```

高度な機能については、[C#でPDFにブックマークを追加する方法は？](pdf-bookmarks-csharp.md)と[ページ番号](https://ironpdf.com/blog/compare-to-other-components/questpdf-add-page-number-to-pdf/)を参照してください。

### PDFをマージまたは分割する方法は？

IronPDFはPDFマージを簡単にします：
```csharp
using IronPdf;

var pdf1 = PdfDocument.FromFile("a.pdf");
var pdf2 = PdfDocument.FromFile("b.pdf");
var merged = PdfDocument.Merge(pdf1, pdf2);
merged.SaveAs("merged.pdf");
```
もう手動でページを扱う必要はありません。

### 簡単にPDFからテキストを抽出できますか？

はい、IronPDFでは1行で可能です：
```csharp
using IronPdf;

var pdf = PdfDocument.FromFile("sample.pdf");
var text = pdf.ExtractAllText();
Console.WriteLine(text);
```

### IronPDFはRazorのような動的テンプレートをサポートしていますか？

絶対にそうです。Razorビューを直接PDFにレンダリングできます。これはASP.NET MVC/Coreアプリにとって救世主です。ウォークスルーについては、[C#でRazorビューをPDFに変換する方法は？](razor-view-to-pdf-csharp.md)をご覧ください。

## iTextSharpからIronPDFへの移行はどれほど難しいですか？

ほとんどのチームが移行の痛みを過大評価しています。私たちの経験：
- **概念実証：** 数時間—ほとんどの既存のHTMLは「そのまま動作しました」。
- **完全な置き換え：** すべてのPDFコードに対して4〜5日、多くの古いコードが削除されました。
- **テスト：** QAに1週間、しかし問題は少ない。
- **合計：** パートタイムで1か月未満、単一の開発者にとって。

ステップバイステップの移行計画については、[iTextSharpからIronPDFへの移行方法は？](migrate-itextsharp-to-ironpdf.md)をご覧ください。

## コストはどのくらいですか？ IronPDFはiText 7と比較してどうですか？

| 年        | iText 7 (6 devs) | IronPDF (6 devs) |
|-----------|------------------|------------------|
| 年 1      | $10,800          | $4,494           |
| 年 2+     | 年間$10,800      | $0               |
| 3年間     | $32,400          | $4,494           |

IronPDFの永久ライセンスモデルは大幅な節約と年間更新の頭痛の種がないことを意味します。

## 移行時に注意すべき落とし穴やゴッチャはありますか？

### Chromiumの起動遅延は問題ですか？

IronPDFで最初のPDF生成はChromiumが起動すると遅くなります。高スループットアプリの場合、起動時