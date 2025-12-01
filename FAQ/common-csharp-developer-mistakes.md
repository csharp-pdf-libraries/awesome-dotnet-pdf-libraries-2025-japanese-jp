---
**🌐 日本語版 (Japanese Translation)**

📖 **English:** [FAQ/common-csharp-developer-mistakes.md](https://github.com/iron-software/awesome-dotnet-pdf-libraries-2025/blob/main/FAQ/common-csharp-developer-mistakes.md)
🇯🇵 **日本語:** [FAQ/common-csharp-developer-mistakes.md](https://github.com/csharp-pdf-libraries/awesome-dotnet-pdf-libraries-2025-jp/blob/main/FAQ/common-csharp-developer-mistakes.md)

---

# C#で開発者がよく犯す間違いとその回避方法は？

新しいC#開発者でも経験豊富な開発者でも、言語や.NETエコシステムに潜む典型的な落とし穴に躓くことがあります。リソースリークから非同期の悪夢まで、これらの間違いの多くは、何に注意すべきかを知っていれば簡単に修正できます。このFAQは、最も頻繁に発生するC#のエラー、その重要性、そして実用的なコードサンプルやベストプラクティスを用いてそれらを回避する方法を強調しています。

---

## C#で`IDisposable`オブジェクトを破棄することがなぜ重要なのか？

`IDisposable`オブジェクト（ファイルハンドルやIronPDFのようなPDFジェネレーターなど）を破棄しないことは、リソースリークにつながり、最終的にアプリケーションをクラッシュさせる可能性があります。`IDisposable`としてマークされた型を見たら、常に`using`ステートメントを使用してください。

```csharp
// Install-Package IronPdf
using IronPdf; // NuGet package
public void CreatePdf(string html)
{
    using var pdf = new PdfDocument();
    pdf.AddHtml(html);
    pdf.SaveAs("output.pdf");
}
```
C# 8は、追加のネスティングが不要なクリーナーな`using var`構文を導入しました。PDFを操作する詳細については、[C#でPDF DOMにアクセスする方法は？](access-pdf-dom-object-csharp.md)を参照してください。

---

## いつ例外をキャッチすべきで、なぜすべての例外をキャッチするべきではないのか？

広範囲に`Exception`をキャッチすることは、バグを隠し、対処すべき重大なエラーを無視する可能性があります。代わりに、処理方法を知っている例外をキャッチしてください。

```csharp
public async Task<string?> FetchDataAsync(HttpClient client, ILogger log)
{
    try
    {
        return await client.GetStringAsync("https://api.example.com");
    }
    catch (HttpRequestException ex)
    {
        log.LogError(ex, "Network issue");
        return null;
    }
    // 処理できるものだけをキャッチしてください！
}
```
予期しない例外を開発中に表面化させることで、本番前に問題を発見できます。

---

## `async`/`await`をC#コードで適切に使用する方法は？

追加の処理が必要な場合に`await`を省略すると、例外が見逃されたり、予期しない動作が発生したりする可能性があります。結果を操作する場合は、常に`async`/`await`を使用してください。

```csharp
public async Task<string> GetContentAsync()
{
    var response = await _httpClient.GetStringAsync("https://api.site.com");
    // 必要に応じてここで追加のロジック
    return response;
}
```
追加のロジックなしでタスクをそのまま返す場合は、直接`Task`を返すことが適切です。PDFからテキストを抽出する詳細については、[C#でPDFからテキストを解析し抽出する方法は？](parse-pdf-extract-text-csharp.md)をチェックしてください。

---

## UIおよびWebアプリで`.Result`または`.Wait()`を使用するべきではない理由は？

特にASP.NETやUIアプリケーションで非同期タスクに`.Result`や`.Wait()`を使用すると、アプリケーションがデッドロックする可能性があります。代わりに、メソッドを完全に非同期にしてください：

```csharp
public async Task LoadUserDataAsync()
{
    var data = await FetchDataAsync();
    // awaitの後にデータを使用
}
```
呼び出しスタック全体で非同期を採用することで、より応答性が高く堅牢なアプリケーションになります。

---

## 反復処理中にコレクションを変更する最も安全な方法は？

反復処理中にコレクションを変更すると例外が発生します。アイテムを安全に削除するには、スナップショットを作成するか、専用のメソッドを使用してください：

```csharp
var numbers = new List<int> {1, 2, 3, 4};
numbers.RemoveAll(n => n % 2 == 0); // 偶数を安全に削除
```
またはコピーを反復処理します：

```csharp
foreach (var n in numbers.Where(x => x < 3).ToList())
    numbers.Remove(n);
```
より高度なPDFページ操作については、[C#を使用してPDFにページを追加、コピー、または削除する方法は？](add-copy-delete-pdf-pages-csharp.md)を参照してください。

---

## 現代のC#でNull参照例外を防ぐ方法は？

特にnullable参照型（C# 8+）を使用する場合、nullチェックは不可欠です。nullabilityを有効にし、メソッドの開始時に`null`をチェックしてください：

```csharp
#nullable enable
public void PrintUserName(User? user)
{
    if (user == null)
        throw new ArgumentNullException(nameof(user));
    Console.WriteLine(user.Name?.ToUpper() ?? "Unknown");
}
```
コンパイラの支援を得るために、ファイルまたはプロジェクト設定で`#nullable`を有効にしてください。

---

## C#で文字列比較に`==`を使用することは安全ですか？

文字列に`==`を使用すると、ケースセンシティビティや文化の違いにより微妙なバグが発生する可能性があります。明示的なオプションを使用して`string.Equals`を好む：

```csharp
if (string.Equals(input, "admin", StringComparison.OrdinalIgnoreCase))
{
    // 正しい、大文字小文字を区別しない比較
}
```
このアプローチは、ユーザー入力に対して信頼性が高く、文化関連の問題を避けます。

---

## `async void`メソッドを使用するのはいつ許容されますか？

イベントハンドラーのUIコードを除き、`async void`を使用するべきではありません。他のすべての非同期メソッドでは、例外が観測可能になるように`Task`または`Task<T>`を返してください。

```csharp
public async Task SyncDataAsync()
{
    await _apiClient.SendDataAsync();
}
// イベントハンドラー（許容される）
private async void Button_Click(object sender, EventArgs e)
{
    await SyncDataAsync();
}
```
イベントハンドラーでない場合は、`async void`を避けてください。

---

## ループ内での文字列連結を避けるべき理由は？

ループ内で繰り返し文字列を連結することは非効率的であり、各操作で新しい文字列が作成されます。より良いパフォーマンスのために`StringBuilder`を使用してください：

```csharp
using System.Text;
var builder = new StringBuilder();
for (int i = 0; i < 1000; i++)
{
    builder.AppendLine($"Row {i}");
}
string result = builder.ToString();
```
これは、大規模なレポートを生成したり、PDF生成のためのコンテンツを準備したりする場合に特に重要です。

---

## ライブラリコードで`.ConfigureAwait(false)`の役割は？

ライブラリコードを書くときは、呼び出し元の同期コンテキストをキャプチャしないように`.ConfigureAwait(false)`を使用して、UIやASP.NETアプリでのデッドロックを防ぎます。

```csharp
public async Task<string> DownloadDataAsync()
{
    return await _httpClient.GetStringAsync("https://api.site.com")
        .ConfigureAwait(false);
}
```
これにより、ライブラリがどのようなコンテキストでも信頼性を持って動作することが保証されます。

---

## `DateTime.UtcNow`を`DateTime.Now`の代わりに使用するべき理由は？

`DateTime.Now`で日付を保存またはログに記録すると、ローカル時間が埋め込まれ、サーバーの場所によって異なり混乱を招く可能性があります。一貫性のために`DateTime.UtcNow`を使用してください：

```csharp
var timestamp = DateTime.UtcNow;
```
表示用にローカル時間に変換してください：

```csharp
var local = TimeZoneInfo.ConvertTimeFromUtc(timestamp, TimeZoneInfo.Local);
```
一貫したタイムスタンプは、デバッグと分析をはるかに容易にします。

---

## C#でユーザー入力を適切に検証する方法は？

ユーザー入力を信用しないでください—システムに入力されるとすぐに検証してください。例えば、メールアドレスを検証する場合：

```csharp
using System.Text.RegularExpressions;
public void Register(string email)
{
    if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        throw new ArgumentException("Invalid email address.");
    // 登録を続ける
}
```
セキュリティ問題を防ぐために、コントローラーやAPIエントリポイントでサニタイズして検証してください。

---

## ASP.NETで`Task.Run()`を使用して「コードを非同期にする」のは正しいことですか？

I/Oバウンド作業のためにASP.NETコントローラーで`Task.Run()`を使用することは、単にスレッドを無駄にするだけです。代わりに、真に非同期のメソッドを使用してください：

```csharp
public async Task<IActionResult> GetData()
{
    var result = await _db.FetchAsync();
    return Ok(result);
}
```
`Task.Run()`は、本当にバックグラウンドで実行する必要があるCPU集約型タスクにのみ正当化されます。

---

## 非同期操作で`CancellationToken`をどのように使用すべきか？

長時間実行される非同期操作では、`CancellationToken`を受け入れて伝播させてください。これにより、ユーザーや呼び出し元がリクエストをキャンセルでき、応答性が向上します。

```csharp
public async Task ProcessFileAsync(string path, CancellationToken cancellationToken)
{
    using var stream = File.OpenRead(path);
    await ReadAllAsync(stream, cancellationToken);
}
```
完全なサポートのために、すべての非同期呼び出しにトークンを渡してください。

---

## 接続文字列やシークレットをハードコーディングすることはなぜ悪いアイデアなのか？

接続文字列などの機密設定を直接コードに格納すると、リークの原因となり、デプロイが柔軟性に欠けることになります。代わりに、設定ファイルや環境変数を使用してください：

```csharp
using Microsoft.Extensions.Configuration;
public class Repository
{
    private readonly string _connStr;
    public Repository(IConfiguration config)
    {
        _connStr = config.GetConnectionString("MyDb");
    }
}
```
本番環境のシークレットには、Azure Key Vaultのような安全なストレージを活用してください。PDFにファイルを安全に添付する方法については、[C#でPDFに添付ファイルを追加する方法は？](add-attachments-pdf-csharp.md)を参照してください。

---

## 安全でモダンなPDF生成コントローラーはC#でどのように見えるべきか？

IronPDFを使用してASP.NET Coreコントローラーでいくつかのベストプラクティスをまとめた例です：

```csharp
// Install-Package IronPdf
using IronPdf;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;

[ApiController]
[Route("reports")]
public class ReportsController : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateReport([FromBody] ReportInput input, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(input.HtmlContent))
            return BadRequest("Content required.");

        using var renderer = new HtmlToPdf();
        var pdfDoc = await renderer.RenderHtmlAsPdfAsync(input.HtmlContent, cancellationToken);

        using var stream = new MemoryStream();
        pdfDoc.SaveAs(stream);
        stream.Position = 0;

        return File(stream, "application/pdf", "report.pdf");
    }
}

public class ReportInput
{
    public string HtmlContent { get; set; }
}
```
このコントローラーは、入力を検証し、リソースを管理し、キャンセルを処理し、非同期で動作します。IronPDFについての詳細は、[ironpdf.com](https://ironpdf.com)と[Iron Software](https://ironsoftware.com)から他の開発者ツールを学びましょう。

---

## C#の一般的な問題に対する迅速な修正方法は？

**非同期が期待通りに動作しない？** `.Result`/`.Wait()`があるか、またはすべての階層で`async`が不足しているかをチェックしてください。  
**「コレクションが変更されました」エラーが発生する？** 反復処理中にコレクションを変更しないでください—`.RemoveAll()`を使用するか、スナップショットを反復処理してください。  
**奇妙なタイムスタンプ？** `DateTime.UtcNow`に切り替えてください。  
**ASP.NETでライブラリコードがハングアップする？** `.ConfigureAwait(false)`を使用してください。  
**非同期コードで未処理の例外？** イベントハンドラーに必要な場合を除き、`async void