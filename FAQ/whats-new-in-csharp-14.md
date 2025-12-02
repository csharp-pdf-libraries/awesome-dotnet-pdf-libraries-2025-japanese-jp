---
**  (Japanese Translation)**

 **English:** [FAQ/whats-new-in-csharp-14.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/FAQ/whats-new-in-csharp-14.md)
 **:** [FAQ/whats-new-in-csharp-14.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/FAQ/whats-new-in-csharp-14.md)

---
# C# 14の最も実用的な新機能とその使用方法は？

C# 14が.NET 10とともに登場し、日々の開発をよりクリーンで、より速く、より表現力豊かにする本当に役立つ機能の波をもたらしました。コレクション式、プライマリコンストラクタ、高度なパターンマッチング、インライン配列によるパフォーマンスの向上が必要かどうかにかかわらず、C# 14にはあなたにとっての何かがあります。このFAQでは、最も影響力のある機能を分析し、それらが輝く場所（そして問題を起こす場所）を示し、すぐにコピーして適応できる実際のコード例を提供します。新機能、その使用方法、注意点について知りたい場合は、正しい場所にいます。

プラットフォーム全体の概要については、[.NET 10の新機能](whats-new-in-dotnet-10.md)を参照し、ガイド付きウォークスルーについては、[C# 14/.NET 10チュートリアル](dotnet-10-csharp-14-tutorial.md)をチェックしてください。

---

## C# 14での統一コレクション式はコレクションの初期化をどのように簡素化しますか？

C# 14は、コレクションの初期化に対してユニバーサルな括弧ベースの構文を導入し、ボイラープレートを削減し、コードをより読みやすく、保守しやすくします。配列、`List<T>`、イミュータブルコレクション、または独自の型を使用している場合でも、単一の直感的な構文を得ることができます。

### 新しい構文は古い方法と比べてどのように異なりますか？

以前は、コレクションを初期化することは、各タイプに対して適切なコンストラクタまたは静的メソッドを選択することを意味し、繰り返しのコードにつながりました。ここでは、以前とC# 14のコレクション式の後の様子を示します：

#### C# 14以前

```csharp
// Install-Package IronPdf
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

var arr = new int[] { 1, 2, 3 };
var list = new List<int> { 1, 2, 3 };
var span = new Span<int>(new int[] { 1, 2, 3 });
var immutable = ImmutableArray.Create(1, 2, 3);
```

#### C# 14コレクション式を使用

```csharp
// Install-Package IronPdf
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

var arr = [1, 2, 3];                        // int[]
List<int> list = [1, 2, 3];                 // List<int>
ImmutableArray<int> immutable = [1, 2, 3];  // ImmutableArray<int>
Span<int> span = [1, 2, 3];                 // Span<int>
```

コンパイラはコンテキストに基づいて何を望んでいるかを判断するため、コレクションタイプ間のリファクタリングがはるかに苦痛ではありません。

### この新しい構文をカスタムコレクションタイプで使用できますか？

はい！クラスに`IEnumerable<T>`を引数に取る適切なコンストラクタまたは静的メソッドがある場合、新しいコレクション式構文を使用できます。

```csharp
public class CustomNumbers : List<int>
{
    public CustomNumbers(IEnumerable<int> numbers) : base(numbers) { }
}

CustomNumbers nums = [4, 5, 6]; // 4, 5, 6を含む
```

これにより、カスタムコレクションがファーストクラスの感覚を持ちます。

### コレクション式でスプレッドオペレータはどのように機能しますか？

スプレッドオペレータ(`..`)を使用すると、コレクションをインラインで結合したり、他のコレクションから要素を挿入したり、コレクションを構築しながらフィルタリングしたりすることができます。

```csharp
var first = [1, 2, 3];
var second = [4, 5, 6];
var merged = [..first, ..second, 7]; // [1, 2, 3, 4, 5, 6, 7]

var odds = [1, 3, 5];
var all = [0, ..odds, 6, 8]; // [0, 1, 3, 5, 6, 8]

// インラインフィルタリング
var nums = [1, 2, 3, 4, 5, 6];
var evens = [..nums.Where(n => n % 2 == 0)]; // [2, 4, 6]
```

これは、[IronPDF](https://ironpdf.com)のような動的ドキュメントを組み立てる際のパイプラインスタイルのコードに非常に便利です。

### コレクション式で型推論はより賢くなりましたか？

絶対にそうです。メソッドのパラメーターや戻り値としてコレクション式を使用でき、型はコンテキストから推論されます：

```csharp
List<string> GetNames() => ["Alice", "Bob", "Charlie"];

void UseScores(List<int> scores) { /* ... */ }
UseScores([100, 95, 80]);
```

もう`new List<T>()`や不必要な変換は必要ありません。

### 空のコレクションの作成や、途中でのフィルタリングはどうですか？

空のコレクションを作成するのは`[]`と同じくらい簡単で、LINQを組み合わせてその場でフィルタリングすることができます：

```csharp
List<int> empty = [];
var names = ["Anna", "Ben", "Charlie"];
var aNames = [..names.Where(n => n.StartsWith("A"))]; // ["Anna"]
```

### コレクション式に関する注意点はありますか？

はい、コンテキストは重要です—時々、期待していた型（例えば、`List<int>`を望んでいたのに`int[]`になる）ではないことがあります。曖昧さがある場合は、型を明示的に宣言してください：

```csharp
List<int> numbers = [];
```

新しい言語機能については、[C# 14の新機能](whats-new-csharp-14.md)を参照してください。

---