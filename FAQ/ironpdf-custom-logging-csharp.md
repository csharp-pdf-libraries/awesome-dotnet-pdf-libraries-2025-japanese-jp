# IronPDFでカスタムログを実装する方法は？

IronPDFはカスタマイズ可能なログをサポートしているため、アプリケーションとプロダクションのニーズに最適な方法でライブラリログをキャプチャ、ルーティング、フォーマットできます。開発中の問題を追跡したい場合、SerilogやApplication Insightsと統合したい場合、または構造化されたログをデータベースにプッシュしたい場合でも、IronPDFを使用して独自のロガーを簡単に使用できます。

---

## なぜIronPDFでカスタムログを使用するべきなのか？

標準では、IronPDFはコンソールにログを書き出しますが、これは基本的なシナリオには適しています。しかし、プロダクションの問題をデバッグする必要がある場合、分散システム間でログを相関させる必要がある場合、または可観測性の要件を満たす必要がある場合は、より多くの制御を望むでしょう。カスタムログを使用すると、IronPDFログの行き先を決定できます—ファイル、データベース、クラウド、または任意の.NETログフレームワーク。この柔軟性は、Azureのような環境でIronPDFを実行している場合（[IronPDF Azureデプロイメントのベストプラクティス](ironpdf-azure-deployment-csharp.md)を参照）、または既存のログパイプラインと統合している場合に特に価値があります。

---

## IronPDFで独自のロガーをどのようにプラグインするか？

IronPDFログを独自のロガーに送信するには、ログモードを`Custom`に設定し、ロガーインスタンスを割り当てます。以下は簡単な例です：

```csharp
// NuGet: Install-Package IronPdf
using IronPdf;
using Microsoft.Extensions.Logging; // NuGet: Microsoft.Extensions.Logging.Abstractions

IronSoftware.Logger.LoggingMode = IronSoftware.Logger.LoggingModes.Custom;
IronSoftware.Logger.CustomLogger = new MyCustomLogger();

var renderer = new ChromePdfRenderer();
var pdf = renderer.RenderHtmlAsPdf("<h1>Hello, custom logging!</h1>");
```

`LoggingMode`を設定し、`CustomLogger`を割り当てることを忘れないでください。どちらかが欠けていると、ロガーは使用されません。

---

## シンプルなカスタムILogger実装はどのようなものか？

基本的なロガーは`Microsoft.Extensions.Logging.ILogger`を実装します。これは構築できる最小の例です：

```csharp
using Microsoft.Extensions.Logging;

public class SimpleConsoleLogger : ILogger
{
    public IDisposable BeginScope<TState>(TState state) => null;
    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception exception, Func<TState, Exception, string> formatter)
    {
        Console.WriteLine($"[{logLevel}] {formatter(state, exception)}");
        if (exception != null)
            Console.WriteLine($"Exception: {exception}");
    }
}
```

IronPDFからのすべてのログは、必要に応じてフォーマット、フィルタリング、またはルーティングできるロガーを通過します。

---

## IronPDFでSerilogや他のログフレームワークをどのように使用するか？

IronPDFは主要な.NETロガーとシームレスに動作します。Serilogの場合、Serilogロガーをアダプターでラップします：

```csharp
// NuGet: Serilog, Serilog.Sinks.Console, Serilog.Sinks.File
using Serilog;
using Microsoft.Extensions.Logging;

public class SerilogLoggerAdapter : ILogger
{
    private readonly Serilog.ILogger _serilog;

    public SerilogLoggerAdapter(Serilog.ILogger serilog) => _serilog = serilog;
    public IDisposable BeginScope<TState>(TState state) => null;
    public bool IsEnabled(LogLevel logLevel) => _serilog.IsEnabled(ConvertLevel(logLevel));

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception exception, Func<TState, Exception, string> formatter)
    {
        var msg = formatter(state, exception);
        switch (logLevel)
        {
            case LogLevel.Information: _serilog.Information(msg); break;
            case LogLevel.Warning: _serilog.Warning(msg); break;
            case LogLevel.Error: _serilog.Error(exception, msg); break;
            default: _serilog.Debug(msg); break;
        }
    }

    private Serilog.Events.LogEventLevel ConvertLevel(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Information => Serilog.Events.LogEventLevel.Information,
            LogLevel.Warning => Serilog.Events.LogEventLevel.Warning,
            LogLevel.Error => Serilog.Events.LogEventLevel.Error,
            _ => Serilog.Events.LogEventLevel.Debug
        };
}

// 使用法
var serilogLogger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
IronSoftware.Logger.CustomLogger = new SerilogLoggerAdapter(serilogLogger);
```

これで、IronPDFログはSerilogのシンク—ファイル、コンソール、クラウドなど—を通過します。クラウドプラットフォームにデプロイしている場合は、[IronPDF Azureデプロイメントのヒント](ironpdf-azure-deployment-csharp.md)を参照してください。

---

## IronPDFログをApplication Insightsに送信することは可能か？

もちろんです。Azure Application Insightsと統合するには、カスタムロガーでTelemetryClientを使用します：

```csharp
// NuGet: Microsoft.ApplicationInsights
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;

public class AppInsightsLogger : ILogger
{
    private readonly TelemetryClient _telemetry;
    public AppInsightsLogger(TelemetryClient telemetry) => _telemetry = telemetry;
    public IDisposable BeginScope<TState>(TState state) => null;
    public bool IsEnabled(LogLevel logLevel) => true;
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception exception, Func<TState, Exception, string> formatter)
    {
        var msg = formatter(state, exception);
        _telemetry.TrackTrace(msg);
        if (exception != null) _telemetry.TrackException(exception);
    }
}

// 使用法
var telemetry = new TelemetryClient();
IronSoftware.Logger.CustomLogger = new AppInsightsLogger(telemetry);
```

クラウドデプロイメントに関する詳細については、[IronPDF Azureデプロイメントのアドバイス](ironpdf-azure-deployment-csharp.md)をご覧ください。

---

## IronPDFログ出力をフィルタリングまたはエンリッチするには？

ログをレベルでフィルタリングしたり、リクエスト/ユーザーIDのようなコンテキストを追加したりできます。ここでは、警告以上のログのみを記録する方法です：

```csharp
using Microsoft.Extensions.Logging;

public class WarningLogger : ILogger
{
    public IDisposable BeginScope<TState>(TState state) => null;
    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Warning;
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel)) return;
        Console.WriteLine($"[{logLevel}] {formatter(state, exception)}");
    }
}
```

ログエントリにデータを追加するために、ロガーをラップし、各ログエントリの前にデータ（例えば、WebアプリケーションのリクエストID）を追加します。

---

## IronPDFイベントをデータベースにログすることは可能か？

はい—データベースにレコードを挿入するカスタムロガーを書きます。こちらは基本的なSQL Serverの例です：

```csharp
// NuGet: Microsoft.Data.SqlClient
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

public class SqlLogger : ILogger
{
    private readonly string _connStr;
    public SqlLogger(string connStr) => _connStr = connStr;
    public IDisposable BeginScope<TState>(TState state) => null;
    public bool IsEnabled(LogLevel logLevel) => true;
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception exception, Func<TState, Exception, string> formatter)
    {
        using var conn = new SqlConnection(_connStr);
        conn.Open();
        var cmd = new SqlCommand("INSERT INTO IronPdfLogs (Level, Message) VALUES (@level, @msg)", conn);
        cmd.Parameters.AddWithValue("@level", logLevel.ToString());
        cmd.Parameters.AddWithValue("@msg", formatter(state, exception));
        cmd.ExecuteNonQuery();
    }
}
```

高ボリュームのシナリオでは、パフォーマンスの問題を避けるためにログの書き込みをバッチ処理することを検討してください。

---

## IronPDFのカスタムログにおける一般的な落とし穴は？

- **LoggingMode.Customを忘れること**：`LoggingMode`と`CustomLogger`の両方を設定する必要があります。
- **例外を無視すること**：ロガーが例外を投げると、ログが失われる可能性があります—ファイル/データベース/ネットワークの書き込みをtry-catchでラップしてください。
- **パフォーマンスへの影響**：特に[PDF生成](https://ironpdf.com/java/how-to/java-fill-pdf-form-tutorial/)のシナリオでは、プロダクションで遅い/忙しいロガーを避けてください。
- **フィルタリングしないこと**：プロダクションで全レベルのすべてをログに記録しないでください—最小ログレベルを設定してください。
- **グローバルロガー**：IronPDFは静的ロガーを使用するため、マルチテナントアプリで実行している場合は調整してください。

---

## IronPDFですべてのログを無効にするには？

最大のパフォーマンスとログ出力なしを必要とする場合は、単純に設定してください：

```csharp
IronSoftware.Logger.LoggingMode = IronSoftware.Logger.LoggingModes.None;
```

ログは完全にオフになりました。トラブルシューティングのために再びオンにすることを忘れないでください！

---

## IronPDFのログおよび関連機能についてもっと学ぶには？

HTMLからPDFへの変換については、[How do I convert HTML to PDF with IronPDF in C#?](html-to-pdf-csharp-ironpdf.md)、ライセンスキーの設定については、[How do I use my IronPDF license key in C#?](ironpdf-license-key-csharp.md)、画像のインポートについては、[How can I convert images to PDF in C#?](image-to-pdf-csharp.md)を参照してください。インストールについては、[How to install .NET 10](how-to-install-dotnet-10.md)を参照してください。公式ドキュメントや例については、[IronPDF](https://ironpdf.com)または[Iron Software](https://ironsoftware.com)を訪問してください。高度なレンダリングについては、[ChromePdfRenderer](https://ironpdf.com/blog/videos/how-to-render-webgl-sites-to-pdf-in-csharp-ironpdf/)をチェックしてください。

---

*[Jacob Mellor](https://ironsoftware.com/about-us/authors/jacobmellor/)、[Iron Software](https://ironsoftware.com)のCTO。2017年以来、開発者の生活を楽にするツールを構築しています。*
---

## 関連リソース

### 📚 チュートリアル & ガイド
- **[HTML to PDFガイド](../html-to-pdf-csharp.md)** — 完全な変換チュートリアル
- **[2025年のベストPDFライブラリ](../best-pdf-libraries-dotnet-2025.md)** — ライブラリ比較
- **[初心者チュートリアル](../csharp-pdf-tutorial-beginners.md)** — 最初のPDFを5分で
- **[決定フローチャート](../choosing-a-pdf-library.md)** — 適切なライブラリを見つける

### 🔧 PDF操作
- **[PDFのマージ & 分割](../merge-split-pdf-csharp.md)** — ドキュメントの結合
- **[デジタル署名](../digital-signatures-pdf-csharp.md)** — 法的に署名
- **[テキストの抽出](../extract-text-from-pdf-csharp.md)** — テキスト抽出
- **[PDFフォームの入力](../fill-pdf-forms-csharp.md)** — フォーム自動化

### 🚀 フレームワーク統合
- **[ASP.NET Core PDF](../asp-net-core-pdf-reports.md)** — Webアプリ統合
- **[Blazor PDF生成](../blazor-pdf-generation.md)** — Blazorサポート
- **[クロスプラットフォームデプロイメント](../cross-platform-pdf-dotnet.md)** — Docker, Linux, Cloud

### 📖 ライブラリドキュメント
- **[IronPDF](../ironpdf/)** — 完全なChromiumレンダリング
- **[QuestPDF](../questpdf/)** — コードファースト生成
- **[PDFSharp](../pdfsharp/)** — オープンソースオプション

---

*「Awesome .NET PDF Libraries 2025」コレクションの一部 — 73のC#/.NET PDFライブラリを比較し、167のFAQ記事を提供。*

---

*41年間、ドキュメント処理の課題を解決してきたJacob Mellor（CTO, Iron Software）。彼の哲学：「何でも名前をつけてください。私は常にそれを学びます。新しいプログラミング言語で最高の仕事をするのは、何が不可能かを知らないときです。」[LinkedIn](https://www.linkedin.com/in/jacob-mellor-iron-software/)でつながる。