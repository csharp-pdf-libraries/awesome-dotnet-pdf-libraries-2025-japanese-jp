---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [FAQ/draw-text-bitmap-pdf-csharp.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/FAQ/draw-text-bitmap-pdf-csharp.md)
🇯🇵 **日本語:** [FAQ/draw-text-bitmap-pdf-csharp.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/FAQ/draw-text-bitmap-pdf-csharp.md)

---

# C#でPDFにテキスト、画像、グラフィックスを描画する方法は？

もちろんです！C#アプリケーションからPDFに動的にスタンプ、透かし、ロゴ、カスタムグラフィックスを追加する必要がある場合、ここが正しい場所です。このFAQでは、「PAID」のオーバーレイのスタンピングからバーコードの挿入、さらには.NETを使用した線や形の描画まで、プログラムでPDFを編集するための実用的なステップ、一般的な落とし穴、パフォーマンスのヒントをすべてカバーしています。

---

## なぜC#を使用してPDFに描画したいのですか？

PDF編集を自動化することで、特に大量のドキュメントを処理する必要がある場合、時間を節約し、手動でのエラーを減らすことができます。C#でPDFに直接描画することで、以下のことが可能になります：

- 請求書に「PAID」、「DRAFT」、またはカスタムラベルを自動的にスタンプします。
- あなたの会社のロゴや透かしをレポートや契約の全ページにオーバーレイします。
- 承認タイムスタンプ、ユーザーノート、デジタル署名などの動的注釈を追加します。
- 機密文書にブランディングとセキュリティマークを適用します。

より高度な描画（線や長方形など）に興味がある場合は、[C#でPDFに線や長方形を描画する方法は？](draw-line-rectangle-pdf-csharp.md)を参照してください。

---

## C#プロジェクトをセットアップしてPDFを編集するにはどうすればよいですか？

始めるには、開発者に焦点を当てた、設定が簡単で、PDFの奇妙な点を処理してくれるIronPdfライブラリが必要です。

**NuGet経由でインストール：**
```powershell
Install-Package IronPdf
```

**必要な名前空間：**
```csharp
using IronPdf;                // PDF操作
using System.Drawing;         // 画像、色、グラフィックス
using System.IO;              // ファイルとストリームの処理
using System.Net.Http;        // ウェブから画像を取得
using System.Threading.Tasks; // 非同期およびバッチ操作用
```

[IronPDF](https://ironpdf.com)は、[Iron Software](https://ironsoftware.com)によってよくサポートされ、定期的に更新されています。

---

## PDFにテキストを追加するには、スタンプや透かしのような？

テキストを描画するのは簡単です。目立つ「PAID」スタンプが必要な場合でも、控えめな注釈が必要な場合でも。

**基本的な例：**
```csharp
using IronPdf;

// PDFを開く
var doc = PdfDocument.FromFile("invoice.pdf");

// 最初のページの座標(300, 400)に「PAID」を配置
doc.DrawText("PAID", 300, 400, 0);

doc.SaveAs("invoice-with-stamp.pdf");
```
**PDF座標系：**  
- X=0は左端で、右に増加します。
- Y=0は下部にあり、上に増加します。
- ページはゼロベースのインデックスを使用します。

**ヒント：** 複数サイズのPDFの場合、動的に寸法を取得します：
```csharp
var dimensions = doc.Pages[0].Size;
int pageWidth = (int)dimensions.Width;
int pageHeight = (int)dimensions.Height;
```

矩形や線を描画する詳細については、[C#でPDFに線や矩形を描画する方法は？](draw-lines-rectangles-pdf-csharp.md)を参照してください。

---

### フォント、色、テキストの回転を制御するにはどうすればよいですか？

テキストスタイリング—フォントファミリー、サイズ、色、さらには回転も—を完全に制御できます。

**スタイル付きテキストの例：**
```csharp
using IronPdf;
using System.Drawing; // 色用

doc.DrawText(
    "CONFIDENTIAL",
    250, 420, 0,
    font: StandardFonts.HelveticaBold,
    fontSize: 36,
    color: Color.Red,
    rotation: 45
);
```
利用可能なフォントには、Helvetica、Courier、TimesRomanなどがあります（`StandardFonts`を参照）。透明度には`Color.FromArgb(alpha, r, g, b)`を使用します。

**複数行のテキスト：** 単に`\n`を使用します：
```csharp
doc.DrawText("APPROVED\n2024-06-15", 100, 650, 0, fontSize: 18);
```

---

### すべてのページにテキストを描画するには、またはページ番号を追加するにはどうすればよいですか？

透かし、スタンプ、またはページ番号を追加するために、各ページをループします。

**すべてのページに透かしを追加：**
```csharp
for (int page = 0; page < doc.PageCount; page++)
{
    doc.DrawText(
        "DRAFT",
        200, 400, page,
        font: StandardFonts.HelveticaBold,
        fontSize: 72,
        color: Color.FromArgb(80, 255, 0, 0), // 半透明の赤
        rotation: 45
    );
}
```

**ページ番号を追加：**
```csharp
for (int page = 0; page < doc.PageCount; page++)
{
    doc.DrawText(
        $"Page {page + 1} of {doc.PageCount}",
        50, 40, page,
        font: StandardFonts.Courier,
        fontSize: 10
    );
}
```

PDFからテキストを抽出する必要がある場合は、[C#でPDFからテキストを抽出する方法は？](extract-text-from-pdf-csharp.md)を確認してください。

---

### PDFに注釈、署名、またはタイムスタンプを追加することはできますか？

はい、ユーザー名、日付、または電子署名を動的に追加して、承認トレイルを作成できます。

**例：**
```csharp
string user = Environment.UserName;
string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

doc.DrawText(
    $"Signed by: {user}\n{dateTime}",
    400, 50, 0,
    font: StandardFonts.Courier,
    fontSize: 10,
    color: Color.Gray
);
```

複雑な注釈ワークフローでは、テキストとグラフィックスを組み合わせることが非常に効果的です。

---

## PDFに画像、ロゴ、または透かしを追加するにはどうすればよいですか？

ロゴ、署名、またはバーコード画像であっても、グラフィックスをオーバーレイする方法は次のとおりです：

**ロゴをオーバーレイ：**
```csharp
using IronPdf;
using System.Drawing;

var doc = PdfDocument.FromFile("report.pdf");
using var logo = new Bitmap("company-logo.png");

// 必要に応じてX、Yを調整して、ロゴを右上に配置
doc.DrawBitmap(logo, 450, 700, 0);

doc.SaveAs("report-branded.pdf");
```
PNGは透明性を保持し、クリスプなオーバーレイを実現します。

---

### 画像のサイズを変更するには、または透明度を追加するにはどうすればよいですか？

挿入された画像のサイズとその透明度を簡単に制御できます。

**画像のサイズ変更：**
```csharp
// (450, 700)に120x60ptのロゴを配置
doc.DrawBitmap(logo, 450, 700, 0, width: 120, height: 60);
```
**透明なPNG：**
```csharp
using var watermark = new Bitmap("transparent-watermark.png");
doc.DrawBitmap(watermark, 180, 350, 0, width: 300, height: 150);
```
PNG/TIFFはアルファチャネルをサポートしていますが、JPEGには透明性がありません。

---

### URLまたはBase64文字列から画像を追加することはできますか？

もちろんです—画像をダウンロードまたはデコードして描画するだけです。

**URLから：**
```csharp
using var client = new HttpClient();
var imageBytes = await client.GetByteArrayAsync("https://mycdn.com/logo.png");

using var ms = new MemoryStream(imageBytes);
using var bitmap = new Bitmap(ms);

doc.DrawBitmap(bitmap, 100, 600, 0);
```

**Base64から：**
```csharp
var imageData = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAA..."); // 切り捨て
using var ms = new MemoryStream(imageData);
using var bitmap = new Bitmap(ms);

doc.DrawBitmap(bitmap, 300, 600, 0);
```

---

### パフォーマンスのために大きな画像をどのように扱うべきですか？

大きな画像はPDFを重くする可能性があります。描画前にサイズを変更します：

```csharp
using var original = new Bitmap("big-photo.jpg");
int targetWidth = 400, targetHeight = 300;

using var resized = new Bitmap(original, new Size(targetWidth, targetHeight));
doc.DrawBitmap(resized, 100, 400, 0);
```
より高品質のスケーリングには、`Graphics.DrawImage`を使用できます。

---

## PDFにカスタムグラフィックス、線、または形を描画するにはどうすればよいですか？

IronPdfはPDFキャンバス上に直接線や形を描画することを直接サポートしていませんが、メモリ内にグラフィックスを画像として簡単に作成し、それをオーバーレイすることができます。

**カスタム「VERIFIED」スタンプを描画する例：**
```csharp
using var canvas = new Bitmap(200, 100);
using var g = Graphics.FromImage(canvas);

g.Clear(Color.Transparent);
g.DrawEllipse(Pens.Blue, 10, 10, 180, 80);
g.DrawString("VERIFIED", new Font("Arial", 24, FontStyle.Bold), Brushes.Green, 30, 35);

// PDFにオーバーレイ
doc.DrawBitmap(canvas, 220, 500, 0);
```
ネイティブに線や長方形を描画するには、[C#でPDFに線や長方形を描画する方法は？](draw-line-rectangle-pdf-csharp.md)および[C#を使用してPDFに複数の線や長方形を描画する方法は？](draw-lines-rectangles-pdf-csharp.md)を確認してください。

---

## PDFにバーコードまたはQRコードを追加するにはどうすればよいですか？

バーコードやQRコードはIronPdfに組み込まれていませんが、画像として生成（IronBarcodeなどのライブラリを使用）し、PDFに配置することができます。

**バーコードの例：**
```csharp
using IronBarCode; // Install-Package IronBarCode

var barcode = BarcodeWriter.CreateBarcode("12345", BarcodeEncoding.Code128);
using var barcodeImage = barcode.ToBitmap();

doc.DrawBitmap(barcodeImage, 50, 100, 0, width: 200, height: 60);
```
**QRコードの例：**
```csharp
var qr = BarcodeWriter.CreateBarcode("https://ironpdf.com", BarcodeEncoding.QRCode);
using var qrImage = qr.ToBitmap();

doc.DrawBitmap(qrImage, 400, 600, 0, width: 100, height: 100);
```

---

## C#でパスワード保護されたPDFを編集できますか？

はい！PDFをロードするときにパスワードを提供するだけです。

```csharp
using IronPdf;

var doc = PdfDocument.FromFile("protected.pdf", "yourpassword");
doc.DrawText("APPROVED", 300, 500, 0, font: StandardFonts.HelveticaBold, fontSize: 36);
doc.SaveAs("protected-stamped.pdf");
```

---

## バッチPDF処理のパフォーマンスのヒントはありますか？

数十（または数百）のPDFにスタンプや透かしをバッチ処理する場合、効率的に作業する方法は次のとおりです：

- **テキスト描画：** 操作ごとに10～30ms。
- **画像：** 操作ごとに50～150ms（画像サイズに依存）。
- **バッチジョブ：** 可能であれば並列処理を使用します。

**フォルダ内のすべてのPDFをバッチ処理：**
```csharp
using System.IO;
using System.Threading.Tasks;

var files = Directory.GetFiles("invoices", "*.pdf");

Parallel.ForEach(files, file =>
{
    var doc = PdfDocument.FromFile(file);
    doc.DrawText("DRAFT", 300, 400, 0,
        font: StandardFonts.HelveticaBold,
        fontSize: 48,
        color: Color.FromArgb(120, 255, 0, 0),
        rotation: 45);
    doc.SaveAs($"stamped/{Path.GetFileName(file)}");
});
```
RAMやCPUに制限がある場合は、過度に並列化しないでください。

---

## 一般的な間違いやトラブルシューティングのヒントは？

注意すべきいくつかの点があります：

- **座標の混乱：** PDFの原点は左下で、Windows Formsのように左上ではありません。
- **ぼやけた画像：** 小さな画像をアップスケーリングするとぼやけます。より高解像度のソースを使用し、ダウンスケールしてください。
- **透明性の喪失：** PNG/TIFF画像のみがアルファチャネルを保持します。
- **フォントがレンダリングされない：** 広範な互換性のために組み込みの`StandardFonts`に固執してください。
- **ファイル保存エラー：** ファイルロックや開いているイン