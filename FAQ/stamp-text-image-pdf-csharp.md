---
**  (Japanese Translation)**

 **English:** [FAQ/stamp-text-image-pdf-csharp.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/FAQ/stamp-text-image-pdf-csharp.md)
 **:** [FAQ/stamp-text-image-pdf-csharp.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/FAQ/stamp-text-image-pdf-csharp.md)

---
# C#でIronPDFを使用してPDFにテキスト、画像、QRコードをスタンプする方法は？

契約書に「CONFIDENTIAL」とオーバーレイしたり、ロゴでPDFをブランド化したり、C#でファイルにデジタル署名をしたりする必要がありますか？IronPDFを使用すると、既存のPDFにテキスト、画像、バーコード、さらにはHTMLを簡単かつ強力にスタンプすることができます。このFAQでは、シンプルなウォーターマークから高度な動的スタンピングまで、必要な基本をカバーしているため、C#でドキュメントワークフローを自信を持って自動化できます。

HRの書類、法的文書の取り扱い、または大量の請求書の生成を行っている場合でも、このガイドでは、見た目とページターゲティングの柔軟なオプションを使用して、オーバーレイを正確に配置する方法を示します。

---

## PDFスタンピングとは何か、なぜ使用するのか？

PDFスタンピングは、テキスト、画像、ウォーターマーク、バーコード、またはHTMLのようなコンテンツを既存のPDFドキュメントの上にオーバーレイするプロセスです。一般的に以下の用途で使用されます：

- 「SAMPLE」や「APPROVED」などの[ウォーターマークの追加](https://ironpdf.com/java/blog/using-ironpdf-for-java/java-watermark-pdf-tutorial/)
- [デジタル署名](https://ironpdf.com/nodejs/examples/digitally-sign-a-pdf/)やスキャンされた署名画像の挿入
- ロゴや会社のブランディングをすべてのページにスタンプ
- トラッキングや検証のためのQRコードやバーコードの埋め込み
- 「COPY」や「ORIGINAL」として文書を明確にマーク

アプリケーションがドキュメントを生成または管理している場合（HRシステム、法律技術、または財務アプリを考えてみてください）、スタンピングによりブランディング、法的コンプライアンス、追跡可能性が保証されます。IronPDFは、直接PDF編集の複雑さを抽象化し、シンプルなC#オブジェクトとメソッドで作業できるようにします。

## IronPDFが提供するPDFオーバーレイ用のスタンパータイプは？

IronPDFは、異なるオーバーレイタイプ用に設計された4つの専用スタンパークラスを提供します：

| スタンパー         | 典型的な使用例                                  |
|-----------------|------------------------------------------------|
| TextStamper     | ウォーターマーク、ラベル、ページ番号               |
| ImageStamper    | ロゴ、署名、承認スタンプ             |
| HtmlStamper     | カスタムレイアウト、免責事項、スタイル要素   |
| BarcodeStamper  | QRコード、バーコード、ドキュメントトラッキング          |

各スタンパーでは、配置、透明度、回転、特定のページのターゲット設定が可能です。複数のスタンパーを組み合わせて、複雑なオーバーレイを作成できます。

## C#でPDFにテキストウォーターマークを追加するには？

最も一般的な使用例は、「CONFIDENTIAL」のようなウォーターマークをすべてのページに斜めにスタンプすることです。IronPDFを使用してこれを行う方法は以下の通りです：

```csharp
// Install-Package IronPdf
using IronPdf;
using IronPdf.Stamps;
using System.Drawing;

var doc = PdfDocument.FromFile("report.pdf");

var watermark = new TextStamper
{
    Text = "CONFIDENTIAL",
    FontFamily = "Arial",
    FontSize = 60,
    FontColor = Color.Red,
    Opacity = 30, // 繊細さのための30%の不透明度
    Rotation = -45,
    IsBold = true,
    HorizontalAlignment = HorizontalAlignment.Center,
    VerticalAlignment = VerticalAlignment.Middle
};

doc.ApplyStamp(watermark);
doc.SaveAs("report-watermarked.pdf");
```

**ヒント：**  
- `Opacity`を調整して、より軽いまたは重いウォーターマークにします。
- 斜めの効果には`Rotation`を使用します。
- テキストの回転の詳細については、[C#を使用してPDF内のテキストを回転させる方法は？](rotate-text-pdf-csharp.md)を参照してください。

## PDFに画像（ロゴや署名など）をスタンプするにはどうすればよいですか？

会社のロゴや署名スキャンなどの画像をオーバーレイするには、`ImageStamper`を使用します。必要に応じて画像を簡単にスケールして配置できます：

```csharp
// Install-Package IronPdf
using IronPdf;
using IronPdf.Stamps;

var pdf = PdfDocument.FromFile("invoice.pdf");

var logoStamp = new ImageStamper("logo.png")
{
    Scale = 24, // 元のサイズの24%に縮小
    HorizontalAlignment = HorizontalAlignment.Left,
    VerticalAlignment = VerticalAlignment.Top,
    HorizontalOffset = new Length(12), // 左から12px
    VerticalOffset = new Length(12),   // 上から12px
    Opacity = 80 // やや透明
};

pdf.ApplyStamp(logoStamp);
pdf.SaveAs("invoice-branded.pdf");
```

**実践的なヒント：**
- コンテンツを覆い隠さないように`Scale`プロパティを使用します。
- 複数の画像（例：ロゴと署名）を適用することで組み合わせます。

PDFから画像を抽出する方法については、[C#でPDFをビットマップ画像に変換する方法は？](pdf-to-image-bitmap-csharp.md)を参照してください。

## HTMLコンテンツをPDFスタンプとしてオーバーレイするにはどうすればよいですか？

CSS、リンク、ブランディングを含むスタイルの「PAID」ボックスなど、リッチなレイアウトが必要な場合、`HtmlStamper`が最適です。ほぼすべてのHTMLとCSSを直接スタンプとしてレンダリングできます。

```csharp
// Install-Package IronPdf
using IronPdf;
using IronPdf.Stamps;

var doc = PdfDocument.FromFile("receipt.pdf");

var htmlStamp = new HtmlStamper
{
    Html = @"
        <div style='
            background: #f0fff0;
            border: 2px solid #28a745;
            color: #28a745;
            font-family: Arial, sans-serif;
            font-size: 24px;
            font-weight: bold;
            border-radius: 10px;
            padding: 10px 28px;
            box-shadow: 1px 2px 3px rgba(0,0,0,.10);
            text-align: center;'>
            PAID<br><small>ご利用ありがとうございます！</small>
        </div>",
    HorizontalAlignment = HorizontalAlignment.Right,
    VerticalAlignment = VerticalAlignment.Top,
    HorizontalOffset = new Length(-38),
    VerticalOffset = new Length(36)
};

doc.ApplyStamp(htmlStamp);
doc.SaveAs("receipt-stamped.pdf");
```

**HTMLを使用する理由：**
- 動的コンテンツとカスタムスタイリングをサポートします。
- 画像、ハイパーリンク、またはテーブルを簡単に埋め込めます。
- 法的免責事項やブランドヘッダーに最適です。

## PDFスタンプとしてQRコードやバーコードを追加できますか？

もちろんです。IronPDFの`BarcodeStamper`を使用すると、位置とサイズを完全に制御しながらQRコードやクラシックなバーコードを挿入できます。

```csharp
// Install-Package IronPdf
using IronPdf;
using IronPdf.Stamps;

var doc = PdfDocument.FromFile("shipping-label.pdf");

var qrCode = new BarcodeStamper("https://example.com/track/12345")
{
    BarcodeType = BarcodeType.QRCode,
    Width = 80,
    Height = 80,
    HorizontalAlignment = HorizontalAlignment.Right,
    VerticalAlignment = VerticalAlignment.Bottom,
    HorizontalOffset = new Length(-18),
    VerticalOffset = new Length(-18)
};

doc.ApplyStamp(qrCode);
doc.SaveAs("shipping-label-with-qr.pdf");
```

**コツ：**
- 異なるフォーマット（QR、Code128、DataMatrix）には、異なる`BarcodeType`値を使用します。
- QRコードはURL、シリアル、または任意のデータを格納できます。
- PDFからバーコードデータを抽出する場合は、専用のライブラリが必要になることがあります。

## PDFページ上でスタンプを正確に配置するにはどうすればよいですか？

配置とオフセットのプロパティを組み合わせて、スタンプを正確に配置したい場所に配置できます：

```csharp
// Install-Package IronPdf
using IronPdf;
using IronPdf.Stamps;
using System.Drawing;

var doc = PdfDocument.FromFile("handbook.pdf");

// ヘッダーを追加
var header = new TextStamper
{
    Text = "Internal Use Only",
    FontColor = Color.OrangeRed,
    FontSize = 14,
    HorizontalAlignment = HorizontalAlignment.Left,
    VerticalAlignment = VerticalAlignment.Top,
    HorizontalOffset = new Length(22),
    VerticalOffset = new Length(14)
};

// フッターを追加
var footer = new TextStamper
{
    Text = "© 2024 ExampleCorp",
    FontSize = 10,
    HorizontalAlignment = HorizontalAlignment.Right,
    VerticalAlignment = VerticalAlignment.Bottom,
    HorizontalOffset = new Length(-18),
    VerticalOffset = new Length(-10)
};

doc.ApplyStamp(header);
doc.ApplyStamp(footer);
doc.SaveAs("handbook-stamped.pdf");
```

**ヒント：** 正のオフセットはスタンプをアンカーから遠ざけ、負のオフセットは引き寄せます。

## 全体のPDFではなく、特定のページのみにスタンプを適用できますか？

はい！`ApplyStamp`にページインデックスまたは配列を渡すことで、特定のページをターゲットにできます。例えば：

```csharp
// Install-Package IronPdf
using IronPdf;
using IronPdf.Stamps;
using System.Linq;

var doc = PdfDocument.FromFile("summary.pdf");

var draftStamp = new TextStamper
{
    Text = "DRAFT",
    FontSize = 60,
    FontColor = System.Drawing.Color.Gray,
    Opacity = 20,
    Rotation = -45,
    HorizontalAlignment = HorizontalAlignment.Center,
    VerticalAlignment = VerticalAlignment.Middle
};

// 最初のページ（ページ0）のみにスタンプを適用
doc.ApplyStamp(draftStamp, 0);

// または、最初と最後のページにスタンプを適用
doc.ApplyStamp(draftStamp, new[] { 0, doc.PageCount - 1 });

// または、奇数ページすべてにスタンプを適用
var oddPages = Enumerable.Range(0, doc.PageCount).Where(i => i % 2 == 1).ToArray();
doc.ApplyStamp(draftStamp, oddPages);

doc.SaveAs("summary-draft.pdf");
```

ページごとの高度なロジックには、異なるスタンプを動的に適用するために反復処理を行います。

## ロゴ、ウォーターマーク、QRコードなど、複数のスタンプを1つのPDFに組み合わせるにはどうすればよいですか？

`ApplyStamp`を複数回呼び出すことで、好きなだけ多くのスタンプをレイヤー化できます。必要に応じて、異なるページターゲットまたはスタンパーでオプションを使用します。

```csharp
// Install-Package IronPdf
using IronPdf;
using IronPdf.Stamps;
using System.Drawing;

var doc = PdfDocument.FromFile("agreement.pdf");

var logo = new ImageStamper("logo.png")
{
    Scale = 22,
    HorizontalAlignment = HorizontalAlignment.Left,
    VerticalAlignment = VerticalAlignment.Top,
    HorizontalOffset = new Length(18),
    VerticalOffset = new Length(18)
};

var watermark = new TextStamper
{
    Text = "CONFIDENTIAL",
    FontFamily = "Arial Black",
    FontSize = 75,
    FontColor = Color.FromArgb(90, 0, 0, 0),
    Opacity = 14,
    Rotation = -32,
    HorizontalAlignment = HorizontalAlignment.Center,
    VerticalAlignment = VerticalAlignment.Middle
};

var qr = new BarcodeStamper("https://verify.com/contract/789")
{
    BarcodeType = BarcodeType.QRCode,
    Width = 80,
    Height = 80,
    HorizontalAlignment = HorizontalAlignment.Right,
    VerticalAlignment = VerticalAlignment.Bottom,
    HorizontalOffset = new Length(-22),
    VerticalOffset = new Length(-22)
};

doc.ApplyStamp(logo);
doc.ApplyStamp(watermark);
doc.ApplyStamp(qr);
doc.SaveAs("agreement-branded.pdf");
```

署名などを組み合わせる方法については、[C#でPDFからテキストを解析および抽出する方法は？](parse-pdf-extract-text-csharp.md)を参照してください。

## ウォーターマークを微妙または目立たせるには、不透明度をどのように使用しますか？

`Opacity`プロパティを使用してウォーターマークの可視性を制御します。値は`0`（見えない）から`100`（完全に固体）までの範囲です。

```csharp
// Install-Package IronPdf
using IronPdf;
using IronPdf.Stamps;

var doc = PdfDocument.FromFile("draft.pdf");

var faint = new TextStamper
{
    Text = "SAMPLE",
    Opacity = 8,
    FontSize = 100,
    Rotation = -40
};

var strong = new TextStamper
{
    Text = "VOID",
    Opacity = 70,
    FontSize = 100,
    FontColor = System.Drawing.Color.Red,
    Rotation = -40
};

doc.ApplyStamp(faint);
doc.ApplyStamp(strong);
doc.SaveAs("draft-watermarked.pdf");
```

**範囲のリマインダー：**  
- 15未満：ほぼ見えない  
- 30–50：典型的なウォーターマーク  
- 70以上：目立つ