---
**  (Japanese Translation)**

 **English:** [FAQ/csharp-random-int.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/FAQ/csharp-random-int.md)
 **:** [FAQ/csharp-random-int.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/FAQ/csharp-random-int.md)

---
# C#でランダムな整数を生成し、ランダム性を扱う方法は？

C#でのランダム数は、ゲーム、テストデータ、シャッフリング、トークン生成などのセキュリティに敏感な操作に不可欠です。しかし、すべてのランダム数が同じように作られているわけではありません。微妙なミスがバグ、セキュリティの欠陥、または非ランダムな結果を引き起こす可能性があります。ランダムな整数を生成したい開発者のための実用的なFAQです。

---

## C#でランダムな整数を生成する最良の方法は？

標準的なアプローチは`Random`クラスを使用します。インスタンスを作成し、`.Next()`を使用します：

```csharp
using System; // 追加のNuGetは不要

var rng = new Random();
int value = rng.Next(1, 101); // 1から100まで（上限は排他的）
Console.WriteLine($"ランダム数値: {value}");
```

パラメータなしで`rng.Next()`を呼び出すと、0から`Int32.MaxValue`までの値が得られます。特定の範囲が必要ですか？最小値（包含的）と最大値（排他的）を渡します。

## Random.Next()を使用する際、範囲を正しく指定する方法は？

覚えておくべきこと：`.Next(min, max)`は`min`を含みますが、**`max`を除外**します。したがって、`rng.Next(1, 7)`は1から6の値を与えます—サイコロと同じです。上限を*包含的*にする必要がある場合は、最大値に1を加えます：

```csharp
int dice = rng.Next(1, 7); // 1から6まで
int inclusive = rng.Next(1, 101); // 1から100まで
```

包含的な範囲が頻繁に必要な場合は、ヘルパーを検討してください：

```csharp
public static int NextInclusive(Random rng, int min, int maxInc)
{
    return rng.Next(min, maxInc + 1);
}
```

## ロトのような一意のランダム数を生成することは可能ですか？

はい。範囲内で一意のランダム数を取得するには、重複を避けるために`HashSet<int>`を使用します：

```csharp
var random = new Random();
var uniqueNumbers = new HashSet<int>();

while (uniqueNumbers.Count < 6)
{
    uniqueNumbers.Add(random.Next(1, 50)); // 1から49まで包含的
}
Console.WriteLine("ロト: " + string.Join(", ", uniqueNumbers));
```

より複雑なドキュメントシナリオについては、[C#でPDFページを追加、コピー、削除する方法](add-copy-delete-pdf-pages-csharp.md)を参照してください。

## ループ内で新しいRandomインスタンスを作成するのは大丈夫ですか？

いいえ、これはやめてください！特にループ内で`Random`オブジェクトを素早く新しく作成すると、システムクロックからシードされるため、繰り返しまたは同一の数値が発生する可能性があります：

```csharp
// 悪い例: 数値が繰り返される可能性が高い
for (int i = 0; i < 5; i++)
{
    var rng = new Random();
    Console.WriteLine(rng.Next(100));
}
```

**ベストプラクティス：** クラスまたはメソッドごとに1つの`Random`を作成し、再利用します：

```csharp
var rng = new Random();
for (int i = 0; i < 5; i++)
{
    Console.WriteLine(rng.Next(100));
}
```

## ランダム数を使用して現実的なテストデータを生成する方法は？

ランダム性は、サンプルの年齢、価格、またはテストデータセットを生成するのに最適です：

```csharp
var rng = new Random();
var ages = new List<int>();
for (int i = 0; i < 10; i++) ages.Add(rng.Next(18, 66));
Console.WriteLine("テスト年齢: " + string.Join(", ", ages));
```

より現実的な数値（小数点以下2桁の価格など）にするには、丸めと組み合わせます：

```csharp
var rng = new Random();
decimal price = Math.Round((decimal)(rng.NextDouble() * 1000), 2);
```

他の場所で小数点を丸める必要がある場合は、[C#で小数点以下2桁に丸める方法](csharp-round-to-2-decimal-places.md)を参照してください。

## 暗号学的に安全なランダム数が必要な場合はどうすればいいですか？

トークン、パスワード、またはセキュリティ関連のものには、`RandomNumberGenerator`を使用します：

```csharp
using System.Security.Cryptography;

int secureInt = RandomNumberGenerator.GetInt32(1, 101); // 1から100まで
```

安全なランダムバイト配列を作成するには（トークンに最適）：

```csharp
byte[] token = new byte[32];
RandomNumberGenerator.Fill(token);
string base64 = Convert.ToBase64String(token);
Console.WriteLine(base64);
```

## テストで再現可能な結果を得るためにRandomをシードする方法は？

`Random`コンストラクタにシード整数を渡します。これにより、結果が繰り返し可能になります：

```csharp
var rng = new Random(1234);
int always33 = rng.Next(100); // 常に33
```

これは、ランダム性を含むユニットテストに不可欠です。データをシャッフルする場合は、テストセットアップでシードされたRNGを常に使用してください。

## マルチスレッドコードでランダム性を扱う方法は？

標準の`Random`はスレッドセーフではありません。並行シナリオでは、`ThreadLocal<Random>`を使用します：

```csharp
using System.Threading;

var threadRng = new ThreadLocal<Random>(() => new Random());

Parallel.For(0, 100, i =>
{
    Console.WriteLine(threadRng.Value.Next(100));
});
```

または、.NET 6+を使用している場合は、組み込みのスレッドセーフな`Random.Shared`を使用します：

```csharp
int n = Random.Shared.Next(1, 101);
```

PDFを並行して生成している場合は、[C#でPDFのMemoryStreamsを扱う方法](pdf-memorystream-csharp.md)を参照してください。

## ランダムバイトまたはダブルを生成する方法は？

バイトの場合：

```csharp
var rng = new Random();
byte[] data = new byte[16];
rng.NextBytes(data);
Console.WriteLine(BitConverter.ToString(data));
```

特定の範囲のダブルの場合：

```csharp
double d = rng.NextDouble(); // 0.0 <= d < 1.0
double scaled = 50.0 + (d * 50.0); // 50.0からちょうど100.0未満まで
```

## シャッフル、サンプリング、または重み付きランダム選択を行う方法は？

**シャッフル（Fisher-Yates）：**

```csharp
public static void Shuffle<T>(T[] items, Random rng)
{
    for (int i = items.Length - 1; i > 0; i--)
    {
        int j = rng.Next(i + 1);
        (items[i], items[j]) = (items[j], items[i]);
    }
}
```

**重み付きランダム選択：**

```csharp
public static T WeightedChoice<T>(T[] options, double[] weights, Random rng)
{
    double total = weights.Sum();
    double r = rng.NextDouble() * total;
    double acc = 0;
    for (int i = 0; i < options.Length; i++)
    {
        acc += weights[i];
        if (r < acc) return options[i];
    }
    return options[^1];
}
```

**置換なしのサンプリング：**

```csharp
var list = new List<string> { "A", "B", "C", "D" };
var selected = list.OrderBy(_ => rng.Next()).Take(2).ToList();
```

## C#でランダム数を扱う際の一般的な落とし穴は？

- **排他的上限：** `Next(1, 7)`は1–6を意味し、*1–7ではありません*。
- **複数のRandomインスタンス：** 短時間に新しい`Random()`を作成するのを避けてください。
- **スレッドセーフティ：** 単一の`Random`をスレッド間で共有しないでください。
- **セキュリティ：** センシティブな情報に`Random`を使用しないでください。代わりに`RandomNumberGenerator`を使用してください。
- **非決定的なテスト：** 信頼できるテスト結果のためには、常にRNGをシードしてください。

PDFの操作については、[C#でPDFに添付ファイルを追加する方法](add-attachments-pdf-csharp.md)や[C#でPDF DOMを操作する方法](access-pdf-dom-object-csharp.md)を参照してください。

## 動的なPDFを生成する際にランダム性をどのように使用しますか？

[IronPDF](https://ironpdf.com)を使用してランダム化されたPDFを簡単に生成できます：

```csharp
using IronPdf; // Install-Package IronPdf

var rng = new Random();
var html = "<h1>Invoice</h1><table border='1'>";
for (int i = 1; i <= 5; i++)
{
    int qty = rng.Next(1, 10);
    decimal price = Math.Round((decimal)(rng.NextDouble() * 100), 2);
    html += $"<tr><td>Product {i}</td><td>{qty}</td><td>${price}</td></tr>";
}
html += "</table>";

var renderer = new IronPdf.ChromePdfRenderer();
renderer.RenderHtmlAsPdf(html).SaveAs("Invoice.pdf");
```

PDFの構造を作成後に操作したい場合は、[C#でPDF DOMにアクセスする方法](access-pdf-dom-object-csharp.md)を参照してください。

## RandomとRandomNumberGeneratorをいつ使用すべきですか？

- ゲーム、テストデータ、または気軽なランダム性には`Random`を使用します。
- トークン、パスワードなどセキュリティに敏感なものには`RandomNumberGenerator`を使用します。

動的なPDF生成とセキュアなドキュメントワークフローについては、[IronPDF](https://ironpdf.com)と[Iron Software](https://ironsoftware.com)をチェックしてください。

---

*[Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/)、[Iron Software](https://ironsoftware.com)のCTO。2017年以来、開発者の生活を楽にするツールを作成。*
---

## 関連リソース

### 📚 チュートリアル & ガイド
- **[HTMLからPDFへのガイド](../html-to-pdf-csharp.md)** — 完全な変換チュートリアル
- **[2025年のベストPDFライブラリ](../best-pdf-libraries-dotnet-2025.md)** — ライブラリ比較
- **[初心者チュートリアル](../csharp-pdf-tutorial-beginners.md)** — 最初のPDFを5分で
- **[選択フローチャート](../choosing-a-pdf-library.md)** — 適切なライブラリを見つける

### 🔧 PDF操作
- **[PDFのマージ＆スプリット](../merge-split-pdf-csharp.md)** — ドキュメントの結合
- **[デジタル署名](../digital-signatures-pdf-csharp.md)** — 法的に署名
- **[テキスト抽出](../extract-text-from-pdf-csharp.md)** — テキスト抽出
- **[PDFフォームの入力](../fill-pdf-forms-csharp.md)** — フォーム自動化

### 🚀 フレームワーク統合
- **[ASP.NET Core PDF](../asp-net-core-pdf-reports.md)** — Webアプリ統合
- **[Blazor PDF生成](../blazor-pdf-generation.md)** — Blazorサポート
- **[クロスプラットフォーム展開](../cross-platform-pdf-dotnet.md)** — Docker、Linux、クラウド

### 📖 ライブラリドキュメント
- **[IronPDF](../ironpdf/)** — 完全なChromiumレンダリング
- **[QuestPDF](../questpdf/)** — コードファースト生成
- **[PDFSharp](../pdfsharp/)** — オープンソースオプション

---

*[Awesome .NET PDF Libraries 2025](../README.md)コレクションの一部 — 73のC#/.NET PDFライブラリを比較し、167のFAQ記事を掲載。*

---

**Jacob Mellor** — IronPDFのクリエーター。C#愛好家、Visual Studioの愛用者、使いやすさを重視したAPIデザイナー。[LinkedIn](https://www.linkedin.com/in/jacob-mellor-iron-software/)でつながる。