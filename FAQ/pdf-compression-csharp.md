---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [FAQ/pdf-compression-csharp.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/FAQ/pdf-compression-csharp.md)
🇯🇵 **日本語:** [FAQ/pdf-compression-csharp.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/FAQ/pdf-compression-csharp.md)

---

# C#でPDFを圧縮して、より小さく、より高速なドキュメントを作成する方法は？

C#でPDFファイルサイズを削減するのは、思っているよりも簡単です—特に適切なツールを使用すれば。メールの添付ファイル制限に収まるようにしたい場合でも、ダウンロードを速めたい場合でも、サーバーのストレージをより遠くまで伸ばしたい場合でも、PDFの圧縮はファイルサイズを30〜70％削減するのに役立ちます。最小限の品質損失で。このFAQでは、.NET開発者向けの実用的な、コードファーストの戦略を通じて、IronPDFを使用してPDFを圧縮する方法、ヒント、トラブルシューティングのアドバイス、および高度なシナリオを歩いていきます。

---

## なぜPDFは最初に非常に大きくなるのですか？

PDFは、いくつかの一般的な要因によってすぐにサイズが膨らむことがあります：

- **高解像度画像**：埋め込まれたロゴ、スクリーンショット、写真は、画面表示に必要なものをはるかに超える、印刷レベルの詳細をしばしば使用します。
- **非効率なグラフィックス形式**：圧縮されていないPNGやTIFFは、JPEGよりもかなり大きくなります。
- **過剰な内部メタデータ**：HTML、XAML、またはオフィスファイルから作成されたPDFは、多くの構造とアクセシビリティデータを詰め込むことができます。
- **エクスポート時の圧縮なし**：多くのツールは、サイズを最適化せずにPDFをエクスポートします。

PDFを生成、共有、または保存していて、巨大なファイルに気づいた場合は、圧縮を検討する時期です。

---

## C#でPDFを1行で圧縮するにはどうすればよいですか？

IronPDFを使用すると、PDF画像のサイズを簡単に削減できます。PDF内のすべての埋め込み画像を単一のメソッド呼び出しで圧縮できます：

```csharp
using IronPdf; // Install-Package IronPdf

var document = PdfDocument.FromFile("big-report.pdf");
document.CompressImages(75); // JPEG品質を75%に使用
document.SaveAs("big-report-compressed.pdf");
```

JPEG品質を指定することにより、IronPDFはPDF内のすべてのラスター画像を再圧縮します。実際には、70〜80％の品質は、目に見える違いが最小限で巨大な削減を提供します。他の形式をPDFに変換する方法については、[Xml To Pdf Csharp](xml-to-pdf-csharp.md)または[Xaml To Pdf Maui Csharp](xaml-to-pdf-maui-csharp.md)をご覧ください。

---

## PDF画像圧縮中に何が起こりますか？

`CompressImages`を呼び出すと、IronPDFはPNG、JPEG、TIFFなどの形式の埋め込み画像をPDFでスキャンします。それから、選択した品質レベルでJPEGとしてそれらを再エンコードします。これにより、特に写真が多いPDFのファイルサイズが劇的に削減されます。

異なる圧縮レベルをテストする方法は次のとおりです：

```csharp
using IronPdf; // Install-Package IronPdf

int[] qualities = { 90, 80, 70, 60 };
foreach (var q in qualities)
{
    var doc = PdfDocument.FromFile("sample.pdf");
    doc.CompressImages(q);
    doc.SaveAs($"sample-compressed-{q}.pdf");
}
```

出力を並べて、品質の損失が目立ち始める場所を確認します。

---

## 正しいJPEG品質をどのように選択すればよいですか？

- **90〜100**：元のものとほぼ区別がつかない。グラフィックスが豊富なマーケティング資料に最適。
- **70〜89**：ビジネスドキュメント、チャート、ほとんどのレポートに最適—ほとんどまたはまったく目に見える損失がありません。
- **50〜69**：最大の圧縮が必要なとき、またはドラフト用に使用します。いくつかのアーティファクトを期待してください。

良い出発点は80です。さらに小さなファイルが必要な場合は、徐々に減らして品質を確認してください。

---

## HTMLまたはXAMLから生成されたPDFをどのように圧縮しますか？

HTMLまたはXAMLからPDFを生成する場合（ダッシュボードやレポートなど）、埋め込まれた高DPI画像がファイルサイズを膨らませる可能性があります。IronPDFを使用すると、コンテンツをレンダリングして結果を一度に圧縮することができます：

```csharp
using IronPdf; // Install-Package IronPdf

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(@"
    <h2>Dashboard</h2>
    <img src='large-image.png' />
");
pdf.CompressImages(75);
pdf.SaveAs("dashboard-compressed.pdf");
```

XAML生成PDFに対しても同じパターンが機能します。詳細については、[Xaml To Pdf Maui Csharp](xaml-to-pdf-maui-csharp.md)を参照してください。

---

### ASP.NET Core APIでアップロードされたPDFを圧縮するにはどうすればよいですか？

アップロードされたPDFを受け取り、圧縮して結果を返すWeb APIを構築できます。これは、SaaSやドキュメント管理アプリに理想的です。

```csharp
using IronPdf;
using Microsoft.AspNetCore.Mvc;
using System.IO;

// Install-Package IronPdf

[HttpPost("api/compress-pdf")]
public async Task<IActionResult> CompressPdf(IFormFile file)
{
    if (file == null || file.Length == 0)
        return BadRequest("ファイルがアップロードされていません。");

    var tempFile = Path.GetTempFileName();
    using (var fs = new FileStream(tempFile, FileMode.Create))
        await file.CopyToAsync(fs);

    var pdf = PdfDocument.FromFile(tempFile);
    pdf.CompressImages(75);

    var compressedFile = Path.GetTempFileName();
    pdf.SaveAs(compressedFile);

    var bytes = await System.IO.File.ReadAllBytesAsync(compressedFile);
    System.IO.File.Delete(tempFile);
    System.IO.File.Delete(compressedFile);

    return File(bytes, "application/pdf", "compressed.pdf");
}
```

このアプローチは、ほとんどのユースケースで本番環境に対応し、高速です。

---

## 画像を圧縮した後でもPDFがまだ大きい場合はどうすればよいですか？

テーブルやアクセシビリティメタデータが多いPDFは、画像圧縮後もまだ大きいままのことがよくあります。これはしばしば「構造ツリー」—テーブルやドキュメント階層を記述するメタデータによるものです。

IronPDFは`CompressStructTree`を提供して、このデータをトリミングします：

```csharp
using IronPdf; // Install-Package IronPdf

var pdf = PdfDocument.FromFile("tables.pdf");
pdf.CompressStructTree();
pdf.SaveAs("tables-compact.pdf");
```

**注意：** 構造データを削除すると、テキスト選択、検索、またはアクセシビリティ機能が壊れる可能性があります。本番環境で使用する前に、またはコンプライアンスに敏感なドキュメントの場合は、十分にテストしてください。PDFのセキュリティとコンプライアンスについての詳細は、[Pdf Security Digital Signatures Csharp](pdf-security-digital-signatures-csharp.md)を参照してください。

---

### 画像と構造圧縮を組み合わせて、さらに小さなファイルを作成できますか？

もちろん—両方の方法を組み合わせると最良の結果が得られます：

```csharp
using IronPdf; // Install-Package IronPdf

var pdf = PdfDocument.FromFile("massive.pdf");
pdf.CompressImages(75);
pdf.CompressStructTree();
pdf.SaveAs("massive-super-compressed.pdf");
```

構造圧縮は不可逆なので、常にオリジナルのバックアップを保持してください。

---

## 圧縮に細かい制御をどのように得られますか？

高度なシナリオの場合は、IronPDFの`CompressionOptions`を使用します。これにより、以下を行うことができます：

- 表示サイズに合わせて画像のサイズを変更
- JPEG品質を調整
- 品質とサイズのトレードオフに対する色のサブサンプリングを制御
- 構造メタデータを削除

例えば：

```csharp
using IronPdf; // Install-Package IronPdf

var pdf = PdfDocument.FromFile("detailed.pdf");
var options = new CompressionOptions
{
    CompressImages = true,
    JpegQuality = 80,
    HighQualityImageSubsampling = false, // 低い=より多くの圧縮
    ShrinkImages = true, // 大きな画像を縮小
    RemoveStructureTree = true
};
pdf.Compress(options);
pdf.SaveAs("detailed-compressed.pdf");
```

**`ShrinkImages`をいつ使用すべきか？** ユーザーが非常に大きな画像を挿入し、小さく表示される場合—これは圧縮前に画像のサイズを縮小します。

---

### クロマサブサンプリングとは何か、そしてそれがPDF圧縮にどのように影響するか？

JPEG色のサブサンプリングは、色チャネルの詳細を削減し、目にはほとんど気づかれないが、スペースを節約します。IronPDFでは、選択できます：

- **4:4:4（高品質）**：色の損失なし。`HighQualityImageSubsampling = true`を設定します。
- **4:2:0（標準）**：一般的なドキュメントに適しています。`HighQualityImageSubsampling = false`を設定します。
- **4:1:1（攻撃的）**：最大の圧縮。ドラフト専用。

クライアント向けPDFやブランディングには、高品質を維持してください。内部レポートには、通常、標準が適しています。

---

## PDFの全フォルダを一括圧縮できますか？

はい！IronPDFを使用すると、多くのファイルを自動的に圧縮することが簡単になり、大量アーカイブや移行に理想的です：

```csharp
using IronPdf;
using System.IO;
// Install-Package IronPdf

string inputDir = @"C:\MyPdfs";
string outputDir = @"C:\MyPdfs\Compressed";
Directory.CreateDirectory(outputDir);

foreach (var file in Directory.GetFiles(inputDir, "*.pdf"))
{
    var pdf = PdfDocument.FromFile(file);
    pdf.CompressImages(75);
    var outFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(file) + "-compressed.pdf");
    pdf.SaveAs(outFile);
}
```

完全な.NET Core互換性については、[Dotnet Core Pdf Generation Csharp](dotnet-core-pdf-generation-csharp.md)を参照してください。

---

## PDFをマージする前または後に圧縮するべきですか？

最良の結果を得るには、まずPDFをマージしてから圧縮を適用します。これにより、コンプレッサーが冗長な画像やメタデータをより効率的に処理できます：

```csharp
using IronPdf; // Install-Package IronPdf

var combined = PdfDocument.Merge(
    PdfDocument.FromFile("a.pdf"),
    PdfDocument.FromFile("b.pdf"),
    PdfDocument.FromFile("c.pdf")
);
combined.CompressImages(75);
combined.SaveAs("merged-compressed.pdf");
```

このアプローチは時間を節約し、圧縮効率を最大化します。

---

## PDFの特定のページまたは画像のみを圧縮するにはどうすればよいですか？

IronPDFは選択的な圧縮をサポートしています。たとえば、2〜5ページの画像のみを圧縮するには：

```csharp
using IronPdf; // Install-Package IronPdf

var pdf = PdfDocument.FromFile("multi-page.pdf");
for (int i = 2; i <= 5; i++)
{
    var page = pdf.Pages[i];
    page.CompressImages(60);
}
pdf.SaveAs("multi-page-partial-compressed.pdf");
```

独自のロジックを拡張できます。たとえば、特定のサイズ以上の画像のみを圧縮する、またはロゴをスキップするなどです。バッチおよび選択的なシナリオについては、[完全なPDF圧縮ガイド](https://ironpdf.com/java/how-to/compress-pdf-java-tutorial/)を参照してください。

---

## どのような圧縮結果を期待できますか？

実際のテストに基づくと：

- **写真が多いPDF**：70〜80％の品質で40〜70％の削減
- **スクリーンショット/図**：30〜50％小さい
- **テーブルが多い（構造が圧縮されている）**：20〜40％小さい
- **混在コンテン