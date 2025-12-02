---
**  (Japanese Translation)**

 **English:** [FAQ/extract-images-from-pdf-csharp.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/FAQ/extract-images-from-pdf-csharp.md)
 **:** [FAQ/extract-images-from-pdf-csharp.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/FAQ/extract-images-from-pdf-csharp.md)

---
# C#でIronPDFを使用してPDFから画像を抽出する方法は？

PDF内に埋め込まれた画像―ロゴ、チャート、スキャンされたページ、または記入可能なフォーム内のグラフィックスなど―を抽出する必要がある場合、C#と[IronPDF](https://ironpdf.com)を使用すると驚くほど簡単です。このFAQでは、.NETでPDF画像を抽出、保存、および処理するための実践的なアプローチを、コードサンプル、メモリのヒント、バッチ処理のコツ、難しいエッジケースのアドバイスと共に紹介します。

---

## なぜC#でPDFから画像を抽出したいのですか？

開発者は、ドキュメント作成、データ分析、バックアップ、またはコンテンツの再利用のためにPDFから画像を抽出する必要がしばしばあります。これを自動化することで、手動でスクリーンショットを取る作業から解放されます―これは遅く、画像品質が低下します。抽出された画像は元の解像度を保持し、これは機械学習、プレゼンテーション、またはコンプライアンスアーカイブにとって価値があります。もしもあなたのワークフローがPDFを含む場合、画像抽出は大きな時短になります。

---

## IronPDFを使用してPDFからすべての画像を抽出する最も簡単な方法は何ですか？

最もシンプルなアプローチは、IronPDFの`ExtractAllImages()`メソッドを使用することです。こちらが最小限で実用的な例です：

```csharp
using IronPdf; // NuGet経由でインストール：Install-Package IronPdf

var pdfDoc = PdfDocument.FromFile("example.pdf");
var images = pdfDoc.ExtractAllImages();

for (int index = 0; index < images.Count; index++)
{
    images[index].SaveAs($"output-image-{index}.png");
}
```

このコードはPDFを読み込み、埋め込まれたすべてのラスター画像を取得し、それらをPNGファイルとして書き出します。これは速く、画像をそのネイティブ品質で取得できます。NuGetパッケージのインストールに関するヘルプが必要な場合は、[How do I use PowerShell to install NuGet packages?](install-nuget-powershell.md)をご覧ください。

---

## PDFからどの画像フォーマットを抽出できますか？

PDFは様々な画像タイプをサポートしています：JPEG、PNG、TIFF、BMPなど。IronPDFはこれらを`AnyBitmap`オブジェクトとして抽出し、出力フォーマットを選択できます：

```csharp
using IronSoftware.Drawing; // AnyBitmap用

var pdfFile = PdfDocument.FromFile("images.pdf");
var allImages = pdfFile.ExtractAllImages();

foreach (var pic in allImages)
{
    pic.SaveAs($"export-{Guid.NewGuid()}.jpg", AnyBitmap.ImageFormat.Jpeg); // JPEGとして保存
    pic.SaveAs($"export-{Guid.NewGuid()}.png", AnyBitmap.ImageFormat.Png);   // またはPNGとして
}
```

**ヒント：** PNGは透明性と詳細を保持し、JPEGは写真に対して小さいサイズです。データURIやbase64画像を扱っている場合は、[How do I handle data URI/base64 images in PDFs?](data-uri-base64-images-pdf-csharp.md)を参照してください。

---

## 特定のPDFページから画像を抽出するにはどうすればよいですか？

時には特定のページからのみ画像が必要な場合があります。必要なものだけをターゲットにする方法はこちらです：

### 単一ページから画像を取得するにはどうすればよいですか？

```csharp
var pdfDoc = PdfDocument.FromFile("slides.pdf");
var imagesOnPage2 = pdfDoc.ExtractImagesFromPage(1); // ページ番号はゼロベース

foreach (var image in imagesOnPage2)
{
    image.SaveAs($"slide2-img-{Guid.NewGuid()}.png");
}
```

### 範囲のページを通してループするにはどうすればよいですか？

例えば、最初の5ページからのみ画像が必要な場合：

```csharp
for (int page = 0; page < 5; page++)
{
    var imgs = pdfDoc.ExtractImagesFromPage(page);
    foreach (var img in imgs)
    {
        img.SaveAs($"pg{page + 1}-{Guid.NewGuid()}.png");
    }
}
```

C#でインデックスを使ってループする方法についてのアドバイスは、[How do I use foreach with an index in C#?](csharp-foreach-with-index.md)を参照してください。

---

## 大きなPDFを扱う際、メモリをどのように管理すればよいですか？

数百ページまたは数千ページのPDFを扱う場合、一度にすべての画像を読み込まないでください。代わりに、ページごとに処理し、画像を迅速に破棄します：

```csharp
for (int i = 0; i < pdfDoc.PageCount; i++)
{
    var pageImages = pdfDoc.ExtractImagesFromPage(i);
    foreach (var img in pageImages)
    {
        img.SaveAs($"page{i + 1}-img-{Guid.NewGuid()}.png");
        img.Dispose(); // すぐにメモリを解放
    }
}
```

このアプローチはメモリの急増を防ぎ、大きなファイルでもバルク処理を安定させます。

---

## 抽出した画像をサイズや他のメタデータによってフィルタリングできますか？

はい！高解像度の画像のみが必要であったり、小さなアイコンを無視したい場合があります。幅、高さ、またはフォーマットなどのメタデータに基づいて画像を検査し、フィルタリングします。

### 画像のメタデータを取得するにはどうすればよいですか？

```csharp
var allImages = pdfDoc.ExtractAllImages();
foreach (var bitmap in allImages)
{
    Console.WriteLine($"Width: {bitmap.Width}, Height: {bitmap.Height}, Format: {bitmap.Format}");
}
```

### 大きな画像のみを保存するにはどうすればよいですか？

例えば、少なくとも800x600ピクセルの画像を保持するには：

```csharp
using System.Linq;

var bigImages = allImages.Where(img => img.Width >= 800 && img.Height >= 600);
foreach (var bigImg in bigImages)
{
    bigImg.SaveAs($"big-{Guid.NewGuid()}.png");
}
```

これにより、関連性のないアセットをスキップして時間を節約できます。

---

## 抽出した画像をメモリまたはクラウドストレージに保存するにはどうすればよいですか？

IronPDFを使用すると、APIやクラウドストレージへの直接アップロードに最適なメモリ内に画像を保持できます。

### 画像をMemoryStreamに保存するにはどうすればよいですか？

```csharp
using System.IO;

foreach (var img in images)
{
    using var memStream = new MemoryStream();
    img.SaveAs(memStream, AnyBitmap.ImageFormat.Png);
    byte[] bytes = memStream.ToArray();
    // データベースやクラウドストレージにバイトをアップロード
}
```

### 抽出した画像をAmazon S3に直接アップロードできますか？

もちろんです！AWS SDKを使用したサンプルはこちらです：

```csharp
// Install-Package AWSSDK.S3
using Amazon.S3;
using Amazon.S3.Transfer;

var s3 = new AmazonS3Client();
var uploader = new TransferUtility(s3);

foreach (var img in images)
{
    using var ms = new MemoryStream();
    img.SaveAs(ms, AnyBitmap.ImageFormat.Png);
    ms.Position = 0;

    var request = new TransferUtilityUploadRequest
    {
        InputStream = ms,
        Key = $"uploads/{Guid.NewGuid()}.png",
        BucketName = "your-bucket"
    };

    uploader.Upload(request);
}
```

AzureやGoogle Cloudのサンプルに興味がある場合は、Iron Softwareチームにお知らせください！

---

## PDFに抽出可能な画像がない（またはベクトルのみ）場合はどうすればよいですか？

一部のPDFにはテキストやベクトルグラフィックス（SVGやパスなど）のみが含まれており、`ExtractAllImages()`では何も見つかりません。

### この状況をどのように検出できますか？

```csharp
var images = pdfDoc.ExtractAllImages();
if (images.Count == 0)
{
    Console.WriteLine("画像が見つかりませんでした―ベクトルグラフィックスのみかもしれません。");
}
```

### ベクトルベースのPDFから画像を抽出するにはどうすればよいですか？

ページをラスタライズ（画像としてレンダリング）することができます：

```csharp
pdfDoc.RasterizeToImageFiles("vector-page-*.png", new ImageRenderingOptions
{
    Dpi = 300 // 出版用の高解像度
});
```

ラスタライズは、テキスト、ベクトル、画像を含む1ページごとのビットマップをキャプチャします。

テキスト抽出についての詳細は、[How do I extract text from a PDF in C#?](extract-text-from-pdf-csharp.md)を参照してください。

---

## フォルダ内の複数のPDFから画像を抽出するにはどうすればよいですか？

IronPDFと.NETを使用したバッチ処理はシンプルです：

```csharp
using System.IO;

var pdfPaths = Directory.GetFiles("pdfs", "*.pdf");

foreach (var path in pdfPaths)
{
    var doc = PdfDocument.FromFile(path);
    var imgs = doc.ExtractAllImages();

    var folder = Path.Combine("extracted", Path.GetFileNameWithoutExtension(path));
    Directory.CreateDirectory(folder);

    for (int idx = 0; idx < imgs.Count; idx++)
    {
        imgs[idx].SaveAs(Path.Combine(folder, $"img-{idx}.png"));
    }
}
```

処理を速めたい場合は、`Parallel.ForEach`を使用してファイルを並行して処理します。

---

## 抽出した画像を高品質（または圧縮された）に保証するにはどうすればよいですか？

出力フォーマットと品質を制御します：

### 損失のない（最高品質の）画像を保存するにはどうすればよいですか？

```csharp
img.SaveAs("sharp.png"); // PNGは損失がない
```

### より小さいJPEGを保存するにはどうすればよいですか？

```csharp
img.SaveAs("small.jpg", AnyBitmap.ImageFormat.Jpeg, 80); // 80%の品質
```

ダイアグラムには、JPEGのアーティファクトを避けるためにPNGを使用してください。

---

## IronPDFはパスワード保護されたPDFから画像を抽出できますか？

はい―パスワードを知っている場合。PDFをロードするときにパスワードを提供するだけです：

```csharp
var doc = PdfDocument.FromFile("locked.pdf", "CorrectPassword");
var images = doc.ExtractAllImages();
// 通常どおり保存
```

---

## IronPDFはPDFフォームに埋め込まれた画像を抽出できますか？

ほとんどの場合、`ExtractAllImages()`はフォームフィールド内の画像（例えば、署名）をキャプチャします。まれなエッジケースや非常に複雑なフォームについては、影響を受けるページをラスタライズすることで、目に見えるすべてのアセットを確実にキャプチャできます。逆のプロセスについては、[How do I add images to a PDF in C#?](add-images-to-pdf-csharp.md)を参照してください。

---

## 抽出した画像を後処理するには（グレースケール、クロップ、リサイズ）どうすればよいですか？

抽出した後、[ImageSharp](https://sixlabors.com/products/imagesharp/)のようなライブラリを使用してさらに処理できます：

```csharp
// Install-Package SixLabors.ImageSharp
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

foreach (var img in images)
{
    using var ms = new MemoryStream();
    img.SaveAs(ms, AnyBitmap.ImageFormat.Png);
    ms.Position = 0;

    using var image = Image.Load(ms);

    image.Mutate(x => x
        .Grayscale()
        .Resize(new ResizeOptions
        {
            Size = new Size(800, 600),
            Mode = ResizeMode.Max
        }));

    image.Save($"processed-{Guid.NewGuid()}.png");
}
```

これは、MLやWeb公開用の画像を準備するのに理想的です。

---

## IronPDFによるPDF画像抽出の速度はどのくらいですか？

パフォーマンスは、ドキュメントのサイズと画像の数に依存します：

- **小さなPDF**：ファイルあたり100-300ms
- **大きなPDF（100+ページ）**：1-5秒
- **バッチ/並行処理**：マルチコアシステムでは処理時間が大幅に短縮

### 画像を並行してバッチ抽出するにはどうすればよいですか？

```csharp
using System.Threading.Tasks;

var pdfs = Directory.GetFiles("incoming", "*.pdf");

Parallel.ForEach(pdfs, pdfFile =>
{
    var doc = PdfDocument.FromFile(pdfFile);
    var imgs = doc.ExtractAllImages();

    var folder = Path.Combine("output", Path.GetFileNameWithoutExtension(pdfFile));
    Directory.CreateDirectory(folder);

    for (int i = 0; i < imgs.Count; i++)
    {
        imgs[i].SaveAs(Path.Combine(folder, $"img-{i}.png"));
    }
});
```

数千を処理する場合は、メモリリークを避けるために画像を迅速に破棄してください。

---

## サムネイルのみを抽出するか、重複を削除するための高度なテク