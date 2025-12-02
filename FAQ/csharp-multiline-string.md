# C#で複数行の文字列をどのように扱うか：生の文字列、逐語的文字列、補間文字列、および実践的なヒント

C#で複数行の文字列を扱うことは、SQLクエリ、JSONブロブ、HTMLテンプレート、あるいはファイルパスなどを考えると、すぐに面倒なことになりがちです。しかし、C#は逐語的文字列、補間文字列、生の文字列リテラルなどの機能を使って、文字列を読みやすく、保守しやすくする強力な方法を提供しています。このFAQでは、それぞれのアプローチをいつ、どのように使用するか、実践的な例を示し、一般的な落とし穴を指摘します—そうすることで、あなたのコードが求める出力と同じくらいクリーンになります。

---

## C#で複数行の文字列を定義する主な方法は何ですか？

C#は複数行の文字列を扱うためのいくつかのアプローチをサポートしており、それぞれに独自の強みがあります。最も一般的な3つを見てみましょう：

### C#で通常の文字列をどのように使用しますか？

通常の文字列はダブルクォートで得られるものです。シンプルですが、改行、タブ、またはファイルパスのエスケープ文字ですぐに問題に直面します。

```csharp
string greeting = "Hello,\nWorld!";
string filePath = "C:\\Users\\Dev\\Documents\\report.txt";
Console.WriteLine(greeting);
// 出力:
// Hello,
// World!
```

通常の文字列は短いスニペットには適していますが、複雑なフォーマットや多くのバックスラッシュが必要になると、読みにくくなります。

### 逐語的文字列（`@""`）はいつ使用すべきですか？

逐語的文字列は`@`で始まり、二重のダブルクォートを除いて、すべてを文字通りに扱います。これは、Windowsのファイルパス、SQLクエリ、および出力とコードを一致させたい任意の複数行のコンテンツに最適です。

```csharp
string path = @"C:\Projects\MyApp\config.json";
string query = @"
SELECT Name, Email
FROM Users
WHERE IsActive = 1
";
```

**ヒント：** 逐語的文字列内のダブルクォートは、単にそれらを二重にします：

```csharp
string json = @"{ ""name"": ""Alice"", ""role"": ""admin"" }";
```

### C# 11+の生の文字列リテラル（`"""..."""`）の特別な点は何ですか？

生の文字列リテラル（C# 11で導入）は三重クォート（`"""`）を使用します。バックスラッシュ、クォート、改行をエスケープすることなく、正確に望む内容を書くことができます。JSON、HTML、XML、または多くの記号を含むものに最適です。

```csharp
string json = """
{
  "user": "Bob",
  "permissions": ["read", "write"]
}
""";
```

- クォートやバックスラッシュに対するエスケープは必要ありません。
- 開始と終了の三重クォートはそれぞれ独自の行になければなりません。
- インデントは終了クォートの位置に基づいて処理されます。

HTMLからPDFへの変換を扱う場合は、[C#でHTML文字列をPDFに変換する方法は？](html-string-to-pdf-csharp.md)を参照してください。

---

## 複数行の文字列に変数を挿入するにはどうすればよいですか？

ほとんどのテンプレートには動的なコンテンツが必要です。文字列を読みやすく、柔軟に保つ方法はこちらです。

### 逐語的文字列での補間はどのように機能しますか？

逐語的文字列の前に`$`を追加（`$@""`）して、変数の補間を有効にします：

```csharp
string username = "Charlie";
int userId = 42;
string sql = $@"
SELECT *
FROM Users
WHERE Id = {userId} AND Name = '{username}'
";
```

- `{expression}`を使用して値を挿入します。
- 実際のアプリではSQLを常にパラメータ化してください—ユーザー入力を直接補間しないでください！

### 生の文字列リテラルで変数を補間するにはどうすればよいですか？

生の文字列リテラルも、`$`を前に付けることで補間できます：

```csharp
string product = "Laptop";
decimal price = 1199.99m;
string html = $"""
<p>Product: {product}</p>
<p>Price: ${price:F2}</p>
""";
```

これはJSON、HTML、またはテンプレートが重要なシナリオにとって非常に便利です。

### 補間された生の文字列で波括弧をどのように扱いますか？

出力の一部として波括弧が必要な場合（CSSやテンプレート構文を考えてみてください）、波括弧を二重にし、`$`も二重にします：

```csharp
string highlight = "#ffd700";
string css = $$"""
.highlight {
  background-color: {{highlight}};
}
""";
```

- `{{variable}}`は変数の値に置き換えられます。
- 単一の`{`または`}`はリテラルの波括弧です。

動的なHTMLおよびPDFワークフローについては、[C#でHTMLをPDFに変換する方法は？](html-to-pdf-csharp.md)を参照してください。

---

## C#で複数行の文字列を使用する実用的なユースケースは何ですか？

これらの文字列タイプが実際のコードベースでどのように現れるか見てみましょう。

### どのようにしてクリーンで安全なSQLクエリを書きますか？

フォーマットが悪いSQLは読みにくく、壊れやすいです。逐語的文字列はクエリを読みやすくし、パラメータ化はそれらを安全に保ちます。

```csharp
using System.Data.SqlClient;

string sql = @"
SELECT Name, Email
FROM Users
WHERE Status = @status
  AND Role = @role
";
using (var cmd = new SqlCommand(sql, connection))
{
    cmd.Parameters.AddWithValue("@status", "Active");
    cmd.Parameters.AddWithValue("@role", "Admin");
    // コマンドを実行...
}
```

ユーザー入力を直接補間しないでください—常にパラメータを使用してください！

### JSONまたは設定テンプレートを簡単に構築するにはどうすればよいですか？

生の文字列リテラル（$"""..."""）はJSONまたは設定データに最適です：

```csharp
string apiKey = "abc123";
string config = $"""
{
  "apiKey": "{apiKey}",
  "timeout": 30
}
""";
```

エスケープは必要ありません。コードは出力とまったく同じに見えます。

### PDF生成のための複数行HTMLテンプレートをどのように使用しますか？

[IronPDF](https://ironpdf.com)を使用してHTMLからPDFを生成する場合、複数行の文字列はテンプレートを読みやすく、保守しやすく保ちます。

```csharp
using IronPdf; // Install-Package IronPdf

string customer = "Grace";
decimal total = 987.65m;
string html = $@"
<html>
  <body>
    <h1>Invoice</h1>
    <p>Customer: {customer}</p>
    <p>Total: ${total:F2}</p>
  </body>
</html>
";

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("Invoice.pdf");
```

PDFとそのDOMを操作する方法については、[C#でPDF DOMオブジェクトにアクセスする方法は？](access-pdf-dom-object-csharp.md)をご覧ください。

### CSS、JavaScript、または他の波括弧形式についてはどうですか？

補間を伴う生の文字列リテラルはCSSやJSスニペットにとって救世主です：

```csharp
string color = "#4caf50";
string css = $$"""
.button {
  background-color: {{color}};
  border-radius: 5px;
}
""";
```

もうエスケープの頭痛や醜い連結に悩む必要はありません。

### エスケープせずにファイルパスを扱うにはどうすればよいですか？

Windowsパスには逐語的文字列（`@""`）が最適です：

```csharp
string logPath = @"C:\Logs\app.log";
```

動的にパスを構築する場合は、補間と逐語的文字列を組み合わせます：

```csharp
string baseDir = @"C:\Data";
string file = "output.txt";
string fullPath = $@"{baseDir}\{file}";
```

PDFでの添付ファイルやファイル操作を扱う場合は、[C#でPDFに添付ファイルを追加する方法は？](add-attachments-pdf-csharp.md)を参照してください。

### コードを散らかさずに大きなテンプレートを使用するにはどうすればよいですか？

本当に長いテンプレート（複数ページのHTMLメールや大きなPDFレイアウトなど）の場合は、リソースとして埋め込みます：

1. **ファイルを追加**（例：`Template.html`）プロジェクトに。
2. **ビルドアクション**を「埋め込みリソース」に設定。
3. **実行時にロード：**

```csharp
using System.Reflection;
using System.IO;

var assembly = Assembly.GetExecutingAssembly();
var resource = "MyAppName.Template.html";

using (var stream = assembly.GetManifestResourceStream(resource))
using (var reader = new StreamReader(stream))
{
    string template = reader.ReadToEnd();
    // 必要に応じてプレースホルダを置き換える
    string output = template.Replace("{{UserName}}", "Maxine");
}
```

小さく、動的なテンプレートの場合は、インラインの複数行文字列を使用してください。

---

## 改行、空白、または長い文字列を分割するにはどうすればよいですか？

### 複数行の文字列を単一行に平坦化するにはどうすればよいですか？

複数行のブロックを1行としてログに記録したり、処理したりする必要がある場合は、改行を置き換えます：

```csharp
string multiline = @"
First line
Second line
Third line
";
string singleLine = multiline.Replace(Environment.NewLine, " ");
Console.WriteLine(singleLine);
// 出力: First line Second line Third line
```

より堅牢な方法（すべての空白行を削除する）：

```csharp
string normalized = string.Join(" ",
    multiline.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
);
```

### 読みやすさのためにコード行にわたって長い文字列を分割するにはどうすればよいですか？

文字列が長いが、出力としては単一行であるべき場合は、連結します：

```csharp
string message = "This is a really long error message that " +
                 "needs to wrap in code, but not in output.";
Console.WriteLine(message);
```

または、逐語的または生の文字列を使用して、見える改行を制御します。

---

## C#で複数行の文字列を使用する際に注意すべき一般的な落とし穴は何ですか？

- **Windowsパスに通常の文字列を使用する：** `\n`、`\t`などで予期しない結果が得られます。`@`を使用してください。
- **逐語的文字列でクォートを二重にするのを忘れる：** `@"He said, "Hi""`は無効です。`@"He said, ""Hi"""`を使用してください。
- **埋め込みファイルの間違ったリソース名：** 実際の名前を確認するには`assembly.GetManifestResourceNames()`を使用してください。
- **`$@""`と`@$""`の混在：** どちらも機能しますが、`$@""`が一般的です。
- **SQLにユーザー入力を補間する：** SQLインジェクションを防ぐために常にパラメータ化してください。
- **生の文字列リテラルのインデント問題：** 閉じる三重クォートが何を削除するかを決定します。配置に注意してください！
- **古いC#で生の文字列を使用しようとする：** 生の文字列リテラルにはC# 11以降が必要です。

言語比較については、[C#はテキスト処理においてPythonとどのように比較されますか？](compare-csharp-to-python.md)をご覧ください。

---

## C#で各文字列構文をいつ使用すべきか？（クイックリファレンス）

| 構文                  | 使用するタイミング                       | 例                                                      |
|---------------------|---------------------------------------|------------------------------------------------------|
| `@"..."`            | 逐語的（改行、パス、SQL）              | ファイルパス、読みやすいSQLクエリ                         |
| `$@"...{var}..."`   | 逐語的 + 補間                          | 動的SQLまたはインラインHTMLテンプレート                  |
| `"""..."""`         | 生（C# 11+、エスケープなし）            | JSON、HTML、大きなXMLブロック                           |
| `$"""...{var}..."""`| 生 + 補間